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
    /// Service MySql lié aux Objectifs.
    /// </summary>
    public class MySqlObjectifService : IObjectifService
    {
        private MySqlConnexion connexion;

        /// <summary>
        /// Méthode permettant d'obtenir l'ensemble des Objectifs sauvegardés dans la base de données.
        /// </summary>
        /// <returns>Une liste contenant les objectifs.</returns>
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

        /// <summary>
        /// Méthode permettant d'obtenir un objectif sauvegardé dans la base de données.
        /// </summary>
        /// <param name="args">Les arguments permettant de retrouver l'objectif.</param>
        /// <returns>Un objet Objectif.</returns>
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

        /// <summary>
        /// Méthode permettant de construire un objet Objectif.
        /// </summary>
        /// <param name="objectif">Un enregistrement de la table Objectifs.</param>
        /// <returns>Un objet Objectif.</returns>
        private Objectif ConstruireObjectif(DataRow objectif)
        {
            return new Objectif()
            {
                IdObjectif = (int)objectif["idObjectif"],
                Nom = (string)objectif["objectif"]
            };
        }
    }
}
