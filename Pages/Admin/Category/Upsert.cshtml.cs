using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eCommerce.DataAccess.Data.Repository;
using eCommerce.DataAccess.Data.Repository.IRepository;
using eCommerce.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace eCommerce.Pages.Admin.Category
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
        public Models.Category Category { get; set; }

        public IActionResult OnGet(int? categoryID)
        {
            Category = new Models.Category();
            if (categoryID != null)
            {
                Category = unitOfWork.CategoryRepository.GetFirstOrDefault(s => s.CategoryID == categoryID);
                if (Category == null)
                {
                    return NotFound();
                }
            }
            return Page();
            
        }

       public IActionResult OnPost()
        {

            if (ModelState.IsValid)
            {
                if (Category.CategoryID == 0)
                {
                    unitOfWork.CategoryRepository.Add(Category);
                }
                else
                {
                    unitOfWork.CategoryRepository.Update(Category);
                }
                unitOfWork.Save();
                return RedirectToPage("./Index");
            }
            return Page();
        }
    }
}
