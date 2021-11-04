using eCommerce.DataAccess.Data.Repository.IRepository;
using eCommerce.Models;
using eCommerce.Models.ViewModels;
using eCommerce.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace eCommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        public OrderController(IUnitOfWork _unitOfWork)
        {
            unitOfWork = _unitOfWork;
        }

        [HttpGet]
        [Authorize]
        public IActionResult Get(string status=null)
        {
            List<OrderDetailsVM> orderListVM = new List<OrderDetailsVM>();

            IEnumerable<OrderHeader> orderHeaders;
            if (User.IsInRole(SD.CustomerRole))
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                orderHeaders = unitOfWork.OrderHeaderRepository.GetAll(s => s.ApplicationUserID == claim.Value, null, "ApplicationUser");
            }
            else
            {

                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                orderHeaders = unitOfWork.OrderHeaderRepository.GetAll(null, null, "ApplicationUser");
            }
            if (status == "cancelled")
            {
                orderHeaders = orderHeaders.Where(s => s.Status == SD.StatusCancelled || s.Status == SD.StatusRefunded || s.Status==SD.PaymentStatusRejected);
            }
            else if (status == "completed")
            {
                orderHeaders = orderHeaders.Where(s => s.Status == SD.StatusCompleted);
            }
            else
            {
                orderHeaders = orderHeaders.Where(s => s.Status == SD.StatusInProccess || s.Status == SD.StatusReady || s.Status==SD.StatusSubmitted || s.Status==SD.PaymentStatusPending);
            }            
            foreach (OrderHeader item in orderHeaders)
            {
                OrderDetailsVM individual = new OrderDetailsVM
                {
                    OrderHeader = item,
                    OrderDetailsList = unitOfWork.OrderDetailsRepository.GetAll(s => s.OrderHeaderID == item.OrderHeaderID).ToList()

                };
                orderListVM.Add(individual);
            }
            return Json(new { data = orderListVM });
        }
    }
}
