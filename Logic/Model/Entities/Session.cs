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

        public string Nom { get; private set; }

        public string NomUtilisateur { get; private set; }

        public string MotDePasse { get; private set; }

        public int Port { get; private set; }

        public string NomBD { get; private set; }

        public Session(string nom, string host, string user, string password, string database, int port = 3306)
            : this()
        {
            HostName_IP = host;
            Nom = nom;
            NomUtilisateur = user;
            MotDePasse = password;
            NomBD = database;
            Port = port;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("name={0};server={1};userid={2};password={3};database={4}", Nom, HostName_IP, NomUtilisateur, MotDePasse, NomBD);
            return sb.ToString();
        }

        public string ToConnexionString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("server={0};userid={1};password={2};database={3}", HostName_IP, NomUtilisateur, MotDePasse, NomBD);
            return sb.ToString();
        }

        #region IEquatable

        public override int GetHashCode()
        {
            return Tuple.Create(Nom, HostName_IP, NomUtilisateur, MotDePasse, Port, NomBD).GetHashCode();
        }

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
            return (this.NomBD == other.NomBD &&
            this.Nom == other.Nom &&
            this.HostName_IP == other.HostName_IP &&
            this.MotDePasse == other.MotDePasse &&
            this.Port == other.Port &&
            this.NomUtilisateur == other.NomUtilisateur);
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
