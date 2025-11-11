using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LevelUp.Domain.Entities
{
    [Table("TB_LEVELUP_REWARD_REDEMPTIONS")]
    public class RewardRedemptionEntity
    {
        [Key]
        [Column("redemption_id")]
        public int Id { get; set; }

        [Required]
        [Column("user_id")]
        public int UserId { get; set; }

        [Required]
        [Column("reward_id")]
        public int RewardId { get; set; }

        [Required]
        [Column("redeemed_at")]
        public DateTime RedeemedAt { get; set; } = DateTime.UtcNow;

        [Required]
        [Column("points_spent")]
        public int PointsSpent { get; set; }

        [ForeignKey("UserId")]
        [JsonIgnore]
        public virtual UserEntity User { get; set; }

        [ForeignKey("RewardId")]
        [JsonIgnore]
        public virtual RewardEntity Reward { get; set; }
    }
}
