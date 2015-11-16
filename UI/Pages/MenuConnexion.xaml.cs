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
            SessionActive = SessionHelper.StringToSessions(Properties.Settings.Default.ActiveSession).First();
        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            if (dgSessions.SelectedItem is Session)
            {
                //Non-nulleable (struct), dont forced cast. Type déjà vérifié juste avant.
                SessionActive = (Session)dgSessions.SelectedItem;
                txHostname.Text = SessionActive.HostName_IP;
                txPort.Text = SessionActive.Port.ToString();
                txUsername.Text = SessionActive.User;
                pswPassowrd.Password = SessionActive.Password;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Session s = new Session("newSession", txHostname.Text, txUsername.Text, pswPassowrd.Password, "toDo", int.Parse(txPort.Text));
            obsSessions.Add(s);
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dgSessions.SelectedItem is Session)
            {
                Session s = (Session)dgSessions.SelectedItem;
                obsSessions.Remove(s);
            }
        }

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            Session newSession = (Session)dgSessions.SelectedItem;
            //if (SessionActive == newSession)
            //    return;

            SwitchActive(obsSessions.IndexOf(SessionActive), obsSessions.IndexOf(newSession));
            SessionActive = newSession;
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
            SwitchActive(obsSessions.IndexOf(SessionActive), obsSessions.IndexOf(SessionActive));
        }
    }
}
