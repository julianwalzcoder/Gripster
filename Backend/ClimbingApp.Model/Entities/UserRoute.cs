using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ClimbingApp.Model.Entities;

namespace ClimbingApp.Model.Entities
{
    public class UserRoute
    {
        [Key]
        [Column(Order = 0)]
        public int UserID { get; set; }

        [Key]
        [Column(Order = 1)]
        public int RouteID { get; set; }

        [Required]
        [MaxLength(50)]
        public string Status { get; set; }

        // Neues Rating-Feld (optional, 1–5)
        [Range(1, 5)]
        public int? Rating { get; set; }

        [ForeignKey("UserID")]
        public virtual User User { get; set; }

        [ForeignKey("RouteID")]
        public virtual Climb Climb { get; set; }
    }
}
