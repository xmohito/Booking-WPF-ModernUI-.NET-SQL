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
using System.Data.SqlClient;
using System.Data;
using WpfApp_1.MVVM.ViewModel;

namespace WpfApp_1.MVVM.View
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : UserControl
    {
        public LoginView()
        {
            InitializeComponent();
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {


            using (VipApartament db = new VipApartament())
            {
              int userChecker = db.User.Where(c => c.Username == txtUsername.Text && c.Pass == txtPassword.Password).Count();
                if(userChecker == 1)
                {
                    Panel dashboard = new Panel();
                    dashboard.Show();
                }
                else
                {
                    MessageBox.Show("Nazwa użytkownika lub hasło została wprowadzona błędnie.");
                }

            }










           
        }
    }

}
