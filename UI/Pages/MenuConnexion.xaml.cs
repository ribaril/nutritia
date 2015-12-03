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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Media;
using System.Text.RegularExpressions;
using Nutritia.UI.Views;

namespace Nutritia.UI.Pages
{
    /// <summary>
    /// Interaction logic for MenuConnexion.xaml
    /// </summary>
    public partial class MenuConnexion : Page
    {
        private Session SessionActive { get; set; }
        private ObservableCollection<Session> obsSessions;

        public MenuConnexion()
        {
            InitializeComponent();

            string stringConnexion = Properties.Settings.Default.Sessions;
            obsSessions = new ObservableCollection<Session>(SessionHelper.StringToSessions(stringConnexion));

            dgSessions.ItemsSource = obsSessions;
            if (!String.IsNullOrWhiteSpace(Properties.Settings.Default.ActiveSession))
            {
                SessionActive = SessionHelper.StringToSessions(Properties.Settings.Default.ActiveSession).First();
            }
        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            if (dgSessions.SelectedItem is Session)
            {
                //Non-nulleable (struct), dont forced cast. Type déjà vérifié juste avant.
                Session s = (Session)dgSessions.SelectedItem;

                txHostname.Text = s.HostName_IP;
                txPort.Text = s.Port.ToString();
                txUsername.Text = s.NomUtilisateur;
                pswPassowrd.Password = s.MotDePasse;
                txDatabaseName.Text = s.NomBD;
                txName.Text = s.Nom;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (IsAFieldEmpty())
                return;
            Session s = new Session(txName.Text, txHostname.Text, txUsername.Text, pswPassowrd.Password, txDatabaseName.Text, int.Parse(txPort.Text));
            if (obsSessions.Contains(s))
                return;
            obsSessions.Add(s);
            ClearFields();
            Properties.Settings.Default.Sessions = SessionHelper.SessionsToString(obsSessions.ToList());
            Properties.Settings.Default.Save();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dgSessions.SelectedItem is Session)
            {
                Session s = (Session)dgSessions.SelectedItem;
                if (s == SessionActive)
                {
                    SystemSounds.Beep.Play();
                    return;
                }
                obsSessions.Remove(s);
                Properties.Settings.Default.Sessions = SessionHelper.SessionsToString(obsSessions.ToList());
                Properties.Settings.Default.Save();
            }
        }

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            Session newSession = (Session)dgSessions.SelectedItem;
            if (SessionActive == newSession)
                return;
            //Valide si le compte actuel existe sur la nouvelle session spécifié et s'il est admins.
            //Sinon l'utilisateur ce coupe la branche sous les pieds.
            if (!IsAdminOnNewConnexion(newSession.ToConnexionString()))
            {
                MessageBox.Show("Impossible de se connecter. Est-ce que votre compte existe sur la base de données et êtes vous administrateur?",
                    "Erreur de connexion", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            SwitchActive(obsSessions.IndexOf(SessionActive), obsSessions.IndexOf(newSession));
            SessionActive = newSession;

            Properties.Settings.Default.ActiveSession = "{" + SessionActive.ToString() + "}";
            Properties.Settings.Default.Save();

            IApplicationService mainWindow = ServiceFactory.Instance.GetService<IApplicationService>();
            mainWindow.Configurer();
            mainWindow.ChangerVue(new MenuPrincipalConnecte());
        }



        private void dgSessions_Loaded(object sender, RoutedEventArgs e)
        {
            //Marche parcequ'on enlève le bold avant..mais on le rajoute.
            if (SessionActive != new Session())
            {
                SwitchActive(obsSessions.IndexOf(SessionActive), obsSessions.IndexOf(SessionActive));
            }
        }

        private void ClearFields()
        {
            dgSessions.SelectedIndex = -1;
            txDatabaseName.Text = String.Empty;
            txHostname.Text = String.Empty;
            txName.Text = String.Empty;
            //txPort.Text = String.Empty;
            txUsername.Text = String.Empty;
            pswPassowrd.Password = String.Empty;
        }

        private bool IsAFieldEmpty()
        {
            return (
                String.IsNullOrWhiteSpace(txDatabaseName.Text) ||
                String.IsNullOrWhiteSpace(txHostname.Text) ||
                String.IsNullOrWhiteSpace(txName.Text) ||
                String.IsNullOrWhiteSpace(txPort.Text) ||
                String.IsNullOrWhiteSpace(txUsername.Text)
                );
        }

        #region ValidationtxPort

        //http://stackoverflow.com/questions/1268552/how-do-i-get-a-textbox-to-only-accept-numeric-input-in-wpf
        private static bool IsTextAllowed(string text)
        {
            Regex regex = new Regex("[^0-9]+"); //regex that matches disallowed text
            return !regex.IsMatch(text);
        }

        private void txPort_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private void txPort_Pasting(object sender, DataObjectPastingEventArgs e)
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
        #endregion

        #region ValidationConnexion


        private void SwitchActive(int previousIndex, int newIndex)
        {
            Setter normal = new Setter(TextBlock.FontWeightProperty, FontWeights.Normal, null);
            Setter bold = new Setter(TextBlock.FontWeightProperty, FontWeights.Bold, null);

            // Enlève le bold de l'ancien
            DataGridRow row = (DataGridRow)dgSessions.ItemContainerGenerator.ContainerFromIndex(previousIndex);
            Style newStyle = new Style(row.GetType());

            newStyle.Setters.Add(normal);
            row.Style = newStyle;

            // Met le bold au nouveau
            row = (DataGridRow)dgSessions.ItemContainerGenerator.ContainerFromIndex(newIndex);
            newStyle = new Style(row.GetType());

            newStyle.Setters.Add(bold);
            row.Style = newStyle;
        }

        private bool IsAdminOnNewConnexion(string stringConnexion)
        {
            try
            {
                IMembreService serviceMembre = new MySqlMembreService(stringConnexion);
                IList<Membre> adminsDansNouvelleSession = serviceMembre.RetrieveAdmins();
                //Lance une exception si le Where ne retourne rien, l'exception est géré de toute façon.
                Membre membreCorrespondant = adminsDansNouvelleSession.Where(membre => membre.NomUtilisateur == App.MembreCourant.NomUtilisateur && membre.MotPasse == App.MembreCourant.MotPasse).First();
                //Redondant, car on retrieves les admins et si membreCorrespondant est null, la lambda lance une exception.
                if (membreCorrespondant != null && membreCorrespondant.EstAdministrateur)
                {
                    //Mauvaise place pour le faire, mais au cas le compte à des info différentes:
                    App.MembreCourant = membreCorrespondant;
                    return true;
                }
                else
                    return false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }

        }


        #endregion
    }
}
