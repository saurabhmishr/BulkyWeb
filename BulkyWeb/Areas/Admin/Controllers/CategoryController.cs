using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Authorize(Roles = SD.Role_Admin)]
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _categoryRepo;
        public CategoryController(IUnitOfWork db)
        {
            _categoryRepo = db;
        }

        public IActionResult Index()
        {
            List<Bulky.Models.Category> objCategoryList = _categoryRepo.Category.GetAll().ToList();
            return View(objCategoryList);
        }

        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                _categoryRepo.Category.Add(category);
                _categoryRepo.Save();
                TempData["success"] = "Category created successfully.";
                return RedirectToAction("Index");
            }
            return View();

        }


        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            else
            {
                Category? category = _categoryRepo.Category.Get(u => u.Id == id);
                if (category == null)
                {
                    return NotFound();
                }
                return View(category);
            }
        }


        [HttpPost]
        public IActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                _categoryRepo.Category.Update(category);
                _categoryRepo.Save();
                TempData["success"] = "Category updated successfully.";
                return RedirectToAction("Index");
            }
            return View();
        }


        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            else
            {
                Category? category = _categoryRepo.Category.Get(u => u.Id == id);
                if (category == null)
                {
                    return NotFound();
                }
                return View(category);
            }
        }


        [HttpPost]
        public IActionResult Delete(Category category)
        {
            if (ModelState.IsValid)
            {
                _categoryRepo.Category.Remove(category);
                _categoryRepo.Save();
                TempData["success"] = "Category deleted successfully.";
                return RedirectToAction("Index");
            }
            return View();
        }
    }
}
