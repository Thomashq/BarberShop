using System.Reflection.PortableExecutable;
using System.Net.Mail;
using System.Security.Claims;
using BarberShop.Models;
using BarberShop.Repository;
using BarberShop.Utility;
using BarberShop.Utility.Interfaces;
using BarberShop.Utility.Resources;
using Microsoft.AspNetCore.Mvc;
using Enum = BarberShop.Utility.Enuns;
using Google.Authenticator;

namespace BarberShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : Controller
    {
        private readonly AppDbContext _appDbContext;
        private readonly IConfiguration _configuration;
        private readonly TwoFactorAuthenticator _tfa;
        private readonly IToken _token;
        private readonly IEmail _mail; 

        public LoginController(AppDbContext appDbContext, IConfiguration configuration, IToken token, IEmail mail)
        {
            _appDbContext = appDbContext;
            _configuration = configuration;
            _token = token;
            _mail = mail;
            _tfa = new TwoFactorAuthenticator();
        }

        [Route("View")]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromForm] Person person)
        {
            try
            {
                LoginModel loginModel = await AuthenticateAsync(person);

                if (loginModel.Status == StatusCodes.Status404NotFound)
                {
                    TempData["ErrorMessage"] = loginModel.Message;
                    return RedirectToAction("Index");
                }

                return RedirectToAction("Index", "Admin");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        public async Task<LoginModel> AuthenticateAsync([FromBody] Person person)
        {
            try
            {
                PersonRepository personRepository = new(_appDbContext);

                person = personRepository.GetPersonToLogin(person.Mail, person.Pwd);

                if (person == null)
                    return new LoginModel { Message = Exceptions.EXC005, Status = 404 };

                var claims = new List<Claim>
                {
                     new Claim(ClaimTypes.Name, person.Id.ToString()),
                     new Claim(ClaimTypes.Role, Enum.GetEnumDescription(person.Role))
                };

                _token.GenerateToken(claims);

                return new LoginModel { Message = Messages.MSG003, Status = 200 };
            }
            catch (Exception ex)
            {
                throw new Exception(Exceptions.EXC005, ex);
            }
        }
        [HttpPost("Register")]
        public async Task<IActionResult> RegisterAsync (Person person)
        {
            try
            {
                PersonRepository personRepository = new(_appDbContext);
                personRepository.CreateNewPerson(person);

                Mail mail = new Mail{
                    MailSubject = Messages.MSG004,
                    MailContent = "Email genérico pra testes",
                    MailContact = person.Mail
                };

                await _appDbContext.SaveChangesAsync();

                bool register = _mail.Send(mail);

                return Json("Registro efetuado com sucesso");
            }
            catch(Exception ex)
            {
                throw new Exception(Exceptions.EXC006, ex);
            }
        }
        [HttpGet]
        public ActionResult<string> GenerateQRCode(string mail)
        {
            try
            {
                string key = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 10);
                SetupCode setupCode = _tfa.GenerateSetupCode("Geração de QR Code", mail, key, false, 3);

                return setupCode.QrCodeSetupImageUrl;
            }
            catch(Exception ex)
            {
                throw new Exception(Exceptions.EXC009, ex);
            }
        }

        [HttpPost("/ValidadeQRCode")]
        public ActionResult<bool> ValidateCode(string code, string key)
        {
            return _tfa.ValidateTwoFactorPIN(key, code);
        }
    }
}