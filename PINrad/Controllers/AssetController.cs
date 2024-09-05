using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using PINrad.Data;
using PINrad.Models;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PINrad.Controllers
{
    public class AssetController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AssetController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Asset
        public async Task<IActionResult> Index(string searchString)
        {
            var assets = from a in _context.Assets.Include(a => a.AssignedUser)
                         select a;

            if (!string.IsNullOrEmpty(searchString))
            {
                assets = assets.Where(s => s.Opis.Contains(searchString));
            }

            return View(await assets.ToListAsync());
        }

        // GET: Asset/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var asset = await _context.Assets
                .Include(a => a.AssignedUser)
                .FirstOrDefaultAsync(m => m.AssetId == id);
            if (asset == null)
            {
                return NotFound();
            }

            return View(asset);
        }

        // GET: Asset/Create
        public IActionResult Create()
        {
            ViewBag.Users = new SelectList(_context.CustomUsers, "CustomUserId", "FullName");
            return View();
        }

        // POST: Asset/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Asset asset)
        {
            if (ModelState.IsValid)
            {
                _context.Add(asset);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Users = new SelectList(_context.CustomUsers, "CustomUserId", "FullName", asset.UserID);
            return View(asset);
        }

        // GET: Asset/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var asset = await _context.Assets.FindAsync(id);
            if (asset == null)
            {
                return NotFound();
            }
            ViewBag.Users = new SelectList(_context.CustomUsers, "CustomUserId", "FullName", asset.UserID);
            return View(asset);
        }

        // POST: Asset/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Asset asset)
        {
            if (id != asset.AssetId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(asset);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AssetExists(asset.AssetId))
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
            ViewBag.Users = new SelectList(_context.CustomUsers, "CustomUserId", "FullName", asset.UserID);
            return View(asset);
        }

        // GET: Asset/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var asset = await _context.Assets
                .Include(a => a.AssignedUser)
                .FirstOrDefaultAsync(m => m.AssetId == id);
            if (asset == null)
            {
                return NotFound();
            }

            return View(asset);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var asset = await _context.Assets.FindAsync(id);

            if (asset != null)  // Provjeri je li asset null prije nego ga se ukloni
            {
                _context.Assets.Remove(asset);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }


        // POST: Asset/ImportAssets
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ImportAssets(IFormFile file)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            if (file != null && file.Length > 0)
            {
                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    using (var package = new ExcelPackage(stream))
                    {
                        var worksheet = package.Workbook.Worksheets[0];
                        var rowCount = worksheet.Dimension.Rows;

                        for (int row = 2; row <= rowCount; row++)
                        {
                            var asset = new Asset
                            {
                                InventarniBroj = int.Parse(worksheet.Cells[row, 1].Text),
                                Opis = worksheet.Cells[row, 2].Text,
                                KupovnaVrijednost = decimal.Parse(worksheet.Cells[row, 3].Text),
                                DatumKupnje = DateTime.Parse(worksheet.Cells[row, 4].Text),
                             //   UserID = int.Parse(worksheet.Cells[row, 5].Text)
                            };

                            _context.Assets.Add(asset);
                        }

                        await _context.SaveChangesAsync();
                    }
                }
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Asset/ExportAssets
        public async Task<IActionResult> ExportAssets()
        {
            var assets = await _context.Assets.Include(a => a.AssignedUser).ToListAsync();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Assets");

                worksheet.Cells[1, 1].Value = "Inventarni Broj";
                worksheet.Cells[1, 2].Value = "Naziv";
                worksheet.Cells[1, 3].Value = "Nabavna Vrijednost";
                worksheet.Cells[1, 4].Value = "Datum Nabave";
                worksheet.Cells[1, 5].Value = "Korisnik";

                for (int i = 0; i < assets.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = assets[i].InventarniBroj;
                    worksheet.Cells[i + 2, 2].Value = assets[i].Opis;
                    worksheet.Cells[i + 2, 3].Value = assets[i].KupovnaVrijednost;
                    worksheet.Cells[i + 2, 4].Value = assets[i].DatumKupnje.ToShortDateString();
                    worksheet.Cells[i + 2, 5].Value = assets[i].AssignedUser?.FullName;
                }

                var stream = new MemoryStream(package.GetAsByteArray());
                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Imovina.xlsx");
            }
        }

        private bool AssetExists(int id)
        {
            return _context.Assets.Any(e => e.AssetId == id);
        }
    }
}
