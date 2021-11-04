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
using Microsoft.AspNetCore.Hosting;
using eCommerce.Models.ViewModels;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using eCommerce.Utility;
using Microsoft.AspNetCore.Identity;

namespace eCommerce.Pages.Admin.Product
{
    [Authorize(Roles = SD.ManagerRole)]
    public class UpsertModel : PageModel
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly UserManager<IdentityUser> userManager;

        public UpsertModel(IUnitOfWork _unitOfWork, IWebHostEnvironment _webHostEnvironment, UserManager<IdentityUser> _userManager)
        {
            unitOfWork = _unitOfWork;
            webHostEnvironment = _webHostEnvironment;
            userManager = _userManager;
        }

        [BindProperty]
        public ProductVM ProductObj { get; set; } 


        public async Task<IActionResult> OnGet(int? productID)
        {


            ProductObj = new ProductVM
            {
                Product = new Models.Product(),
                CategoryList = new Dictionary<int,string>(),
                ProductTypeList = new Dictionary<int, string>(),
                SellerList = new Dictionary<string, string>()
            };

         
            foreach (Models.Category category in unitOfWork.CategoryRepository.GetAll())
            {
                ProductObj.CategoryList.Add(category.CategoryID, category.Name);
            }
            
            foreach (Models.ProductType productType in unitOfWork.ProductTypeRepository.GetAll())
            {
                ProductObj.ProductTypeList.Add(productType.ProductTypeID, productType.Name);
            }


            var users = await userManager.GetUsersInRoleAsync(SD.SellerRole);
            foreach (ApplicationUser applicationUser in users)
            {
               
                ProductObj.SellerList.Add(applicationUser.Id, applicationUser.LastName);
            }
            if (productID != null)
            {
                ProductObj.Product = unitOfWork.ProductRepository.GetFirstOrDefault(s => s.ProductID == productID);
                if (ProductObj.Product == null)
                {
                    return NotFound();
                }
            }
            return Page();
        }


        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public  IActionResult OnPost()           
        {
            string wwwrootPath = webHostEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;

            if (!ModelState.IsValid)
            {
                return RedirectToPage("./Index");
            }
                if (ProductObj.Product.ProductID == 0)
                {

                    string fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(wwwrootPath, @"images\products");
                    var extension = Path.GetExtension(files[0].FileName);

                    using (var fileStream = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                    {
                        files[0].CopyTo(fileStream);
                    }
                    ProductObj.Product.Image = @"\images\products\" + fileName + extension;


                    unitOfWork.ProductRepository.Add(ProductObj.Product);

                }
            
            else
            {
                var objFromDb = unitOfWork.ProductRepository.Get(ProductObj.Product.ProductID);
                if (files.Count > 0)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(wwwrootPath, @"images\products");
                    var extension = Path.GetExtension(files[0].FileName);

                    var imagePath = Path.Combine(wwwrootPath, objFromDb.Image.TrimStart('\\'));

                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }


                    using (var fileStream = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                    {
                        files[0].CopyTo(fileStream);
                    }
                    ProductObj.Product.Image = @"\images\products\" + fileName + extension;
                }
                else
                {
                    ProductObj.Product.Image = objFromDb.Image;
                }



                unitOfWork.ProductRepository.Update(ProductObj.Product);

            }
            unitOfWork.Save();
            return RedirectToPage("./Index");


        }
    }
}
