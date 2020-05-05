using GeneratorController;
using System.Windows.Controls;

namespace WindowsGeneratorView
{
    public partial class SegmentingProperties : UserControl
    {
        public SegmentingProperties(BuildingsViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }
    }
}
