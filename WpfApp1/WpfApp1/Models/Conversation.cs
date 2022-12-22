using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDDD49Template.Models
{
    public class Conversation
    {
        private string name;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private string id;
        public string Id
        {
            get { return id; }
            set { id = value; }
        }

        private DateTime date;
        public DateTime Date
        {
            get { return date; }
            set { date = value; }
        }

        private List<JSONMessage> messages;
        public List<JSONMessage> Messages
        { 
            get { return messages; }
            set { messages = value; }
        }

        public Conversation(string name, JSONMessage msg)
        {
            messages= new List<JSONMessage>();
            messages.Add(msg);
            this.Name = name;
            this.Id = name + DateTime.Now.Year.ToString() + DateTime.Now.DayOfYear.ToString() + 
                DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();
            this.Date = DateTime.Now;
        }
    }
}
