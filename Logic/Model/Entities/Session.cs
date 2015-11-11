using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nutritia.Logic.Model.Entities
{
    [Serializable]
    public struct Session
    {
        public string HostName_IP { get; private set; }

        public string Name { get; private set; }

        public string User { get; private set; }

        public string Password { get; private set; }

        public int Port { get; private set; }

        public string DatabaseName { get; private set; }

        public Session(string nom, string host, string user, string password, string database, int port = 3306)
        :this()
        {
            HostName_IP = host;
            Name = nom;
            User = user;
            Password = password;
            DatabaseName = database;
            Port = port;
        }
    }
}
