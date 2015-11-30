using Nutritia.Logic.Model.Entities;
using Nutritia.UI.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
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
        private IDonService donService;
        private int valeurDon;
        public bool isNomGood = false;
        public bool isNoCarteGood = false;
        public bool isExpirationGood = false;
        public bool isCSCGood = false;
        private ModePaiement modePaiement;

        public Don()
        {
            donService = ServiceFactory.Instance.GetService<IDonService>();
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
                MontreLabelErreur();
                if (isNomGood && isNoCarteGood && isExpirationGood && isCSCGood)
                {
                    Transaction transaction = new Transaction(txtProprietaire.Text, valeurDon, modePaiement);
                    if (String.IsNullOrWhiteSpace(App.MembreCourant.NomUtilisateur))
                        donService.Insert(transaction);
                    else
                        donService.Insert(App.MembreCourant, transaction);
                    VideChamps();

                    (new FenetreRecuDon(transaction)).ShowDialog();
                }
                else
                    SystemSounds.Beep.Play();
            }
        }

        [Obsolete]
        private void ValidationChamps()
        {
            //btnConfirmer.IsEnabled = isNomGood && isNoCarteGood && isExpirationGood && isCSCGood;
        }

        private void EnablingBouton()
        {
            bool areFieldsEmpty = (String.IsNullOrWhiteSpace(txtCSC.Text) || String.IsNullOrWhiteSpace(txtDateExpiration.Text) ||
                String.IsNullOrWhiteSpace(txtNoCarte.Text) || String.IsNullOrWhiteSpace(txtProprietaire.Text));

            btnConfirmer.IsEnabled = !areFieldsEmpty;
        }

        private void MontreLabelErreur()
        {
            if (!isNoCarteGood)
                lblErreurNoCarte.Visibility = Visibility.Visible;
            if (!isNomGood)
                lblErreurNom.Visibility = Visibility.Visible;
            if (!isExpirationGood)
                lblErreurExpiration.Visibility = Visibility.Visible;
            if (!isCSCGood)
                lblErreurCSC.Visibility = Visibility.Visible;
        }

        private void ValidationNom()
        {
            // https://stackoverflow.com/questions/2385701/regular-expression-for-first-and-last-name
            Regex regexNom = new Regex("^[a-z ,.'-]+$", RegexOptions.IgnoreCase);
            isNomGood = regexNom.IsMatch(txtProprietaire.Text);

            if (isNomGood)
            {
                lblErreurNom.Visibility = Visibility.Hidden; ;
            }
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

            if (isAmex)
            {
                modePaiement = ModePaiement.Amex;
            }
            else if (isVisa)
            {
                modePaiement = ModePaiement.Visa;
            }
            else if (isMastercard)
            {
                modePaiement = ModePaiement.MasterCard;
            }

            if (isMastercard || isVisa || isAmex)
            {
                isNoCarteGood = true;
                lblErreurNoCarte.Visibility = Visibility.Hidden;
            }
            else
            {
                isNoCarteGood = false;
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
                }
            }
            catch (System.FormatException e)
            {
                isExpirationGood = false;
            }


        }


        private void txtCSC_TextChanged(object sender, TextChangedEventArgs e)
        {
            ValidationCSC();
            ValidationChamps();
            EnablingBouton();
        }

        private void txtDateExpiration_TextChanged(object sender, TextChangedEventArgs e)
        {
            ValidationDateExpiration();
            ValidationChamps();
            EnablingBouton();
        }

        private void txtNoCarte_TextChanged(object sender, TextChangedEventArgs e)
        {
            ValidationNoCarte();
            ValidationChamps();
            EnablingBouton();
        }

        private void txtProprietaire_TextChanged(object sender, TextChangedEventArgs e)
        {
            ValidationNom();
            ValidationChamps();
            EnablingBouton();
        }

        private void VideChamps()
        {
            txtCSC.Text = String.Empty;
            txtDateExpiration.Text = String.Empty;
            txtNoCarte.Text = String.Empty;
            txtProprietaire.Text = String.Empty;
        }

        private void txtNo_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private void txtNo_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                String text = (String)e.DataObject.GetData(typeof(String));
                if (!IsTextAllowed(text))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }


        //http://stackoverflow.com/questions/1268552/how-do-i-get-a-textbox-to-only-accept-numeric-input-in-wpf
        private static bool IsTextAllowed(string text)
        {
            Regex regex = new Regex("[^0-9]+"); //regex that matches disallowed text
            return !regex.IsMatch(text);
        }

        //http://stackoverflow.com/questions/1268552/how-do-i-get-a-textbox-to-only-accept-numeric-input-in-wpf
        private static bool IsTextAllowedDate(string text)
        {
            Regex regex = new Regex("[^0-9/]+"); //regex that matches disallowed text
            return !regex.IsMatch(text);
        }

        private void txtDateExpiration_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowedDate(e.Text);
        }

        private void txtDateExpiration_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                String text = (String)e.DataObject.GetData(typeof(String));
                if (!IsTextAllowedDate(text))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }
    }
}
