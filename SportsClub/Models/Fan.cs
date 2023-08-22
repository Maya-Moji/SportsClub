using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace SportsClub.Models
{
    public class Fan
    {
        //id
        public int ID { get; set; }

        //last name
        [Required]
        [StringLength(50)]
        [Column("LastName")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        //first name
        [Required]
        [StringLength(50)]
        [Column("FirstName")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        //DOB
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString ="{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Column("BirthDate")]
        [Display(Name = "Birth Date")]
        public DateTime BirthDate { get; set; }

        //full name
        [Display(Name = "Full Name")]
        public string FullName
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }

        public ICollection<Subscription> Subscriptions { get; set; }

    }
}
