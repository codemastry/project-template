using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TalentsApi.Models.AppSettings;
using TalentsApi.Models.Email;

namespace TalentsApi.Services
{
    public class EmailService
    {
        private readonly AppSettingsModel _appSettingsModel;
        private readonly ILogger<EmailService> _logger;

        public EmailService(AppSettingsModel appSettingsModel,
            ILogger<EmailService> logger)
        {
            _logger = logger;
            _appSettingsModel = appSettingsModel;
        }

        public async Task Send(MailMessageModel message)
        {
            try
            {
                var client = new SendGridClient(_appSettingsModel.SendGrid.ApiKey);
                var from = new EmailAddress(_appSettingsModel.SendGrid.SenderAddress, _appSettingsModel.SendGrid.SenderName);
                var subject = message.Subject;
                var to = new EmailAddress(message.RecepientEmail, message.RecepientFullname);
                var plainTextContent = message.Body;
                var htmlContent = message.HtmlBody;
                var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
                msg.AddHeader("Priority", "Urgent");
                msg.AddHeader("Importance", "high");

                if (message.Attachments.Any())
                {
                    foreach (var attachment in message.Attachments)
                    {
                        if (!File.Exists(attachment.Path))
                        {
                            _logger.LogError($"File attachment Not found { JsonConvert.SerializeObject(attachment) }");
                            continue;
                        }
                        Byte[] bytes = File.ReadAllBytes(attachment.Path);
                        msg.Attachments = new List<SendGrid.Helpers.Mail.Attachment>
                        {
                            new SendGrid.Helpers.Mail.Attachment
                            {
                                Content = Convert.ToBase64String(bytes),
                                Filename = attachment.Name,
                                Type = attachment.Type,
                                Disposition = "attachment",
                            }
                        };
                    }
                }

                var response = await client.SendEmailAsync(msg);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Empty);
            }
        }
    }
}
