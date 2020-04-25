using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneratorController
{
    public class BuildingsViewModel : IViewModel
    {
        public string GenerateIconPath => Path.Combine(rootDir, @"data/generate-icon.png");
        public string ExportIconPath => Path.Combine(rootDir, @"data/export-icon.png");
        public string HelpIconPath => Path.Combine(rootDir, @"data/info-icon.png");

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

        public void RegisterChangingItem(INotifyPropertyChanged v)
        {
            v.PropertyChanged += (s, e) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Grammar"));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private static string rootDir => Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.FullName;

    }
}
