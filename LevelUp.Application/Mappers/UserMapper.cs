using LevelUp.Application.Dtos.User;
using LevelUp.Domain.Entities;

namespace LevelUp.Application.Mappers
{
    public static class UserMapper
    {

        public static UserEntity ToEntity(this UserCreateDto dto, string passwordHash)
        {
            return new UserEntity
            {
                FullName = dto.FullName,
                Email = dto.Email,
                PasswordHash = passwordHash,
                JobTitle = dto.JobTitle,
                TeamId = dto.TeamId,
                Role = "USER"
            };
        }

        public static UserResponseDto ToResponseDto(this UserEntity user)
        {
            return new UserResponseDto(
                            user.Id,
                            user.FullName,
                            user.Email,
                            user.JobTitle,
                            user.PointBalance,
                            user.Role,
                            user.TeamId
                        );
        }
    }
}
