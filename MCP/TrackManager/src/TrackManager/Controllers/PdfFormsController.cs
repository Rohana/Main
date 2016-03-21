using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.Data.Entity;
using TrackManager.Models;
using Microsoft.Framework.Runtime;using Microsoft.AspNet.Hosting;using System.Collections.Generic;

namespace TrackManager.Controllers
{
    public class PdfFormsController : Controller
    {
        private ApplicationDbContext _context;
        private PdfForm _pdfForm;
        private readonly IHostingEnvironment _hostingEnvironment;

        public PdfFormsController(ApplicationDbContext context, IHostingEnvironment env)
        {
            _context = context;
            _hostingEnvironment = env;
            _pdfForm = new PdfForm();
        }

        // GET: PdfForms
        public  IActionResult Index()
        {
            ViewData["Message"] = "Trying to Auto fill";
            return View();
        }

        // GET: PdfForms/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            PdfForm pdfForm = await _context.PdfForm.SingleAsync(m => m.Id == id);
            if (pdfForm == null)
            {
                return HttpNotFound();
            }

            return View(pdfForm);
        }

        // GET: PdfForms/Create
     
        public IActionResult Create()
        {
            var formValues = new Dictionary<string, object>();

         //   _pdfForm.AutoFill(_hostingEnvironment.WebRootPath, formValues);
            return RedirectToAction("Index");
         //   return View();
        }
        
        // POST: PdfForms/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PdfForm pdfForm)
        {
            
            if (ModelState.IsValid)
            {
                string error;
                _pdfForm.AutoFill(_hostingEnvironment.WebRootPath, pdfForm,out error);
                ViewData["Message"] = error;
            }
            return View(pdfForm);
        }
        
        // GET: PdfForms/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            PdfForm pdfForm = await _context.PdfForm.SingleAsync(m => m.Id == id);
            if (pdfForm == null)
            {
                return HttpNotFound();
            }
            return View(pdfForm);
        }

        // POST: PdfForms/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PdfForm pdfForm)
        {
            if (ModelState.IsValid)
            {
                _context.Update(pdfForm);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(pdfForm);
        }

        // GET: PdfForms/Delete/5
        [ActionName("Delete")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            PdfForm pdfForm = await _context.PdfForm.SingleAsync(m => m.Id == id);
            if (pdfForm == null)
            {
                return HttpNotFound();
            }

            return View(pdfForm);
        }

        // POST: PdfForms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            PdfForm pdfForm = await _context.PdfForm.SingleAsync(m => m.Id == id);
            _context.PdfForm.Remove(pdfForm);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
