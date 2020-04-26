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
            /*bool approvedDecimalPoint = false;

            if (e.Text == ".")
            {
                if (!((TextBox)sender).Text.Contains("."))
                    approvedDecimalPoint = true;
            } */
            //!(char.IsDigit(e.Text, e.Text.Length - 1) || approvedDecimalPoint) || 
            if (!float.TryParse(((TextBox)sender).Text + e.Text, out var res))
                e.Handled = true;
        }

        public static void OnPreviewTextBoxInteger(object sender, TextCompositionEventArgs e)
        {
            //var text = (sender as TextBox).Text;
            //bool noTrailingZero = text == "" || text[0] != '0';
            // !char.IsDigit(e.Text, e.Text.Length - 1) || !noTrailingZero
            if (!int.TryParse(((TextBox)sender).Text + e.Text, out var res))
            {
                e.Handled = true;
            }
        }

        public static void OnPreviewTextBoxCode(object sender, TextCompositionEventArgs e)
        {
            var text = (sender as TextBox).Text;
            var regex = new Regex("[A-Z]");
            e.Handled = !regex.IsMatch(e.Text);
        }
    }
}
