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
    /// Page avec les options de connexion sur différent serveur de base de données
    /// </summary>
    public partial class MenuConnexion : Page
    {
        //La session actuellement utilisé comme connexion
        private Session SessionActive { get; set; }
        private ObservableCollection<Session> obsSessions;

        public MenuConnexion()
        {
            InitializeComponent();
            //string de connexion contenant toute les sessions séparé par {} et dans le format attendu par MySql,
            //valeur mit sur ce qui est enregistré dans les paramètres de l'utilisateur.
            string stringConnexion = Properties.Settings.Default.Sessions;
            //Utilise la classe SessionHelper pour pouvoir récupérer une liste de plusieurs sessions avec stringConnexion et les enregistre dans notre
            //observableCollection.
            obsSessions = new ObservableCollection<Session>(SessionHelper.StringToSessions(stringConnexion));

            //Configure la source de notre dataGrid.
            dgSessions.ItemsSource = obsSessions;
            //Si dans la configuration du logiciel il y a une session marqué comme active
            // (Devrait être toujours le cas)
            if (!String.IsNullOrWhiteSpace(Properties.Settings.Default.ActiveSession))
            {
                //Utilise SessionHelper pour transformer la string de connexion en un objet session (retourne une liste, mais sera juste dans seul élément, car un seul actif)
                //et l'enregistre dans SessionActive.
                SessionActive = SessionHelper.StringToSessions(Properties.Settings.Default.ActiveSession).First();
            }
        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            //Vérification pour le cas où rien n'est sélectionné dans la dataGrid, nous ne voulons pas exécuter le code.
            if (dgSessions.SelectedItem is Session)
            {
                //Non-nulleable (struct), donc forced cast. Type déjà vérifié juste avant.
                Session s = (Session)dgSessions.SelectedItem;

                //Change le text des textBox et Password box selon la valeur de la session sélectionné.
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
            //Lance la méthode pour vérifier si un champs est vide et retourne si c'est le cas, nous ne voulons pas enregistre une session 
            //sans avoir toute les informations nécessaires.
            if (IsAFieldEmpty())
                return;
            //Crée une nouvelle session à partir de la valeur de textBox.
            Session s = new Session(txName.Text, txHostname.Text, txUsername.Text, pswPassowrd.Password, txDatabaseName.Text, int.Parse(txPort.Text));
            //Si obsSessions contient déjà la même session retourne, car nous ne voulons pas enregistrer un doublon.
            if (obsSessions.Contains(s))
                return;
            //Le rajoute dans notre observableCollection.
            obsSessions.Add(s);
            //Vide les champs de l'interface.
            ClearFields();
            //Avec SessionHelper, prend la liste de session de obsSessions et le met dans le format connexion string attendu par MySql, mais avec chaque
            //session séparé par {}. Enregistre la nouvelle configuration du logiciel.
            Properties.Settings.Default.Sessions = SessionHelper.SessionsToString(obsSessions.ToList());
            Properties.Settings.Default.Save();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            //Vérification pour le cas où rien n'est sélectionné dans la dataGrid, nous ne voulons pas exécuter le code.
            if (dgSessions.SelectedItem is Session)
            {
                //Non-nulleable (struct), donc forced cast. Type déjà vérifié juste avant.
                Session s = (Session)dgSessions.SelectedItem;
                //Nous ne voulons pas pouvoir supprimer la session active.
                if (s == SessionActive)
                {
                    //Joue un beep d'erreur windows.
                    SystemSounds.Beep.Play();
                    return;
                }
                obsSessions.Remove(s);
                //Avec SessionHelper, prend la liste de session de obsSessions et le met dans le format connexion string attendu par MySql, mais avec chaque
                //session séparé par {}. Enregistre la nouvelle configuration du logiciel.
                Properties.Settings.Default.Sessions = SessionHelper.SessionsToString(obsSessions.ToList());
                Properties.Settings.Default.Save();
            }
        }

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            //Si aucune session n'est sélectionné dans la dataGrid, fait jouer un petit beep d'erreur windows.
            if (dgSessions.SelectedIndex != -1)
            {
                //Non-nulleable (struct), donc forced cast. Type assumé.
                Session newSession = (Session)dgSessions.SelectedItem;
                //Si nous voulons nous connecter à la session déjà active, il n'y a rien d'autre à faire.
                if (SessionActive == newSession)
                    return;
                //Valide si le compte actuel existe sur la nouvelle session spécifié et s'il est admins.
                //Sinon l'utilisateur se coupe la branche sous les pieds.
                if (!IsAdminOnNewConnexion(newSession.ToConnexionString()))
                {
                    MessageBox.Show("Impossible de se connecter. Est-ce que votre compte existe sur la base de données et êtes vous administrateur?",
                        "Erreur de connexion",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                //Lance la méthode pour faire le changement de session active dans l'UI (le mettre en gras).
                //Prend comme paramètre l'index dans obsSessions de l'ancienne session et la prochaine.
                SwitchActive(obsSessions.IndexOf(SessionActive), obsSessions.IndexOf(newSession));

                SessionActive = newSession;

                //Enregistre la nouvelle session active dans la configuration du logiciel.
                Properties.Settings.Default.ActiveSession = "{" + SessionActive.ToString() + "}";
                Properties.Settings.Default.Save();

                //Récupère l'instance de mainWindow et lance la méthode Configurer pour reconstruire les services MySql avec la nouvelle connexion.
                IApplicationService mainWindow = ServiceFactory.Instance.GetService<IApplicationService>();
                mainWindow.Configurer();
                //Retourne au menu principal connecté parce qu'il est déjà connecté s'il a eu accès â MenuConnexion et pour garantir un état valide si par example
                //il était dans une fenêtre qui utilisait les services MySql de l'ancienne connexion.
                mainWindow.ChangerVue(new MenuPrincipalConnecte());
            }
            else
                //Beep d'erreur microsoft.
                SystemSounds.Beep.Play();
        }


        /// <summary>
        /// Après le chargement complète de la dataGrid (tout les éléments visual qui en fait partie, etc.)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgSessions_Loaded(object sender, RoutedEventArgs e)
        {
            //S'il y a une session active (struct donc value type. On regarde si différent que qu'est-ce qu'une new Session donne, car ne peut pas comparé à un null).
            if (SessionActive != new Session())
            {
                //Pour mettre le nom de la session active de départ en gras, lance la même méthode utilisé lors d'un changement de session.
                //Marche parcequ'on enlève le bold de la session du premier paramètre, mais on le rajoute à selui du deuxième paramètre.
                SwitchActive(obsSessions.IndexOf(SessionActive), obsSessions.IndexOf(SessionActive));
            }
        }

        /// <summary>
        /// Méthode qui vide tout les champs de données de l'interface sauf txPort qui reste avec une valeur par défaut qui est le port standard d'un serveur MySql
        /// </summary>
        private void ClearFields()
        {
            //Enlève la sélection sur la dataGrid s'il y en avait une.
            dgSessions.SelectedIndex = -1;
            txDatabaseName.Text = String.Empty;
            txHostname.Text = String.Empty;
            txName.Text = String.Empty;
            //txPort.Text = String.Empty;
            txUsername.Text = String.Empty;
            pswPassowrd.Password = String.Empty;
        }

        /// <summary>
        /// Vérifie si un champ de données est vide, excluant le champ de mot de passe qui est optionnel
        /// </summary>
        /// <returns>Vrai si un ou plus est vide, sinon faux</returns>
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

        /// <summary>
        /// Méthode lancé avant que le texte soit rajouté à une textBox
        /// Utilisé pour avoir des champs numériques seulement
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txPort_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            //Si un des caractères match un caractère défendu, ne l'écrit pas.
            e.Handled = !IsTextAllowed(e.Text);
        }

        /// <summary>
        /// Méthode lancé lorsqu'une commande de copie est exécuté
        /// Utilisé pour empêcher de copier des caractères défendu pour les champs numériques
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txPort_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            //Vérifie que le type de donnée copié est un String.
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                //Récupère le String qui veut être copié.
                String text = (String)e.DataObject.GetData(typeof(String));
                //Vérifie si text contient des caractères défendu, si oui, la commande de copie est cancellée.
                if (!IsTextAllowed(text))
                {
                    e.CancelCommand();
                }
            }
            //Pas un String, on cancelle.
            else
            {
                e.CancelCommand();
            }
        }
        #endregion

        #region ValidationConnexion

        /// <summary>
        /// Méthode pour mettre en gras le nom de la session actuellement active dans la dataGrid lors d'un changement de session
        /// Si aucune session n'était auparavent en gras, mettre previousIndex et newIndex de la même session
        /// </summary>
        /// <param name="previousIndex">Position dans l'index de la collection servant d'ItemsSource de la dataGrid réprésentant l'ancienne session actuellement en gras</param>
        /// <param name="newIndex">Position dans l'index de la collection servant d'ItemsSource de la dataGrid réprésentant la session qui sera maintenant en gras</param>
        private void SwitchActive(int previousIndex, int newIndex)
        {
            //La joie de faire un style en code-behind. Plusieurs objets doivent êtres créés en plusieurs étapes.

            //Définie les Setters qui appliquent un FontWeights sur la propriété FontWeightProperty d'un TextBlock
            //Ici un FontWeight Bold et Normal.
            Setter normal = new Setter(TextBlock.FontWeightProperty, FontWeights.Normal, null);
            Setter bold = new Setter(TextBlock.FontWeightProperty, FontWeights.Bold, null);

            //Récupère la row de la DataGrid à l'index previousIndex.
            DataGridRow row = (DataGridRow)dgSessions.ItemContainerGenerator.ContainerFromIndex(previousIndex);
            //Crée un nouveau Style avec comme targetType DataGridRow.
            Style newStyle = new Style(row.GetType());
            // Rajoute le setter normal au style.
            newStyle.Setters.Add(normal);
            //Redéfinie le style de la row. En gros, le FontWeight et devenu Normal pour la row.
            row.Style = newStyle;

            // Mêmes opérations, mais pour la row à l'index newIndex et met le setter du style à bold.
            row = (DataGridRow)dgSessions.ItemContainerGenerator.ContainerFromIndex(newIndex);
            newStyle = new Style(row.GetType());

            newStyle.Setters.Add(bold);
            //Le FontWeight est maintenant en Bold pour la row.
            row.Style = newStyle;
        }

        /// <summary>
        /// Méthode qui vérifie si le compte membre actuellement connecté existe sur un serveur de base de données et s'il est administrateur
        /// </summary>
        /// <param name="stringConnexion">String de la connexion du serveur de base de données qui sera testé dans le format attendu par MySql</param>
        /// <returns>Vrai si le compte membre existe et s'il est administrateur, sinon faux</returns>
        private bool IsAdminOnNewConnexion(string stringConnexion)
        {
            try
            {
                //Crée un nouveau MySqlMembreService avec la stringConnexion spécifique.
                IMembreService serviceMembre = new MySqlMembreService(stringConnexion);

                //Récupère seulement les admins
                IList<Membre> adminsDansNouvelleSession = serviceMembre.RetrieveAdmins();
                //Lance une exception si le Where ne retourne rien, l'exception est géré de toute façon par le try/catch.
                //Vérifie si un compte avec le même nom d'utilisateur et mot de passe que selui du compte membre actuellement connecté existe.
                Membre membreCorrespondant = adminsDansNouvelleSession.Where(membre => membre.NomUtilisateur == App.MembreCourant.NomUtilisateur && membre.MotPasse == App.MembreCourant.MotPasse).First();
                //Redondant, car on retrieves les admins et si membreCorrespondant est null, la lambda lance une exception.
                if (membreCorrespondant != null && membreCorrespondant.EstAdministrateur)
                {
                    //Mauvaise méthode pour le faire, mais au cas le compte à des info différentes sur le nouveau serveur de base de données, il faudrait remplacer App.MembreCourant
                    //avant le changement de session:
                    App.MembreCourant = membreCorrespondant;
                    return true;
                }
                else
                    return false;
            }
            //N'importe quel erreur dans le teste, on retourne false pour empêcher le changement de connexion.
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }

        }


        #endregion
    }
}
