using DayDoc.Components.Mvc;
using DayDoc.Web.Data;
using DayDoc.Web.Models;
using Microsoft.AspNetCore.Http;
using FastEndpoints;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using HttpPostAttribute = Microsoft.AspNetCore.Mvc.HttpPostAttribute;
using DayDoc.Web.Endpoints.Models;

namespace DayDoc.Web.Controllers
{
    public class DocController : _BaseAuthController
    {
        private readonly ILogger<HomeController> _logger;

        public DocController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        private async Task ViewBagLoad(Doc doc)
        {
            //var owns = await _context.Companies.AsNoTracking()
            //    .Where(m => m.CompType == CompType.Own)
            //    .OrderByDescending(m => m.Id)
            //    .ToListAsync();

            //var customers = await _context.Companies.AsNoTracking()
            //    .Where(m => m.CompType == CompType.Customer)
            //    .OrderByDescending(m => m.Id)
            //    .ToListAsync();

            var ownResp = await new CompanyListRequest { CompType = CompType.Own }.ExecuteAsync(HttpContext.RequestAborted);
            var ownSL = new SelectList(ownResp.Companies, nameof(Company.Id), nameof(Company.Name), doc.OwnCompanyId);
            ViewBag.OwnCompanyId = ownSL;

            var customerResp = await new CompanyListRequest { CompType = CompType.Customer }.ExecuteAsync(HttpContext.RequestAborted);
            var contagentSL = new SelectList(customerResp.Companies, nameof(Company.Id), nameof(Company.Name), doc.ContragentId);            
            ViewBag.ContragentId = contagentSL;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateXML(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            string? errorMessage = null;

            if (ModelState.IsValid)
            {
                try
                {
                    await new XmlDocCreateRequest { DocId = id ?? 0}.ExecuteAsync(HttpContext.RequestAborted);
                    
                    //errorMessage = "OK!";
                    //return RedirectToAction(nameof(Details), new { id, errorMessage });
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex.Message);

                    errorMessage = "Unable to save changes. " +
                        "Try again, and if the problem persists, " +
                        "see your system administrator.";
                }
                catch(ArgumentNullException ex)
                {
                    errorMessage = ex.Message;
                }
            }
            else
            {
                errorMessage = "ModelState invalid!";
            }

            return RedirectToAction(nameof(Details), new { id, errorMessage });
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<ActionResult> DeleteXML(int? id, int? xmlDocId)
        {
            string? errorMessage = null;

            if (id == null || xmlDocId == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {

                //var xmlDoc = await _db.XmlDocs.FindAsync(xmlDocId);
                //if (xmlDoc == null || xmlDoc.DocId != id)
                //{
                //    return NotFound();
                //}

                try
                {
                    //_db.Remove(xmlDoc);
                    //await _db.SaveChangesAsync();
                    //string xmlFile = GetXmlFilePath(xmlDoc.DocId, xmlDoc.FileName);
                    //System.IO.File.Delete(xmlFile);
                    await new XmlDocDeleteRequest { DocId = id ?? 0, Id = xmlDocId ?? 0 }.ExecuteAsync(HttpContext.RequestAborted);

                    //errorMessage = "Deleted!";
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex.Message);

                    errorMessage =
                        "Delete failed. Try again, and if the problem persists " +
                        "see your system administrator.";
                }
                catch(IOException ex)
                {
                    _logger.LogError(ex.Message);
                    errorMessage = ex.Message;
                }
            }
            else
            {
                errorMessage = "ModelState invalid!";
            }
           
            return RedirectToAction(nameof(Details), new { id, errorMessage });
        }

        public async Task<ActionResult> GetXML(int? id, int? xmlDocId)
        {
            string? errorMessage = null;

            if (id == null || xmlDocId == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                //var xmlDoc = await _db.XmlDocs.FindAsync(xmlDocId);
                //if (xmlDoc == null || xmlDoc.DocId != id)
                //{
                //    return NotFound();
                //}
                //string filePath = GetXmlFilePath(xmlDoc.DocId, xmlDoc.FileName);
                //string fileName = Path.GetFileName(xmlFile);
                //string contentType = "application/xml";

                var file = await new XmlDocGetFileRequest { Id = xmlDocId ?? 0}.ExecuteAsync(HttpContext.RequestAborted);
                if (file == null || file.FilePath == null)
                {
                    return NotFound();
                }

                return PhysicalFile(file.FilePath, file.ContentType, file.FileName);
            }
            else
            {
                errorMessage = "ModelState invalid!";
            }

            return RedirectToAction(nameof(Details), new { id, errorMessage });
        }

        // GET: DocController
        public async Task<ActionResult> Index()
        {
            //var docs = await _db.Docs.AsNoTracking()
            //    .Include(m => m.OwnCompany)
            //    .Include(m => m.Contragent)
            //    .OrderByDescending(m => m.Id)
            //    .ToListAsync();

            var res = await new DocListRequest { Filter = null }.ExecuteAsync(HttpContext.RequestAborted);
            var docs = res.Docs;

            return View(docs);
        }

        // GET: DocController/Details/5
        public async Task<ActionResult> Details(int? id, string? errorMessage)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var doc = await _db.Docs.AsNoTracking()
            //    .Include(m => m.OwnCompany)
            //    .Include(m => m.Contragent)
            //    .Include(m => m.XmlDocs)
            //    .FirstOrDefaultAsync(m => m.Id == id);

            var resp = await new DocGetRequest { Id = id ?? 0 }.ExecuteAsync(HttpContext.RequestAborted);
            var doc = resp.Doc;

            if (doc == null)
            {
                return NotFound();
            }

            var xmlResp = await new XmlDocListRequest { DocId = id ?? 0 }.ExecuteAsync(HttpContext.RequestAborted);
            doc.XmlDocs = xmlResp.XmlDocs ?? new();

            ViewData["ErrorMessage"] = errorMessage;
            return View(doc);
        }

