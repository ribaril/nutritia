using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace Nutritia.UI.Views
{
    /// <summary>
    /// Interaction logic for FenetreInfoMembre.xaml
    /// </summary>
    public partial class FenetreInfoMembre : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public Membre membre { get; set; }

        public FenetreInfoMembre()
        {
            InitializeComponent();

        }

        public Object MembreContent
        {
            get { return (Object)GetValue(MembreProperty); }
            set
            {
                SetValue(MembreProperty, value);
                NotifyPropertyChanged("MembreContent");
            }
        }

        private void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        public static readonly DependencyProperty MembreProperty = DependencyProperty.Register("MembreContent", typeof(Membre), typeof(FenetreInfoMembre), new FrameworkPropertyMetadata(null));



        private void TextBlock_MouseEnter(object sender, MouseEventArgs e)
        {
            Membre m = MembreContent as Membre;
            Console.WriteLine("");
        }
    }
}
