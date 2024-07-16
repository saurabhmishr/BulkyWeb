using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Authorize(Roles =SD.Role_Admin)]
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _ProductRepo;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork db, IWebHostEnvironment webHostEnvironment)
        {
            _ProductRepo = db;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            List<Bulky.Models.Product> objProductList = _ProductRepo.Product.GetAll(includeProperties: "category").ToList();
            return View(objProductList);
        }

        public IActionResult Upsert(int? id)
        {
            IEnumerable<SelectListItem> CategoryList = _ProductRepo.Category
                .GetAll().Select(u => new SelectListItem
                {
                    Text = u.name,
                    Value = u.Id.ToString()
                });
            //ViewBag.CategoryList = CategoryList;


            ProductVM productVM = new ProductVM
            {
                CategoryList = CategoryList,
                product = new Product()
            };

            if (id == null || id == 0)
            {
                return View(productVM);
            }
            else
            {
                //update
                productVM.product = _ProductRepo.Product.Get(u => u.Id == id);
                return View(productVM);
            }

        }


        [HttpPost]
        public IActionResult Upsert(ProductVM obj, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;

                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwRootPath, @"images\product\");

                    if (!string.IsNullOrEmpty(obj.product.ImageUrl))
                    {
                        //Delete the old image
                        var oldImagePath = Path.Combine(wwwRootPath, obj.product.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    obj.product.ImageUrl = @"\images\product\" + fileName;
                }
                if (obj.product.Id == 0)
                {
                    _ProductRepo.Product.Add(obj.product);
                }
                else
                {
                    _ProductRepo.Product.Update(obj.product);
                }

                _ProductRepo.Save();
                TempData["success"] = "Product created successfully.";
                return RedirectToAction("Index");
            }
            return View();

        }




        //#region old delete logic
        //public IActionResult Delete(int? id)
        //{
        //    if (id == null || id == 0)
        //    {
        //        return NotFound();
        //    }
        //    else
        //    {
        //        Product? Product = _ProductRepo.Product.Get(u => u.Id == id);
        //        if (Product == null)
        //        {
        //            return NotFound();
        //        }
        //        return View(Product);
        //    }
        //}
        //[HttpPost]
        //public IActionResult Delete(Product Product)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _ProductRepo.Product.Remove(Product);
        //        _ProductRepo.Save();
        //        TempData["success"] = "Product deleted successfully.";
        //        return RedirectToAction("Index");
        //    }
        //    return View();
        //}
        //#endregion

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Bulky.Models.Product> objProductList = _ProductRepo.Product.GetAll(includeProperties: "category").ToList();
            return Json(new { data = objProductList });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var productToBeDeleted = _ProductRepo.Product.Get(u => u.Id == id);
            if (productToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, productToBeDeleted.ImageUrl.TrimStart('\\'));
            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }

            _ProductRepo.Product.Remove(productToBeDeleted);
            _ProductRepo.Save();

            return Json(new { success = true, message = "Deleted successfully." });
        }
        #endregion
    }
}
