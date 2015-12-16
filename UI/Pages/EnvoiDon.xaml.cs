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
    /// Page pour l'envoi de dons
    /// </summary>
    public partial class EnvoiDon : Page
    {
        private IDonService donService;
        private int valeurDon;
        public bool isNomGood = false;
        public bool isNoCarteGood = false;
        public bool isExpirationGood = false;
        public bool isCSCGood = false;
        private ModePaiement modePaiement;

        public EnvoiDon()
        {
            donService = ServiceFactory.Instance.GetService<IDonService>();
            InitializeComponent();
        }



        private void btnConfirmer_Click(object sender, RoutedEventArgs e)
        {
            //Récupère tout les enfants de type RadioButton de wrapMontant et enregistre selui sélectionné dans btnChecked.
            RadioButton btnChecked = wrapMontant.Children.OfType<RadioButton>().FirstOrDefault(r => r.IsChecked == true);
            int valeurRadio;
            //Prend la valeur en string du contenu du radioButton.
            string stringContent = btnChecked.Content.ToString();
            //Convertir le string en int, c'est le montant du don, enlève le dernier caractère car c'est le signe de dollard.
            bool IsInt = int.TryParse(stringContent.Remove(stringContent.Length - 1), out valeurRadio);
            if (IsInt)
            {
                valeurDon = valeurRadio;
                //Méthode qui affiche les labels de messages d'erreurs au dessus des champs avec des données erronnées. 
                MontreLabelErreur();
                //Si tout les champs sont valides
                if (isNomGood && isNoCarteGood && isExpirationGood && isCSCGood)
                {
                    //Crée un nouveau objet Don avec la valeur des champs et le mode de paiement (le type de carte détecté).
                    Don don = new Don(txtProprietaire.Text, valeurDon, modePaiement);
                    //Si l'utilisateur n'est pas connecté à un compte, fait juste insérer le don dans la base de données avec le service.
                    if (String.IsNullOrWhiteSpace(App.MembreCourant.NomUtilisateur))
                        donService.Insert(don);
                    else
                        //Sinon appel la méthode qui peut associer le compte membre avec son don.
                        donService.Insert(App.MembreCourant, don);
                    //Méthode qui vide tout les champs de données.
                    VideChamps();
                    //Affiche la fenêtre de reçu du don qui viend d'être envoyé.
                    (new FenetreRecuDon(don)).ShowDialog();
                }
                else
                    //Si au moins un champ n'est pas valide, joue un beep d'erreur de microsoft.
                    SystemSounds.Beep.Play();
            }
        }

        /// <summary>
        /// Méthode qui active le bouton pour envoyer le don lorsque tout les champs sont remplis
        /// </summary>
        private void EnablingBouton()
        {
            bool areFieldsEmpty = (String.IsNullOrWhiteSpace(txtCSC.Text) || String.IsNullOrWhiteSpace(txtDateExpiration.Text) ||
                String.IsNullOrWhiteSpace(txtNoCarte.Text) || String.IsNullOrWhiteSpace(txtProprietaire.Text));

            btnConfirmer.IsEnabled = !areFieldsEmpty;
        }

        /// <summary>
        /// Méthode qui affiche le label d'erreur correspondant au champ de donnée non valide
        /// </summary>
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

        /// <summary>
        /// Méthode pour valider le champ du nom de propriétaire de la carte de crédit
        /// </summary>
        private void ValidationNom()
        {
            // https://stackoverflow.com/questions/2385701/regular-expression-for-first-and-last-name
            Regex regexNom = new Regex("^[a-z ,.'-]+$", RegexOptions.IgnoreCase);
            isNomGood = regexNom.IsMatch(txtProprietaire.Text);
            //S'il passe la validation, on cache de nouveau le label d'erreur associé au champ.
            if (isNomGood)
            {
                lblErreurNom.Visibility = Visibility.Hidden;
            }
        }

        /// <summary>
        /// Méthode pour valider le champ du numéro de carte de crédit
        /// </summary>
        private void ValidationNoCarte()
        {
            //Utilise les trois regex connues pour reconnaitre le type de carte entre MasterCard, Visa et American Express.
            // http://www.regular-expressions.info/creditcard.html
            Regex regexMasterCard = new Regex("^5[1-5][0-9]{14}$");
            Regex regexVisa = new Regex("^4[0-9]{12}(?:[0-9]{3})?$");
            Regex regexAmex = new Regex("^3[47][0-9]{13}$");
            string txtNo = txtNoCarte.Text;
            //Passe à travers les trois regex pour essayer de trouver un match.
            bool isMastercard = regexMasterCard.IsMatch(txtNo);
            bool isVisa = regexVisa.IsMatch(txtNo);
            bool isAmex = regexAmex.IsMatch(txtNo);
            //Désactive les images de carte de crédit selon le match trouvé. Pour faire ressortir le type de carte détecté.
            imgAmex.IsEnabled = isAmex;
            imgVisa.IsEnabled = isVisa;
            imgMasterCard.IsEnabled = isMastercard;

            //Met la valeur de modePaiement selon le type de carte détecté avec notre classe singleton ModePaiement.
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

            //Le type est détecté, donc on cache le label d'erreur associé au champ.
            if (isMastercard || isVisa || isAmex)
            {
                isNoCarteGood = true;
                lblErreurNoCarte.Visibility = Visibility.Hidden;
            }
            //Type non détecté.
            else
            {
                isNoCarteGood = false;
            }
        }

        /// <summary>
        /// Méthode qui valide le code de sécurité de la carte de crédit
        /// </summary>
        private void ValidationCSC()
        {
            //Fait juste vérifier si c'est 3 numéro avec une regex.
            Regex regexCSC = new Regex("^[0-9]{3}$");
            if (regexCSC.IsMatch(txtCSC.Text))
            {
                //Valide, donc cache le label d'erreur associé au champ.
                isCSCGood = true;
                lblErreurCSC.Visibility = Visibility.Hidden;
            }
            else
            {
                isCSCGood = false;
            }
        }

        /// <summary>
        /// Méthode pour valider la date d'expiration de la carte de crédit
        /// </summary>
        private void ValidationDateExpiration()
        {
            DateTime today = DateTime.Now;
            DateTime dateExpiration;
            //Regex du style: dd/dd (d pour digit)
            Regex regexDate = new Regex("^[0-9]{2}/[0-9]{2}$");

            //Filtration initial inutile. La regex devrait être enlevé à l'avenir.
            string stringDate = (regexDate.Match(txtDateExpiration.Text)).Value;
            try
            {
                //On essaie de parser stringDate en DateTime avec le format MM/yy
                //Si le parse échoue, on tombe dans le catch géré.
                dateExpiration = DateTime.ParseExact(stringDate, "MM/yy", App.culture);

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
            //Mauvais format de DateTime.
            catch (System.FormatException)
            {
                isExpirationGood = false;
            }


        }

        /// <summary>
        /// Méthode pour valider à chaque caractère écrit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtCSC_TextChanged(object sender, TextChangedEventArgs e)
        {
            ValidationCSC();
            EnablingBouton();
        }

        /// <summary>
        /// Méthode pour valider à chaque caractère écrit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtDateExpiration_TextChanged(object sender, TextChangedEventArgs e)
        {
            ValidationDateExpiration();
            EnablingBouton();
        }

        /// <summary>
        /// Méthode pour valider à chaque caractère écrit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtNoCarte_TextChanged(object sender, TextChangedEventArgs e)
        {
            ValidationNoCarte();
            EnablingBouton();
        }

        /// <summary>
        /// Méthode pour valider à chaque caractère écrit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtProprietaire_TextChanged(object sender, TextChangedEventArgs e)
        {
            ValidationNom();
            EnablingBouton();
        }

        /// <summary>
        /// Méthode qui vide tout les champs de données et fait réactiver les images des cartes de crédits
        /// </summary>
        private void VideChamps()
        {
            txtCSC.Text = String.Empty;
            txtDateExpiration.Text = String.Empty;
            txtNoCarte.Text = String.Empty;
            txtProprietaire.Text = String.Empty;
            imgAmex.IsEnabled = true;
            imgMasterCard.IsEnabled = true;
            imgVisa.IsEnabled = true;
        }

        /// <summary>
        /// Méthode avant de rajouter du texte dans le champ du numéro de carte de crédit
        /// Utilisé pour permettre d'écrire des valeur numériques uniquement
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtNo_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        /// <summary>
        /// Méthode exécuter lorsqu'une commande de copie est lancé, pour valider que seulement du texte numérique est coller dans le champ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtNo_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            //Vérifie que les données sont de type String
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                String text = (String)e.DataObject.GetData(typeof(String));
                if (!IsTextAllowed(text))
                {
                    //Annule la commande si le texte n'est pas numérique.
                    e.CancelCommand();
                }
            }
            //Annule le coller, car n'est pas du texte.
            else
            {
                e.CancelCommand();
            }
        }


        //http://stackoverflow.com/questions/1268552/how-do-i-get-a-textbox-to-only-accept-numeric-input-in-wpf
        /// <summary>
        /// Méthode vérifiant que le text contient que des caractères numériques
        /// </summary>
        /// <param name="text">String à vérifier</param>
        /// <returns></returns>
        private static bool IsTextAllowed(string text)
        {
            Regex regex = new Regex("[^0-9]+"); //regex that matches disallowed text
            return !regex.IsMatch(text);
        }

        //http://stackoverflow.com/questions/1268552/how-do-i-get-a-textbox-to-only-accept-numeric-input-in-wpf
        /// <summary>
        /// Méthode vérifiant que le text contient que des caractères numériques et la barre oblique
        /// </summary>
        /// <param name="text">String à vérifier</param>
        /// <returns></returns>
        private static bool IsTextAllowedDate(string text)
        {
            Regex regex = new Regex("[^0-9/]+"); //regex that matches disallowed text
            return !regex.IsMatch(text);
        }

        /// <summary>
        /// Méthode avant de rajouter du texte dans le champ de la date d'expiration de la carte de crédit
        /// Utilisé pour permettre d'écrire des valeur caractères numériques et la barre oblique uniquement pour respecter le format attendu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtDateExpiration_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowedDate(e.Text);
        }


        /// <summary>
        /// Méthode exécuter lorsqu'une commande de copie est lancé, pour valider que seulement du texte numérique ou une barre oblique est coller dans le champ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtDateExpiration_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            //Vérifie que les données sont de type String
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                String text = (String)e.DataObject.GetData(typeof(String));
                if (!IsTextAllowedDate(text))
                {
                    //Annule le coller si le text contient des caractères défendus.
                    e.CancelCommand();
                }
            }
            //Annule le coller, car n'est pas un String.
            else
            {
                e.CancelCommand();
            }
        }
    }
}
