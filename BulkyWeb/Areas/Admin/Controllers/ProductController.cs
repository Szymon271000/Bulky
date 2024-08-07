﻿
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
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
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
            IList<Product> objCategoryList = _unitOfWork.productRepository.GetAll(includeProperties: "Category").ToList();
            

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

       
        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            IList<Product> objCategoryList = _unitOfWork.productRepository.GetAll(includeProperties: "Category").ToList();
            return Json(new {data =  objCategoryList});
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            Product productToDelete = _unitOfWork.productRepository.Get(x => x.Id ==id);
            if (productToDelete == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            string oldImagePath =
            Path.Combine(_webHostEnvironment.WebRootPath, 
            productToDelete.ImageUrl.TrimStart('\\'));

            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }

            _unitOfWork.productRepository.Remove(productToDelete);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Delete Successful" });

        }
        #endregion
    }
}
