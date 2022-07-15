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
using System.Data;
using WpfApp1;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace WpfApp_1.MVVM.View
{
    /// <summary>
    /// Interaction logic for BookingView.xaml
    /// </summary>
    public partial class BookingView : UserControl
    {
        public BookingView()
        {
            InitializeComponent();


        }


        /// <summary>
        /// Wysyła mail z potwiedzeniem rezerwacji użytkowika, uwczesnie sprawdzając czy podany mail jest prawidłowy.
        /// W treści maila znajdują się wybrane przez uzytkownika daty zameldowania oraz w podsumowaniu widnieje cena, którą należy zapłacić.
        /// </summary>
    private void bookSendMail()
        {
            string body = "<h1>Witaj, " + txtName.Text + "<br/></h1>";
            body += "Dziękujemy za dokonanie rezerwacji.<br/><br/>";
            body += "Data zameldowania: " + datePicker1.SelectedDate.Value + "<br/>";
            body += "Data wymeldowania: " + datePicker2.SelectedDate.Value + "<br/><br/>";
            body += "Da zapłaty: " + showPrice.Text + " zł<br/>";

            try
            {
                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress("vip.apartaments@interia.pl", "Vip Apartaments");
                    mail.To.Add(txtMail.Text);
                    mail.Subject = "Potwierdzenie dokonania rezerwacji";
                    mail.Body = body;
                    mail.IsBodyHtml = true;

                    using (SmtpClient smtp = new SmtpClient("poczta.interia.pl", 587))
                    {
                        smtp.Credentials = new System.Net.NetworkCredential("vip.apartaments@interia.pl", "vipapartament123");
                        smtp.EnableSsl = true;
                        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                        smtp.Send(mail);
                    }
                }
            }
            catch
            {
                MessageBox.Show("Potwierdzenie nie zostało wysłane na maila, ponieważ podano błędny mail", "Potwierdzenie", MessageBoxButton.OK, MessageBoxImage.Information);
            }

        }
        /// <summary>
        /// Sprawdza czy różnica w latach miedzy dniem dzisiejszym, a dniem wybranym przez użytkownika jest wieksza niz 18 lat,
        /// jeżeli nie to wyświetlony zostaje komunikat, że użytkownik nie ma ukończone 18 lat
        /// </summary>
        private void birthDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            var ageInYears = GetDifferenceInYears(birthDate.SelectedDate.Value, DateTime.Today);
            if (ageInYears < 18)
            {
                ageLabel.Text = "* nie masz ukończone 18 lat";
            }
            else
            {
                ageLabel.Text = "";
            }
        }
        /// <summary>
        /// Zlicza lata
        /// </summary>
        int GetDifferenceInYears(DateTime startDate, DateTime endDate)
        {
            return (endDate.Year - startDate.Year - 1) +
                (((endDate.Month > startDate.Month) ||
                ((endDate.Month == startDate.Month) && (endDate.Day >= startDate.Day))) ? 1 : 0);
        }

        /// <summary>
        /// Odpowiedzialne za pokazywanie 'zł' przy cenie.
        /// Jeżeli cena nie jest akutalnie wyświetlana, napis 'zł' również się nie wyświetla
        /// </summary>
        public void showZl()
        { 
            if (showPrice.Text != "- - - -")
            {
                zl.Text = "zł";
            }
            else
            {
                zl.Text = "";
            }
        }
        /// <summary>
        /// Odpowiedzialne za całą logikę formularza, po wyzwoleniu przycisku sprawdza czy wypełnione są wszystkie pola, czy daty zameldowania zostały wybrane poprawnie oraz czy użytkownik ma ukończone 18lat.
        /// Jeżeli wszystkie warunki zostaną spełnione to zatwierdzenie rezerwacji zostanie zakończone powodzeniem.
        /// W zależności od zaistniałych nieścisłości zostanie wyświetlony odpowiedni komunikat z błędem, np. gdy daty zameldowania zostaną wybrane źle pojawi się komunikat o źle wybranych datach.
        /// </summary>
        public void Button_Click(object sender, RoutedEventArgs e)
        {
            showZl();
            
            string Name = txtName.Text;
            string Surname = txtSurname.Text;
            string Phone = txtPhone.Text;
            string Email = txtMail.Text;
            DateTime? BirthDate = birthDate.SelectedDate;
            DateTime? CheckIn = datePicker1.SelectedDate;
            DateTime? CheckOut = datePicker2.SelectedDate;

            int room_type = 0;
            if (btnStandard.IsChecked == true)
            {
                room_type = 1;
            }
            else if (btnSuperior.IsChecked == true)
            {
                room_type = 3;
            }
            else if (btnDeluxe.IsChecked == true)
            {
                room_type = 2;
            }
            else
            {
                ErrorLabel.Content = "Wypełnij wszystkie dane!";
            }
            int paymentMethod = 0;
            if (btnByCard.IsChecked == true)
            {
                paymentMethod = 2;
            }
            else if (btnByCash.IsChecked == true)
            {
                paymentMethod = 1;
            }
            else
            {
                ErrorLabel.Content = "Wypełnij wszystkie dane!";
            }

            if (ageLabel.Text != "* nie masz ukończone 18 lat" && !string.IsNullOrWhiteSpace(Name) && !string.IsNullOrWhiteSpace(Surname) && !string.IsNullOrWhiteSpace(Phone) && !string.IsNullOrWhiteSpace(Email) && BirthDate != null && room_type != 0 && paymentMethod != 0 && CheckIn != null && CheckOut != null && datePicker1.SelectedDate.Value != null && datePicker2.SelectedDate.Value != null && showPrice.Text != "- - - -")
            {
                using (DbConn db = new DbConn())
                {
                    var insertedClient = new client
                    {
                        FirstName = Name,
                        LastName = Surname,
                        Phone = Phone,
                        Email = Email,
                        BirthDate = BirthDate.Value
                    };
                    db.clients.Add(insertedClient);
                    db.SaveChanges();

                    var insertedBook = new booking
                    {
                        Id_client = insertedClient.Id,
                        Id_room = room_type,
                        Id_method_of_payment = paymentMethod,
                        ToPay = Convert.ToInt32(showPrice.Text),
                        Pay = false
                    };
                    db.bookings.Add(insertedBook);
                    db.SaveChanges();

                    var insertedDetails = new detail
                    {
                        Id_book = insertedBook.Id,
                        DateFrom = datePicker1.SelectedDate.Value,
                        DateTo = datePicker2.SelectedDate.Value
                    };
                    db.details.Add(insertedDetails);
                    db.SaveChanges();
                }
                bookSendMail();
                MessageBox.Show("Dokonano rezerwacji", "Potwierdzenie", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                ErrorLabel.Content = "Wypełnij wszystkie dane!";
            }
            if (!datePicker1.SelectedDate.HasValue || !datePicker2.SelectedDate.HasValue)
            {

                return;
            }
            DateTime start = datePicker1.SelectedDate.Value.Date;
            DateTime finish = datePicker2.SelectedDate.Value.Date;
            TimeSpan difference = finish.Subtract(start);


            int daysCount = Int32.Parse(difference.TotalDays.ToString());

            if (daysCount > 0)
            {
                //textBlock10.Text = difference.TotalDays.ToString();
                
            }
            else
            {
                textBlock10.Text = "* Wybierz daty poprawnie";
            }
        }


        private void Standard_Checked(object sender, RoutedEventArgs e)
        {
            checkPrice();
            showZl();

        }

        private void Superior_Checked(object sender, RoutedEventArgs e)
        {
            checkPrice();
            showZl();

        }

        private void Deluxe_Checked(object sender, RoutedEventArgs e)
        {
            checkPrice();
            showZl();
        }

       

        public void payByCash(object sender, RoutedEventArgs e)
        {

        }

        public void payByCard(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// Odpwoiedzialne za obliczanie ceny.
        /// W pierwszej kolejności sprawdzaje jest czy daty zameldowania są wybrane i czy są wybrane poprawnie
        /// Następnie w zależności od wybranego standardu apartamentu i ilości dni wyliczana jest cena
        /// </summary>
        public void checkPrice()
        {
            showZl();
            if (!datePicker1.SelectedDate.HasValue || !datePicker2.SelectedDate.HasValue)
            {
               
                return;
            }
            DateTime start = datePicker1.SelectedDate.Value.Date;
            DateTime finish = datePicker2.SelectedDate.Value.Date;
            TimeSpan difference = finish.Subtract(start);


            int daysCount = Int32.Parse(difference.TotalDays.ToString());
           
            if (daysCount > 0)
            {
               
                if (btnStandard.IsChecked == true)
                {
                    int price = 300;
                    int finalPrice = price * daysCount;
                    showPrice.Text = finalPrice.ToString();
                }
                else if (btnSuperior.IsChecked == true)
                {
                    int price = 500;
                    int finalPrice = price * daysCount;
                    showPrice.Text = finalPrice.ToString();
                }
                else if (btnDeluxe.IsChecked == true)
                {
                    int price = 700;
                    int finalPrice = price * daysCount;
                    showPrice.Text = finalPrice.ToString();
                }
            }
            else
            {
                showPrice.Text = "- - - -";
            }


            
        }
        /// <summary>
        /// Sprawdza poprawność wybranych dat, oraz odpowiednie zastosowanie showZl()
        /// </summary>
        private void datePicker1_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            checkPrice();

            if (datePicker1.SelectedDate.HasValue && datePicker2.SelectedDate.HasValue)
            {
                if (datePicker1.SelectedDate.Value > datePicker2.SelectedDate.Value)
                {
                    textBlock10.Text = "* Wybierz daty poprawnie";
                    showZl();
                }
                else
                {
                    textBlock10.Text = "";
                    showZl();
                }
            }
            else
            {
                textBlock10.Text = "";
                showZl();
            }
            
        }
        /// <summary>
        /// Odpowiedzialne za otwarcie okna z formularzem kontaktowym.
        /// </summary>
        private void ask(object sender, RoutedEventArgs e)
        {
            AskForm dashboard = new AskForm();
            dashboard.Show();
        }
        /// <summary>
        /// Ustawia możliwość wpisywania wyłącznie wartości numerycznych w polu numeru telefonu
        /// </summary>
        private void txtPhone_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);

        }
    }
    }

