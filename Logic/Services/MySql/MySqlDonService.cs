using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Nutritia.Logic.Model.Entities;

namespace Nutritia
{
    public class MySqlDonService : IDonService
    {
        private MySqlConnexion connexion;

        public IList<Transaction> RetrieveAll()
        {
            IList<Transaction> resultat = new List<Transaction>();

            try
            {
                connexion = new MySqlConnexion();
                string requete = "SELECT * FROM AllDons";

                DataSet dataSetDon = connexion.Query(requete);
                DataTable tableDon = dataSetDon.Tables[0];

                foreach (DataRow don in tableDon.Rows)
                {
                    resultat.Add(ConstruireDon(don));
                }
            }
            catch (MySqlException)
            {
                throw;
            }
            return resultat;
        }

        public void Insert(Transaction don)
        {
            try
            {
                connexion = new MySqlConnexion();
                string requete = string.Format("INSERT INTO Dons (idModePaiement, nom, montant) VALUES ( (SELECT idModePaiement FROM ModesPaiement WHERE nom = '{0}'), '{1}', {2})", don.ModePaiementTransaction, don.NomAuteur, don.Montant);
                connexion.Query(requete);
            }
            catch (MySqlException)
            {
                throw;
            }
        }

        private Transaction ConstruireDon(DataRow don)
        {
            DateTime DateHeureTransaction = (DateTime)don["dateDon"];
            float Montant = (int)don["montant"];
            string NomAuteur = (string)don["Auteur"];
            ModePaiement mode = ModePaiement.StringToValue((string)don["ModePaiement"]);

            return new Transaction(NomAuteur, Montant, mode);
        }

    }
}
