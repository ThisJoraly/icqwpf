using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace icqwpf
{
    public class PrivateMessage
    {
        public int Id { get; set; }
        public User Sender { get; set; }
        public User Recipient { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
