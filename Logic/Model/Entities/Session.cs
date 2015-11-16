using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nutritia.Logic.Model.Entities
{
    public struct Session : IEquatable<Session>
    {
        public string HostName_IP { get; private set; }

        private string nom;

        public string Name
        {
            get
            {
                return nom;
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                    nom = value;
                else
                    return;
            }
        }

        public string User { get; private set; }

        public string Password { get; private set; }

        public int Port { get; private set; }

        public string DatabaseName { get; private set; }

        public Session(string nom, string host, string user, string password, string database, int port = 3306)
            : this()
        {
            HostName_IP = host;
            Name = nom;
            User = user;
            Password = password;
            DatabaseName = database;
            Port = port;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("name={0};server={1};userid={2};password={3};database={4}", Name, HostName_IP, User, Password, DatabaseName);
            return sb.ToString();
        }

        public string ToConnexionString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("server={0};userid={1};password={2};database={3}", HostName_IP, User, Password, DatabaseName);
            return sb.ToString();
        }

        #region IEquatable

        public override bool Equals(object right)
        {
            if (object.ReferenceEquals(right, null))
                return false;

            if (object.ReferenceEquals(this, right))
                return true;

            if (this.GetType() != right.GetType())
                return false;

            return this.Equals((Session)right);
        }

        public bool Equals(Session other)
        {
            return (this.DatabaseName == other.DatabaseName &&
            this.HostName_IP == other.HostName_IP &&
            this.Name == other.Name &&
            this.Password == other.Password &&
            this.Port == other.Port &&
            this.User == other.User);
        }

        public static bool operator ==(Session left, Session right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Session left, Session right)
        {
            return !(left == right);
        }

        #endregion
    }
}
