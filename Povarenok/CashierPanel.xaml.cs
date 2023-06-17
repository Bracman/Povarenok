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
using System.Data;
using MySql.Data.MySqlClient;

namespace Povarenok
{
    /// <summary>
    /// Логика взаимодействия для CashierPanel.xaml
    /// </summary>
    public partial class CashierPanel : Window
    {
        public CashierPanel()
        {
            InitializeComponent();           
           
        }

        private IList<Product> products = new List<Product>();


        public void  loadProducts()
        {
            try
            {
                DataTable dataTable = new DataTable();

                dataTable.Rows.Clear();
                ConnectionDataBase dataBase = new ConnectionDataBase();
                dataTable = dataBase.StoredProcedureNotParametr("selectProducts");
               
                foreach (DataRow dr in dataTable.Rows)
                {
                    products.Add(new Product
                    {
                        nameProduct = dr[0].ToString()
                    });

                }
                listDates.ItemsSource = products;        

                

            }
            catch (Exception x)
            {
                MessageBox.Show(x.GetBaseException().ToString(), "Error",
                           MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public class Product
        {
            public string nameProduct { get; set; }
            public bool IsSelected { get; set; }
        }

      
        public void addColumnDataGrid(string [] headers)
        {
            dataGrid.Columns.Clear();
            foreach (string header in headers)
            {
                DataGridTextColumn c = new DataGridTextColumn();
                c.Header = header;
                dataGrid.AutoGenerateColumns = false;
                dataGrid.CanUserResizeColumns = false;
                dataGrid.CanUserResizeRows = false;
                dataGrid.Columns.Add(c);
                
               
            }
        }

        private void DishRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            loadProducts();
            
        }

        private void ProductRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            
        }

        private void activateSearch_Click(object sender, RoutedEventArgs e)
        {
            var selectedPassages = products.Where(ps => ps.IsSelected == true).ToList();
            string message="";
            foreach (var passage in selectedPassages)
            {
                message += passage.nameProduct + ",";
            }
            DataTable dataTable = new DataTable();                     
           
            ConnectionDataBase dataBase = new ConnectionDataBase();
            dataTable = dataBase.StoredProcedure("find_dishes","input", message);
            dataGrid.ItemsSource = dataTable.DefaultView;


        }
    }
}
