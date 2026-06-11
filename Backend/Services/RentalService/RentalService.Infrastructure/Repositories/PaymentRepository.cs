using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using RentalService.Domain.Payments;

namespace RentalService.Infrastructure.Repositories;

public class PaymentRepository(RentalServiceContext dbContext) : IPaymentRepository
{
    public async Task<Payment> GetPaymentAsync(Guid paymentId, CancellationToken cancellationToken = default)
    {
        var payment = await dbContext.Payments
            .Include(x => x.Transactions)
            .FirstOrDefaultAsync(p => p.Id == paymentId, cancellationToken);
        return payment;
    }

    public async Task<Payment> GetPaymentByRentIdAsync(Guid rentalId, CancellationToken cancellationToken = default)
    {
        var payment = await dbContext.Payments
            .Include(x => x.Transactions)
            .FirstOrDefaultAsync(p => p.RentalId == rentalId, cancellationToken);
        return payment;
    }

    public async Task<Dictionary<Guid, Payment>> GetPaymentsByRentIdsAsync(IEnumerable<Guid> rentalIds, CancellationToken cancellationToken = default)
    {
        return await dbContext.Payments
            .Include(x => x.Transactions)
            .Where(p => rentalIds.Contains(p.RentalId))
            .ToDictionaryAsync(p => p.RentalId, cancellationToken);
    }

    public async Task AddPaymentAsync(Payment payment, CancellationToken cancellationToken = default)
    {
        await dbContext.Payments.AddAsync(payment, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdatePaymentAsync(Payment payment, CancellationToken cancellationToken = default)
    {
        // PaymentTransaction.Amount is configured as an owned type (OwnsOne) and the
        // Transactions navigation uses PropertyAccessMode.Field. EF Core's automatic
        // DetectChanges treats newly added PaymentTransaction instances in the
        // _transactions backing field as updates (not inserts) for matching Ids, which
        // causes SaveChanges to emit UPDATE ... WHERE Id = @p that matches no rows and
        // triggers DbUpdateConcurrencyException. After a failed SaveChanges the tracked
        // entity remains in the ChangeTracker in Modified state, and on retry the new
        // instance is reported as already tracked - so we must clear tracked payment
        // transactions first. The most robust solution is to bypass EF for the new
        // transactions and emit a raw SQL INSERT for each one. Existing transactions
        // (already in the DB) are detected by ExternalTransactionId and skipped so we
        // don't try to re-insert them and get a PK conflict.
        // Schema of payment_transactions:
        //   Id, amount, currency, Type, Method, ExternalTransactionId, Status,
        //   FailureReason, CreatedAtUtc, CompletedAtUtc, PaymentId (shadow FK).
        foreach (var existing in dbContext.ChangeTracker
                     .Entries<PaymentTransaction>()
                     .Where(e => e.State != EntityState.Detached)
                     .ToList())
        {
            existing.State = EntityState.Detached;
        }

        var existingInDb = await dbContext.PaymentTransactions
            .Where(t => EF.Property<Guid>(t, "PaymentId") == payment.Id)
            .Select(t => t.ExternalTransactionId)
            .ToListAsync(cancellationToken);
        var existingSet = new HashSet<string>(existingInDb);

        foreach (var tx in payment.Transactions)
        {
            // Skip transactions that are already persisted.
            if (existingSet.Contains(tx.ExternalTransactionId))
                continue;

            // Detach any tracked state for this exact instance as well.
            var entry = dbContext.Entry(tx);
            if (entry.State != EntityState.Detached)
            {
                entry.State = EntityState.Detached;
            }

            const string insertSql = @"
INSERT INTO [payment_transactions]
    ([Id], [amount], [currency], [Type], [Method], [ExternalTransactionId], [Status],
     [FailureReason], [CreatedAtUtc], [CompletedAtUtc], [PaymentId])
VALUES
    (@p_id, @p_amount, @p_currency, @p_type, @p_method, @p_externalId, @p_status,
     @p_failureReason, @p_createdAt, @p_completedAt, @p_paymentId)";

            await dbContext.Database.ExecuteSqlRawAsync(
                insertSql,
                new[]
                {
                    new Microsoft.Data.SqlClient.SqlParameter("@p_id", tx.Id),
                    new Microsoft.Data.SqlClient.SqlParameter("@p_amount", tx.Amount.Amount),
                    new Microsoft.Data.SqlClient.SqlParameter("@p_currency", tx.Amount.Currency),
                    new Microsoft.Data.SqlClient.SqlParameter("@p_type", tx.Type.Name),
                    new Microsoft.Data.SqlClient.SqlParameter("@p_method", tx.Method.Name),
                    new Microsoft.Data.SqlClient.SqlParameter("@p_externalId", tx.ExternalTransactionId),
                    new Microsoft.Data.SqlClient.SqlParameter("@p_status", tx.Status.Name),
                    new Microsoft.Data.SqlClient.SqlParameter("@p_failureReason", (object?)tx.FailureReason ?? DBNull.Value),
                    new Microsoft.Data.SqlClient.SqlParameter("@p_createdAt", tx.CreatedAtUtc),
                    new Microsoft.Data.SqlClient.SqlParameter("@p_completedAt", (object?)tx.CompletedAtUtc ?? DBNull.Value),
                    new Microsoft.Data.SqlClient.SqlParameter("@p_paymentId", payment.Id),
                },
                cancellationToken);
        }

        // Update the payment aggregate (status) via change tracker.
        var paymentEntry = dbContext.Entry(payment);
        if (paymentEntry.State == EntityState.Detached)
        {
            dbContext.Payments.Attach(payment);
            paymentEntry.State = EntityState.Modified;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}