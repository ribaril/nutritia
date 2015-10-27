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
    /// Service MySql lié aux restrictions alimentaires.
    /// </summary>
    public class MySqlRestrictionAlimentaireService : IRestrictionAlimentaireService
    {

        private MySqlConnexion connexion;

        /// <summary>
        /// Méthode permettant d'obtenir l'ensemble des restrictions alimentaires de la base de données.
        /// </summary>
        /// <returns>Une liste contenant les restrictions alimentaires.</returns>
        public IList<RestrictionAlimentaire> RetrieveAll()
        {
            IList<RestrictionAlimentaire> resultat = new List<RestrictionAlimentaire>();

            try
            {
                connexion = new MySqlConnexion();

                string requete = "SELECT * FROM RestrictionsAlimentaires";

                DataSet dataSet = connexion.Query(requete);
                DataTable table = dataSet.Tables[0];

                foreach (DataRow restrictionAlimentaire in table.Rows)
                {
                    resultat.Add(ConstruireRestrictionAlimentaire(restrictionAlimentaire));
                }

            }
            catch (MySqlException)
            {
                throw;
            }

            return resultat;
        }

        /// <summary>
        /// Méthode permettant d'obtenir une restriction alimentaire de la base de données.
        /// </summary>
        /// <param name="args">Les arguments permettant de récupérer la restriction alimentaire.</param>
        /// <returns>Un objet RestrictionAlimentaire.</returns>
        public RestrictionAlimentaire Retrieve(RetrieveRestrictionAlimentaireArgs args)
        {

            RestrictionAlimentaire restrictionAlimentaire;

            try
            {
                connexion = new MySqlConnexion();

                string requete = string.Format("SELECT * FROM RestrictionsAlimentaires WHERE idRestrictionAlimentaire = {0}", args.IdRestrictionAlimentaire);

                DataSet dataSet = connexion.Query(requete);
                DataTable table = dataSet.Tables[0];

                restrictionAlimentaire = ConstruireRestrictionAlimentaire(table.Rows[0]);

            }
            catch (Exception)
            {
                throw;
            }

            return restrictionAlimentaire;

        }

        /// <summary>
        /// Méthode permettant de construire un objet RestrictionAlimentaire.
        /// </summary>
        /// <param name="restriction">Un enregistrement de la table RestrictionsAlimentaires.</param>
        /// <returns>Un objet RestrictionAlimentaire.</returns>
        private RestrictionAlimentaire ConstruireRestrictionAlimentaire(DataRow restriction)
        {
            return new RestrictionAlimentaire()
            {
                IdRestrictionAlimentaire = (int)restriction["idRestrictionAlimentaire"],
                Nom = (string)restriction["restrictionAlimentaire"]
            };
        }
    }
}