using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelUp.Application.Dtos.User
{
    public record UserUpdateDto(string FullName, string Email, string? JobTitle, string Role, int? TeamId, int? PointBalance);
}
