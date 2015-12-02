using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Nutritia
{
    public class MySqlSuiviPlatService : ISuiviPlatService
    {
        private MySqlConnexion connexion;
        private readonly IPlatService platService;

        /// <summary>
        /// Constructeur par défaut de la classe.
        /// </summary>
        public MySqlSuiviPlatService ()
	    {
            platService = new MySqlPlatService();
	    }

        /// <summary>
        /// Méthode permettant d'obtenir un ensemble de plats sauvegardé dans la base de données.
        /// </summary>
        /// <param name="args">Les arguments permettant de retrouver les plats.</param>
        /// <returns>Une liste contenant les plats.</returns>
        public IList<Plat> RetrieveSome(RetrieveSuiviPlatArgs args)
        {
            List<Plat> resultat = new List<Plat>();

            try
            {
                connexion = new MySqlConnexion();

                string requete = string.Format("SELECT * FROM SuiviPlats WHERE idMembre = {0}", args.IdMembre);

                DataSet dataSetPlats = connexion.Query(requete);
                DataTable tablePlats = dataSetPlats.Tables[0];

                foreach (DataRow rowPlat in tablePlats.Rows)
                {
                    Plat plat = platService.Retrieve(new RetrievePlatArgs { IdPlat = ((int)rowPlat["idPlat"]) });

                    plat.EstTricherie = ((bool)rowPlat["estTricherie"]);

                    resultat.Add(plat);
                }
            }
            catch (Exception)
            {
                throw;
            }

            return resultat;
        }

        /// <summary>
        /// Méthode permettant d'insérer le suivi des plats d'un membre.
        /// </summary>
        /// <param name="listePlats">La liste des plats.</param>
        /// <param name="membre">Le membre correspondant.</param>
        public void Insert(IList<Plat> listePlats, Membre membre)
        {
            try
            {
                connexion = new MySqlConnexion();

                foreach (Plat plat in listePlats)
                {
                    string requete = string.Format("INSERT INTO SuiviPlats (idMembre, idPlat, estTricherie) VALUES ({0}, {1}, {2})", membre.IdMembre, plat.IdPlat, plat.EstTricherie);
                    connexion.Query(requete);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Méthode permettant de mettre à jour le suivi des plats d'un membre.
        /// </summary>
        /// <param name="listePlats">La liste des plats.</param>
        /// <param name="membre">Le membre correspondant.</param>
        public void Update(IList<Plat> listePlats, Membre membre)
        {
            try
            {
                connexion = new MySqlConnexion();

                string requeteEffacerSuivi = string.Format("DELETE FROM SuiviPlats WHERE idMembre = {0}", membre.IdMembre);
                connexion.Query(requeteEffacerSuivi);

                Insert(listePlats, membre);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
