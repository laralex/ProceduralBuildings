using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneratorController
{
    public class GrammarNode : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public ViewModelGrammar Root { get; set; }
        public ObservableCollection<GrammarNode> Children { get; private set; }
        // ABSTRACT PRIVATE SET
        public string Name { get; set; }
        public GrammarNode(string name)
        {
            Name = name;
            Children = new ObservableCollection<GrammarNode>();
        }

        /*
        public GrammarNode AddChild(string name)
        {
            var child = new GrammarNode(name) { Root = this.Root };
            Root.RegisterChangingChild(child);
            m_children.Add(child);
            TriggerPropertyChanged("Children");
            return child;
        }

        public void TriggerPropertyChanged(string propertyChanged)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyChanged));
        }

        public void RemoveChild(GrammarNode node)
        {
            m_children.Remove(node);
        }
        
        private IList<GrammarNode> m_children;
        */
    }
}
