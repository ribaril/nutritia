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

        }
    }
}
