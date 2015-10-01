using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nutritia
{
    public class MySqlRestrictionAlimentaireService : IRestrictionAlimentaireService
    {

        private MySqlConnexion connexion;

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

        private RestrictionAlimentaire ConstruireRestrictionAlimentaire(DataRow res)
        {
            return new RestrictionAlimentaire()
            {
                // TODO : ENlever l'attribut estSelectionne des classes.
                IdRestrictionAlimentaire = (int)res["idRestrictionAlimentaire"],
                Nom = (string)res["restrictionAlimentaire"]
            };
        }
    }
}
