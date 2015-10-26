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
    /// Interaction logic for Don.xaml
    /// </summary>
    public partial class Don : Page
    {
        private int valeurDon;

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
                //Etc...
            }
        }
    }
}
