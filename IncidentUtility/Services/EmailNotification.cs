using Microsoft.Identity.Client;
using System.Diagnostics;
using System.Net; // Provides classes, functions for Network Programming (HTTP, SMPT.. etc)
using System.Net.Mail;
using System.Threading.Tasks;

namespace IncidentUtility.Services
{
    public class EmailNotification
    {

       public class EmailResult
       {
          public bool isSuccess {get; set;}
          public string details { get; set; }
       }


        EmailResult emailResult = new EmailResult();

        public async Task<EmailResult> SendEmail(int actionPlanId, string emailAddress)
        {    
            try
            {
                SmtpClient client = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential("rfhtorres28@gmail.com", "balq jevo jyzb susz")

                };

                MailMessage email = new MailMessage("rfhtorres28@gmail.com", emailAddress)
                {
                    Subject = "test",
                    Body = $"Action Plan {actionPlanId} is overdue. Please check the monitoring app",
                };


                await client.SendMailAsync(email);

                emailResult.isSuccess = true;
                emailResult.details = "Email is sent";

                return emailResult;

            }

            catch (Exception err)
            {
                emailResult.isSuccess = false;
                emailResult.details = err.Message;
                Debug.WriteLine(emailResult.details);

                return emailResult;
            }

        }

    }
}
