using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WindowsGeneratorView
{
    public partial class IntervalField : UserControl, INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        public IntervalField()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty TextBoxWidthProperty =
            DependencyProperty.Register("TextBoxWidth", typeof(int), typeof(IntervalField));
        public int TextBoxWidth
        {
            get => (int)(this.GetValue(TextBoxWidthProperty));
            set
            {
                this.SetValue(TextBoxWidthProperty, value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TextBoxWidth"));
            }
        }

        public static readonly DependencyProperty MaxValueProperty =
        DependencyProperty.Register("MaxValue", typeof(string), typeof(IntervalField),
            new PropertyMetadata("", null, MaxValueChangeCallback));
        public string MaxValue
        {
            get => this.GetValue(MaxValueProperty) as string;
            set
            {
                this.SetValue(MaxValueProperty, value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MaxValue"));
            }
        }

        private static object MaxValueChangeCallback(DependencyObject d, object value)
        {
            var newVal = ((IntervalField)d).CoerceValueToBounds(value as string, true);
            if (newVal.CompareTo(value as string) != 0)
            {
                ((IntervalField)d).MaxValue = newVal;
            }
            return newVal;
        }

        private static object MinValueChangeCallback(DependencyObject d, object value)
        {
            var newVal = ((IntervalField)d).CoerceValueToBounds(value as string, false);
            if (newVal.CompareTo(value as string) != 0)
            {
                ((IntervalField)d).MinValue = newVal;
            }
            return newVal;
        }

        private string CoerceValueToBounds(string value, bool isNewMax)
        {
            if (value == "")
            {
                if (isNewMax) return MinValue;
                else return MaxValue;
            }
            switch (FilterType)
            {
                case TextBoxFilter.Decimal: case TextBoxFilter.Integer:
                    if (isNewMax)
                    {
                        if (!double.TryParse(MinValue, out var min))
                            return value;
                        var candidate = Math.Max(double.Parse(value), min);
                        if (double.Parse(value) == candidate)
                        {
                            return value;
                        }
                        return candidate.ToString();
                    }
                    else
                    {
                        if (!double.TryParse(MaxValue, out var max))
                            return value;
                        var candidate = Math.Min(double.Parse(value), max);
                        if (double.Parse(value) == candidate)
                        {
                            return value;
                        }
                        return candidate.ToString();
                    }
                default:
                    return value;
            }
        }

        public static readonly DependencyProperty MinValueProperty =
        DependencyProperty.Register("MinValue", typeof(string), typeof(IntervalField),
            new PropertyMetadata("", null, MinValueChangeCallback));
        public string MinValue
        {
            get => this.GetValue(MinValueProperty) as string;
            set
            {                                             
                this.SetValue(MinValueProperty, value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MinValue"));
            }
        }

        public static readonly DependencyProperty LabelValueProperty =
        DependencyProperty.Register("LabelValue", typeof(string), typeof(IntervalField));
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
            DependencyProperty.Register("FilterType", typeof(TextBoxFilter), typeof(IntervalField));


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
