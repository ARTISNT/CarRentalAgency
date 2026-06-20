using UserService.Application.EmailOutbox;

namespace UserService.Application.Abstractions;

public interface IEmailOutboxRepository
{
    void Add(EmailOutboxEntry entry);
}
