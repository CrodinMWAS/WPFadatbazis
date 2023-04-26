using System;
using System.Collections.Generic;
using System.IO;
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
using Microsoft.Win32;
using MySql.Data.MySqlClient;

namespace WpfAppAdatbazis
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {

        // AZ ADATBÁZIS REKORDJAINAK NEVÉBŐL KIVETTEM AZ ÉKEZETEKET!!!!!!!
        const string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=hardver;";
        public MySqlConnection SQLConnection = new MySqlConnection(connectionString);
        public List<Termek> termekek = new List<Termek>();
        public MainWindow()
        {
            InitializeComponent();
            LoadDatabase();
            LoadDropdownIn();
            LoadContentIn();

        }

        public void LoadDatabase()
        {
            try
            {
                SQLConnection.Open();
            }
            catch (MySqlException err)
            {
                MessageBox.Show("Unable to connect to the database!");
                MessageBox.Show(err.Message);
                Environment.Exit(0);
            }
        }

        public MySqlDataReader Query(string par)
        {
            string SQLString = par;
            MySqlCommand SQLquery = new MySqlCommand(SQLString, SQLConnection);
            try
            {
                MySqlDataReader SQLreturn = SQLquery.ExecuteReader();
                return SQLreturn;
            }
            catch (MySqlException err)
            {
                Console.WriteLine("Unable to connect to the database!");
                Console.WriteLine(err.Message);
                throw;
            }
        }
        
        public void LoadDropdownIn()
        {
            cbKategoria.Items.Add(" ");
            cbGyarto.Items.Add(" ");
            cbKategoria.SelectedIndex = 0;
            cbGyarto.SelectedIndex = 0;
            MySqlDataReader returned = Query("SELECT DISTINCT Kategoria FROM termekek");
            while (returned.Read())
            {
                cbKategoria.Items.Add(returned.GetString("Kategoria"));
            }
            returned.Close();
            returned = Query("SELECT DISTINCT Gyarto FROM termekek");
            while (returned.Read())
            {
                cbGyarto.Items.Add(returned.GetString("Gyarto"));
            }
            returned.Close();
        }

        public void LoadContentIn(string par = "SELECT * FROM termekek")
        {
            termekek.Clear();
            dgTermekek.ItemsSource = termekek;
            
            MySqlDataReader returned = Query(par);
            while (returned.Read())
            {
                Termek newProduct = new Termek(
                    returned.GetString("Kategoria"),
                    returned.GetString("Gyarto"),
                    returned.GetString("Nev"),
                    returned.GetInt32("Ar"),
                    returned.GetInt32("Garido")
                    );
                termekek.Add(newProduct);
            }
            returned.Close();

            dgTermekek.ItemsSource = termekek;
            dgTermekek.Items.Refresh();
        }
        private void btnSzukit_Click(object sender, RoutedEventArgs e)
        {
            string queryString = "SELECT * FROM termekek";

            if (cbGyarto.SelectedIndex != 0 && cbKategoria.SelectedIndex != 0 && txtTermek.Text != "")
            {
                queryString += $" WHERE Kategoria LIKE '{cbKategoria.SelectedItem}' AND Gyarto LIKE '{cbGyarto.SelectedItem}' AND Nev LIKE '%{txtTermek.Text}%'";
            }
            else if (cbGyarto.SelectedIndex != 0 && cbKategoria.SelectedIndex != 0)
            {
                queryString += $" WHERE Kategoria LIKE '{cbKategoria.SelectedItem}' AND Gyarto LIKE '{cbGyarto.SelectedItem}'";
            }
            else if (cbKategoria.SelectedIndex != 0)
            {
                queryString += $" WHERE Kategoria LIKE '{cbKategoria.SelectedItem}'";
            }
            else if(cbGyarto.SelectedIndex != 0)
            {
                queryString += $" WHERE Gyarto LIKE '{cbGyarto.SelectedItem}'";
            }

            LoadContentIn(queryString);
            queryString = "";
        }

        private void DataBaseclose()
        {
            SQLConnection.Close();
            SQLConnection.Dispose();
        }

        private void btnMentes_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
            StreamWriter sw = new StreamWriter(openFileDialog.FileName);
                foreach (var item in termekek)
                {
                    sw.WriteLine(item.ToCSVString());
                }
                sw.Close();

            }
        }
    }
}
