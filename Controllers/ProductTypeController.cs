using eCommerce.DataAccess.Data.Repository.IRepository;
using eCommerce.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductTypeController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        public ProductTypeController(IUnitOfWork _unitOfWork)
        {
            unitOfWork = _unitOfWork;
        }
        [HttpGet]
        public IActionResult Get()
        {
            return Json(new { data = unitOfWork.ProductTypeRepository.GetAll() });
        }
        [HttpDelete("{productTypeID}")]
        public IActionResult Delete(int productTypeID)
        {
            var ProductTypeToDelete = unitOfWork.ProductTypeRepository.GetFirstOrDefault(s => s.ProductTypeID == productTypeID);
            if (ProductTypeToDelete == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            else
            {
                unitOfWork.ProductTypeRepository.Delete(ProductTypeToDelete);
                unitOfWork.Save();
                return Json(new { success = true, message = "Product Type deleted successfully" });
            }
        }
    }
}
