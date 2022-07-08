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

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

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
                    // mail.Body = "Witaj" + txtName.Text + "</br>" + "Data zameldowania:" + datePicker1.SelectedDate.Value + "</br>" + "Data wymeldowania" + datePicker2.SelectedDate.Value;
                    mail.IsBodyHtml = true;

                    using (SmtpClient smtp = new SmtpClient("poczta.interia.pl", 587))
                    {
                        smtp.Credentials = new System.Net.NetworkCredential("vip.apartaments@interia.pl", "vipapartament12345");
                        smtp.EnableSsl = true;
                        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                        smtp.Send(mail);
                        //MessageBox.Show("Wyslano!");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Potwierdzenie nie zostało wysłane na maila, ponieważ podano błędny mail", "Potwierdzenie", MessageBoxButton.OK, MessageBoxImage.Information);
            }

        }



        public void Button_Click(object sender, RoutedEventArgs e)
        {
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


            if (Name != "" && Surname != "" && Phone != "" && Email != "" && BirthDate != null && room_type != 0 && paymentMethod != 0 && CheckIn != null && CheckOut != null && datePicker1.SelectedDate.Value != null && datePicker2.SelectedDate.Value != null && showPrice.Text != "- - - -")
            {
                if (datePicker1.SelectedDate.Value > datePicker2.SelectedDate.Value)
                {
                    textBlock10.Text = "* Wybierz daty poprawnie";
                }

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














            // This block sets the TextBlock to a sensible default if dates haven't been picked
            if (!datePicker1.SelectedDate.HasValue || !datePicker2.SelectedDate.HasValue)
            {
                //textBlock10.Text = "Wybierz daty";
                return;
            }

            // Because the nullable SelectedDate properties must have a value to reach this point, 
            // we can safely reference them - otherwise, these statements throw, as you've discovered.
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

        public void Standard_Checked(object sender, RoutedEventArgs e)
        {
            checkPrice();

        }

        public void Superior_Checked(object sender, RoutedEventArgs e)
        {
            checkPrice();

        }

        public void Deluxe_Checked(object sender, RoutedEventArgs e)
        {
            checkPrice();
        }

       

        public void payByCash(object sender, RoutedEventArgs e)
        {

        }

        public void payByCard(object sender, RoutedEventArgs e)
        {

        }

        public void checkPrice()
        {
            if (!datePicker1.SelectedDate.HasValue || !datePicker2.SelectedDate.HasValue)
            {
                //textBlock10.Text = "Wybierz daty";
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

        private void datePicker1_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            checkPrice();
        }

        private void ask(object sender, RoutedEventArgs e)
        {
            AskForm dashboard = new AskForm();
            dashboard.Show();
        }
    }
    }

