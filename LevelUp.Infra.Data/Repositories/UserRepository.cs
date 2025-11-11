using LevelUp.Domain.Common;
using LevelUp.Domain.Entities;
using LevelUp.Domain.Errors;
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
                .FirstOrDefaultAsync(u => u.Email == email && u.PasswordHash == passwordHash);

            if (userAuth is null)
            {
                throw new NoContentException("Usuário não encontrado.");
            }

            return userAuth;
        }

        public async Task<UserEntity?> CreateAsync(UserEntity user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<UserEntity?> DeleteAsync(int id)
        {
            var user = await GetByIdAsync(id);

            if (user is null)
            {
                throw new IdNotFoundException($"Usuário com ID: {id} - não encontrado para deletar.");
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<PageResultModel<IEnumerable<UserEntity>>> GetAllAsync(int offset = 0, int take = 10)
        {
            var total = await _context.Users.CountAsync();

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
                .FirstOrDefaultAsync(u => u.Email == email);

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
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user is null)
            {
                throw new NoContentException("Usuário não encontrado.");
            }

            return user;
        }

        public async Task<UserEntity?> UpdateAsync(int id, UserEntity user)
        {
            var existingUser = await _context.Users.FindAsync(id);
            if (existingUser != null)
            {
                existingUser.FullName = user.FullName;
                existingUser.Email = user.Email;
                existingUser.JobTitle = user.JobTitle;
                existingUser.TeamId = user.TeamId;
                existingUser.Role = user.Role;

                if (!string.IsNullOrWhiteSpace(user.PasswordHash))
                {
                    existingUser.PasswordHash = user.PasswordHash;
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
            if (user != null)
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
