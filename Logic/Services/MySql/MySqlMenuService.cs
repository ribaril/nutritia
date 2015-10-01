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

                foreach (DataRow menu in tableMenus.Rows)
                {
                    resultat.Add(ConstruireMenu(menu));
                }

            }
            catch (MySqlException)
            {
                throw;
            }

            return resultat;

        }

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
                DateCreation = (DateTime)menu["dateMenu"]
            };
        }
    }
}
