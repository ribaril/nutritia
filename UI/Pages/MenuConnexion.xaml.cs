using Nutritia.Logic.Model.Entities;
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
            string stringConnexion = Properties.Settings.Default.ActiveSession;
            String[] words = stringConnexion.Split(';');
            Session s = new Session(words[0], null, words[1], words[2], words[3]);
            Console.WriteLine();
            for (int i = 0; i < words.Length; i++)
            {
                if (! String.IsNullOrEmpty(words[i]))
                {
                    int index = words[i].IndexOf('=');
                    if (index != -1)
                    {
                        words[i] = words[i].Substring(index +1);
                    }
                }
            }
            Console.WriteLine(words);
            Console.WriteLine();
        }
    }
}
