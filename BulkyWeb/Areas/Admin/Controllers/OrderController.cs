using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("admin")]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public OrderVm OrderVm { get; set; }
        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(int id)
        {
            OrderVm = new()
            {
                orderHeader = _unitOfWork.orderHeaderRepository.Get(x => x.Id == id, includeProperties: "ApplicationUser"),
                orderDetail = _unitOfWork.orderDetailRepository.GetAll(x => x.OrderHeaderId == id, includeProperties: "Product")
            };
            return View(OrderVm);
        }
        [HttpPost]
        [Authorize(Roles =SD.Role_Admin +"," + SD.Role_Employee)]
        public IActionResult UpdateOrderDetails()
        {
            var orderHeaderFromDb = _unitOfWork.orderHeaderRepository.Get(x => x.Id == OrderVm.orderHeader.Id);

            orderHeaderFromDb.Name = OrderVm.orderHeader.Name;
            orderHeaderFromDb.PhoneNumber = OrderVm.orderHeader.PhoneNumber;
            orderHeaderFromDb.StreetAddress = OrderVm.orderHeader.StreetAddress;
            orderHeaderFromDb.City = OrderVm.orderHeader.City;
            orderHeaderFromDb.State = OrderVm.orderHeader.State;
            orderHeaderFromDb.PostalCode = OrderVm.orderHeader.PostalCode;

            if (!string.IsNullOrEmpty(OrderVm.orderHeader.Carrier))
            {
                orderHeaderFromDb.Carrier = OrderVm.orderHeader.Carrier;
            }
            if (!string.IsNullOrEmpty(OrderVm.orderHeader.TrackingNumber))
            {
                orderHeaderFromDb.Carrier = OrderVm.orderHeader.TrackingNumber;
            }
            _unitOfWork.orderHeaderRepository.Update(orderHeaderFromDb);
            _unitOfWork.Save();
            TempData["Success"] = "Order Details Updated Successfully."; 

            return RedirectToAction(nameof(Details), new {id = orderHeaderFromDb.Id});
        }
        #region API CALLS
        [HttpGet]
        public IActionResult GetAll(string status)
        {
            IEnumerable<OrderHeader> objOrderHeaders = _unitOfWork.orderHeaderRepository.GetAll(includeProperties: "ApplicationUser").ToList();

            switch (status)
            {
                case "pending":
                    objOrderHeaders = objOrderHeaders.Where(x => x.PaymentStatus == SD.StatusPending);
                    break;
                case "inprocess":
                    objOrderHeaders = objOrderHeaders.Where(x => x.PaymentStatus == SD.StatusInProgress);
                    break;
                case "completed":
                    objOrderHeaders = objOrderHeaders.Where(x => x.PaymentStatus == SD.StatusShipped);
                    break;
                case "approved":
                    objOrderHeaders = objOrderHeaders.Where(x => x.PaymentStatus == SD.StatusApproved || x.PaymentStatus == SD.PaymentStatusDelayedPayment);
                    break;
                default:
                    break;
            }
            return Json(new { data = objOrderHeaders });
        }
        #endregion

    }
}
