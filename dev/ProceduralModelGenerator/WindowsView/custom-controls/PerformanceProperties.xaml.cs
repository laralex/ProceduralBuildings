using GeneratorController;
using System.Windows.Controls;

namespace WindowsGeneratorView
{
    public partial class PerformanceProperties : UserControl
    {
        public PerformanceProperties(AssetsViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }
    }
}
