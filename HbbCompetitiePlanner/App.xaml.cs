using System.Globalization;
using System.Windows;

namespace HbbCompetitiePlanner {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        public App() {
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("nl-NL");
        }
    }
}
