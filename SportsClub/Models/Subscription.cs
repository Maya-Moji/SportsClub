
namespace SportsClub.Models
{
    public class Subscription
    {
        //id
        public int FanId { get; set; }

        //sportClubId
        public string SportClubId { get; set; }


        public Fan Fan { get; set; }
        
        public SportClub SportClub { get; set; }

    }
}
