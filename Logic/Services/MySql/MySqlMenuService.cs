using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nutritia
{
    public class MySqlMenuService : IMenuService
    {
        private MySqlConnexion connexion;
        private readonly IPlatService platService;

        public MySqlMenuService()
        {
            platService = ServiceFactory.Instance.GetService<IPlatService>();
        }

        public IList<Menu> RetrieveAll(RetrieveMenuArgs args)
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

        // Ajouter les plats.
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

            }
            catch (MySqlException)
            {
                throw;
            }

            return menu;

        }

        private Menu ConstruireMenu(DataRow menu)
        {
            return new Menu()
            {
                IdMenu = (int)menu["idMenu"],
                DateCreation = (DateTime)menu["dateMenu"],
                ListePlats = new List<Plat>()
            };
        }
    }
}
