using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SportsClub.Models
{
    //public enum MembershipTier
    //{
    //    Alpha, Beta, Omega
    //}
    public class SportClub
    {
        //id
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Display(Name="Registration Number")]
        public string Id { get; set; }

        //title
        [Required]
        [StringLength(50)]
        [MinLength(3)]
        public string Title { get; set; }

        //fee
        [DataType(DataType.Currency)]
        [Column(TypeName ="money")]
        public decimal Fee { get; set; }

        public ICollection<Subscription> Subscriptions { get; set; }
        public ICollection<News> News { get; set; }

    }
}
