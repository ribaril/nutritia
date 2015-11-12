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

namespace Nutritia.UI.Pages
{
    /// <summary>
    /// Interaction logic for MenuConnexion.xaml
    /// </summary>
    public partial class MenuConnexion : Page
    {
        public MenuConnexion()
        {
            InitializeComponent();
            string stringConnexion = Properties.Settings.Default.Sessions;
            List<Session> ss = SessionHelper.StringToSessions(stringConnexion);

            Console.WriteLine();
            Console.WriteLine(SessionHelper.SessionsToString(ss));

            dgSessions.ItemsSource = ss;

        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            if (dgSessions.SelectedItem is Session)
            {
                //Non-nulleable (struct), dont forced cast. Type déjà vérifié juste avant.
                Session session = (Session)dgSessions.SelectedItem;
                txHostname.Text = session.HostName_IP;
                txPort.Text = session.Port.ToString();
                txUsername.Text = session.User;
                pswPassowrd.Password = session.Password;
            }
        }
    }
}
