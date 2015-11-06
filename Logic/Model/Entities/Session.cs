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

        public string Nom { get; private set; }

        public string User { get; private set; }

        public string Password { get; private set; }

        public int Port { get; private set; }

        public string DatabaseName { get; private set; }

        public Session(string host, string nom, string user, string password, string database, int port = 3306)
        {
            HostName_IP = host;
            Nom = nom;
            User = user;
            Password = password;
            DatabaseName = database;
            Port = port;
        }
    }
}
