using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TalentsApi.Models.Email
{
    public class MailMessageModel
    {
        public string RecepientEmail { get; set; }
        public string RecepientFullname { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string HtmlBody { get; set; }
        public List<MailAttachmentDto> Attachments { get; set; } = new List<MailAttachmentDto>();
    }
}
