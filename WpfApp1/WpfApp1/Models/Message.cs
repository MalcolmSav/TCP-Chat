using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDDD49Template.Models
{
    public class JSONMessage
    {
        public string RequestType { get; set; }
        public DateTime Date { get; set; }
        public string UserName { get; set; }
       
        public string Message { get; set; }
    }
}
