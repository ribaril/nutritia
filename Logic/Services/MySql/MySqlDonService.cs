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
                string requete = string.Format("INSERT INTO Dons (idModePaiement, nom, montant, noTransaction) VALUES ( (SELECT idModePaiement FROM ModesPaiement WHERE nom = '{0}'), '{1}', {2}, '{3}')", don.ModePaiementTransaction, don.NomAuteur, don.Montant, don.NoTransaction);
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
            float Montant = (float)don["montant"];
            string NomAuteur = (string)don["Auteur"];
            ModePaiement mode = ModePaiement.StringToValue((string)don["ModePaiement"]);

            return new Transaction(NomAuteur, Montant, mode);
        }


        public void Insert(Membre membre, Transaction transaction)
        {
            Insert(transaction);

            try
            {
                connexion = new MySqlConnexion();
                string requete = string.Format("INSERT INTO DonsMembres (idMembre, idDon) VALUES ( (SELECT idMembre FROM Membres WHERE nomUtilisateur = '{0}'), (SELECT idDon FROM Dons WHERE noTransaction = '{1}')) ", membre.NomUtilisateur, transaction.NoTransaction);
                connexion.Query(requete);
            }
            catch (MySqlException)
            {
                throw;
            }
        }

        public DateTime LastTimeDon()
        {
            DateTime last = DateTime.MinValue;
            try
            {
                connexion = new MySqlConnexion();
                string requete = " SELECT * FROM LastTimeDon";

                DataSet dataSetDon = connexion.Query(requete);
                DataTable tableDon = dataSetDon.Tables[0];
                if (!tableDon.Rows[0].IsNull("derniereMaj"))
                    last = (DateTime)tableDon.Rows[0]["derniereMaj"];
            }
            catch (MySqlException)
            {
                throw;
            }
            return last;
        }
    }
}
