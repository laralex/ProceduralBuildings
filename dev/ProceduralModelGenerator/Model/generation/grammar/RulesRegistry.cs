using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProceduralBuildingsGeneration
{
    class RulesRegistry
    {
        public IList<Rule> Rules { get; private set; }
        public RulesRegistry()
        {
            Rules = new List<Rule>();
        }

        public void AddRule(ISet<Type> antedecent, IList<GrammarNode> consequence)
        {
            Rules.Add(new Rule
            {
                Antedecent = antedecent,
                Consequence = consequence,
                ApplicabilityCheckF = (nodes) => true,
            });
        }

        public void AddRule(ISet<Type> antedecent, IList<GrammarNode> consequence, Func<ISet<Type>, bool> applicabilityCheck)
        {
            Rules.Add(new Rule
            {
                Antedecent = antedecent,
                Consequence = consequence,
                ApplicabilityCheckF = applicabilityCheck,
            });
        }
    }

    class Rule
    {
        public ISet<Type> Antedecent;
        public IList<GrammarNode> Consequence;
        public Func<ISet<Type>, bool> ApplicabilityCheckF;
    }                                        
}
