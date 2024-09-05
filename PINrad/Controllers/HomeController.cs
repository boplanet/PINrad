using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PINrad.Data;
using PINrad.Models;
using System.Diagnostics;
using System.Linq;

namespace PINrad.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var viewModel = new HomeIndexViewModel
            {
                LatestUsers = _context.Users.OrderByDescending(u => u.Id).Take(5).ToList(),
                LatestAssets = _context.Assets.OrderByDescending(a => a.AssetId).Take(5).ToList(),
                LatestAssignments = _context.Assignments.Include(a => a.CustomUser).Include(a => a.Asset).OrderByDescending(a => a.AssignmentDate).Take(5).ToList(),
            };

            return View(viewModel);
        }

       /*  [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
       public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult About()
        {
            return View();
        }*/
    }
}
