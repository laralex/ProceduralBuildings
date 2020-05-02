using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace WindowsGeneratorView
{
    public class TextValidators
    {
        public static void OnPreviewTextBoxDecimal(object sender, TextCompositionEventArgs e)
        {
            if (!float.TryParse(((TextBox)sender).Text + e.Text, out var res))
                    e.Handled = true;
            if (res < 0.0f)
                e.Handled = true;
        }

        public static void OnPreviewTextBoxInteger(object sender, TextCompositionEventArgs e)
        {
            if (!int.TryParse(((TextBox)sender).Text + e.Text, out var res))
                e.Handled = true;
            if (res < 0)
                e.Handled = true;
        }

        public static void OnPreviewTextBoxCode(object sender, TextCompositionEventArgs e)
        {
            var text = (sender as TextBox).Text;
            var regex = new Regex("[A-Z]");
            e.Handled = !regex.IsMatch(e.Text);
        }

        public static void OnPreviewTextInput(object sender, TextCompositionEventArgs e, TextBoxFilter filterType)
        {
            switch (filterType)
            {
                case TextBoxFilter.Integer:
                    TextValidators.OnPreviewTextBoxInteger(sender, e);
                    break;
                case TextBoxFilter.Decimal:
                    TextValidators.OnPreviewTextBoxDecimal(sender, e);
                    break;
                case TextBoxFilter.LatinLetters:
                    TextValidators.OnPreviewTextBoxCode(sender, e);
                    break;
                default:
                    break;
            }
        }
    }
    public enum TextBoxFilter
    {
        None, Integer, Decimal, LatinLetters
    }
}
