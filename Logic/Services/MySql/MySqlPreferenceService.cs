using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nutritia
{
    public class MySqlPreferenceService : IPreferenceService
    {
        private MySqlConnexion connexion;

        public IList<Preference> RetrieveAll()
        {
            IList<Preference> resultat = new List<Preference>();

            try
            {
                connexion = new MySqlConnexion();

                string requete = "SELECT * FROM Preferences";

                DataSet dataSet = connexion.Query(requete);
                DataTable table = dataSet.Tables[0];

                foreach (DataRow preference in table.Rows)
                {
                    resultat.Add(ConstruirePreference(preference));
                }

            }
            catch (MySqlException)
            {
                throw;
            }

            return resultat;

        }

        public Preference Retrieve(RetrievePreferenceArgs args)
        {

            Preference preference;

            try
            {
                connexion = new MySqlConnexion();

                string requete = string.Format("SELECT * FROM Preferences WHERE idPreference = {0}", args.IdPreference);

                DataSet dataSet = connexion.Query(requete);
                DataTable table = dataSet.Tables[0];

                preference = ConstruirePreference(table.Rows[0]);

            }
            catch (Exception)
            {
                throw;
            }

            return preference;

        }

        private Preference ConstruirePreference(DataRow preference)
        {
            return new Preference()
            {
                // TODO : ENlever l'attribut estSelectionne des classes.
                IdPreference = (int)preference["idPreference"],
                Nom = (string)preference["preference"]
            };
        }
    }
}
