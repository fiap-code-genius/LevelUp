using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LevelUp.Domain.Entities
{
    [Table("TB_LEVELUP_USERS")]
    [Index(nameof(Email), IsUnique = true)]
    public class UserEntity
    {
        [Key]
        [Column("user_id")]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        [Column("full_name")]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        [EmailAddress]
        [Column("email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(1000)]
        [Column("password_hash")]
        public string PasswordHash { get; set; } = string.Empty;

        [StringLength(100)]
        [Column("job_title")]
        public string? JobTitle { get; set; }

        [Required]
        [Column("point_balance")]
        public int PointBalance { get; set; } = 0;

        [Column("team_id")]
        public int? TeamId { get; set; }

        [Required]
        [StringLength(20)]
        [Column("role")]
        public string Role { get; set; } = "USER";

        [Required]
        [Column("CREATED_AT")]
        public DateTime CreatedAt { get; set; }

        [Column("UPDATED_AT")]
        public DateTime? UpdatedAt { get; set; }

        [Required]
        [Column("IS_ACTIVE", TypeName = "CHAR(1)")]
        public char IsActive { get; set; }

        [ForeignKey("TeamId")]
        [JsonIgnore]
        public virtual TeamEntity? Team { get; set; }

        [JsonIgnore]
        public virtual ICollection<RewardRedemptionEntity> RewardRedemptions { get; set; } = new List<RewardRedemptionEntity>();
    }
}
