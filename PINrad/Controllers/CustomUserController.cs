using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Bouncycastle;
using iText.Bouncycastleconnector;
using iText.Bouncycastlefips;
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
    public class CustomUserController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CustomUserController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CustomUser customUser)
        {
         
            if (ModelState.IsValid)
            {
                _context.Add(customUser);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(customUser);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customUser = await _context.CustomUsers.FindAsync(id);

            if (customUser == null)
            {
                return NotFound();
            }

            return View(customUser);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CustomUser customUser)
        {
            if (id != customUser.CustomUserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(customUser);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomUserExists(customUser.CustomUserId))
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

            return View(customUser);
        }

        // Dodatna metoda za provjeru postoji li korisnik
        private bool CustomUserExists(int id)
        {
            return _context.CustomUsers.Any(e => e.CustomUserId == id);
        }

        public async Task<IActionResult> Index(string searchString)
        {
            var users = from u in _context.CustomUsers
                        select u;

            if (!string.IsNullOrEmpty(searchString))
            {
                users = users.Where(s => s.FullName.Contains(searchString) || s.Email.Contains(searchString));
            }

            return View(await users.ToListAsync());
        }
        public IActionResult Create()
        {
            ViewBag.Users = new SelectList(_context.CustomUsers, "CustomUserId", "PunoIme");
            return View();
        }

        public async Task<IActionResult> Report(int id)
        {
            var user = await _context.CustomUsers
                .Include(u => u.AssignedAssets)
                .FirstOrDefaultAsync(u => u.CustomUserId == id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        public async Task<IActionResult> ExportToPdf(int id)
        {
            var user = await _context.CustomUsers
                .Include(u => u.AssignedAssets)
                .FirstOrDefaultAsync(u => u.CustomUserId == id);

            if (user == null)
            {
                return NotFound();
            }

            using (var memoryStream = new MemoryStream())
            {
                var writer = new PdfWriter(memoryStream);
                var pdf = new PdfDocument(writer);
                var document = new Document(pdf);

                document.Add(new Paragraph($"Izvještaj za korisnika: {user.FullName}"));
                document.Add(new Paragraph($"Odjel: {user.Department}"));
                document.Add(new Paragraph($"Email: {user.Email}"));
                document.Add(new Paragraph(" "));

                document.Add(new Paragraph("Dodijeljena imovina:"));
                var table = new Table(4);
                table.AddCell("Inventarni Broj");
                table.AddCell("Naziv");
                table.AddCell("Nabavna Vrijednost");
                table.AddCell("Datum Nabave");

                foreach (var asset in user.AssignedAssets)
                {
                    table.AddCell(asset.InventarniBroj.ToString());
                    table.AddCell(asset.Opis);
                    table.AddCell(asset.KupovnaVrijednost.ToString());
                    table.AddCell(asset.DatumKupnje.ToShortDateString());
                }

                document.Add(table);
                document.Close();

                return File(memoryStream.ToArray(), "application/pdf", $"{user.FullName}_Izvjestaj.pdf");
            }
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customUser = await _context.CustomUsers
                .FirstOrDefaultAsync(m => m.CustomUserId == id);

            if (customUser == null)
            {
                return NotFound();
            }

            return View(customUser);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var customUser = await _context.CustomUsers.FindAsync(id);
            if (customUser != null)
            {
                _context.CustomUsers.Remove(customUser);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ImportUsers(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                using (var stream = new MemoryStream())
                {
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                    await file.CopyToAsync(stream);
                    using (var package = new ExcelPackage(stream))
                    {
                        var worksheet = package.Workbook.Worksheets[0];
                        var rowCount = worksheet.Dimension.Rows;

                        for (int row = 2; row <= rowCount; row++)
                        {
                            var user = new CustomUser
                            {
                                FullName = worksheet.Cells[row, 1].Text,
                                Department = worksheet.Cells[row, 2].Text,
                                Email = worksheet.Cells[row, 3].Text
                            };

                            _context.CustomUsers.Add(user);
                        }

                        await _context.SaveChangesAsync();
                    }
                }
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ExportUsers()
        {
            var users = await _context.CustomUsers.ToListAsync();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Users");

                worksheet.Cells[1, 1].Value = "Ime";
                worksheet.Cells[1, 2].Value = "Odjel";
                worksheet.Cells[1, 3].Value = "Email";

                for (int i = 0; i < users.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = users[i].FullName;
                    worksheet.Cells[i + 2, 2].Value = users[i].Department;
                    worksheet.Cells[i + 2, 3].Value = users[i].Email;
                }

                var stream = new MemoryStream(package.GetAsByteArray());
                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Korisnici.xlsx");
            }
        }
    }
}
