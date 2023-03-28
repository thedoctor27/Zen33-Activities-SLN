using activities.Models;
using Mailjet.Client;
using Mailjet.Client.Resources;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.CodeAnalysis.CSharp;
using Newtonsoft.Json.Linq;

namespace activities.Extensions
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _config;

        public MailJetSettings _mail { get; set; }

        public EmailSender(IConfiguration config)
        {
            _config = config;
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // throw new NotImplementedException();
            return Execute(email, subject, htmlMessage);
        }

        private async Task Execute(string email, string subject, string body)
        {

            _mail = _config.GetSection("MailJet").Get<MailJetSettings>();

            MailjetClient client = new MailjetClient(_mail.ApiKey, _mail.SecretKey)
            {
                // Version = ApiVersion.V3_1, in the sample but not working
                BaseAdress = "https://api.mailjet.com",
            };
            MailjetRequest request = new MailjetRequest
            {
                // Resource = Send.Resource, in the sample but not working
                Resource = SendV31.Resource,
            }
               .Property(Send.Messages, new JArray {
                new JObject {
                 {"From", new JObject {
                  {"Email", "mobi2009ro@gmail.com"},
                  {"Name", "FindRoomates"}
                  }},
                 {"To", new JArray {
                  new JObject {
                   {"Email", email},
                  // {"Name", "passenger 1"}
                   }
                  }},
                 {"Subject", subject},
                 {"HTMLPart", body}
                 }
                   });
            //MailjetResponse response = await client.PostAsync(request);
            await client.PostAsync(request);

        }
    }
}
