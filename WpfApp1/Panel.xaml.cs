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
using System.Windows.Shapes;
using System.Data.SqlClient;
using System.Data;
using WpfApp_1.MVVM.ViewModel;
using WpfApp1;

namespace WpfApp_1
{
    /// <summary>
    /// Interaction logic for Panel.xaml
    /// </summary>
    public partial class Panel : Window
    {
        public Panel()
        {
            InitializeComponent();
            //LoadGrid();
            byleco();
        }


        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            
        }

        private void Grid_Initialized(object sender, EventArgs e)
        {
            this.MouseLeftButtonDown += delegate { DragMove(); };

        }

        public void LoadGrid()
        {


            using (DbConn db = new DbConn())
            {
                //var listItems = db.clients.ToList();
                //datagrid.ItemsSource = listItems;


            }
            //SqlCommand cmd = new SqlCommand("select * from clients", sqlCon);
            //DataTable dt = new DataTable();
            //sqlCon.Open();
            //SqlDataReader sdr = cmd.ExecuteReader();
            //dt.Load(sdr);
            //sqlCon.Close();
            //datagrid.ItemsSource = dt.DefaultView;

        }
        public void clearData()
        {
            name_txt.Clear();
            surname_txt.Clear();
            phone_txt.Clear();
            mail_txt.Clear();
            date_txt.Clear();
        }
        private void ClearDataBtn_Click(object sender, RoutedEventArgs e)
        {
            clearData();
        }

        public bool isValid()
        {
            if (name_txt.Text == string.Empty || surname_txt.Text == string.Empty ||
                phone_txt.Text == string.Empty || mail_txt.Text == string.Empty ||
                date_txt.Text == string.Empty)
            {
                MessageBox.Show("Wprowadz wszystkie dane!", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }

        private void InsertBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void UpdateBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void combobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            using (DbConn db = new DbConn())
            {
                int getId = (int)combobox.SelectedValue;
                var client = db.clients.First(c => c.Id == getId);
                name_txt.Text = client.FirstName;
                surname_txt.Text = client.LastName;
                phone_txt.Text = client.Email;




                

            }

        }

        private void byleco()
        {
            //List<client> list = new List<client>();
            //using(DbConn db = new DbConn())
            //{
            //    var listItems = db.clients.ToList();
            //    list = listItems;
            //    combobox.ItemsSource = list;
            //    combobox.DisplayMemberPath = "Id";
            //    combobox.SelectedValuePath = "Id";
            //}


            List<dataInDataGrid> list = new List<dataInDataGrid>();
            using (DbConn db = new DbConn())
            {



                //var id_finder = (from clients in db.clients
                //                 join booking in db.bookings on clients.Id equals booking.Id_client
                //                 select new
                //                 {
                //                     Id = booking.Id,
                //                     Imie = clients.FirstName,
                //                     Nazwisko = clients.LastName,
                //                     Telefon = clients.Phone,
                //                     Email = clients.Email,
                //                     DataUrodzenia = clients.BirthDate

                //                 }).ToList();

                //foreach (var data in id_finder)
                //{
                //    datagrid.Items.Add(new dataInDataGrid { Id = data.Id, Imie = data.Imie, Nazwisko = data.Nazwisko, Telefon = data.Telefon, Email = data.Email, DataUrodzenia = data.DataUrodzenia });

                //}
                var id_finder = (from booking in db.bookings
                                 join clients in db.clients on booking.Id_client equals clients.Id
                                 join details in db.details on booking.Id equals details.Id_book
                                 join payments in db.payments on booking.Id_method_of_payment equals payments.Id
                                 select new
                                 {
                                   Id = booking.Id,
                                   Imie = clients.FirstName,
                                   Nazwisko = clients.LastName,
                                   Telefon = clients.Phone,
                                   Email = clients.Email,
                                   DataUrodzenia = clients.BirthDate,
                                   Zameldowanie = details.DateFrom,
                                   Wymeldowanie = details.DateTo,
                                   RodzajPlatnosci = payments.payment_method,
                                   DoZaplaty = booking.ToPay,
                                   CzyZaplacil = booking.Pay
                                   
                                 }).ToList();

                foreach (var data in id_finder)
                {
                    datagrid.Items.Add(new dataInDataGrid {
                                                           Id = data.Id,
                                                           Imie = data.Imie,
                                                           Nazwisko = data.Nazwisko,
                                                           Telefon = data.Telefon,
                                                           Email = data.Email,
                                                           DataUrodzenia = data.DataUrodzenia,
                                                           Zameldowanie = data.Zameldowanie,
                                                           Wymeldowanie = data.Wymeldowanie,
                                                           RodzajPlatnosci = data.RodzajPlatnosci,
                                                           DoZaplaty = data.DoZaplaty,
                                                           CzyZaplacil = data.CzyZaplacil
                                                           });

                }
             
            }

        }
    }

    public class dataInDataGrid
    {
        public int Id { get; set; }
        public string Imie { get; set; }
        public string Nazwisko { get; set; }
        public string Telefon { get; set; }
        public string Email { get; set; }
        public DateTime DataUrodzenia { get; set; }
        public DateTime Zameldowanie { get; set; }
        public DateTime Wymeldowanie { get; set; }
        public string RodzajPlatnosci { get; set; }
        public int DoZaplaty { get; set; }
        public bool CzyZaplacil { get; set; }
    }
}
