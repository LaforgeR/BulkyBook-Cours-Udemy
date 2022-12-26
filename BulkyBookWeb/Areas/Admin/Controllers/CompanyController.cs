using BulkyBook.DataAccess;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;

        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }
       
        public IActionResult Upsert(int? id)
        {
            Company company = new();
            if (id == null)
            {
               
                return View(company);
            }
            else
            {
                company =_unitOfWork.Company.GetFirstOrDefault(u=>u.Id == id);
                return View(company);
            }
           

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Company obj, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
               
                if(obj.Id==0)
                {
                    _unitOfWork.Company.Add(obj);
                    TempData["Success"] = "Company created !";
                }
                else
                {
                    _unitOfWork.Company.Update(obj);
                    TempData["Success"] = "Company updated !";
                }
                _unitOfWork.Save();
                return RedirectToAction("Index");
            }
            return View(obj);
        }
       
        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            var companyList = _unitOfWork.Company.GetAll();
            return Json(new { data= companyList });
        }
        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var obj = _unitOfWork.Company.GetFirstOrDefault(u => u.Id == id);
            if (obj == null)
            {
                return Json(new { success = false, message = "Erreur lors de la suppression." });
            }
            
            _unitOfWork.Company.Remove(obj);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Suppression réussie." });
        }
        #endregion
    }
}
