using BulkyWeb.Data;
using BulkyWeb.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;
        public CategoryController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            IList<Category> objCategoryList = _db.Categories.ToList(); 
            return View(objCategoryList);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category categoryToAdd)
        {
            _db.Categories.Add(categoryToAdd);
            _db.SaveChanges();
            return RedirectToAction("Index", "Category");
        }
    }
}
