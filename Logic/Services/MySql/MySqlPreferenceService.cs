using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nutritia
{
    /// <summary>
    /// Service MySql lié aux Préférences.
    /// </summary>
    public class MySqlPreferenceService : IPreferenceService
    {
        private MySqlConnexion connexion;

        /// <summary>
        /// Méthode permettant d'obtenir l'ensemble des Préférences sauvegardés dans la base de données.
        /// </summary>
        /// <returns>Une liste contenant les préférences.</returns>
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

        /// <summary>
        /// Méthode permettant d'obtenir une préférence sauvegardé dans la base de données.
        /// </summary>
        /// <param name="args">Les arguments permettant de retrouver la préférence.</param>
        /// <returns>Un objet Preference.</returns>
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

        /// <summary>
        /// Méthode permettant de construire un objet Preference.
        /// </summary>
        /// <param name="preference">Un enregistrement de la table Preferences.</param>
        /// <returns>Un objet Preference.</returns>
        private Preference ConstruirePreference(DataRow preference)
        {
            return new Preference()
            {
                IdPreference = (int)preference["idPreference"],
                Nom = (string)preference["preference"]
            };
        }
    }
}
