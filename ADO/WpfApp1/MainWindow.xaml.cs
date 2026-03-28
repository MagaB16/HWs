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

namespace WpfApp1
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

        private void All_Click(object sender, RoutedEventArgs e)
        {
            using SqlConnection conn = new("Data Source=localhost;Database=School;Trusted_Connection=True;");
            conn.Open();
            var command = new SqlCommand("SELECT * FROM Students", conn);

            SqlDataReader reader = command.ExecuteReader();
            var schema = reader.GetColumnSchema();
            foreach (var column in schema)
            {
                Results.Text += $"{column.ColumnName} \t";
            }   
            while (reader.Read())
            {
                Results.Text += $"\n{reader.GetInt32(0)}\t{reader.GetString(1)}\t{reader.GetString(2)}\t{reader.GetInt32(3)}";
            }
            conn.Close();
        }

        private void ShowMax_Click(object sender, RoutedEventArgs e)
        {
            using SqlConnection conn = new("Data Source=localhost;Database=School;Trusted_Connection=True;");
            conn.Open();
            var command = new SqlCommand($"SELECT Max({Max.Text}) FROM Students", conn);

            var reader = command.ExecuteScalar();
            Results.Text = reader.ToString();
        }

        private void ShowMin_Click(object sender, RoutedEventArgs e)
        {
            using SqlConnection conn = new("Data Source=localhost;Database=School;Trusted_Connection=True;");
            conn.Open();
            var command = new SqlCommand($"SELECT Min({Min.Text}) FROM Students", conn);

            var reader = command.ExecuteScalar();
            Results.Text = reader.ToString();
        }

        private void Enter_Click(object sender, RoutedEventArgs e)
        {
            using SqlConnection conn = new("Data Source=localhost;Database=School;Trusted_Connection=True;");
            conn.Open();
            var command = new SqlCommand(commandent.Text, conn);

            SqlDataReader reader = command.ExecuteReader();
            var schema = reader.GetColumnSchema();
            foreach (var column in schema)
            {
                Results.Text += $"{column.ColumnName} \t";
            }
            while (reader.Read())
            {
                Results.Text += $"\n{reader.GetInt32(0)}\t{reader.GetString(1)}\t{reader.GetString(2)}\t{reader.GetInt32(3)}";
            }
            conn.Close();
        }
    }
}
