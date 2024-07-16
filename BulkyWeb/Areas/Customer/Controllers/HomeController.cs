using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BulkyWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _ProductRepo;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public HomeController(IUnitOfWork db, IWebHostEnvironment webHostEnvironment)
        {
            _ProductRepo = db;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> objProductList = _ProductRepo.Product.GetAll(includeProperties: "category").ToList();
            return View(objProductList);
        }

        public IActionResult Details(int? id)
        {
            Product product = _ProductRepo.Product.Get(u=>u.Id==id, includeProperties: "category");
            return View(product);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}