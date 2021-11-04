using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eCommerce.DataAccess.Data.Repository.IRepository;
using eCommerce.Models;
using eCommerce.Models.ViewModels;
using eCommerce.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Stripe;

namespace eCommerce.Pages.Admin.Order
{
    public class OrderDetailsModel : PageModel
    {
        private readonly IUnitOfWork unitOfWork;

        public OrderDetailsModel(IUnitOfWork _unitOfWork)
        {
            unitOfWork = _unitOfWork;
        }

        [BindProperty]
        public OrderDetailsVM OrderDetailsVM { get; set; }

        public void OnGet(int orderHeaderID)
        {
            OrderDetailsVM = new OrderDetailsVM()
            {
                OrderHeader = unitOfWork.OrderHeaderRepository.GetFirstOrDefault(s => s.OrderHeaderID == orderHeaderID),
                OrderDetailsList = unitOfWork.OrderDetailsRepository.GetAll(s => s.OrderHeaderID == orderHeaderID).ToList()
            };

            OrderDetailsVM.OrderHeader.ApplicationUser = unitOfWork.ApplicationUserRepository.GetFirstOrDefault(s => s.Id == OrderDetailsVM.OrderHeader.ApplicationUserID);
        }

        public IActionResult OnPostOrderConfirm(int OrderHeaderID)
        {
            OrderHeader orderHeader = unitOfWork.OrderHeaderRepository.GetFirstOrDefault(s => s.OrderHeaderID == OrderHeaderID);
            orderHeader.Status = SD.StatusCompleted;
            unitOfWork.OrderHeaderRepository.Update(orderHeader);
            unitOfWork.Save();
            return RedirectToPage("OrderList");

        }

        public IActionResult OnPostOrderCancel(int OrderHeaderID)
        {
            OrderHeader orderHeader = unitOfWork.OrderHeaderRepository.GetFirstOrDefault(s => s.OrderHeaderID == OrderHeaderID);
            orderHeader.Status = SD.StatusCancelled;
            unitOfWork.OrderHeaderRepository.Update(orderHeader);
            unitOfWork.Save();
            return RedirectToPage("OrderList");

        }

        public IActionResult OnPostOrderRefund(int OrderHeaderID)
        {
            OrderHeader orderHeader = unitOfWork.OrderHeaderRepository.GetFirstOrDefault(s => s.OrderHeaderID == OrderHeaderID);

            var options = new RefundCreateOptions
            {
                Charge = orderHeader.TransactionID,
                Amount = Convert.ToInt32(orderHeader.OrderTotal * 100),
                Reason = RefundReasons.RequestedByCustomer,

            };
            var service = new RefundService();
            service.Create(options);


            orderHeader.Status = SD.StatusRefunded;
            unitOfWork.OrderHeaderRepository.Update(orderHeader);
            unitOfWork.Save();
            return RedirectToPage("OrderList");

        }
    }
}
