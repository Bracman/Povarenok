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

            labelTypeDish.IsEnabled = false;
            comboxTypeDish.IsEnabled = false;
            loadTypeofDish();
        }

        private IList<Product> products = new List<Product>();


        public void  loadProducts()
        {
            try
            {
                products.Clear();
                DataTable dataTable = new DataTable();
                dataTable.Rows.Clear();
                ConnectionDataBase dataBase = new ConnectionDataBase();
                dataTable = dataBase.StoredProcedureNotParametr("selectProducts");
               
                foreach (DataRow dr in dataTable.Rows)
                {
                    products.Add(new Product
                    {
                        inputDates = dr[0].ToString()
                    });

                }
                listDates.ItemsSource = null;
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
            public string inputDates { get; set; }
            public bool IsSelected { get; set; }
        }
         
        

        private void DishRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            loadProducts();
            datagridProduct.Visibility = Visibility.Hidden;
            dataGrid.Visibility = Visibility.Visible;
            labelTypeDish.IsEnabled = false;
            comboxTypeDish.IsEnabled = false;
            activateSearch.IsEnabled = true;
        }

        private void ProductRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            datagridProduct.Visibility = Visibility.Visible;
            dataGrid.Visibility = Visibility.Hidden;
            labelTypeDish.IsEnabled = true;
            comboxTypeDish.IsEnabled = true;
            
        }

        private void activateSearch_Click(object sender, RoutedEventArgs e)
        {
            var selectedPassages = products.Where(ps => ps.IsSelected == true).ToList();
            string message="";
            foreach (var passage in selectedPassages)
            {
                message += passage.inputDates + ",";
            }
            DataTable dataTable = new DataTable(); 
            
            if(DishRadioButton.IsChecked==true)
            {
                ConnectionDataBase dataBase = new ConnectionDataBase();
                dataTable = dataBase.StoredProcedure("find_dishes", "input", message);
                dataGrid.ItemsSource = dataTable.DefaultView;
            }
            if (ProductRadioButton.IsChecked==true)
            {
                ConnectionDataBase dataBase = new ConnectionDataBase();
                dataTable = dataBase.StoredProcedure("withdrawProducts", "dish", message);
                datagridProduct.ItemsSource = dataTable.DefaultView;
            }
           
            


        }
        
        public void loadTypeofDish()
        {
            try
            {
                DataTable dataTable = new DataTable();

                dataTable.Rows.Clear();
                ConnectionDataBase dataBase = new ConnectionDataBase();
                dataTable = dataBase.StoredProcedureNotParametr("selectTypeDish");

                comboxTypeDish.ItemsSource = dataTable.DefaultView;
                comboxTypeDish.DisplayMemberPath = "nameTypeDish";
                comboxTypeDish.SelectedValuePath = "codeTypeDish";
            }
            catch (Exception x)
            {
                MessageBox.Show(x.GetBaseException().ToString(), "Error",
                           MessageBoxButton.OK, MessageBoxImage.Error);
            }


        }

        private void comboxTypeDish_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                products.Clear();
                DataRowView oDataRowView = comboxTypeDish.SelectedItem as DataRowView;
                if (oDataRowView != null)     
                   
                {
                    DataTable dataTable = new DataTable();
                    dataTable.Rows.Clear();
                    ConnectionDataBase dataBase = new ConnectionDataBase();
                    dataTable = dataBase.StoredProcedure("selectDish", "namet", oDataRowView.Row["nameTypeDish"].ToString());

                    
                    foreach (DataRow dr in dataTable.Rows)
                    {
                        products.Add(new Product
                        {
                            inputDates = dr[0].ToString()
                        });

                    }
                    listDates.ItemsSource = null;
                    listDates.ItemsSource = products;
                }                           

            }
            catch (Exception x)
            {
                MessageBox.Show(x.GetBaseException().ToString(), "Error",
                           MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
