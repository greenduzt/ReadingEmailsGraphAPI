using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadingEmailsGraphAPI.Models
{
    public class Email
    {
        public Int64 ID { get; set; }
        public string Name { get; set; }
        public string From { get; set; }
        public DateTime ReceivedDateTime { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public string FileName { get; set; }
        public string Status { get; set; }
        public bool TransferredToHubSpot { get; set; }

    }
}
