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
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;

        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }
       
        public IActionResult Upsert(int? id)
        {
            ProductViewModel productViewModel = new()
            {
                product = new Product(),
                categoryList = _unitOfWork.Category.GetAll().Select(i=>new SelectListItem
                    {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),
                coverTypeList = _unitOfWork.CoverType.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),

            };
            if (id == null)
            {
                //create product
               
                return View(productViewModel);
            }
            else
            {
                //update product
                productViewModel.product=_unitOfWork.Product.GetFirstOrDefault(u=>u.Id == id);
                return View(productViewModel);
            }
           

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductViewModel obj, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _hostEnvironment.WebRootPath;
                if(file != null)
                {
                    string filename = Guid.NewGuid().ToString();
                    var upload = Path.Combine(wwwRootPath, @"images\products");
                    var extension = Path.GetExtension(file.FileName);
                    if(obj.product.ImageUrl!=null)
                    {
                        var oldImagePath = Path.Combine(wwwRootPath,obj.product.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                            System.IO.File.Delete(oldImagePath);
                    }
                    using(var fileStreams = new FileStream(Path.Combine(upload,filename+extension),FileMode.Create))
                    {
                        file.CopyTo(fileStreams);
                    }
                    obj.product.ImageUrl = @"\images\products\" + filename + extension;
                }
                if(obj.product.Id==0)
                {
                    _unitOfWork.Product.Add(obj.product);
                }
                else
                {
                    _unitOfWork.Product.Update(obj.product);
                }
                _unitOfWork.Save();
                TempData["Success"] = "Product created !";
                return RedirectToAction("Index");
            }
            return View(obj);
        }
       
        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            var productList = _unitOfWork.Product.GetAll(includeProperties: "Category,CoverType");
            return Json(new { data= productList});
        }
        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var obj = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == id);
            if (obj == null)
            {
                return Json(new { success = false, message = "Erreur lors de la suppression." });
            }
            var oldImagePath = Path.Combine(_hostEnvironment.WebRootPath, obj.ImageUrl.TrimStart('\\'));
            if (System.IO.File.Exists(oldImagePath))
                System.IO.File.Delete(oldImagePath);
            _unitOfWork.Product.Remove(obj);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Suppression réussie." });
        }
        #endregion
    }
}
