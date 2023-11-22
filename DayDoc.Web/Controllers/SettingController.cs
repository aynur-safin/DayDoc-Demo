using DayDoc.Components.Mvc;
using DayDoc.Web.Data;
using DayDoc.Web.Endpoints.Models;
using DayDoc.Web.Models;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HttpPostAttribute = Microsoft.AspNetCore.Mvc.HttpPostAttribute;

namespace DayDoc.Web.Controllers
{
    public class SettingController : _BaseAuthController
    {
        private readonly ILogger<HomeController> _logger;
        //private readonly AppDbContext _db;

        public SettingController(ILogger<HomeController> logger/*, AppDbContext db*/)
        {
            //_db = db;
            _logger = logger;
        }

        private async Task ViewBagLoad(Setting setting)
        {
            //var companies = await _db.Companies.AsNoTracking()
            //    .Where(m => m.CompType == CompType.Own)
            //    .ToListAsync();
            //var ownSL = new SelectList(companies, nameof(Company.Id), nameof(Company.Name), setting.OwnCompanyId);

            var ownResp = await new CompanyListRequest { CompType = CompType.Own }.ExecuteAsync(HttpContext.RequestAborted);
            var ownSL = new SelectList(ownResp.Companies, nameof(Company.Id), nameof(Company.Name), setting.OwnCompanyId);
            ViewBag.OwnCompanyId = ownSL;
        }

        // GET: SettingController
        public async Task<ActionResult> Index()
        {
            //var settings = await _db.Settings.AsNoTracking()
            //    .Include(m => m.OwnCompany)
            //    .ToListAsync();

            var res = await new SettingListRequest { Filter = null }.ExecuteAsync(HttpContext.RequestAborted);
            var settings = res.Settings;

            return View(settings);
        }

        // GET: SettingController/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var setting = await _db.Settings
            //    .AsNoTracking()
            //    .Include(m => m.OwnCompany)
            //    .FirstOrDefaultAsync(m => m.Id == id);

            var resp = await new SettingGetRequest { Id = id ?? 0 }.ExecuteAsync(HttpContext.RequestAborted);
            var setting = resp.Setting;

            if (setting == null)
            {
                return NotFound();
            }

            return View(setting);
        }

        // GET: SettingController/Create
        public async Task<ActionResult> Create()
        {
            var setting = new Setting();
            await ViewBagLoad(setting);
            return View(setting);
        }

        // POST: SettingController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([BindExcludeId] Setting setting)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //_db.Add(setting);
                    //await _db.SaveChangesAsync();

                    var response = await new SettingCreateRequest { Setting = setting }.ExecuteAsync(HttpContext.RequestAborted);

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

            await ViewBagLoad(setting);
            return View(setting);
        }

        // GET: SettingController/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var setting = await _db.Settings
            //    .AsNoTracking()
            //    .FirstOrDefaultAsync(m => m.Id == id);

            var resp = await new SettingGetRequest { Id = id ?? 0 }.ExecuteAsync(HttpContext.RequestAborted);
            var setting = resp.Setting;

            if (setting == null)
            {
                return NotFound();
            }

            await ViewBagLoad(setting);
            return View(setting);
        }

        // POST: SettingController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, Setting setting)
        {
            if (id != setting.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    //_db.Update(setting);
                    //await _db.SaveChangesAsync();

                    var response = await new SettingUpdateRequest { Setting = setting }.ExecuteAsync(HttpContext.RequestAborted);

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

            await ViewBagLoad(setting);
            return View(setting);
        }

        // GET: SettingController/Delete/5
        public async Task<ActionResult> Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var setting = await _db.Settings
            //    .AsNoTracking()
            //    .Include(m => m.OwnCompany)
            //    .FirstOrDefaultAsync(m => m.Id == id);

            var resp = await new SettingGetRequest { Id = id ?? 0 }.ExecuteAsync(HttpContext.RequestAborted);
            var setting = resp.Setting;

            if (setting == null)
            {
                return NotFound();
            }

            if (saveChangesError.GetValueOrDefault())
            {
                ViewData["ErrorMessage"] =
                    "Delete failed. Try again, and if the problem persists " +
                    "see your system administrator.";
            }

            return View(setting);
        }

        // POST: SettingController/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            //var setting = await _db.Settings.FindAsync(id);
            //if (setting == null)
            //{
            //    return RedirectToAction(nameof(Index));
            //}

            try
            {
                //_db.Settings.Remove(setting);
                //await _db.SaveChangesAsync();

                await new SettingDeleteRequest { Id = id }.ExecuteAsync(HttpContext.RequestAborted);

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
