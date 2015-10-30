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
    /// Logique d'interaction pour FenetreSauvegarderMenu.xaml
    /// </summary>
    public partial class FenetreSauvegarderMenu : Window
    {
        /// <summary>
        /// Constructeur par défaut de la classe.
        /// </summary>
        public FenetreSauvegarderMenu()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Méthode permettant de valider le nom du menu.
        /// </summary>
        /// <returns>Vrai si valide. Faux dans le cas contraire.</returns>
        private bool ValiderNom()
        {
            string nom = txtNom.Text;

            if(!string.IsNullOrEmpty(nom))
            {
                if(nom.Length < 2 || nom.Length > 15)
                {
                    return false;
                }
                else
                {
                    if (!string.IsNullOrEmpty(ServiceFactory.Instance.GetService<IMenuService>().Retrieve(new RetrieveMenuArgs { Nom = nom }).Nom))
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Événement lancé sur un clique du bouton Sauvegarder.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSauvegarder_Click(object sender, RoutedEventArgs e)
        {
            if(ValiderNom())
            {
                DialogResult = true;
                Close();
            }
            else
            {
                lblNom.Foreground = Brushes.Red;
            }
        }
    }
}
