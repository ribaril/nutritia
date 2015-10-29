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
    /// Service MySql lié aux Menus.
    /// </summary>
    public class MySqlMenuService : IMenuService
    {
        private MySqlConnexion connexion;
        private readonly IPlatService platService;

        /// <summary>
        /// Constructeur par défaut de la classe.
        /// </summary>
        public MySqlMenuService()
        {
            platService = ServiceFactory.Instance.GetService<IPlatService>();
        }

        /// <summary>
        /// Méthode permettant d'obtenir un ensemble de menus sauvegardé dans la base de données.
        /// </summary>
        /// <param name="args">Les arguments permettant de retrouver les menus.</param>
        /// <returns>Une liste contenant les menus.</returns>
        public IList<Menu> RetrieveSome(RetrieveMenuArgs args)
        {
            IList<Menu> resultat = new List<Menu>();

            try
            {
                connexion = new MySqlConnexion();

                string requete = string.Format("SELECT * FROM Menus WHERE idMembre = {0}", args.IdMembre);

                if(args.DateMenu != DateTime.MinValue)
                {
                    requete += string.Format(" AND dateMenu = {0}", args.DateMenu);
                }

                DataSet dataSetMenus = connexion.Query(requete);
                DataTable tableMenus = dataSetMenus.Tables[0];

                foreach (DataRow rowMenu in tableMenus.Rows)
                {
                    Menu menu = ConstruireMenu(rowMenu);
                    
                    // Ajout des plats du menu.
                    requete = string.Format("SELECT * FROM MenusPlats WHERE idMenu = {0}", menu.IdMenu);

                    DataSet dataSetPlats = connexion.Query(requete);
                    DataTable tablePlats = dataSetPlats.Tables[0];

                    foreach (DataRow rowPlat in tablePlats.Rows)
                    {
                        menu.ListePlats.Add(platService.Retrieve(new RetrievePlatArgs{IdPlat = (int)rowPlat["idPlat"]}));
                    }

                    resultat.Add(menu);

                }

            }
            catch (MySqlException)
            {
                throw;
            }

            return resultat;

        }

        /// <summary>
        /// Méthode permettant d'obtenir un menu sauvegardé dans la base de données.
        /// </summary>
        /// <param name="args">Les arguments permettant de retrouver le menu.</param>
        /// <returns>Un objet Menu.</returns>
        public Menu Retrieve(RetrieveMenuArgs args)
        {
            Menu menu;

            try
            {
                connexion = new MySqlConnexion();

                string requete = string.Format("SELECT * FROM Menus WHERE idMembre = {0}", args.IdMembre);

                if (args.DateMenu != DateTime.MinValue)
                {
                    requete += string.Format(" AND dateMenu = {0}", args.DateMenu);
                }

                DataSet dataSetMenus = connexion.Query(requete);
                DataTable tableMenus = dataSetMenus.Tables[0];

                menu = ConstruireMenu(tableMenus.Rows[0]);

                // Ajout des plats du menu.
                requete = string.Format("SELECT * FROM MenusPlats WHERE idMenu = {0}", menu.IdMenu);

                DataSet dataSetPlats = connexion.Query(requete);
                DataTable tablePlats = dataSetPlats.Tables[0];

                foreach (DataRow rowPlat in tablePlats.Rows)
                {
                    menu.ListePlats.Add(platService.Retrieve(new RetrievePlatArgs { IdPlat = (int)rowPlat["idPlat"] }));
                }

            }
            catch (MySqlException)
            {
                throw;
            }

            return menu;

        }

        /// <summary>
        /// Méthode permettant de construire un objet Menu.
        /// </summary>
        /// <param name="menu">Un enregistrement de la table Menus.</param>
        /// <returns>Un objet Menu.</returns>
        private Menu ConstruireMenu(DataRow menu)
        {
            return new Menu()
            {
                IdMenu = (int)menu["idMenu"],
                Nom = (string)menu["nom"],
                DateCreation = (DateTime)menu["dateMenu"],
                ListePlats = new List<Plat>()
            };
        }
    }
}
