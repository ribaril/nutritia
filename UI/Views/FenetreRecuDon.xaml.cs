using Nutritia.Logic.Model.Entities;
using Nutritia.Toolkit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Nutritia.UI.Views
{
    /// <summary>
    /// Fenêtre pour afficher un recu pour un don
    /// </summary>
    public partial class FenetreRecuDon : Window
    {
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="don">Objet Don pour initialiser les labels de la fenêtre</param>
        public FenetreRecuDon(Don don)
        {
            InitializeComponent();
            lblDate.Content = don.DateHeureTransaction.ToString("dd/MM/yy");
            lblHeure.Content = don.DateHeureTransaction.ToString("HH:mm");
            lblModePaiement.Content = don.ModePaiementTransaction.ToString();
            lblMontant.Content = don.Montant.ToString() + "$";
            lblNom.Content = don.NomAuteur;
            //lblNoTransaction.Content += " " + transaction.NoTransaction.ToString();
            imgQrCode.Source = QrCodeHelper.getQrBitmap(don.ToString());
        }
    }
}
