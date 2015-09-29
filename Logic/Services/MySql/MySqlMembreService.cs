using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Nutritia
{
    public class MySqlMembreService : IMembreService
    {
        private MySqlConnexion connexion;

        // Manque les liste des autres objets à faire
        public IList<Membre> RetrieveAll()
        {
            IList<Membre> resultat = new List<Membre>();
            try
            {
                connexion = new MySqlConnexion();

                string requete = "SELECT * FROM Membres";

                DataSet dataset = connexion.Query(requete);
                DataTable table = dataset.Tables[0];

                foreach (DataRow membre in table.Rows)
                {
                    resultat.Add(ConstruireMembre(membre));
                }

            }
            catch (MySqlException)
            {
                throw;
            }

            return resultat;
        }

     
        public Membre Retrieve(RetrieveMembreArgs args)
        {
            throw new NotImplementedException();
        }
       

        private Membre ConstruireMembre(DataRow row)
        {
            return new Membre()
            {
                IdMembre = (int)row["idMembre"],
                Nom = (string)row["nom"],
                Prenom = (string)row["prenom"],
                Taille = (int)row["taille"],
                Masse = (double)row["masse"],
                DateNaissance = (DateTime)row["dateNaissance"],
                MotPasse = (string)row["motPasse"],
                EstAdministrateur = (bool)row["estAdministrateur"],
                EstBanni = (bool)row["estBanni"]
            };
        }
    }
}