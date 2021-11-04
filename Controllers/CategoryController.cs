using eCommerce.DataAccess.Data.Repository.IRepository;
using eCommerce.Utility;
using Microsoft.AspNetCore.Authorization;
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
    
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        public CategoryController(IUnitOfWork _unitOfWork)
        {
            unitOfWork = _unitOfWork;

        }
        [HttpGet]
        public IActionResult Get()
        {
            return Json(new { data = unitOfWork.CategoryRepository.GetAll() });
        }
        [HttpDelete("{categoryID}")]
        public IActionResult Delete(int categoryID)
        {
            Models.Category category = unitOfWork.CategoryRepository.GetFirstOrDefault(s => s.CategoryID == categoryID);
            if (category == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            unitOfWork.CategoryRepository.Delete(category);
            unitOfWork.Save();
            return Json(new { success = true, message = "Deleting successfully!" });
        }
    }
}
