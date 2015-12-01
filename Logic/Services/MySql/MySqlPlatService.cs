using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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
        public MySqlPlatService()
        {
            alimentService = ServiceFactory.Instance.GetService<IAlimentService>();
        }

        /// <summary>
        /// Méthode permettant d'obtenir l'ensemble des plats sauvegardé dans la base de données.
        /// </summary>
        /// <returns>Une liste contenant les plats.</returns>
        public IList<Plat> RetrieveAll()
        {
            List<Plat> resultat = new List<Plat>();

            try
            {
                connexion = new MySqlConnexion();

                string requete = "SELECT * FROM Plats p INNER JOIN TypesPlats tp ON tp.idTypePlat = p.idTypePlat INNER JOIN Membres m ON m.idMembre = p.idMembre";

                DataSet dataSetPlats = connexion.Query(requete);
                DataTable tablePlats = dataSetPlats.Tables[0];

                foreach (DataRow rowPlat in tablePlats.Rows)
                {
                    Plat plat = ConstruirePlat(rowPlat);

                    plat.ListeIngredients = RetrieveAlimentsPlat(new RetrievePlatArgs { IdPlat = plat.IdPlat });

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

                string requete = "SELECT * FROM Plats p INNER JOIN TypesPlats tp ON tp.idTypePlat = p.idTypePlat INNER JOIN Membres m ON m.idMembre = p.idMembre";

                if (args.Categorie != null && args.Categorie != string.Empty)
                {
                    requete += string.Format(" WHERE typePlat = '{0}'", args.Categorie);
                }

                if (args.NbResultats != null)
                {
                    if (args.PlusPopulaires == true)
                    {
                        requete += " ORDER BY note DESC ";
                    }
                    else if (args.PlusPopulaires == false)
                    {
                        requete += " ORDER BY note ASC ";
                    }
                    else if (args.PlusPopulaires == null)
                    {
                        if (args.Depart != null && args.Depart != string.Empty)
                        {
                            requete += " ORDER BY idPlat ";

                            if (args.Depart == "Fin")
                            {
                                requete += "DESC";
                            }
                            else
                            {
                                requete += "ASC";
                            }
                        }
                    }

                    requete += string.Format(" LIMIT {0} ", args.NbResultats);

                }

                DataSet dataSetPlats = connexion.Query(requete);
                DataTable tablePlats = dataSetPlats.Tables[0];

                foreach (DataRow rowPlat in tablePlats.Rows)
                {
                    Plat plat = ConstruirePlat(rowPlat);

                    plat.ListeIngredients = RetrieveAlimentsPlat(new RetrievePlatArgs { IdPlat = plat.IdPlat });

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
                string requete = string.Format("SELECT * FROM Plats p INNER JOIN TypesPlats tp ON tp.idTypePlat = p.idTypePlat INNER JOIN Membres m ON m.idMembre = p.idMembre WHERE idPlat = '{0}'", args.IdPlat);

                DataSet dataSetPlats = connexion.Query(requete);
                DataTable tablePlats = dataSetPlats.Tables[0];

                plat = ConstruirePlat(tablePlats.Rows[0]);

                plat.ListeIngredients = RetrieveAlimentsPlat(new RetrievePlatArgs { IdPlat = plat.IdPlat });

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
                alimentTmp.Quantite = (double)rowPlatsAliments["quantite"] * alimentTmp.Mesure;
                listeIngredients.Add(alimentTmp);
            }

            return listeIngredients;
        }

        /// <summary>
        /// Méthode de mise à jour d'un plat dans la base de données.
        /// </summary>
        /// <param name="unAliment"></param>
        public void Update(Plat unPlat)
        {
            try
            {
                connexion = new MySqlConnexion();

                string requeteTypePlat = string.Format("SELECT * FROM TypesPlats WHERE typePlat = '{0}'", unPlat.TypePlat);

                DataSet dataSetTypes = connexion.Query(requeteTypePlat);
                DataTable tableTypes = dataSetTypes.Tables[0];

                int idType = 0;

                foreach (DataRow rowType in tableTypes.Rows)
                {
                    idType = (int)rowType["idTypePlat"];
                }

                string note = unPlat.Note.ToString();

                if (note.Contains(","))
                {
                    note = note.Replace(",", ".");
                }

                string requeteUpdate = string.Format("UPDATE Plats SET idTypePlat = {0}, nom = '{1}', imageUrl = '{2}', note = '{3}', nbVotes = {4}, description = '{5}' WHERE idPlat = {6}", idType, unPlat.Nom.Replace("'", "''"), unPlat.ImageUrl, note, unPlat.NbVotes, unPlat.Description, unPlat.IdPlat);
                connexion.Query(requeteUpdate);

                int idPlat = (int)unPlat.IdPlat;

                string requeteDelete = string.Format("DELETE FROM PlatsAliments WHERE idPlat = {0}", idPlat);
                connexion.Query(requeteDelete);

                for (int i = 0; i < unPlat.ListeIngredients.Count; i++)
                {
                    string requeteInsertAlimentPlat = string.Format("INSERT INTO PlatsAliments (idPlat, idAliment, quantite) VALUES ({0}, {1}, {2})", idPlat, unPlat.ListeIngredients[i].IdAliment, unPlat.ListeIngredients[i].Quantite);
                    connexion.Query(requeteInsertAlimentPlat);
                }

            }
            catch (Exception)
            {
                throw;
            }
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
                Description = (string)plat["description"],
                TypePlat = (string)plat["typePlat"],
                Note = note,
                NbVotes = (int)plat["nbVotes"],
                ImageUrl = (string)plat["imageUrl"]
            };
        }

        /// <summary>
        /// Méthode d'insertion d'un nouveau plat dans la base de données.
        /// </summary>
        /// <param name="unAliment"></param>
        public void Insert(Plat unPlat)
        {
            try
            {
                connexion = new MySqlConnexion();

                string requeteTypePlat = string.Format("SELECT * FROM TypesPlats WHERE typePlat = '{0}'", unPlat.TypePlat);

                DataSet dataSetTypes = connexion.Query(requeteTypePlat);
                DataTable tableTypes = dataSetTypes.Tables[0];

                int idType = 0;

                foreach (DataRow rowType in tableTypes.Rows)
                {
                    idType = (int)rowType["idTypePlat"];
                }

                string requeteCreateur = string.Format("SELECT * FROM Membres WHERE nomUtilisateur = '{0}'", unPlat.Createur);

                DataSet dataSetCreateur = connexion.Query(requeteCreateur);
                DataTable tableCreateur = dataSetCreateur.Tables[0];

                int idMembre = 0;

                foreach (DataRow rowCreateur in tableCreateur.Rows)
                {
                    idMembre = (int)rowCreateur["idMembre"];
                }

                string requeteInsert = string.Format("INSERT INTO Plats (idMembre, idTypePlat, nom, description, imageUrl, dateAjout) VALUES ({0}, {1}, '{2}', '{3}', '{4}', '{5}')", idMembre, idType, unPlat.Nom.Replace("'", "''"), unPlat.Description, unPlat.ImageUrl, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                connexion.Query(requeteInsert);

                string requetePlat = string.Format("SELECT * FROM Plats WHERE nom = '{0}'", unPlat.Nom.Replace("'", "''"));

                DataSet dataSetPlat = connexion.Query(requetePlat);
                DataTable tablePlat = dataSetPlat.Tables[0];

                int idPlat = 0;

                foreach (DataRow rowPlat in tablePlat.Rows)
                {
                    idPlat = (int)rowPlat["idPlat"];
                }

                for (int i = 0; i < unPlat.ListeIngredients.Count; i++)
                {
                    string requeteInsertAlimentPlat = string.Format("INSERT INTO PlatsAliments (idPlat, idAliment, quantite) VALUES ({0}, {1}, {2})", idPlat, unPlat.ListeIngredients[i].IdAliment, unPlat.ListeIngredients[i].Quantite);
                    connexion.Query(requeteInsertAlimentPlat);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
