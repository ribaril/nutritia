using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Nutritia.UI.Pages
{
    /// <summary>
    /// Interaction logic for Don.xaml
    /// </summary>
    public partial class Don : Page
    {
        private int valeurDon;
        public bool isNomGood = false;
        public bool isNoCarteGood = false;
        public bool isExpirationGood = false;
        public bool isCSCGood = false;

        public Don()
        {
            InitializeComponent();
        }

        private void btnConfirmer_Click(object sender, RoutedEventArgs e)
        {
            RadioButton btnChecked = wrapMontant.Children.OfType<RadioButton>().FirstOrDefault(r => r.IsChecked == true);

            int valeurRadio;
            string stringContent = btnChecked.Content.ToString();
            bool IsInt = int.TryParse(stringContent.Remove(stringContent.Length - 1), out valeurRadio);
            if (IsInt)
            {
                valeurDon = valeurRadio;
                MessageBox.Show("Merci pour le don", "Don - Confirmation");

            }
        }

        private void ValidationChamps()
        {
            btnConfirmer.IsEnabled = isNomGood && isNoCarteGood && isExpirationGood && isCSCGood;
        }


        private void ValidationNom()
        {
            // https://stackoverflow.com/questions/2385701/regular-expression-for-first-and-last-name
            Regex regexNom = new Regex("^[a-z ,.'-]+$", RegexOptions.IgnoreCase);
            isNomGood = regexNom.IsMatch(txtProprietaire.Text);

            if (!isNomGood)
            {
                lblErreurNom.Visibility = Visibility.Visible;
            }
            else
                lblErreurNom.Visibility = Visibility.Hidden;
        }

        private void ValidationNoCarte()
        {
            // http://www.regular-expressions.info/creditcard.html
            Regex regexMasterCard = new Regex("^5[1-5][0-9]{14}$");
            Regex regexVisa = new Regex("^4[0-9]{12}(?:[0-9]{3})?$");
            Regex regexAmex = new Regex("^3[47][0-9]{13}$");
            string txtNo = txtNoCarte.Text;
            bool isMastercard = regexMasterCard.IsMatch(txtNo);
            bool isVisa = regexVisa.IsMatch(txtNo);
            bool isAmex = regexAmex.IsMatch(txtNo);

            imgAmex.IsEnabled = isAmex;
            imgVisa.IsEnabled = isVisa;
            imgMasterCard.IsEnabled = isMastercard;

            if (isMastercard || isVisa || isAmex)
            {
                isNoCarteGood = true;
                lblErreurNoCarte.Visibility = Visibility.Hidden;
            }
            else
            {
                isNoCarteGood = false;
                lblErreurNoCarte.Visibility = Visibility.Visible;
            }
        }

        private void ValidationCSC()
        {
            Regex regexCSC = new Regex("^[0-9]{3}$");
            if (regexCSC.IsMatch(txtCSC.Text))
            {
                isCSCGood = true;
                lblErreurCSC.Visibility = Visibility.Hidden;
            }
            else
            {
                isCSCGood = false;
                lblErreurCSC.Visibility = Visibility.Visible;
            }
        }

        private void ValidationDateExpiration()
        {
            DateTime today = DateTime.Now;
            DateTime dateExpiration;
            Regex regexDate = new Regex("^[0-9]{2}/[0-9]{2}$");

            Console.WriteLine(regexDate.IsMatch(txtDateExpiration.Text));
            string stringDate = (regexDate.Match(txtDateExpiration.Text)).Value;
            Console.WriteLine(stringDate);
            try
            {
                dateExpiration = DateTime.ParseExact(stringDate, "MM/yy", App.culture);
                Console.WriteLine(dateExpiration);
                //Expire au début du mois indiqué, donc pas de égal.
                if (dateExpiration > today)
                {
                    isExpirationGood = true;
                    lblErreurExpiration.Visibility = Visibility.Hidden;
                }
                else
                {
                    isExpirationGood = false;
                    lblErreurExpiration.Visibility = Visibility.Visible;
                }
            }
            catch (System.FormatException e)
            {
                isExpirationGood = false;
                lblErreurExpiration.Visibility = Visibility.Visible;
            }


        }

        private void txtProprietaire_LostFocus(object sender, RoutedEventArgs e)
        {
            //ValidationNom();
        }

        private void txtNoCarte_LostFocus(object sender, RoutedEventArgs e)
        {
            //ValidationNoCarte();
        }

        private void txtDateExpiration_LostFocus(object sender, RoutedEventArgs e)
        {
            // ValidationDateExpiration();
        }

        private void txtCSC_LostFocus(object sender, RoutedEventArgs e)
        {
            //ValidationCSC();
        }

        private void txtCSC_TextChanged(object sender, TextChangedEventArgs e)
        {
            ValidationCSC();
            ValidationChamps();
        }

        private void txtDateExpiration_TextChanged(object sender, TextChangedEventArgs e)
        {
            ValidationDateExpiration();
            ValidationChamps();
        }

        private void txtNoCarte_TextChanged(object sender, TextChangedEventArgs e)
        {
            ValidationNoCarte();
            ValidationChamps();
        }

        private void txtProprietaire_TextChanged(object sender, TextChangedEventArgs e)
        {
            ValidationNom();
            ValidationChamps();
        }

    }
}
