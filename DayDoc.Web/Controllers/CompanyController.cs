using DayDoc.Components.Mvc;
using DayDoc.Web.Data;
using DayDoc.Web.Endpoints.Models;
using DayDoc.Web.Models;
using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HttpPostAttribute = Microsoft.AspNetCore.Mvc.HttpPostAttribute;

namespace DayDoc.Web.Controllers
{    
    public class CompanyController : _BaseAuthController
    {
        private readonly ILogger<HomeController> _logger;
        //private readonly AppDbContext _db;

        public CompanyController(ILogger<HomeController> logger, AppDbContext db)
        {
            //_db = db;
            _logger = logger;
        }

        private async Task ViewBagLoad(Company company)
        {
            //var companies = await _db.Companies.AsNoTracking()
            //    .Where(m => m.CompType == CompType.Edo)
            //    .OrderByDescending(m => m.Id)
            //    .ToListAsync();
            //var edoCompanySL = new SelectList(companies, nameof(Company.Id), nameof(Company.Name), company.EdoCompanyId);

            var res = await new CompanyListRequest { CompType = CompType.Edo }.ExecuteAsync(HttpContext.RequestAborted);
            var edoCompanySL = new SelectList(res.Companies, nameof(Company.Id), nameof(Company.Name), company.EdoCompanyId);

            ViewBag.EdoCompanyId = edoCompanySL;
        }

        // GET: CompanyController
        public async Task<ActionResult> Index()
        {
            //var companies = await _context.Companies.AsNoTracking()
            //    .Include(m => m.EdoCompany)
            //    .OrderByDescending(m => m.CompType)
            //        .ThenByDescending(m => m.Id)
            //    .ToListAsync();

            var res = await new CompanyListRequest { Filter = null }.ExecuteAsync(HttpContext.RequestAborted);
            var companies = res.Companies;

            return View(companies);
        }

        // GET: CompanyController/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var company = await _context.Companies.AsNoTracking()
            //    .Include(m => m.EdoCompany)
            //    .FirstOrDefaultAsync(m => m.Id == id);

            var res = await new CompanyGetRequest { Id = id ?? 0 }.ExecuteAsync(HttpContext.RequestAborted);
            var company = res.Company;

            if (company == null)
            {
                return NotFound();
            }

            return View(company);
        }

        // GET: CompanyController/Create
        public async Task<ActionResult> Create()
        {
            var company = new Company();
            await ViewBagLoad(company);
            return View(company);
        }

        // POST: CompanyController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([BindExcludeId] Company company)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //_context.Add(company);
                    //await _context.SaveChangesAsync();
                    var response = await new CompanyCreateRequest { Company = company }.ExecuteAsync(HttpContext.RequestAborted);

                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex.Message);

                ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists " +
                    "see your system administrator.");
            }

            await ViewBagLoad(company);
            return View(company);
        }

        // GET: CompanyController/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var company = await _context.Companies
            //    .AsNoTracking()
            //    .FirstOrDefaultAsync(m => m.Id == id);

            var res = await new CompanyGetRequest { Id = id ?? 0 }.ExecuteAsync(HttpContext.RequestAborted);
            var company = res.Company;

            if (company == null)
            {
                return NotFound();
            }

            await ViewBagLoad(company);
            return View(company);
        }

        // POST: CompanyController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, Company company)
        {
            if (id != company.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    //_context.Update(company);
                    //await _context.SaveChangesAsync();
                    var response = await new CompanyUpdateRequest { Company = company }.ExecuteAsync(HttpContext.RequestAborted);

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex.Message);

                    ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists, " +
                        "see your system administrator.");
                }
            }

            await ViewBagLoad(company);
            return View(company);
        }

        // GET: CompanyController/Delete/5
        public async Task<ActionResult> Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var company = await _db.Companies
            //    .AsNoTracking()
            //    .FirstOrDefaultAsync(m => m.Id == id);

            var res = await new CompanyGetRequest { Id = id ?? 0 }.ExecuteAsync(HttpContext.RequestAborted);
            var company = res.Company;

            if (company == null)
            {
                return NotFound();
            }

            if (saveChangesError.GetValueOrDefault())
            {
                ViewData["ErrorMessage"] =
                    "Delete failed. Try again, and if the problem persists " +
                    "see your system administrator.";
            }
            
            return View(company);
        }

        // POST: CompanyController/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            //var company = await _context.Companies.FindAsync(id);
            //if (company == null)
            //{
            //    return RedirectToAction(nameof(Index));
            //}

            try
            {
                //_context.Companies.Remove(company);
                //await _context.SaveChangesAsync();
                await new CompanyDeleteRequest { Id = id }.ExecuteAsync(HttpContext.RequestAborted);

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction(nameof(Delete), new { id = id, saveChangesError = true });
            }
        }
    }
}