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
using System.Text.RegularExpressions;

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
            comboBoxList();
        }

        /// <summary>
        /// Nacisniecie przycisku powoduje zamkniecie aktualnego okna.
        /// </summary>
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();

        }
        /// <summary>
        /// Odpowiada za możliwość przesuwania aplikacji po ekranie w momencie przycisniecia lewego przycisku myszy
        /// </summary>
        private void Grid_Initialized(object sender, EventArgs e)
        {
            this.MouseLeftButtonDown += delegate { DragMove(); };

        }

       /// <summary>
       /// Czyści wszystkie pola z wprowadzonego tekstu
       /// </summary>
        public void clearData()
        {
            name_txt.Clear();
            surname_txt.Clear();
            phone_txt.Clear();
            mail_txt.Clear();
            birthNamePicker.SelectedDate = null;
        }
        private void ClearDataBtn_Click(object sender, RoutedEventArgs e)
        {
            clearData();
        }
 

        /// <summary>
        /// Po wciśnieciu przycisku porównuje wybraną wartość w comboboxie z id rezerwacji w bazie danych i usuwa wybraną pozycje z bazy
        /// </summary>
        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            using (DbConn db = new DbConn())
            {
                int getId = (int)combobox.SelectedValue;
                var context = db.bookings.First(b => b.Id == getId);
                var context1 = db.details.First(b => b.Id_book == getId);
                db.bookings.Remove(context);
                db.details.Remove(context1);
                db.SaveChanges();
                byleco();
                combobox.SelectedValue = null;
                comboBoxList();
                clearData();

            }



        }


        /// <summary>
        /// W momencie wybrania wartości z comboboxa jest ona przyrównywana do id rezerwacji, na podstawie której pola w panelu zostają automatycznie wypełnione
        /// </summary>
        private void combobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            using (DbConn db = new DbConn())
            {
                if (combobox.SelectedValue != null)
                {
                    int getId = (int)combobox.SelectedValue;
                    var query = db.clients.Join(db.bookings, c => c.Id, b => b.Id_client, (c, b) => new { c = c, b = b }).First(d => d.b.Id == getId);
                    name_txt.Text = query.c.FirstName;
                    surname_txt.Text = query.c.LastName;
                    phone_txt.Text = query.c.Phone;
                    mail_txt.Text = query.c.Email;
                    birthNamePicker.SelectedDate = query.c.BirthDate;
                }
            }
        }
        /// <summary>
        /// Tworzy liste w comboboxie na podstawie utworzsonych rezerwacji
        /// </summary>
        private void comboBoxList()
        {
            List<booking> finder = new List<booking>();
            using (DbConn db = new DbConn())
            {
                var listItems = db.bookings.ToList();
                finder = listItems;
                combobox.ItemsSource = finder;
                combobox.DisplayMemberPath = "Id";
                combobox.SelectedValuePath = "Id";
            }
        }
        /// <summary>
        /// Odpowaida za inicjalizacje datagrida, łączy ze sobą kilka tabel z bazy danych i w logiczny sposób wyświetla wszystkie niezbędne informacje
        /// potrzebne obsludze do zarzadzania rezerwacjami
        /// </summary>
        private void byleco()
        {
           


            List<dataInDataGrid> list = new List<dataInDataGrid>();

            list.Clear();

            using (DbConn db = new DbConn())
            {
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

                datagrid.Items.Clear();
                foreach (var data in id_finder)
                {
                    datagrid.Items.Add(new dataInDataGrid
                    {
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
        /// <summary>
        /// Uzycie przycisku spowoduje sprawdzenie czy została wybrana wartosc z comboxa, w przeciwnym razie wyswietlany zostaje komunikat z bledem.
        /// Następnie wartość wybrana z comboboxa zostaje powiazana z id rezerwacji z bazy danych i odpowiednio powiązane dane zostają automaycznie wpisane w pola tekstowe,
        /// Na końcu program sprawdza czy wszystkie pola zostały wypełnione, jeżeli tak to wszystkie wpisane dane zostają poddane operacji update.
        /// Natomiast jeżeli chociaż jedno pole bedzie puste wyświetli sie komunikat o niewypełnieniu pól.
        /// </summary>
        private void UpdateBtn_Click(object sender, RoutedEventArgs e)
        {
            using (DbConn db = new DbConn())
            {
                if (combobox.SelectedValue != null && !string.IsNullOrWhiteSpace(name_txt.Text) && !string.IsNullOrWhiteSpace(surname_txt.Text) && !string.IsNullOrWhiteSpace(phone_txt.Text) && !string.IsNullOrWhiteSpace(mail_txt.Text) && birthNamePicker.SelectedDate.Value != null)
                {
                    int getId = (int)combobox.SelectedValue;
                    var ID_b = db.bookings.First(b => b.Id == getId);

                    var query = db.clients.Join(db.bookings, c => c.Id, b => b.Id_client, (c, b) => new { c = c, b = b }).First(d => d.b.Id == getId);
                    if (ID_b != null)
                    {
                        query.c.FirstName = name_txt.Text;
                        query.c.LastName = surname_txt.Text;
                        query.c.Phone = phone_txt.Text;
                        query.c.Email = mail_txt.Text;
                        query.c.BirthDate = birthNamePicker.SelectedDate.Value;
                        db.SaveChanges();
                        MessageBox.Show("Zmiany zostały wprowadzone", "Potwierdzenie", MessageBoxButton.OK, MessageBoxImage.Information);
                        byleco();


                    }
                }
                else if (!string.IsNullOrWhiteSpace(name_txt.Text) || !string.IsNullOrWhiteSpace(surname_txt.Text) || !string.IsNullOrWhiteSpace(phone_txt.Text) || !string.IsNullOrWhiteSpace(mail_txt.Text) || birthNamePicker.SelectedDate.Value == null)
                {
                    MessageBox.Show("Wypełnij wszystkie pola", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    MessageBox.Show("Wybierz Id!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                }



            }
        }


        /// <summary>
        /// Funkacja na poststawie wybranego id z comboboxa powiązuje je z rezerwacją i mamy możliwość zarządzania statusem płatności.
        /// Opcja TAK powoduje zatwierdzenie płatności
        /// Opcja NIE powoduje anulowanie płatności
        /// </summary>
        private void checkpayment_Click(object sender, RoutedEventArgs e)
        {
            if (combobox.SelectedValue != null)
            {
                bool pay = false;
                if (btnYes.IsChecked == true)
                {
                    pay = true;
                    MessageBox.Show("Potwierdzono wpłatę", "Potwierdzenie", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (btnNo.IsChecked == true)
                {
                    pay = false;
                    MessageBox.Show("Anulowano wpłatę", "Anulowanie", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Zaznacz przycisk", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                using (DbConn db = new DbConn())
                {
                    int getId = (int)combobox.SelectedValue;
                    var payment = db.bookings.First(c => c.Id == getId);
                    if (payment != null)
                    {
                        payment.Pay = pay;
                        db.SaveChanges();
                        byleco();
                    }

                }
            }
            else
            {
                MessageBox.Show("Wybierz Id!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        public void btnNo_Checked(object sender, RoutedEventArgs e)
        {

        }

        public void btnYes_Checked(object sender, RoutedEventArgs e)
        {

        }
        /// <summary>
        /// Pozwala na wprowadzanie tylko numerycznych wartości do pola z numerem telefonu
        /// </summary>
        private void phone_txt_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
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