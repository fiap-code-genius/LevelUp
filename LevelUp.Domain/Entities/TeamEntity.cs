using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LevelUp.Domain.Entities
{
    [Table("TB_LEVELUP_TEAMS")]
    [Index(nameof(TeamName), IsUnique = true)]
    public class TeamEntity
    {
        [Key]
        [Column("team_id")]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        [Column("team_name")]
        public string TeamName { get; set; } = string.Empty;

        [JsonIgnore]
        public virtual ICollection<UserEntity> Users { get; set; } = new List<UserEntity>();
    }
}
