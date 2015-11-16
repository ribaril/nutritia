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

            Console.WriteLine();
        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            if (dgSessions.SelectedItem is Session)
            {
                //Non-nulleable (struct), dont forced cast. Type déjà vérifié juste avant.
                Session s = (Session)dgSessions.SelectedItem;

                txHostname.Text = s.HostName_IP;
                txPort.Text = s.Port.ToString();
                txUsername.Text = s.User;
                pswPassowrd.Password = s.Password;
                txDatabaseName.Text = s.DatabaseName;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Session s = new Session("newSession", txHostname.Text, txUsername.Text, pswPassowrd.Password, txDatabaseName.Text, int.Parse(txPort.Text));
            obsSessions.Add(s);
            Properties.Settings.Default.Sessions = SessionHelper.SessionsToString(obsSessions.ToList());
            Properties.Settings.Default.Save();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dgSessions.SelectedItem is Session)
            {
                Session s = (Session)dgSessions.SelectedItem;
                if (s.Name == SessionActive.Name)
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

            SwitchActive(obsSessions.IndexOf(SessionActive), obsSessions.IndexOf(newSession));
            SessionActive = newSession;

            Properties.Settings.Default.ActiveSession = "{" + SessionActive.ToString() + "}";
            Properties.Settings.Default.Save();
        }

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

        private void dgSessions_Loaded(object sender, RoutedEventArgs e)
        {
            //Marche parcequ'on enlève le bold avant..mais on le rajoute.
            if (SessionActive != new Session())
            {
                SwitchActive(obsSessions.IndexOf(SessionActive), obsSessions.IndexOf(SessionActive));
            }
        }
    }
}
