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
using System.Net.Mail;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for AskForm.xaml
    /// </summary>
    public partial class AskForm : Window
    {
        public AskForm()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Grid_Initialized(object sender, EventArgs e)
        {
            this.MouseLeftButtonDown += delegate { DragMove(); };

        }


        private void sendMail()
        {
            string body = "<h3>Klient, " + email_txt.Text + " zadał zapytanie<br/><br/><br/></h3>";
            body += "Treść: <br/>" + context_txt.Text + "<br/>";

            try
            {
                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress("vip.apartaments@interia.pl", "Formularz Vip Apartaments");
                    mail.To.Add("vip.apartaments@interia.pl");
                    mail.Subject = topic_txt.Text;
                    mail.Body = body;
                    mail.IsBodyHtml = true;

                    using (SmtpClient smtp = new SmtpClient("poczta.interia.pl", 587))
                    {
                        smtp.Credentials = new System.Net.NetworkCredential("vip.apartaments@interia.pl", "vipapartament123456");
                        smtp.EnableSsl = true;
                        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                        errorMsg.Content = "Twoje zapytanie zostało przesłane!";
                        email_txt.Text = "";
                        topic_txt.Text = "";
                        context_txt.Text = "";
                        
                        smtp.Send(mail);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void SendBtn_Click(object sender, RoutedEventArgs e)
        {
            if(!string.IsNullOrWhiteSpace(context_txt.Text) && !string.IsNullOrWhiteSpace(email_txt.Text) && !string.IsNullOrWhiteSpace(topic_txt.Text))
            {
            sendMail();
            }
            else
            {
                errorMsg.Content = "Wypełnij wszystkie pola";
            }
        }
    }
}
