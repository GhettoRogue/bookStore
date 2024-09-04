using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace servise
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)] //все клиенты работают с одним сервисом
    public class ChatService : IChatService
    {
        List <User> users = new List<User>();
        int idCount = 1;

        public int Connect(string name)
        {
            User user = new User()
            {
                Id = idCount,
                Name = name,
                opCon = OperationContext.Current
            };
            
            idCount++;
            
            SendMessage(user.Name + "вошел(а) в чат", 0);
            users.Add(user);
            
            return user.Id;
        }

        public void Disconnect(int id)
        {
            var user = users.FirstOrDefault(x => x.Id == id);
            if (user != null) 
            { 
                users.Remove(user);
                SendMessage(user.Name + "вышел(а) из чата", 0);
            }
        }

        public void SendMessage(string msg, int id)
        {
            foreach (var user in users)
            {
                string fullMsg = "[" + DateTime.Now.ToShortTimeString() + "] ";

                var u = users.FirstOrDefault(x => x.Id == id);
                if (u != null)
                {
                    fullMsg += u.Name + ": ";
                }

                fullMsg += msg;

                user.opCon.GetCallbackChannel<IChatServiceCallback>().MessageCallback(fullMsg);
            }
        }
    }
}
