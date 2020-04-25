using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneratorController
{
    public class ViewModelGrammar : GrammarNode, INotifyPropertyChanged
    {
        public ViewModelGrammar(string name = null) : base(name) 
        {
            Root = this;
        }

        public void RegisterChangingChild(GrammarNode node)
        {
            //node.PropertyChanged += (s, e) => TriggerPropertyChanged("Children");
        }

        //public new event PropertyChangedEventHandler PropertyChanged;
    }
}
