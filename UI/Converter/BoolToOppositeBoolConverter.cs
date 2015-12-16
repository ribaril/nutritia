using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Nutritia.UI.Converter
{
    //http://www.codeproject.com/Articles/24330/WPF-Bind-to-Opposite-Boolean-Value-Using-a-Convert
    /// <summary>
    /// Class pour faire du binding avec la valeur opposé d'un bool
    /// Utilisé pour désactiver le checkBox d'un compte membre bannis dans la fenêtre des droits d'administration
    /// </summary>
    public class BoolToOppositeBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(bool))
                throw new InvalidOperationException("The target must be a boolean");

            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
