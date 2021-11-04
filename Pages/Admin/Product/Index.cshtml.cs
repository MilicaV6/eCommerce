using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eCommerce.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace eCommerce.Pages.Admin.Product
{
    [Authorize(Roles = SD.ManagerRole)]
    public class IndexModel : PageModel
    {
        
        public void OnGet()
        {
        }
    }
}
