using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace eCommerce.Pages.Customer.ShoppingCart
{
    public class OrderConfirmationModel : PageModel
    {
        [BindProperty]
        public int orderID { get; set; }
        public void OnGet(int id)
        {
            orderID = id;
        }
    }
}
