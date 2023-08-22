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
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using System.IO;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;


namespace SportsClub.Controllers
{
    public class NewsController : Controller
    {
        private readonly SportsDbContext _context;
        private readonly IFileProvider _fileProvider;
        private readonly IHostingEnvironment _hostingEnvironment;

        public NewsController(SportsDbContext context, IFileProvider fileprovider, IHostingEnvironment env)
        {
            _context = context;
            _fileProvider = fileprovider;
            _hostingEnvironment = env;
        }

        // GET: News
        public async Task<IActionResult> Index(String ID)
        {

            var viewModel = new NewsViewModel();

            if (ID != null)
            {
                //Set viewModel's SportClub property to retrieve the SportClub whose ID we have
                var sportsClub = _context.SportClubs.Include(i => i.News).Where(i => i.Id == ID).Single();
                viewModel.SportClub = sportsClub;
                //viewModel.SportClub = (from record in _context.SportClubs where record.Id == ID select record).ToList().Single();

                TempData["SportsClubTitle"] = viewModel.SportClub.Title;
                TempData["SportsClubId"] = viewModel.SportClub.Id;
                TempData.Keep();// keeping TempData["SportsClubTitle"] and TempData["SportsClubId"] for when a call to no parameter Create happens
                viewModel.News = sportsClub.News;
            }

             return View(viewModel);
        }


        // GET: News/Create
        public IActionResult Create()
        {
            TempData.Keep(); // keeping TempData["SportsClubTitle"] and TempData["SportsClubId"] for when a call to Create with POST happens
            return View();
        }

        // POST: News/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormFile file)
        {
            if (file != null)
            {
                string randomFileName = Path.GetRandomFileName();

                String pathForStream, pathToSave;

                FileInfo fi = new FileInfo(file.FileName);
                    var webPath = _hostingEnvironment.WebRootPath;
                    pathForStream = Path.Combine("",webPath + @"\Images\" + randomFileName + fi.Extension);
                    pathToSave = @"\Images\" + randomFileName + fi.Extension;

                //saving file to wwwroot through file stream
                    using(var stream = new FileStream(pathForStream, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                // find the related sports club
                var sportClub = await _context.SportClubs.FindAsync(TempData["SportsClubId"].ToString());

                //create an instanse of News for the input file
                var news = new News {FileName = randomFileName, Url = pathToSave, SportClubId = TempData["SportsClubId"].ToString(),SportClub=sportClub };
                _context.News.Add(news);

                // add the new News to sports club
                sportClub.News.Add(news);

            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index", new { ID = TempData["SportsClubId"].ToString() });
        }

        // GET: News/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.News == null)
            {
                return NotFound();
            }

            var news = await _context.News
                .FirstOrDefaultAsync(m => m.NewsId == id);
            if (news == null)
            {
                return NotFound();
            }

            return View(news);
        }

        // POST: News/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int newsId, string sportClubId)
        {
            if (_context.News == null)
            {
                return Problem("Entity set 'SportsDbContext.News'  is null.");
            }
            //find the news instance with composite key
            var news = await _context.News.FindAsync(newsId, sportClubId);
            if (news != null)
            {
                string path = Directory.GetParent(@"wwwroot/") + news.Url;
                //remove file from wwwroot through file stream
                System.IO.File.Delete(path);
                //remove file from database
                _context.News.Remove(news);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index", new { ID = sportClubId });
        }

        private bool NewsExists(int newsId)
        {
          return _context.News.Any(e => e.NewsId == newsId);
        }
    }
}
