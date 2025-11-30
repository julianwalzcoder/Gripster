using ClimbingApp.Model.Entities;
using ClimbingApp.Model.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;


namespace ClimbingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class GradeController : ControllerBase
    {
        protected GradeRepository Repository { get; }
        public GradeController(GradeRepository repository)
        {
            Repository = repository;
        }

        [HttpGet("{id}")]
        public ActionResult<Grade> GetGrade([FromRoute] int id)
        {
            Grade grade = Repository.GetGradeById(id);
            if (grade == null)
            {
                return NotFound();
            }
            return Ok(grade);
        }

        [HttpGet]
        public ActionResult<IEnumerable<Grade>> GetGrades()
        {
            return Ok(Repository.GetGrades());
        }

        [HttpPost]
        public ActionResult Post([FromBody] Grade grade)
        {
            if (grade == null)
            {
                return BadRequest("Grade info not correct");
            }
            bool status = Repository.InsertGrade(grade);
            if (status)
            {
                return Ok();
            }
            return BadRequest();
        }

        [HttpPut]
        public ActionResult UpdateGrade([FromBody] Grade grade)
        {
            if (grade == null)
            {
                return BadRequest("Grade info not correct");
            }
            Grade existingGrade = Repository.GetGradeById(grade.Id);
            if (existingGrade == null)
            {
                return NotFound($"Grade with id {grade.Id} not found");
            }
            bool status = Repository.UpdateGrade(grade);
            if (status)
            {
                return Ok();
            }
            return BadRequest("Something went wrong");
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteGrade([FromRoute] int id)
        {
            Grade existingGrade = Repository.GetGradeById(id);
            if (existingGrade == null)
            {
                return NotFound($"Grade with id {id} not found");
            }
            bool status = Repository.DeleteGrade(id);
            if (status)
            {
                return NoContent();
            }
            return BadRequest($"Unable to delete grade with id {id}");
        }
    }
}
