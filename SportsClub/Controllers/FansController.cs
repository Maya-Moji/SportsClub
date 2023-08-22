using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SportsClub.Data;
using SportsClub.Models;
using SportsClub.Models.ViewModels;

namespace SportsClub.Controllers
{
    public class FansController : Controller
    {
        private readonly SportsDbContext _context;

        public FansController(SportsDbContext context)
        {
            _context = context;
        }

        // GET: Fans
        public async Task<IActionResult> Index(int ID)
        {
            var viewModel = new SportClubViewModel
            {
                Fans = await _context.Fans
                .Include(i => i.Subscriptions)
                .ThenInclude(i => i.SportClub)
                .OrderBy(i => i.LastName)
                .ToListAsync()
            };
            if(ID != 0)
            {
                ViewData["FanID"] = ID;
                //find the id of the target fan and through Subscriptions, get its SportClubs
                viewModel.SportClubs = viewModel.Fans.Where(
                x => x.ID == ID).Single().Subscriptions.Select(x => x.SportClub).OrderBy(i => i.Title);
            }
            return View(viewModel);
        }

        // GET: Fans/EditSubscriptions/5
        public async Task<IActionResult> EditSubscriptions(int ID)
        {
            var viewModel = new FanSubscriptionViewModel();
            //Set viewModel's Fan property to retrieve the fan whose ID we have
            //viewModel.Fan = (from record in _context.Fans where record.ID == ID select record).ToList().Single();
            viewModel.Fan = await _context.Fans.FindAsync(ID);

            if (ID != 0)
            {
                //Create viewData to retrieve info in EditSubscription view
                ViewData["FanID"] = ID;
                ViewData["FanName"] = viewModel.Fan.FullName;

                //Get a list of all existing clubs and their subscriptions
                List<SportClub> allClubs = new List<SportClub>();
                allClubs = await _context.SportClubs
                .Include(i => i.Subscriptions)
                .ThenInclude(i => i.Fan) //from SportClubs DbSet, include Subscriptions entity, and from there, include Fan entity
                .OrderBy(i => i.Title)
                .ToListAsync();

                int count = 0;

                // fill subscriptionViewMod list of SportClubSubscriptionViewModel with allClubs data for title, sportsclubid, and ismember attributes
                List< SportClubSubscriptionViewModel> subscriptionViewMod = new List<SportClubSubscriptionViewModel>();
                foreach (var club in allClubs)
                {
                    subscriptionViewMod.Add(new SportClubSubscriptionViewModel { Title = club.Title, SportClubId = club.Id, IsMember=false });
                    
                    foreach (var subscription in club.Subscriptions)
                    {
                        if(subscription.FanId == ID)
                        {
                            subscriptionViewMod[count].IsMember = true;
                        }
                    }
                    count++;
                }

                // fill IEnumerable<SportClubSubscriptionViewModel> in FanSubscriptionViewModel with subscriptionViewMod data
                viewModel.Subscriptions = subscriptionViewMod.OrderBy(i => i.Title).OrderBy(i=>i.IsMember);

            }

            return View(viewModel);

        }

        // action AddSubscriptions - takes FanId and SportsClubId as parameters (form the composite key for the Subscription table) and adds the fan to the list of sports club subscribers
        public async Task<IActionResult> AddSubscriptions(String sportClubId, int fanId)
        {
            //create a new subscription record
            var subscriptions = new Subscription[]
            {
                new Subscription{FanId=fanId,SportClubId=sportClubId}
            };
            foreach (var subscription in subscriptions)
            {
                _context.Subscriptions.Add(subscription);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("EditSubscriptions", new {ID=fanId}); //passing fanId to EditSubscriptions action
        }

        // action RemoveSubscriptions  - takes FanId and SportsClubId as parameters (form the composite key for the Subscription table) and removes the fan from the list of sports club subscribers
        public async Task<IActionResult> RemoveSubscriptions(String sportClubId, int fanId)
        {
            foreach (var subscription in _context.Subscriptions)
            {
                if (subscription.FanId == fanId && subscription.SportClubId == sportClubId)
                {
                    _context.Subscriptions.Remove(subscription);
                }
            }
            await _context.SaveChangesAsync();

            return RedirectToAction("EditSubscriptions", new { ID = fanId }); //passing fanId to EditSubscriptions action
        }

        // GET: Fans/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Fans == null)
            {
                return NotFound();
            }

            var fan = await _context.Fans
                .FirstOrDefaultAsync(m => m.ID == id);
            if (fan == null)
            {
                return NotFound();
            }

            return View(fan);
        }

        // GET: Fans/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Fans/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,LastName,FirstName,BirthDate")] Fan fan)
        {
            if (ModelState.IsValid)
            {
                _context.Add(fan);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(fan);
        }

        // GET: Fans/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Fans == null)
            {
                return NotFound();
            }

            var fan = await _context.Fans.FindAsync(id);
            if (fan == null)
            {
                return NotFound();
            }
            return View(fan);
        }

        // POST: Fans/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,LastName,FirstName,BirthDate")] Fan fan)
        {
            if (id != fan.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(fan);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FanExists(fan.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(fan);
        }

        // GET: Fans/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Fans == null)
            {
                return NotFound();
            }

            var fan = await _context.Fans
                .FirstOrDefaultAsync(m => m.ID == id);
            if (fan == null)
            {
                return NotFound();
            }

            return View(fan);
        }

        // POST: Fans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Fans == null)
            {
                return Problem("Entity set 'SportsDbContext.Fans'  is null.");
            }
            var fan = await _context.Fans.FindAsync(id);
            if (fan != null)
            {
                _context.Fans.Remove(fan);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FanExists(int id)
        {
          return _context.Fans.Any(e => e.ID == id);
        }

    }
}
