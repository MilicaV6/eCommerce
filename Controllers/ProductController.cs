using eCommerce.DataAccess.Data.Repository.IRepository;
using eCommerce.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace eCommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IWebHostEnvironment webHostEnvironment;
        
        public ProductController(IUnitOfWork _unitOfWork,IWebHostEnvironment _webHostEnvironment)
        {
            unitOfWork = _unitOfWork;
            webHostEnvironment = _webHostEnvironment;
        
        }
        [HttpGet]
        public IActionResult Get()
        {
            return Json(new { data = unitOfWork.ProductRepository.GetAll(null,null,"Category,ProductType,ApplicationUser") });
        }
        [HttpDelete("{productID}")]
        public IActionResult Delete(int productID)
        {
            try {
                var ProductToDelete = unitOfWork.ProductRepository.GetFirstOrDefault(s => s.ProductID == productID);
                if (ProductToDelete == null)
                {
                    return Json(new { success = false, message = "Error while deleting" });
                }
                else
                {
                    var imagePath = Path.Combine(webHostEnvironment.WebRootPath, ProductToDelete.Image.TrimStart('\\'));
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                    unitOfWork.ProductRepository.Delete(ProductToDelete);
                    unitOfWork.Save();
                    
                }
            }
            catch(Exception e)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            return Json(new { success = true, message = "Product Type deleted successfully" });
        }
    }
}
