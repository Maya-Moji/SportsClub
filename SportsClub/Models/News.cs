
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace SportsClub.Models
{
    public class News
    {
        [Required]
        [Display(Name = "News ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int NewsId { get; set; }

        //sportClubId
        [Required]
        public string SportClubId { get; set; }

        [Required]
        [Display(Name = "File Name")]
        [Column("FileName")]
        [StringLength(50, MinimumLength = 1)]
        public string FileName { get; set; }

        [Required]
        [Display(Name = "URL")]
        [Column("URL")]
        public string Url { get; set; }

        public SportClub SportClub { get; set; }
    }
}
