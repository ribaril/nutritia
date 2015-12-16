using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
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


// https://stackoverflow.com/questions/17099042/filter-wpf-datagrid-values-from-a-textbox
namespace Nutritia.UI.Views
{

    /// <summary>
    /// userControl qui offre la possibiliter de filtrer des données contenu dans une dataGrid à partir d'un champs de recherche.
    /// </summary>
    public partial class SearchBox : UserControl, INotifyPropertyChanged
    {
        #region Attribut
        //Event lorsque des propriétés sont changés
        public event PropertyChangedEventHandler PropertyChanged;

        //La vue de la dataGrid
        private ICollectionView dataGridCollection;
        //Le string de la boite txtRecherche
        private string filterString;

        //Attribut de comparaisons du système, est utilisé dans les fonctions de recherche de .NET.
        private CompareOptions compareOptions;

        // Enum pour pouvoir faire du binding sur le mode de filtrage.
        // Pour spécifier le mode de filtration.
        public enum Mode { StartWith, Contains }

        //bool pour pouvoir faire du binding et déterminer si nous voulons une recherche qui prend on compte les majuscules ou non.
        public bool IsCaseSensitive { get; set; }


        #endregion

        //Permet de faire du Binding avec des propriétés personalisées ou juste d'exposé des propriétés d'un contrôle
        // composante à la fenêtre qui consomme l'userControl.
        #region DependencyProperty
        //ItemsSource de la dataGrid
        public static readonly DependencyProperty ItemsSourceProperty = DataGrid.ItemsSourceProperty.AddOwner(typeof(SearchBox));
        //SelectedItem de la dataGrid
        public static readonly DependencyProperty SelectedItemProperty = DataGrid.SelectedItemProperty.AddOwner(typeof(SearchBox));
        //SelectionMode de la dataGrid
        public static readonly DependencyProperty SelectionModeProperty = DataGrid.SelectionModeProperty.AddOwner(typeof(SearchBox));
        //Propriété du texte du Watermark de la boite de recherche. Valeur par défault à string.Empty pour le contrôle (lors de la création).
        public static readonly DependencyProperty WatermarkContentProperty = DependencyProperty.Register("WatermarkContent", typeof(string), typeof(SearchBox), new FrameworkPropertyMetadata(string.Empty));

        //https://stackoverflow.com/questions/10952684/how-to-expose-a-control-in-xaml
        //Propriétés des Columns de la dataGrid pour permettre à la fenêtre consommatrice de définir les columns elle même.
        public static readonly DependencyProperty ColumnsProperty = DependencyProperty.Register("Columns", typeof(ObservableCollection<DataGridColumn>), typeof(SearchBox));

        //Propriété FilterMode pour spécifier le mode de filtrage. Par défault c'est une recherche à savoir s'il commence par la chaine.
        public static readonly DependencyProperty FilterModeProperty = DependencyProperty.Register("FilterMode", typeof(Mode), typeof(SearchBox), new FrameworkPropertyMetadata(Mode.StartWith));

        //Propriété IsCaseSensitive pour spécifier si la recherche prend en compte les lettres majuscules. Par défault à false.
        public static readonly DependencyProperty IsCaseSensitiveProperty = DependencyProperty.Register("IsCaseSensitive", typeof(bool), typeof(SearchBox), new FrameworkPropertyMetadata(false));

        #endregion


        public SearchBox()
        {
            //Rajoute les columns défini par le binding de la fenêtre consommatrice à partir de la propriété ColumnsProperty
            Columns = new ObservableCollection<DataGridColumn>();
            //Évênement CollectionChanged (sender, event args)
            Columns.CollectionChanged += (s, a) =>
            {
                dgResults.Columns.Clear();
                foreach (var column in this.Columns)
                    dgResults.Columns.Add(column);
            };
            InitializeComponent();
            //Selon la valeur IsCaseSensitive, défini le comparateur.
            if (IsCaseSensitive)
                compareOptions = CompareOptions.None;
            else
                compareOptions = CompareOptions.IgnoreCase;

        }

        #region Property

        public ICollectionView DataGridCollection
        {
            get { return dataGridCollection; }
            set
            {
                dataGridCollection = value;
                NotifyPropertyChanged("DataGridCollection");
            }
        }

        /// <summary>
        /// Propriété du mode de filtrage de la recherche
        /// </summary>
        public Mode FilterMode
        {
            get { return (Mode)GetValue(FilterModeProperty); }
            set { SetValue(FilterModeProperty, value); }
        }

        /// <summary>
        /// Propriété du mode de sélection de la dataGrid
        /// </summary>
        public DataGridSelectionMode SelectionMode
        {
            get { return (DataGridSelectionMode)GetValue(SelectionModeProperty); }
            set { SetValue(SelectionModeProperty, value); }
        }

        /// <summary>
        /// Propriété ItemsSource de la dataGrid
        /// </summary>
        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        /// <summary>
        /// Propriété SelectedItem de la dataGrid
        /// </summary>
        public Object SelectedItem
        {
            get { return (Object)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        /// <summary>
        /// Propriété du texte de filigrane affiché dans la txtBox de filtration
        /// </summary>
        public string WatermarkContent
        {
            get { return GetValue(WatermarkContentProperty).ToString(); }
            set { SetValue(WatermarkContentProperty, value); }
        }

        /// <summary>
        /// Propriété du texte utilisé pour faire la recherche dans la dataGrid
        /// </summary>
        public string FilterString
        {
            get { return filterString; }
            set
            {
                filterString = value;
                NotifyPropertyChanged("FilterString");
                FilterCollection();
            }
        }

        /// <summary>
        /// Propriété Columns de la dataGrid
        /// </summary>
        public ObservableCollection<DataGridColumn> Columns
        {
            get { return (ObservableCollection<DataGridColumn>)GetValue(ColumnsProperty); }
            set { SetValue(ColumnsProperty, value); }
        }

        #endregion

        /// <summary>
        /// Lance un événement lorsqu'une propriété est changé.
        /// </summary>
        /// <param name="property">Nom de la propriété changé</param>
        private void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        /// <summary>
        /// Remet à jour la vue de la dataGrid avec le filtre
        /// </summary>
        private void FilterCollection()
        {
            if (dataGridCollection != null)
            {
                dataGridCollection.Refresh();
            }
        }


        /// <summary>
        /// Évènement lorsque txtRecherche change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtRecherche_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox t = sender as TextBox;
            if (t != null)
            {
                //Met la valeur de la propriété FilterString à jour 
                FilterString = t.Text;
            }
        }

        /// <summary>
        /// Méthode pour valider la donnée d'entré avec le string de filtrage selon le Mode de filtrage sélectionné
        /// </summary>
        /// <param name="entre">Donnée d'entré qu'il faut valider avec le filtre</param>
        /// <returns>True si valide, False sinon</returns>
        public bool Filter(string entre)
        {
            if (FilterMode == Mode.Contains)
            {
                //Si FilterString est contenu dans entre, retourne l'emplacement de l'index.
                //Si l'index retourné est >= 0, il est à quelque part à l'intérieur ou au début.
                return App.culture.CompareInfo.IndexOf(entre, FilterString, compareOptions) >= 0;
            }
            else if (FilterMode == Mode.StartWith)
            {
                //Si FilterString est contenu dans entre, retourne l'emplacement de l'index.
                //Si l'index retourné est == 0, il est exactement au début.
                return App.culture.CompareInfo.IndexOf(entre, FilterString, compareOptions) == 0;
            }
            throw new InvalidEnumArgumentException("Mode non implémenté");
        }
    }

}
