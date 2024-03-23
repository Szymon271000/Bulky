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
            if (categoryToAdd.Name == categoryToAdd.DisplayOrder.ToString())
            {
                ModelState.AddModelError("Name", "The Display Order cannot match the name");
            }
            if (ModelState.IsValid)
            {
                _db.Categories.Add(categoryToAdd);
                _db.SaveChanges();
                return RedirectToAction("Index", "Category");
            }
            return View();
        }
    }
}
