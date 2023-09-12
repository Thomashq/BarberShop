﻿using BarberShop.Models;
using BarberShop.Utility.Interfaces;
using BarberShop.Utility;
using System.Net.Mail;
using System.Net;

namespace BarberShop.Services
{
    public class MailService : IEmail
    {
        private readonly IConfiguration _configuration;

        public MailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public bool Send(Mail mail)
        {
            try
            {
                MailApiConfig mailApi = new MailApiConfig()
                {
                    Host = _configuration.GetValue<string>("MailApiConfig:Host"),
                    Name = _configuration.GetValue<string>("MailApiConfig:Name"),
                    UserName = _configuration.GetValue<string>("MailApiConfig:UserName"),
                    Password = _configuration.GetValue<string>("MailApiConfig:Password"),
                    Port = _configuration.GetValue<int>("MailApiConfig:Port"),
                    MailFrom = _configuration.GetValue<string>("MailApiConfig:MailFrom"),
                    MailTo = _configuration.GetValue<string>("MailApiConfig:MailTo")
                };

                MailMessage mailMessage = new MailMessage()
                {
                    From = new MailAddress(mailApi.UserName, mailApi.Name)
                };

                mailMessage.To.Add(mailApi.MailTo);
                mailMessage.Subject = mail.MailSubject;
                mailMessage.Body = mail.MailContent;
                mailMessage.IsBodyHtml = true;
                mailMessage.Priority = MailPriority.High;

                using (SmtpClient smtp = new SmtpClient(mailApi.Host, mailApi.Port))
                {
                    smtp.Credentials = new NetworkCredential(mailApi.UserName, mailApi.Password);
                    smtp.EnableSsl = true;

                    smtp.Send(mailMessage);
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