        // GET: DocController/Create
        public async Task<ActionResult> Create()
        {
            //var setting = await _db.Settings.AsNoTracking()
            //    .Select(m => new { m.OwnCompanyId, m.WorkName }).FirstOrDefaultAsync();

            var resp = await new SettingGetDefaultRequest { }.ExecuteAsync(HttpContext.RequestAborted);
            var setting = resp.Setting;

            Doc doc = new() { OwnCompanyId = setting?.OwnCompanyId ?? 0, WorkName = setting?.WorkName };

            await ViewBagLoad(doc);
            return View(doc);
        }

        // POST: DocController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([BindExcludeId] Doc doc)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //_db.Add(doc);
                    //await _db.SaveChangesAsync();

                    var response = await new DocsCreateRequest { Doc = doc }.ExecuteAsync(HttpContext.RequestAborted);

                    //return RedirectToAction(nameof(Index));
                    return RedirectToAction(nameof(Details), new { id = doc.Id });
                }
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex.Message);

                ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists " +
                    "see your system administrator.");
            }

            await ViewBagLoad(doc);
            return View(doc);
        }

        // GET: DocController/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var doc = await _db.Docs
            //    .AsNoTracking()
            //    .FirstOrDefaultAsync(m => m.Id == id);

            var res = await new DocGetRequest { Id = id ?? 0 }.ExecuteAsync(HttpContext.RequestAborted);
            var doc = res.Doc;

            if (doc == null)
            {
                return NotFound();
            }

            await ViewBagLoad(doc);
            return View(doc);
        }

        // POST: DocController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, Doc doc)
        {
            if (id != doc.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    //_db.Update(doc);
                    //await _db.SaveChangesAsync();
                    var response = await new DocUpdateRequest { Doc = doc }.ExecuteAsync(HttpContext.RequestAborted);

                    //return RedirectToAction(nameof(Index));
                    return RedirectToAction(nameof(Details), new { id = doc.Id });
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex.Message);

                    ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists, " +
                        "see your system administrator.");
                }
            }

            await ViewBagLoad(doc);
            return View(doc);
        }

        // GET: DocController/Delete/5
        public async Task<ActionResult> Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var doc = await _db.Docs.AsNoTracking()
            //    .Include(m => m.OwnCompany)
            //    .Include(m => m.Contragent)
            //    .FirstOrDefaultAsync(m => m.Id == id);

            var res = await new DocGetRequest { Id = id ?? 0 }.ExecuteAsync(HttpContext.RequestAborted);
            var doc = res.Doc;

            if (doc == null)
            {
                return NotFound();
            }

            if (saveChangesError.GetValueOrDefault())
            {
                ViewData["ErrorMessage"] =
                    "Delete failed. Try again, and if the problem persists " +
                    "see your system administrator.";
            }

            return View(doc);
        }

        // POST: DocController/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            //var doc = await _db.Docs.FindAsync(id);
            //if (doc == null)
            //{
            //    return RedirectToAction(nameof(Index));
            //}

            try
            {
                //_db.Docs.Remove(doc);
                //await _db.SaveChangesAsync();
                await new DocDeleteRequest { Id = id }.ExecuteAsync(HttpContext.RequestAborted);

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
