
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
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            IList<Company> objCategoryList = _unitOfWork.companyRepository.GetAll().ToList();
            

            return View(objCategoryList);
        }

        public IActionResult Upsert(int? id)
        {
            if (id == null || id == 0)
            {
                return View(new Company());
            }
            else
            {
                Company companyObj = _unitOfWork.companyRepository.Get(x => x.Id == id);
                return View(companyObj);
            }
        }

        [HttpPost]
        public IActionResult Upsert(Company companyToAdd)
        {
            if (ModelState.IsValid)
            {
                if (companyToAdd.Id == 0)
                {
                    _unitOfWork.companyRepository.Add(companyToAdd);
                }
                else
                {
                    _unitOfWork.companyRepository.Update(companyToAdd);
                }
                _unitOfWork.Save();
                TempData["success"] = "Category created successfully";
                return RedirectToAction("Index", "Company");
            }
            else
            {
                return View(companyToAdd);
            }
        }

       
        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            IList<Company> objCategoryList = _unitOfWork.companyRepository.GetAll().ToList();
            return Json(new {data =  objCategoryList});
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            Company CompanyToDelete = _unitOfWork.companyRepository.Get(x => x.Id ==id);
            if (CompanyToDelete == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            _unitOfWork.companyRepository.Remove(CompanyToDelete);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Delete Successful" });

        }
        #endregion
    }
}
