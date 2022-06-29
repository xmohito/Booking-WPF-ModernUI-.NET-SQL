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
            LoadGrid();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Grid_Initialized(object sender, EventArgs e)
        {
            this.MouseLeftButtonDown += delegate { DragMove(); };
        }
        SqlConnection sqlCon = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Admin\Desktop\projekt\WpfApp_1\WpfApp_1\VipApartament.mdf;Integrated Security=True");

        public void LoadGrid()
        {
            SqlCommand cmd = new SqlCommand("select * from clients", sqlCon);
            DataTable dt = new DataTable();
            sqlCon.Open();
            SqlDataReader sdr = cmd.ExecuteReader();
            dt.Load(sdr);
            sqlCon.Close();
            datagrid.ItemsSource = dt.DefaultView;
        }
        public void clearData()
        {
            name_txt.Clear();
            surname_txt.Clear();
            phone_txt.Clear();
            mail_txt.Clear();
            date_txt.Clear();
            id_txt.Clear();

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
            try
            {
                if (isValid())
                {
                    SqlCommand cmd = new SqlCommand("INSERT INTO clients VALUES (@Name, @Surname, @Phone, @Mail, @BirthDate)", sqlCon);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@Name", name_txt.Text);
                    cmd.Parameters.AddWithValue("@Surname", surname_txt.Text);
                    cmd.Parameters.AddWithValue("@Phone", phone_txt.Text);
                    cmd.Parameters.AddWithValue("@Mail", name_txt.Text);
                    cmd.Parameters.AddWithValue("@BirthDate", date_txt.Text);
                    sqlCon.Open();
                    cmd.ExecuteNonQuery();
                    sqlCon.Close();
                    LoadGrid();
                    MessageBox.Show("Zmiany zostały wprowadzone", "Saved", MessageBoxButton.OK, MessageBoxImage.Information);
                    clearData();
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            sqlCon.Open();
            SqlCommand cmd = new SqlCommand("delete from clients where ID = " +id_txt.Text+ " ", sqlCon);
            try
            {
                cmd.ExecuteNonQuery();
                MessageBox.Show("Rekord został usunięty z bazy danych", "Deleted", MessageBoxButton.OK, MessageBoxImage.Information);
                sqlCon.Close();
                clearData();
                LoadGrid();
                sqlCon.Close();
            }
            catch (SqlException ex)
            {
               MessageBox.Show("Nie usunięto rekordu" +ex.Message);
            }
            finally
            {
                sqlCon.Close();
            }
        }

        private void UpdateBtn_Click(object sender, RoutedEventArgs e)
        {
            sqlCon.Open();
            SqlCommand cmd = new SqlCommand("update clients set Name ='"+name_txt.Text+"', Surname = '"+surname_txt.Text+"', Phone = '"+phone_txt.Text+"', Mail = '"+mail_txt.Text+"', BirthDate = '"+date_txt.Text+"' WHERE ID = '"+id_txt.Text+"' ", sqlCon);
            try
            {
                cmd.ExecuteNonQuery();
                MessageBox.Show("Rekor został uaktualniony", "Updated", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch(SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                sqlCon.Close();
                clearData();
                LoadGrid();
            }
        }
    }
}
