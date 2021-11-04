using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using eCommerce.DataAccess;
using eCommerce.Models;
using eCommerce.DataAccess.Data.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using eCommerce.Utility;

namespace eCommerce.Pages.Admin.Product_Type
{
    [Authorize(Roles = SD.ManagerRole)]
    public class UpsertModel : PageModel
    {
        private readonly IUnitOfWork unitOfWork;

        public UpsertModel(IUnitOfWork _unitOfWork)
        {
            unitOfWork = _unitOfWork;
        }

        [BindProperty]
        public ProductType ProductType { get; set; }


        public IActionResult OnGet(int? productTypeID)
        {
            ProductType = new ProductType();
            if (productTypeID != null)
            {
                ProductType = unitOfWork.ProductTypeRepository.GetFirstOrDefault(s => s.ProductTypeID == productTypeID);
                if(ProductType==null)
                {
                    return NotFound();
                }
            }
            return Page();
        }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public IActionResult OnPost()           
        {
          
            if (ModelState.IsValid)
            {
                if (ProductType.ProductTypeID == 0)
                {
                    unitOfWork.ProductTypeRepository.Add(ProductType);
                }
                else
                {
                    unitOfWork.ProductTypeRepository.Update(ProductType);
                }
                unitOfWork.Save();

                return RedirectToPage("./Index");
            }

            return Page();
             
        }
    }
}
