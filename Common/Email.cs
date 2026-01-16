
using System.Net;
using System.Net.Mail;

namespace MESCHECKLIST.Common
{
    public static class Email
    {
        static string EmailFromAddress;
        static string smtpAddress;
        static string portNumber;
        static string password;
        static string enableSSL;
        static string ExternalEmailFromAddress;
        static string smptUserName;
        static Email()
        {
            var _configuration = new ConfigurationBuilder()
                                                  .AddJsonFile("appSettings.Development.json")
                                                  .Build();
            IConfigurationSection appSettings = _configuration.GetSection("AppSettings");
            EmailFromAddress = appSettings["EmailFromAddress"];
            smtpAddress = appSettings["smtpAddress"];
            portNumber = appSettings["portNumber"];
            password = appSettings["password"];
            enableSSL = appSettings["enableSSL"];
            ExternalEmailFromAddress = appSettings["ExternalEmailFromAddress"];
            smptUserName = appSettings["smptUserName"];

        }
   
    }
}

