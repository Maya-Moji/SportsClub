using Microsoft.AspNetCore.Mvc.Rendering;
using System.Reflection;

namespace SportsClub.Models.ViewModels
{
    public class FanSubscriptionViewModel
    {
        public Fan Fan { get; set; }
        public IEnumerable<SportClubSubscriptionViewModel> Subscriptions { get; set; }


    }
}
