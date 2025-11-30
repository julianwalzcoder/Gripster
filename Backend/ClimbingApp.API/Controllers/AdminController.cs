using ClimbingApp.Model.Entities;
using ClimbingApp.Model.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;


namespace ClimbingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class AdminController : ControllerBase
    {
        protected AdminRepository Repository { get; }
        public AdminController(AdminRepository repository)
        {
            Repository = repository;
        }

        [HttpGet("{id}")]
        public ActionResult<Admin> GetAdmin([FromRoute] int id)
        {
            Admin admin = Repository.GetAdminById(id);
            if (admin == null)
            {
                return NotFound();
            }
            return Ok(admin);
        }

        [HttpGet]
        public ActionResult<IEnumerable<Admin>> GetAdmins()
        {
            return Ok(Repository.GetAdmins());
        }

        [HttpPost]
        public ActionResult Post([FromBody] Admin admin)
        {
            if (admin == null)
            {
                return BadRequest("Admin info not correct");
            }
            bool status = Repository.InsertAdmin(admin);
            if (status)
            {
                return Ok();
            }
            return BadRequest();
        }

        [HttpPut]
        public ActionResult UpdateAdmin([FromBody] Admin admin)
        {
            if (admin == null)
            {
                return BadRequest("Admin info not correct");
            }
            Admin existingAdmin = Repository.GetAdminById(admin.Id);
            if (existingAdmin == null)
            {
                return NotFound($"Admin with id {admin.Id} not found");
            }
            bool status = Repository.UpdateAdmin(admin);
            if (status)
            {
                return Ok();
            }
            return BadRequest("Something went wrong");
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteAdmin([FromRoute] int id)
        {
            Admin existingAdmin = Repository.GetAdminById(id);
            if (existingAdmin == null)
            {
                return NotFound($"Admin with id {id} not found");
            }
            bool status = Repository.DeleteAdmin(id);
            if (status)
            {
                return NoContent();
            }
            return BadRequest($"Unable to delete admin with id {id}");
        }
    }
}
