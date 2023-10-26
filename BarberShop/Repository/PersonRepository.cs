using BarberShop.Models;
using BarberShop.Utility;
using BarberShop.Utility.Resources;
using Microsoft.AspNetCore.Mvc;

namespace BarberShop.Repository
{
    public class PersonRepository
    {
        private readonly AppDbContext _appDbContext;

        public PersonRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public Person GetPersonToLogin(string userName, string password)
        {
            try
            {
                Person person = new();
                PasswordEncryption passwordEncryption = new(password);

                password = passwordEncryption.Encrypt(password);

                person = _appDbContext.Person.FirstOrDefault(a => a.UserName == userName && a.Pwd == password);

                return person;
            }
            catch (Exception ex)
            {
                throw new Exception(Exceptions.EXC005, ex);
            }
        }

        public Person GetPersonById(long id)
        {
            try
            {
                return _appDbContext.Person.FirstOrDefault(x => x.Id == id);
            }
            catch (Exception ex)
            {
                throw new Exception(Exceptions.EXC001, ex);
            }
        }

        public List<Person> GetAll()
        {
            try
            {
                return _appDbContext.Person.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(Exceptions.EXC004, ex);
            }
        }

        public ActionResult<dynamic> CreateNewPerson(Person person)
        {
            try
            {
                PasswordEncryption passwordEncryption = new PasswordEncryption(person.Pwd);
                person.Pwd = passwordEncryption.Encrypt(person.Pwd);

                _appDbContext.Add(person);
                
                if(person.Role == Enuns.Role.Barber)
                {
                    PersonBarber personBarber = new PersonBarber();
                    personBarber.PersonId = person.Id;
                    personBarber.CreationDate = DateTime.Now;
                    _appDbContext.PersonBarber.Add(personBarber);
                }

                return person;
            }
            catch (Exception ex)
            {
                throw new Exception(Exceptions.EXC002, ex);
            }
        }

        public ActionResult<dynamic> DeletePerson(long id)
        {
            try
            {
                Person personToDelete = _appDbContext.Person.FirstOrDefault(x => x.Id == id);

                if (personToDelete != null)
                {
                    _appDbContext.Person.Remove(personToDelete);
                    return Messages.MSG002; //substituir a string depois
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(Exceptions.EXC003, ex);
            }
        }
    }
}
