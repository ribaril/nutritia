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
    /// Logique d'interaction pour FenetreVote.xaml
    /// </summary>
    public partial class FenetreVote : Window
    {
        /// <summary>
        /// Constructeur par défaut de la classe.
        /// </summary>
        public FenetreVote()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Événement lancé sur un clique du bouton Confirmer.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAnnuler_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Événement lancé sur un clique du bouton Confirmer.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnConfirmer_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Fonctionnalité à venir dans la version 1.");
        }
    }
}
