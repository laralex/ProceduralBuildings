using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WindowsGeneratorView
{
    public partial class LabeledTextBox : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public LabeledTextBox()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty TextBoxWidthProperty =
            DependencyProperty.Register("TextBoxWidth", typeof(int), typeof(LabeledTextBox));
        public int TextBoxWidth
        {
            get => (int)(this.GetValue(TextBoxWidthProperty));
            set
            {
                this.SetValue(TextBoxWidthProperty, value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TextBoxWidth"));
            }
        }

        public static readonly DependencyProperty TextBoxValueProperty =
            DependencyProperty.Register("TextBoxValue", typeof(string), typeof(LabeledTextBox),
                new PropertyMetadata("1", new PropertyChangedCallback((d, a) =>
                {
                    if (d != null)
                        (d as LabeledTextBox).TextBoxValue =
                            (a.NewValue as string == "" ? a.OldValue : a.NewValue) as string;
                })));
                
        public string TextBoxValue
        {
            get => this.GetValue(TextBoxValueProperty) as string;
            set
            {
                this.SetValue(TextBoxValueProperty, value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TextBoxValue"));
            }
                
        }

        public static readonly DependencyProperty LabelValueProperty =
            DependencyProperty.Register("LabelValue", typeof(string), typeof(LabeledTextBox));
        public string LabelValue
        {
            get => this.GetValue(LabelValueProperty) as string;
            set
            {
                this.SetValue(LabelValueProperty, value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("LabelValue"));
            }
        }

        public static readonly DependencyProperty FilterTypeProperty =
            DependencyProperty.Register("FilterType", typeof(TextBoxFilter), typeof(LabeledTextBox));
        public TextBoxFilter FilterType
        {
            get => (TextBoxFilter)(this.GetValue(FilterTypeProperty));
            set
            {
                this.SetValue(FilterTypeProperty, value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FilterType"));
            }
        }

        private void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextValidators.OnPreviewTextInput(sender, e, FilterType);
        }
    }
}
