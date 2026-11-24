using Dalil_Academy.Core.Entities;

namespace Dalil_Academy.Core.Repository;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
        Task<bool> EmailExistsAsync(string email);
    Task<User> CreateAsync(User user, Student student);
}