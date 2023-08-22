using Microsoft.AspNetCore.Mvc.Rendering;
using System.Reflection;

namespace SportsClub.Models.ViewModels
{
    public class SportClubSubscriptionViewModel
    {
        public string SportClubId { get; set; }
        public string Title { get; set; }
        public bool IsMember { get; set; }

    }
}
