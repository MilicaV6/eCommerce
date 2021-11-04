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
    public class ApplicationUserController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        public ApplicationUserController(IUnitOfWork _unitOfWork)
        {
            unitOfWork = _unitOfWork;

        }
        [HttpGet]
        public IActionResult Get()
        {
            return Json(new { data = unitOfWork.ApplicationUserRepository.GetAll() });
        }
        [HttpPost]
        public IActionResult LockUnlock([FromBody]string id)
        {
            ApplicationUser userFromDb  = unitOfWork.ApplicationUserRepository.GetFirstOrDefault(s => s.Id == id);
            if (userFromDb == null)
            {
                return Json(new { success = false, message = "Error while Locking/Unlocking" });
            }
            if(userFromDb.LockoutEnd!=null && userFromDb.LockoutEnd > DateTime.Now)
            {
                userFromDb.LockoutEnd = DateTime.Now;
            }
            else
            {
                userFromDb.LockoutEnd = DateTime.Now.AddYears(100);
            }
            unitOfWork.ApplicationUserRepository.Update(userFromDb);
            unitOfWork.Save();
            return Json(new { success = true, message = "Done successfully!" });
        }
    }
}
