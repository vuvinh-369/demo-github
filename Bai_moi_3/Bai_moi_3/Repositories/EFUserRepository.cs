using Bai_moi_3.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bai_moi_3.Repositories
{
    public class EFUserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public EFUserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.users.ToListAsync(); // Nếu bạn đổi tên DbSet là 'Users' thì đổi ở đây
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            // FirstOrDefaultAsync an toàn hơn FindAsync nếu bạn muốn include các mối quan hệ
            return await _context.users.FirstOrDefaultAsync(p => p.Id == id); // Nếu bạn đổi tên DbSet là 'Users' thì đổi ở đây
        }

        public async Task AddAsync(User user)
        {
            _context.users.Add(user); // Nếu bạn đổi tên DbSet là 'Users' thì đổi ở đây
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(User user)
        {
            _context.users.Update(user); // Nếu bạn đổi tên DbSet là 'Users' thì đổi ở đây
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var user = await _context.users.FindAsync(id); // Nếu bạn đổi tên DbSet là 'Users' thì đổi ở đây
            if (user != null) // Nên kiểm tra null trước khi xóa
            {
                _context.users.Remove(user); // Nếu bạn đổi tên DbSet là 'Users' thì đổi ở đây
                await _context.SaveChangesAsync();
            }
        }
    }
}