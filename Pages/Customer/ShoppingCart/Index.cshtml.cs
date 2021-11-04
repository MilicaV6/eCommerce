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

namespace eCommerce.Pages.Customer.ShoppingCart
{
    public class IndexModel : PageModel       
    {
        private readonly IUnitOfWork unitOfWork;
        public IndexModel(IUnitOfWork _unitOfWork)
        {
            unitOfWork = _unitOfWork;
        }
        public OrderDetailsCart OrderDetailsCartVM { get; set; }
        public void OnGet()
        {
            OrderDetailsCartVM = new OrderDetailsCart()
            {
                OrderHeader = new OrderHeader(),
                listCart=new List<Models.ShoppingCart>()
                
            };
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {


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
            }
        }
        public IActionResult OnPostIncrement(int cartID)
        {
            var cart = unitOfWork.ShoppingCartRepository.GetFirstOrDefault(s => s.ShoppingCartID == cartID);
            unitOfWork.ShoppingCartRepository.IncrementCartCount(cart, 1);
            unitOfWork.ShoppingCartRepository.Update(cart);
            unitOfWork.Save();
            return Redirect("/Customer/ShoppingCart/Index");
        }

        public IActionResult OnPostDecrement(int cartID)
        {
            var cart = unitOfWork.ShoppingCartRepository.GetFirstOrDefault(s => s.ShoppingCartID == cartID);
            if (cart.Count == 1)
            {

                unitOfWork.ShoppingCartRepository.Delete(cart);
                unitOfWork.Save();

                var num = unitOfWork.ShoppingCartRepository.GetAll(s => s.ApplicationUserID == cart.ApplicationUserID).ToList().Count;
                HttpContext.Session.SetInt32(SD.ShoppingCart, num);
            }
            else
            {
                unitOfWork.ShoppingCartRepository.DecrementCartCount(cart, 1);
                unitOfWork.ShoppingCartRepository.Update(cart);
                unitOfWork.Save();
            }
           
            return Redirect("/Customer/ShoppingCart/Index");
        }
        public IActionResult OnPostRemove(int cartID)
        {
            var cart = unitOfWork.ShoppingCartRepository.GetFirstOrDefault(s => s.ShoppingCartID == cartID);
         
            unitOfWork.ShoppingCartRepository.Delete(cart);
            unitOfWork.Save();

            var num = unitOfWork.ShoppingCartRepository.GetAll(s => s.ApplicationUserID == cart.ApplicationUserID).ToList().Count;
            HttpContext.Session.SetInt32(SD.ShoppingCart, num);

            return Redirect("/Customer/ShoppingCart/Index");
        }

    }
}
