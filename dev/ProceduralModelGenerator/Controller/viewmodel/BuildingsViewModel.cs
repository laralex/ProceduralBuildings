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
        public float SpaceUnitsPerMeter = 1.0f;
        public IViewModel BasementSettings { get; set; }
        public IViewModel RoofSettings { get; set; }
        public IViewModel HorizontalSplitSettings { get; set; }
        public IViewModel VerticalSplitSettings { get; set; }
        public IViewModel WindowsSettings { get; set; }
        public IViewModel DoorsSettings { get; set; }
        private string m_seedString;
        public string SeedString
        {
            get => m_seedString;
            set
            {
                m_seedString = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SeedString"));
            }
        }

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
    }
}
