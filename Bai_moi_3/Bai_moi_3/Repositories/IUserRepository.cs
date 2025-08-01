using Bai_moi_3.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bai_moi_3.Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User?> GetByIdAsync(int id); // Đổi thành Task<User?> để rõ ràng hơn nếu không tìm thấy
        Task AddAsync(User user);
        Task UpdateAsync(User user);
        Task DeleteAsync(int id);
    }
}