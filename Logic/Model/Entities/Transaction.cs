using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nutritia.Logic.Model.Entities
{

    public class Transaction
    {

        private Guid noTransaction;

        public static uint ProchainNoTransaction { get; private set; }

        public Guid NoTransaction { get { return noTransaction; } }

        public DateTime DateHeureTransaction { get; private set; }
        public string NomAuteur { get; private set; }
        public double Montant { get; private set; }
        public ModePaiement ModePaiementTransaction { get; private set; }

        public Transaction()
        {
            noTransaction = Guid.NewGuid();
        }

        public Transaction(string nom, double montant, ModePaiement modePaiement)
            : this()
        {
            NomAuteur = nom;
            Montant = montant;
            ModePaiementTransaction = modePaiement;
        }

        public Transaction(string nom, double montant, ModePaiement modePaiement, DateTime dateHeure)
            : this(nom, montant, modePaiement)
        {
            DateHeureTransaction = dateHeure;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Don Nutritia Inc.");
            sb.AppendLine();
            sb.Append("No. Transaction: ");
            sb.Append(noTransaction);
            sb.AppendLine();
            sb.Append("Propriétaire de la carte : ");
            sb.Append(NomAuteur);
            sb.AppendLine();
            sb.Append("Montant: ");
            sb.Append(Montant);
            sb.Append("$");
            sb.AppendLine();
            sb.Append("Mode de paiement: ");
            sb.Append(ModePaiementTransaction);
            sb.AppendLine();
            sb.Append("Date de transaction: ");
            sb.Append(DateHeureTransaction);
            sb.AppendLine();

            return sb.ToString();
        }
    }
}
