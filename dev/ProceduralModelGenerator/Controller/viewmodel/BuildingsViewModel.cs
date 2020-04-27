using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace GeneratorController
{
    public class BuildingsViewModel : IViewModel
    {
        public string GenerateIconPath => Path.Combine(rootDir, @"data/generate-icon.png");
        public string ExportIconPath => Path.Combine(rootDir, @"data/export-icon.png");
        public string HelpIconPath => Path.Combine(rootDir, @"data/info-icon.png");
        public string VisualizeIconPath => Path.Combine(rootDir, @"data/visualize-icon.png");

        private ViewModelGrammar m_grammar;
        public ViewModelGrammar Grammar
        {
            get => m_grammar;
            set
            {
                m_grammar = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Grammar"));
            }
        }

        private UserControl m_propertyPanel;
        public UserControl PropertiesPanel
        {
            get => m_propertyPanel;
            set
            {
                m_propertyPanel = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PropertiesPanel"));
            }
        }

        public void RegisterChangingItem(INotifyPropertyChanged v)
        {
            v.PropertyChanged += (s, e) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Grammar"));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private static string rootDir => Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.FullName;

    }
}
