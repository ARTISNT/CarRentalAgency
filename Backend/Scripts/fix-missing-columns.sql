-- Quick fix script for existing DBs that don't have the new columns yet
-- Run against the rental-service DB (CarRentalAgency)

IF NOT EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'payment_transactions' AND COLUMN_NAME = 'Description'
)
BEGIN
    ALTER TABLE payment_transactions ADD [Description] nvarchar(500) NULL;
    PRINT 'Added Description column to payment_transactions';
END
ELSE
BEGIN
    PRINT 'Description column already exists on payment_transactions';
END
GO

IF NOT EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'payments' AND COLUMN_NAME = 'final_amount'
)
BEGIN
    ALTER TABLE payments ADD [final_amount] decimal(18,2) NULL;
    ALTER TABLE payments ADD [final_currency] nvarchar(3) NULL;
    PRINT 'Added final_amount and final_currency columns to payments';
END
ELSE
BEGIN
    PRINT 'final_amount/final_currency already exist on payments';
END
GO

-- Run against payment-service DB (PaymentService)

IF NOT EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'Transactions' AND COLUMN_NAME = 'Description'
)
BEGIN
    ALTER TABLE Transactions ADD [Description] nvarchar(500) NULL;
    PRINT 'Added Description column to Transactions';
END
ELSE
BEGIN
    PRINT 'Description column already exists on Transactions';
END
GO

IF NOT EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'Transactions' AND COLUMN_NAME = 'ExternalReceiptUrl'
)
BEGIN
    ALTER TABLE Transactions ADD [ExternalReceiptUrl] nvarchar(500) NULL;
    PRINT 'Added ExternalReceiptUrl column to Transactions';
END
ELSE
BEGIN
    PRINT 'ExternalReceiptUrl column already exists on Transactions';
END
GO
