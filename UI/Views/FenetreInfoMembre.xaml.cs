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
        public static readonly DependencyProperty MembreProperty = DependencyProperty.Register("MembreContent", typeof(Membre), typeof(FenetreInfoMembre), new FrameworkPropertyMetadata(null, PropertyChangedCallback));


        public Membre membre { get; set; }

        public FenetreInfoMembre()
        {
            InitializeComponent();

            // Header de la fenetre
            App.Current.MainWindow.Title = "Nutritia - Info Membre";

        }

        public Object MembreContent
        {
            get { return (Object)GetValue(MembreProperty); }
            set
            {
                SetValue(MembreProperty, value);
                OnPropertyChanged("MembreContent");
            }
        }

        private void OnPropertyChanged(string property)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(property));
        }

        private void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, e);
        }



        private static void PropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            // this is the method that is called whenever the dependency property's value has changed
            FenetreInfoMembre f = dependencyObject as FenetreInfoMembre;
            f.txtBlock.Inlines.Clear();
            Membre m;
            if (args.NewValue is Membre)
            {
                m = args.NewValue as Membre;
                foreach (Inline item in MemberToDisplay(m))
                {
                    f.txtBlock.Inlines.Add(item);
                }
            }
        }


        private static string Indent(int level)
        {
            return "     ".PadLeft(level);
        }

        private static List<Inline> MemberToDisplay(Membre m)
        {

            //StringBuilder sb = new StringBuilder(" ", 0, nbrIndentSpace * indentLevel);

            List<Inline> ic = new List<Inline>();
            ic.Add(new Run(m.NomUtilisateur));
            ic.Add(new LineBreak());
            ic.Add(new Run(Indent(1)));
            ic.Add(new Run(m.Prenom + " " + m.Nom));
            ic.Add(new LineBreak());
            ic.Add(new Run(Indent(1)));
            ic.Add(new Run(m.Age.ToString()));
            ic.Add(new Run(" " + Properties.Resources.An));

            //ic.Add(new Bold(new Run(" my")));
            //ic.Add(new Run(" faithful"));
            //ic.Add(new Underline(new Run(" computer")));
            //ic.Add(new Run(". "));
            //ic.Add(new Italic(new Run("You rock!")));

            return ic;
        }
    }
}
