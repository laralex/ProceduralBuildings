using System.Windows;

namespace WpfVisualizer
{
    public partial class App : Application
    {
        private void UnhandledExceptionHandler(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show("FAILED to load the model, it doesn't have proper structure: " + e.Exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            e.Handled = true;
        }
    }
}
