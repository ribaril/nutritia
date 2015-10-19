using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nutritia
{
    /// <summary>
    /// Service MySql lié aux Plats.
    /// </summary>
    public class MySqlPlatService : IPlatService
    {
        private MySqlConnexion connexion;
        private readonly IAlimentService alimentService;

        /// <summary>
        /// Constructeur par défaut de la classe.
        /// </summary>
        public MySqlPlatService ()
	    {
            alimentService = ServiceFactory.Instance.GetService<IAlimentService>();
	    }

        /// <summary>
        /// Méthode permettant d'obtenir un ensemble de plats sauvegardé dans la base de données.
        /// </summary>
        /// <param name="args">Les arguments permettant de retrouver les plats.</param>
        /// <returns>Une liste contenant les plats.</returns>
        public IList<Plat> RetrieveSome(RetrievePlatArgs args)
        {
            List<Plat> resultat = new List<Plat>();

            try
            {
                connexion = new MySqlConnexion();

                string requete = string.Format("SELECT * FROM Plats p INNER JOIN TypesPlats tp ON tp.idTypePlat = p.idTypePlat INNER JOIN Membres m ON m.idMembre = p.idMembre WHERE typePlat = '{0}'", args.Categorie);

                DataSet dataSetPlats = connexion.Query(requete);
                DataTable tablePlats = dataSetPlats.Tables[0];

                foreach (DataRow rowPlat in tablePlats.Rows)
                {
                    resultat.Add(ConstruirePlat(rowPlat));
                }
            }
            catch (Exception)
            {
                throw;
            }

            return resultat;
        }

        /// <summary>
        /// Méthode permettant d'obtenir un plat sauvegardé dans la base de données.
        /// </summary>
        /// <param name="args">Les arguments permettant de retrouver le plat.</param>
        /// <returns>Un objet Plat.</returns>
        public Plat Retrieve(RetrievePlatArgs args)
        {
            Plat plat;

            try
            {
                connexion = new MySqlConnexion();


                string requete = string.Format("SELECT * FROM Plats p INNER JOIN TypesPlats tp ON tp.idTypePlat = p.idTypePlat INNER JOIN Membres m ON m.idMembre = p.idMembre WHERE typePlat = '{0}'", args.Categorie);

                DataSet dataSetPlats = connexion.Query(requete);
                DataTable tablePlats = dataSetPlats.Tables[0];

                plat = ConstruirePlat(tablePlats.Rows[0]);

            }
            catch (Exception)
            {
                throw;
            }

            return plat;
        }

        /// <summary>
        /// Méthode permettant d'obtenir les aliments d'un plat.
        /// </summary>
        /// <param name="args">Les arguments permettant de retrouver les aliments du plat.</param>
        /// <returns>Une liste contenant les aliments.</returns>
        public IList<Aliment> RetrieveAlimentsPlat(RetrievePlatArgs args)
        {
            List<Aliment> listeIngredients = new List<Aliment>();

            string requete = string.Format("SELECT * FROM PlatsAliments WHERE idPlat = {0}", args.IdPlat);

            DataSet dataSetPlatsAliments = connexion.Query(requete);
            DataTable tablePlatsAliments = dataSetPlatsAliments.Tables[0];

            foreach (DataRow rowPlatsAliments in tablePlatsAliments.Rows)
            {
                Aliment alimentTmp = alimentService.Retrieve(new RetrieveAlimentArgs { IdAliment = (int)rowPlatsAliments["idAliment"] });
                alimentTmp.Quantite = (double)rowPlatsAliments["quantite"];
                listeIngredients.Add(alimentTmp);
            }

            return listeIngredients;
        }

        /// <summary>
        /// Méthode permettant de construire un objet Plat.
        /// </summary>
        /// <param name="plat">Un enregistrement de la table Plats.</param>
        /// <returns>Un objet Plat.</returns>
        private Plat ConstruirePlat(DataRow plat)
        {
            double? note = null;

            if (!(plat["note"] is DBNull))
            {
                note = (double?)plat["note"];
            }

            return new Plat()
            {
                IdPlat = (int)plat["idPlat"],
                Createur = (string)plat["nomUtilisateur"],
                Nom = (string)plat["nom"],
                TypePlat = (string)plat["typePlat"],
                Note = note,
                ImageUrl = (string)plat["imageUrl"],
                ListeIngredients = new List<Aliment>()
            };
        }
    }
}
