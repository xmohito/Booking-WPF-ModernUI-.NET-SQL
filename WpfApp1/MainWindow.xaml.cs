using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp_1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Nacisniecie przycisku powoduje zamknięcie aktualnego okna
        /// </summary>
        private void CloseBtn(object sender, RoutedEventArgs e)
        {
            this.Close();
            //WindowState = WindowState.Minimized;
        }

        /// <summary>
        /// Odpowiada za możliwość przesuwania aplikacji po ekranie w momencie przycisniecia lewego przycisku myszy
        /// </summary>
        private void Grid_Initialized(object sender, EventArgs e)
        {
            this.MouseLeftButtonDown += delegate { DragMove(); };
        }
        /// <summary>
        /// Naciśnięcie przycisku powoduje zminimalizowanie aplikacj, jendocześnie nie zatrzymując działania programu
        /// </summary>
        private void MinimalizeBtn(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
    }
}
