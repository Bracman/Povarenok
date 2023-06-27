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
            loadDishes();
        }

        private IList<Dates> dates = new List<Dates>();


        public void  loadProducts()
        {
            try
            {
                dates.Clear();
                DataTable dataTable = new DataTable();
                dataTable.Rows.Clear();
                ConnectionDataBase dataBase = new ConnectionDataBase();
                dataTable = dataBase.StoredProcedureNotParametr("selectProducts");
               
                foreach (DataRow dr in dataTable.Rows)
                {
                    dates.Add(new Dates
                    {
                        inputDates = dr[0].ToString()
                    });

                }
                listDates.ItemsSource = null;
                listDates.ItemsSource = dates;        

            }
            catch (Exception x)
            {
                MessageBox.Show(x.GetBaseException().ToString(), "Error",
                           MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public class Dates
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
            var selectedPassages = dates.Where(ps => ps.IsSelected == true).ToList();
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

                comboxTypeOfDishOrderClient.ItemsSource = dataTable.DefaultView;
                comboxTypeOfDishOrderClient.DisplayMemberPath = "nameTypeDish";
                comboxTypeOfDishOrderClient.SelectedValuePath = "codeTypeDish";
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
                dates.Clear();
                DataRowView oDataRowView = comboxTypeDish.SelectedItem as DataRowView;
                if (oDataRowView != null)     
                   
                {
                    DataTable dataTable = new DataTable();
                    dataTable.Rows.Clear();
                    ConnectionDataBase dataBase = new ConnectionDataBase();
                    dataTable = dataBase.StoredProcedure("selectDish", "namet", oDataRowView.Row["nameTypeDish"].ToString());

                    
                    foreach (DataRow dr in dataTable.Rows)
                    {
                        dates.Add(new Dates
                        {
                            inputDates = dr[0].ToString()
                        });

                    }
                    listDates.ItemsSource = null;
                    listDates.ItemsSource = dates;
                }                           

            }
            catch (Exception x)
            {
                MessageBox.Show(x.GetBaseException().ToString(), "Error",
                           MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public void loadDishes()
        {
            try
            {
                dates.Clear();
                DataTable dataTable = new DataTable();
                dataTable.Rows.Clear();
                ConnectionDataBase dataBase = new ConnectionDataBase();
                dataTable = dataBase.StoredProcedureNotParametr("loadDishes");

                foreach (DataRow dr in dataTable.Rows)
                {
                    dates.Add(new Dates
                    {
                        inputDates = dr[0].ToString()
                    });

                }
                listBoxOrderDish.ItemsSource = null;
                listBoxOrderDish.ItemsSource = dates;

            }
            catch (Exception x)
            {
                MessageBox.Show(x.GetBaseException().ToString(), "Error",
                           MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void listBoxOrderDish_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ListBoxItem listBoxItem = (ListBoxItem)sender;
            String selectedItem = (String)listBoxItem.DataContext;

            // Добавьте выбранный элемент в DataGrid
            datagridOrder.Items.Add(selectedItem);
        }
    }
}
