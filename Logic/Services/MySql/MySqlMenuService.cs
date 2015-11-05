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
            Menu menu = new Menu();

            try
            {
                connexion = new MySqlConnexion();

                string requete = string.Format("SELECT * FROM Menus WHERE Nom = '{0}'", args.Nom);

                DataSet dataSetMenus = connexion.Query(requete);
                DataTable tableMenus = dataSetMenus.Tables[0];

                if(tableMenus.Rows.Count != 0)
                {
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
            }
            catch (MySqlException)
            {
                throw;
            }

            return menu;

        }

        /// <summary>
        /// Méthode permettant d'insérer un menu dans la base de données.
        /// </summary>
        /// <param name="menu">Le menu à insérer.</param>
        public void Insert(Menu menu)
        {
            try
            {
                connexion = new MySqlConnexion();

                string requete = string.Format("INSERT INTO Menus (idMembre, nom, nbPersonnes, dateMenu) VALUES ({0}, '{1}', {2}, '{3}')", App.MembreCourant.IdMembre, menu.Nom, menu.NbPersonnes, menu.DateCreation.ToString("yyyy-MM-dd"));
                connexion.Query(requete);

                foreach(Plat platCourant in menu.ListePlats)
                {
                    requete = string.Format("INSERT INTO MenusPlats (idMenu, idPlat) VALUES ({0}, {1})", Retrieve(new RetrieveMenuArgs { Nom = menu.Nom }).IdMenu,  platCourant.IdPlat);
                    connexion.Query(requete);
                }
            }
            catch(MySqlException)
            {
                throw;
            }
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
                NbPersonnes = (int)menu["nbPersonnes"],
                DateCreation = (DateTime)menu["dateMenu"],
                ListePlats = new List<Plat>()
            };
        }
    }
}
