
using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;

        }
        public IActionResult Index()
        {
            IList<Product> objCategoryList = _unitOfWork.productRepository.GetAll().ToList();
            

            return View(objCategoryList);
        }

        public IActionResult Upsert(int? id)
        {
            IEnumerable<SelectListItem> CategoryList = _unitOfWork.categoryRepository.GetAll().Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString()
            });

            ProductViewModel productVM = new()
            {
                CategoryList = CategoryList,
                Product = new Product()
            };

            if (id == null || id == 0)
            {
                return View(productVM);
            }
            else
            {
                productVM.Product = _unitOfWork.productRepository.Get(x => x.Id == id);
                return View(productVM);
            }
        }

        [HttpPost]
        public IActionResult Upsert(ProductViewModel productToAdd, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwRootPath, @"images\product");

                    if (!string.IsNullOrEmpty(productToAdd.Product.ImageUrl))
                    {
                        var oldImagePath = 
                            Path.Combine(wwwRootPath, productToAdd.Product.ImageUrl.TrimStart('\\'));

                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                    productToAdd.Product.ImageUrl = @"\images\product\" + fileName;
                }

                if (productToAdd.Product.Id == 0)
                {
                    _unitOfWork.productRepository.Add(productToAdd.Product);
                }
                else
                {
                    _unitOfWork.productRepository.Update(productToAdd.Product);
                }
                _unitOfWork.Save();
                TempData["success"] = "Category created successfully";
                return RedirectToAction("Index", "Product");
            }
            else
            {
                productToAdd.CategoryList = _unitOfWork.categoryRepository.GetAll().Select(x => new SelectListItem()
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                });

                return View(productToAdd);
            }
        }

       

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Product? productFromDb = _unitOfWork.productRepository.Get(x => x.Id == id);
            if (productFromDb == null)
            {
                return NotFound();
            }
            return View(productFromDb);
        }

        [HttpPost, ActionName("Delete")]

        public IActionResult DeletePOST(int? id)
        {
            Product? productFromDb = _unitOfWork.productRepository.Get(x => x.Id == id);
            if (productFromDb == null)
            {
                return NotFound();
            }
            _unitOfWork.productRepository.Remove(productFromDb);
            _unitOfWork.Save();
            TempData["success"] = "Category deleted successfully";
            return RedirectToAction("Index");
        }
    }
}
