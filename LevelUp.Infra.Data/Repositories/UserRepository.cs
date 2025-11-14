using LevelUp.Domain.Common;
using LevelUp.Domain.Entities;
using LevelUp.Domain.Errors;
using LevelUp.Domain.Interfaces;
using LevelUp.Infra.Data.AppData;
using Microsoft.EntityFrameworkCore;

namespace LevelUp.Infra.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationContext _context;

        public UserRepository(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<UserEntity?> AuthenticateAsync(string email, string passwordHash)
        {
            var userAuth = await _context.Users
                .AsNoTracking()
                .Include(u => u.Team)
                .FirstOrDefaultAsync(u => u.Email == email && u.PasswordHash == passwordHash && u.IsActive == 'Y');

            if (userAuth is null)
            {
                throw new NoContentException("Usuário não encontrado.");
            }

            return userAuth;
        }

        public async Task<UserEntity?> CreateAsync(UserEntity user)
        {
            user.CreatedAt = DateTime.UtcNow;
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<UserEntity?> DeleteAsync(int id)
        {
            /*
                Método de soft delete para manter o histórico dos usuários
                mas não permitir mais acesso ao sistema.
            */

            var user = await _context.Users.FindAsync(id);

            if (user is null || user.IsActive == 'N')
            {
                throw new IdNotFoundException($"Usuário com ID: {id} - não encontrado para deletar.");
            }

            user.IsActive = 'N';
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<PageResultModel<IEnumerable<UserEntity>>> GetAllAsync(int offset = 0, int take = 10)
        {
            var query = _context.Users.Where(u => u.IsActive == 'Y');

            var total = await query.CountAsync();

            var data = await _context.Users
                .Include(u => u.Team)
                .OrderBy(u => u.FullName)
                .Skip(offset)
                .Take(take)
                .AsNoTracking()
                .ToListAsync();

            return new PageResultModel<IEnumerable<UserEntity>>
            {
                Data = data,
                Take = take,
                Offset = offset,
                Total = total
            };
        }

        public async Task<UserEntity?> GetByEmailAsync(string email)
        {
            var user = await _context.Users
                .AsNoTracking()
                .Include(u => u.Team)
                .FirstOrDefaultAsync(u => u.Email == email && u.IsActive == 'Y');

            if (user is null)
            {
                throw new EmailNotFoundException($"Usuário com Email: {email} - Não encontrado.");
            }

            return user;
        }

        public async Task<UserEntity?> GetByIdAsync(int id)
        {
            var user = await _context.Users
                .AsNoTracking()
                .Include(u => u.Team)
                .FirstOrDefaultAsync(u => u.Id == id && u.IsActive == 'Y');

            if (user is null)
            {
                throw new NoContentException("Usuário não encontrado.");
            }

            return user;
        }

        public async Task<UserEntity?> UpdateAsync(int id, UserEntity user)
        {
            var existingUser = await _context.Users.FindAsync(id);
            if (existingUser != null && existingUser.IsActive == 'Y')
            {
                existingUser.FullName = user.FullName;
                existingUser.Email = user.Email;
                existingUser.JobTitle = user.JobTitle;
                existingUser.TeamId = user.TeamId;
                existingUser.Role = user.Role;
                existingUser.UpdatedAt = DateTime.UtcNow;

                if (!string.IsNullOrWhiteSpace(user.PasswordHash))
                {
                    existingUser.PasswordHash = user.PasswordHash;
                }

                if (user.PointBalance != -1)
                {
                    existingUser.PointBalance = user.PointBalance;
                }

                _context.Users.Update(existingUser);
                await _context.SaveChangesAsync();
                return existingUser;
            }
            return null;
        }

        public async Task<bool> UpdateUserPointsAsync(int userId, int newPointBalance)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null && user.IsActive == 'Y')
            {
                user.PointBalance = newPointBalance;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
