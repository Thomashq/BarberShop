using BarberShop.Models;
using BarberShop.Repository;
using BarberShop.Utility;
using BarberShop.Utility.Resources;
using Microsoft.AspNetCore.Mvc;

namespace BarberShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonController : Controller
    {
        private readonly AppDbContext _appDbContext;

        public PersonController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        [HttpGet]
        public List<Person> GetAllPerson()
        {
            try
            {
                PersonRepository personRepository = new PersonRepository(_appDbContext);

                return personRepository.GetAll();
            }
            catch (Exception ex)
            {
                throw new Exception(Exceptions.EXC004, ex);
            }
        }

        [HttpGet("{id}")]
        public ActionResult<Person> GetpersonById(long id)
        {
            try
            {
                PersonRepository personRepository = new PersonRepository(_appDbContext);

                return personRepository.GetPersonById(id);
            }
            catch (Exception ex)
            {
                throw new Exception(Exceptions.EXC001, ex);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Addperson(Person person)
        {
            try
            {
                PersonRepository personRepository = new PersonRepository(_appDbContext);
                personRepository.CreateNewPerson(person);

                await _appDbContext.SaveChangesAsync();

                return Ok(Messages.MSG001);
            }
            catch (Exception ex)
            {
                throw new Exception(Exceptions.EXC002, ex);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePerson(long id)
        {
            try
            {
                PersonRepository personRepository = new PersonRepository(_appDbContext);
                var result = personRepository.DeletePerson(id);

                if (result == null)
                    return BadRequest();

                await _appDbContext.SaveChangesAsync();
                return Ok(Messages.MSG001);
            }
            catch (Exception ex)
            {
                throw new Exception(Exceptions.EXC003, ex);
            }
        }
    }
}
