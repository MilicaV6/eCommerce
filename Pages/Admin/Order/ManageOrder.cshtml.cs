using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eCommerce.DataAccess.Data.Repository.IRepository;
using eCommerce.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using eCommerce.Models;
using eCommerce.Utility;
using Stripe;

namespace eCommerce.Pages.Admin.Order
{
    public class ManageOrderModel : PageModel
    {
        private readonly IUnitOfWork unitOfWork;
        public ManageOrderModel(IUnitOfWork _unitOfWork)
        {
            unitOfWork = _unitOfWork;
        }
        [BindProperty]
        public List<OrderDetailsVM> orderDetailsListVM { get; set; }

 
        public void OnGet(string status = null)
        {

            orderDetailsListVM = new List<OrderDetailsVM>();
            List<OrderHeader> orderHeaders = unitOfWork.OrderHeaderRepository.GetAll
                (s=>s.Status==SD.StatusSubmitted || s.Status==SD.StatusInProccess).OrderByDescending(s=>s.DeliveryTime).ToList();
                  foreach (OrderHeader item in orderHeaders)
            {
                OrderDetailsVM individual = new OrderDetailsVM
                {
                    OrderHeader = item,
                    OrderDetailsList = unitOfWork.OrderDetailsRepository.GetAll(s => s.OrderHeaderID == item.OrderHeaderID).ToList()

                };
                orderDetailsListVM.Add(individual);
            }
        } 
        public IActionResult OnPostOrderPrepare(int OrderHeaderID)
        {
            OrderHeader orderHeader = unitOfWork.OrderHeaderRepository.GetFirstOrDefault(s => s.OrderHeaderID == OrderHeaderID);
            orderHeader.Status = SD.StatusInProccess;
            unitOfWork.OrderHeaderRepository.Update(orderHeader);
            unitOfWork.Save();
            return RedirectToPage("ManageOrder");

        }

        public IActionResult OnPostOrderReady(int OrderHeaderID)
        {
            OrderHeader orderHeader = unitOfWork.OrderHeaderRepository.GetFirstOrDefault(s => s.OrderHeaderID == OrderHeaderID);
            orderHeader.Status = SD.StatusReady;
            unitOfWork.OrderHeaderRepository.Update(orderHeader);
            unitOfWork.Save();
            return RedirectToPage("ManageOrder");

        }

        public IActionResult OnPostOrderCancel(int OrderHeaderID)
        {
            OrderHeader orderHeader = unitOfWork.OrderHeaderRepository.GetFirstOrDefault(s => s.OrderHeaderID == OrderHeaderID);
            orderHeader.Status = SD.StatusCancelled;
            unitOfWork.OrderHeaderRepository.Update(orderHeader);
            unitOfWork.Save();
            return RedirectToPage("ManageOrder");

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
            return RedirectToPage("ManageOrder");

        }
    }
}
