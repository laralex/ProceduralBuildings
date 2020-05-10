﻿using GeneratorController;
using System;
using System.Windows.Controls;
using System.Windows.Data;

namespace WindowsGeneratorView
{
    public partial class RoofProperties : UserControl
    {
        public RoofProperties(BuildingsViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }

    }

    public class RoofStyleToIdxConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (int)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (RoofStyle)value;
        }
    }

}
