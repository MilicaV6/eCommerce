using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using eCommerce.DataAccess.Data.Repository.IRepository;
using eCommerce.Models;
using eCommerce.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace eCommerce.Pages.Customer.Home
{
    public class IndexModel : PageModel
    {
        private readonly IUnitOfWork unitOfWork;
        public IndexModel(IUnitOfWork _unitOfWork)
        {
            unitOfWork = _unitOfWork;
        }
        public IEnumerable<Product> ProductList { get; set; }
        public IEnumerable<Category> CategoryList { get; set; }
        public void OnGet()
        {
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                int shoppingCartNum = unitOfWork.ShoppingCartRepository.GetAll(s => s.ApplicationUserID == claim.Value).ToList().Count;
                HttpContext.Session.SetInt32(SD.ShoppingCart, shoppingCartNum);
            }

            ProductList = unitOfWork.ProductRepository.GetAll(null, null, "Category,ProductType,ApplicationUser");
            CategoryList = unitOfWork.CategoryRepository.GetAll(null, q => q.OrderBy(c => c.DisplayOrder), null);
        }
    }
}
