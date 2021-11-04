using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using eCommerce.DataAccess.Data.Repository.IRepository;
using eCommerce.Models;
using eCommerce.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace eCommerce.Pages.Customer.Home
{
    [Authorize]
    public class DetailsModel : PageModel
    {
        private readonly IUnitOfWork unitOfWork;
        public DetailsModel(IUnitOfWork _unitOfWork)
        {
            unitOfWork = _unitOfWork;
        }
        [BindProperty]
        public Models.ShoppingCart ShoppingCart { get; set; }
        public void OnGet(int id)
        {
            ShoppingCart = new Models.ShoppingCart()
            {
                Product = unitOfWork.ProductRepository.GetFirstOrDefault
                (includeProperties: "Category,ProductType,ApplicationUser", filter: c => c.ProductID == id),
                ProductID = id
            };
        } 
        public  IActionResult OnPost()
        {
            if (ModelState.IsValid)
            {
                var claimsIdentity = (ClaimsIdentity)this.User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

                ShoppingCart.ApplicationUserID = claim.Value;
                Models.ShoppingCart scFromDB = unitOfWork.ShoppingCartRepository.GetFirstOrDefault(
                    c => c.ApplicationUserID == ShoppingCart.ApplicationUserID && c.ProductID == ShoppingCart.ProductID);
                if (scFromDB == null)
                {
                    unitOfWork.ShoppingCartRepository.Add(ShoppingCart);
                }
                else
                {
                    scFromDB.Count = unitOfWork.ShoppingCartRepository.IncrementCartCount(scFromDB, ShoppingCart.Count);
                    unitOfWork.ShoppingCartRepository.Update(scFromDB);
                }
               
                unitOfWork.Save();
                var count = unitOfWork.ShoppingCartRepository.GetAll
                    (c => c.ApplicationUserID == ShoppingCart.ApplicationUserID).ToList().Count;
                HttpContext.Session.SetInt32(SD.ShoppingCart, count);
                return RedirectToPage("Index");
            }
            else
            {
                ShoppingCart.Product = unitOfWork.ProductRepository.GetFirstOrDefault
                 (includeProperties: "Category,ProductType", filter: c => c.ProductID == ShoppingCart.ProductID);
                return Page();
            }
        }
    }
}
