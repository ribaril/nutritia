using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nutritia
{
    public class MySqlObjectifService : IObjectifService
    {
        private MySqlConnexion connexion;

        public IList<Objectif> RetrieveAll()
        {
            IList<Objectif> resultat = new List<Objectif>();

            try
            {
                connexion = new MySqlConnexion();

                string requete = "SELECT * FROM Objectifs";

                DataSet dataSet = connexion.Query(requete);
                DataTable table = dataSet.Tables[0];

                foreach (DataRow objectif in table.Rows)
                {
                    resultat.Add(ConstruireObjectif(objectif));
                }

            }
            catch (MySqlException)
            {
                throw;
            }

            return resultat;

        }

        public Objectif Retrieve(RetrieveObjectifArgs args)
        {

            Objectif objectif;

            try
            {
                connexion = new MySqlConnexion();

                string requete = string.Format("SELECT * FROM Objectifs WHERE idObjectif = {0}", args.IdObjectif);

                DataSet dataSet = connexion.Query(requete);
                DataTable table = dataSet.Tables[0];

                objectif = ConstruireObjectif(table.Rows[0]);

            }
            catch (Exception)
            {
                throw;
            }

            return objectif;

        }

        private Objectif ConstruireObjectif(DataRow objectif)
        {
            return new Objectif()
            {
                // TODO : ENlever l'attribut estSelectionne des classes.
                IdObjectif = (int)objectif["idObjectif"],
                Nom = (string)objectif["objectif"]
            };
        }
    }
}
