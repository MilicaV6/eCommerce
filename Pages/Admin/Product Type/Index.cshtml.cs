using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using eCommerce.DataAccess;
using eCommerce.Models;
using Microsoft.AspNetCore.Authorization;
using eCommerce.Utility;

namespace eCommerce.Pages.Admin.Product_Type
{
    [Authorize(Roles = SD.ManagerRole)]
    public class IndexModel : PageModel
    {
        private readonly eCommerce.DataAccess.ApplicationDbContext _context;

        public IndexModel(eCommerce.DataAccess.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<ProductType> ProductType { get;set; }

        public async Task OnGetAsync()
        {
            ProductType = await _context.ProductTypes.ToListAsync();
        }
    }
}
