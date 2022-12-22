using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1;

namespace TDDD49Template.Models
{
    public class DataHandler
    {

        public void InitConvo(Conversation convo)
        {
         
            var json_info = JsonConvert.SerializeObject(convo, new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            });
            File.WriteAllText("Conversations/Conversation_" + convo.Id + ".json", json_info);
        }
        public List<Conversation> GetHistory()
        {
            List<Conversation> temp = new List<Conversation>();
            var convoFiles = Directory.GetFiles("Conversations/", "*.json");

            foreach (string file in convoFiles)
            {
                var jsonString = File.ReadAllText(file);
                Conversation jsonObject = JsonConvert.DeserializeObject<Conversation>(jsonString);

                temp.Add(jsonObject);
            }
            List<Conversation> SortedList = temp.OrderBy(o => o.Date).ToList();
            SortedList.Reverse();
            return SortedList;
        }

        public void UpdateConvo(Conversation convo)
        {
            var jsonFile = JsonConvert.SerializeObject(convo, new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            });
            File.WriteAllText("Conversations/Conversation_" + convo.Id + ".json", jsonFile);
        }

    }
}
