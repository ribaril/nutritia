using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace Nutritia
{
    public class MySqlVersionLogicielService : IVersionLogicielService
    {
        private MySqlConnexion connexion;

        private VersionLogiciel ConstruireVersionLogiciel(DataRow version)
        {
            return new VersionLogiciel(version["version"].ToString(), version["changelog"].ToString(), version["downloadLink"].ToString(), (DateTime)version["datePublication"]);
        }

        public VersionLogiciel RetrieveLatest()
        {
            try
            {
                VersionLogiciel latestVersion;
                connexion = new MySqlConnexion();
                string requete = "SELECT * FROM CurrentVersion";

                DataSet dataSetVersionsLogiciel = connexion.Query(requete);
                DataTable tableVersionsLogiciel = dataSetVersionsLogiciel.Tables[0];

                latestVersion = ConstruireVersionLogiciel(tableVersionsLogiciel.Rows[0]);
                return latestVersion;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                Console.WriteLine(e.Message);
                return new VersionLogiciel();
            }

        }
    }
}
