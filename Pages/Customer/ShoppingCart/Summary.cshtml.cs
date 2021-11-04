using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using eCommerce.DataAccess.Data.Repository.IRepository;
using eCommerce.Models;
using eCommerce.Models.ViewModels;
using eCommerce.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Stripe;

namespace eCommerce.Pages.Customer.ShoppingCart
{
    public class SummaryModel : PageModel
    {
        private readonly IUnitOfWork unitOfWork;
        public SummaryModel(IUnitOfWork _unitOfWork)
        {
            unitOfWork = _unitOfWork;
        }
        [BindProperty]
        public OrderDetailsCart OrderDetailsCartVM { get; set; }
        public IActionResult OnGet()
        {
            OrderDetailsCartVM = new OrderDetailsCart()
            {
                OrderHeader = new OrderHeader()
            };
            OrderDetailsCartVM.OrderHeader.OrderTotal = 0;
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            IEnumerable<Models.ShoppingCart> shoppingCarts = unitOfWork.ShoppingCartRepository.GetAll(s => s.ApplicationUserID == claim.Value);

            if (shoppingCarts != null)
            {
                OrderDetailsCartVM.listCart = shoppingCarts.ToList();
            }
            foreach (var cartLis in OrderDetailsCartVM.listCart)
            {
                cartLis.Product = unitOfWork.ProductRepository.GetFirstOrDefault(s => s.ProductID == cartLis.ProductID);
                OrderDetailsCartVM.OrderHeader.OrderTotal += (cartLis.Product.Cost * cartLis.Count);
            }
            ApplicationUser applicationUser = unitOfWork.ApplicationUserRepository.GetFirstOrDefault(s => s.Id == claim.Value);
            OrderDetailsCartVM.OrderHeader.DeliveryName = applicationUser.FullName;
            OrderDetailsCartVM.OrderHeader.DeliveryTime = DateTime.Now;
            OrderDetailsCartVM.OrderHeader.PhoneNumber = applicationUser.PhoneNumber;
            return Page();

        }
        public IActionResult OnPost(string stripeToken)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            OrderDetailsCartVM.listCart = unitOfWork.ShoppingCartRepository.GetAll(s => s.ApplicationUserID == claim.Value).ToList();

            OrderDetailsCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
            OrderDetailsCartVM.OrderHeader.OrderDate = DateTime.Now;
            OrderDetailsCartVM.OrderHeader.ApplicationUserID = claim.Value;
            OrderDetailsCartVM.OrderHeader.Status = SD.PaymentStatusPending;
            OrderDetailsCartVM.OrderHeader.DeliveryTime = Convert.ToDateTime
                (OrderDetailsCartVM.OrderHeader.DeliveryDate.ToShortDateString() + " "+ OrderDetailsCartVM.OrderHeader.DeliveryTime.ToShortTimeString());

            List<OrderDetails> orderDetailsList = new List<OrderDetails>();
            unitOfWork.OrderHeaderRepository.Add(OrderDetailsCartVM.OrderHeader);
            unitOfWork.Save();

            foreach(var item in OrderDetailsCartVM.listCart)
            {
                item.Product = unitOfWork.ProductRepository.GetFirstOrDefault(s => s.ProductID == item.ProductID);
                OrderDetails orderDetails = new OrderDetails
                {
                    ProductID = item.ProductID,
                    OrderHeaderID = OrderDetailsCartVM.OrderHeader.OrderHeaderID,
                    Descriotion=item.Product.Description,
                    Name=item.Product.Name,
                    Cost=item.Product.Cost,
                    Count=item.Count
                    
                };
                OrderDetailsCartVM.OrderHeader.OrderTotal += (orderDetails.Count * orderDetails.Cost);
                unitOfWork.OrderDetailsRepository.Add(orderDetails);
              

            }
            OrderDetailsCartVM.OrderHeader.OrderTotal =Convert.ToDouble( String.Format("{0:.##}", OrderDetailsCartVM.OrderHeader.OrderTotal));
            unitOfWork.ShoppingCartRepository.DeleteRange(OrderDetailsCartVM.listCart);
            HttpContext.Session.SetInt32(SD.ShoppingCart, 0);
            
            unitOfWork.Save();

            if (stripeToken != null)
            {

                // `source` is obtained with Stripe.js; see https://stripe.com/docs/payments/accept-a-payment-charges#web-create-token
                var options = new ChargeCreateOptions
                {
                    Amount = Convert.ToInt32(OrderDetailsCartVM.OrderHeader.OrderTotal * 100),
                    Currency = "usd",
                    Source = stripeToken,
                    Description = "Order ID: " + OrderDetailsCartVM.OrderHeader.OrderHeaderID,
                };
                var service = new ChargeService();
                Charge charge = service.Create(options);

                OrderDetailsCartVM.OrderHeader.TransactionID = charge.Id;

                if (charge.Status.ToLower() == "succeeded")
                {
                    OrderDetailsCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusApproved;
                    OrderDetailsCartVM.OrderHeader.Status = SD.StatusSubmitted;
                }
                else
                {
                    OrderDetailsCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusRejected;
                }
            }
            else
            {
                OrderDetailsCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusRejected;
            }
            unitOfWork.OrderHeaderRepository.Update(OrderDetailsCartVM.OrderHeader);
            unitOfWork.Save();
            return RedirectToPage("/Customer/ShoppingCart/OrderConfirmation", new { id = OrderDetailsCartVM.OrderHeader.OrderHeaderID });
        }
    }
}
