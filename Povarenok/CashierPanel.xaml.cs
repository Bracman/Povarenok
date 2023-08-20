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
using System.Collections.ObjectModel;
using System.Windows.Threading;
using System.Text.RegularExpressions;

namespace Povarenok
{
    /// <summary>
    /// Логика взаимодействия для CashierPanel.xaml
    /// </summary>
    public partial class CashierPanel : Window
    {
       public class DishDataSet
        {
            public string nameDishDataSet { get; set; }
            public int countDishDataSet { get; set; }
            public int priceDishDataSet { get; set; }
            public int codeDishDataSet { get; set; }
        }


        public class SomeClass {
            public static int s;

            public int d;

        }



        public ObservableCollection<DishDataSet> DishDataSets { get; } = new ObservableCollection<DishDataSet>();

        public CashierPanel()
        {
            InitializeComponent();

            labelTypeDish.IsEnabled = false;
            comboxTypeDish.IsEnabled = false;
            loadTypeofDish();
            datagridOrder.ItemsSource = DishDataSets;
            loadKeyOrder();
            NowTime();
           
        }
          
        public void NowTime()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Start();            
        }
        void timer_Tick(object sender, EventArgs e)
        {
            timeTextBlock.Text = DateTime.Now.ToShortTimeString();
        }

        private IList<Dates> dates = new List<Dates>();

        private IList<MenuDishes> menuDishes = new List<MenuDishes>();

        
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
        public class TotalAmountItem
        {
            public string Name { get; set; }
            public decimal TotalAmount { get; set; }
        }
        private decimal CalculateTotalAmount()
        {
            decimal totalAmount = DishDataSets.Sum(item => item.priceDishDataSet*item.countDishDataSet);
            return totalAmount;
        }
        public class Dates
        {
            public string inputDates { get; set; }
            public bool IsSelected { get; set; }
        }

        public class MenuDishes
        {
            public string nameDish { get; set; }
            public string priceDish { get; set; }

            public string codeDish { get; set; }
        }
        private ObservableCollection<TotalAmountItem> TotalAmountItems = new ObservableCollection<TotalAmountItem>();
        private void AddTotalAmountRowToDataGrid()
        {
            // Создайте объект TotalAmountItem с общей суммой
            decimal totalAmount = CalculateTotalAmount(); // Здесь рассчитайте общую сумму всех блюд, добавленных в DataGrid

            calculateAmountTextBlock.Text = totalAmount.ToString() + " " + "рублей";           
            
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
         public void loadKeyOrder()
        {
            DataTable dataTable = new DataTable();
            dataTable.Rows.Clear();
            ConnectionDataBase dataBase = new ConnectionDataBase();
            dataTable = dataBase.StoredProcedureNotParametr("keyOrder");
            string key="";
            foreach (DataRow dr in dataTable.Rows)
            {
                key = dr[0].ToString();

            }
            textblockKeyOrder.Text = key.ToString();
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
                menuDishes.Clear();
                DataTable dataTable = new DataTable();
                dataTable.Rows.Clear();
                ConnectionDataBase dataBase = new ConnectionDataBase();
                dataTable = dataBase.StoredProcedureNotParametr("loadDishes");

                comboboxOrdered.ItemsSource = dataTable.DefaultView;
                comboboxOrdered.DisplayMemberPath = "nameDish";
                comboboxOrdered.SelectedValuePath = "codeDish";
            }
            catch (Exception x)
            {
                MessageBox.Show(x.GetBaseException().ToString(), "Error",
                           MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        StringBuilder stringBuilder = new StringBuilder();
        List<string> codeDishes = new List<string>();
        private void listBoxMenu_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = (Button)sender;

            TextBlock nameTextBlock = (TextBlock)clickedButton.FindName("textblockNameDish");
            TextBlock priceTextBlock = (TextBlock)clickedButton.FindName("textblockPriceDish");
            TextBlock codeTextBlock = (TextBlock)clickedButton.FindName("textblockCodeDish");
            
            string nameValue = nameTextBlock.Text;
            string priceValue = priceTextBlock.Text;
            string codeValue = codeTextBlock.Text;

            stringBuilder.Append(string.Join(";", codeDishes));

            var existingItem = DishDataSets.FirstOrDefault(item => item.nameDishDataSet == nameValue.ToString() && item.priceDishDataSet==Convert.ToInt32(priceValue) && item.codeDishDataSet==Convert.ToInt32(codeValue));
           
            if (datagridOrder.Items.Count<=6)
            {
                if (existingItem != null)
                {
                    if (existingItem.countDishDataSet < 2)
                    {
                        // Увеличиваем значение второго столбца
                        existingItem.countDishDataSet++;
                        CollectionViewSource.GetDefaultView(DishDataSets).Refresh();
                        AddTotalAmountRowToDataGrid();
                       
                    }
                    else
                    {
                        // Показываем сообщение с предупреждением
                        MessageBox.Show("Нельзя добавить больше двух порций.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else
                {
                    // Создание нового объекта MyDataItem и добавление его в коллекцию MyData
                    DishDataSet dataSet = new DishDataSet { nameDishDataSet = nameValue, countDishDataSet = 1, priceDishDataSet = Convert.ToInt32(priceValue),codeDishDataSet = Convert.ToInt32(codeValue)};
                    DishDataSets.Add(dataSet);
                }
                CollectionViewSource.GetDefaultView(DishDataSets).Refresh();
                AddTotalAmountRowToDataGrid();
            }
            else
            {
                MessageBox.Show("Нельзя добавить больше 6 строк.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            }               
            
        }
       
        private void listBoxOrderDish_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedDate = listBoxOrderDish.SelectedItem as Dates;

            if (selectedDate != null)
            {
                datagridOrder.Items.Add(selectedDate);
               
            }

            ((ListBox)sender).SelectedItem = null;           
        }

        private void comboxTypeOfDishOrderClient_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                menuDishes.Clear();
                DataRowView oDataRowView = comboxTypeOfDishOrderClient.SelectedItem as DataRowView;
                if (oDataRowView != null)

                {
                    DataTable dataTable = new DataTable();
                    dataTable.Rows.Clear();
                    ConnectionDataBase dataBase = new ConnectionDataBase();
                    dataTable = dataBase.StoredProcedure("selectDish", "namet", oDataRowView.Row["nameTypeDish"].ToString());

                    foreach (DataRow dr in dataTable.Rows)
                    {
                        menuDishes.Add(new MenuDishes
                        {
                            codeDish = dr[0].ToString(),
                            nameDish = dr[1].ToString(),
                            priceDish = dr[2].ToString()
                        });

                    }
                     listBoxOrderDish.ItemsSource = null;
                     listBoxOrderDish.ItemsSource = menuDishes;
                }

            }
            catch (Exception x)
            {
                MessageBox.Show(x.GetBaseException().ToString(), "Error",
                           MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void buttonCloseOrder_Click(object sender, RoutedEventArgs e)
        {

            List<int> rowDataList = new List<int>();

            // Перебираем строки в DataGrid
            foreach (var item  in datagridOrder.Items)
            {
                DishDataSet dataSet = (DishDataSet)item;

                int codeValues = dataSet.codeDishDataSet;
                rowDataList.Add(codeValues);
            }

            string res = string.Join(";", rowDataList);


            if (rowDataList.Count > 0)
            {
                res += ";";
            }        
          
            decimal totalAmount = CalculateTotalAmount();
            StringBuilder stringBuilder = new StringBuilder();
            Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    {"orderedDishes", res},
                    {"numOrder",textblockKeyOrder.Text },
                    {"totalAmount",totalAmount.ToString()},
                    {"dateOrder",dateTextBlock.Text },
                    {"timeOrder",timeTextBlock.Text }
                    
                };
            DataTable dataTable = new DataTable();
            ConnectionDataBase dataBase = new ConnectionDataBase();
            dataTable = dataBase.StoredProcedureWithArray("insertOrders", parameters);
        }
        private void datagridOrder_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete || e.Key == Key.Back)
            {
                calculateAmountTextBlock.Text = "0" + " " + "рублей";
            }
        }

        private void checkBoxDate_Checked(object sender, RoutedEventArgs e)
        {
            startCalendar.Visibility = Visibility.Visible;
            startCalendar.IsEnabled = true;
            
        }      

        private void checkBoxDate_Unchecked(object sender, RoutedEventArgs e)
        {
           startCalendar.IsEnabled = false;
           endCalendar.IsEnabled = false;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            ActivatedRadioButtonOrdered();
            ActivatedRadioButtonUnordered();
            loadDishes();
            loadIngredients();
            gridAddProduct.Visibility = Visibility.Hidden;
        }
        
        private void checkboxDishes_Checked(object sender, RoutedEventArgs e)
        {
            comboboxOrdered.IsEnabled = true;
           
        }

        private void buttonSearchOrdered_Click(object sender, RoutedEventArgs e)
        {
            if(radioButtonOrdered.IsChecked == true)
            {
                SearchOrderedDishes();
            }
            else
            {
                SearchUnorderedDishes();
            }
           
           
        }
        private void SearchOrderedDishes()
        {
            string code = "0";
            string startDate = "0";
            string endDate = "0";

            if (comboboxOrdered.IsEnabled == true && startCalendar.IsEnabled == false && endCalendar.IsEnabled == false)
            {
                code = comboboxOrdered.SelectedValue.ToString();
                startDate = "0";
                endDate = "0";
            }
            else if (comboboxOrdered.IsEnabled == false && startCalendar.IsEnabled == true && endCalendar.IsEnabled == true)
            {
                code = "0";
                startDate = startCalendar.SelectedDate.Value.Date.ToShortDateString();
                endDate = endCalendar.SelectedDate.Value.Date.ToShortDateString();
            }
            else
            {
                code = comboboxOrdered.SelectedValue.ToString();
                startDate = startCalendar.SelectedDate.Value.Date.ToShortDateString();
                endDate = endCalendar.SelectedDate.Value.Date.ToShortDateString();
            }

            
            ConnectionDataBase dataBase = new ConnectionDataBase();

            Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    {"codeOrdered", code},
                    {"startDateOrdered",startDate},
                    {"endDateOrdered",endDate}
                };
            DataTable dataTable1 = dataBase.StoredProcedureWithArray("loadOrdered", parameters);
            datagridOrdered.ItemsSource = dataTable1.DefaultView;
        }
        private void  SearchUnorderedDishes()
        {
            string cost="min";
            string startDate="0";
            string endDate="0";

            if (radioButtonMin.IsChecked == true)
            {
                cost = "min";
                startDate = startDatePeriod.SelectedDate.Value.Date.ToShortDateString();
                endDate = endDatePeroid.SelectedDate.Value.Date.ToShortDateString();
            }       
            else if(radioButtonAvg.IsChecked == true)
            {
                cost = "avg";
                startDate = startDatePeriod.SelectedDate.Value.Date.ToShortDateString();
                endDate = endDatePeroid.SelectedDate.Value.Date.ToShortDateString();
            }
            else
            {
                cost = "max";
                startDate = startDatePeriod.SelectedDate.Value.Date.ToShortDateString();
                endDate = endDatePeroid.SelectedDate.Value.Date.ToShortDateString();
            }

            try
            {
                    ConnectionDataBase dataBase = new ConnectionDataBase();
                    Dictionary<string, object> parameters = new Dictionary<string, object>
                   {
                      {"cost", cost},
                      {"startDatePeriod",startDate},
                      {"endDatePeriod",endDate}
                   };
                DataTable dataTable1 = dataBase.StoredProcedureWithArray("loadUnorderedDishes", parameters);
                datagridUnorderedDishes.ItemsSource = dataTable1.DefaultView;
            }
            catch(Exception ex)
            {

                    System.Windows.MessageBox.Show("Не выбрана период времени", "Ошибка формирования отчета",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
             }
           
        }
        public void  ActivatedRadioButtonOrdered()
        {
            startCalendar.IsEnabled = false;
            endCalendar.IsEnabled = false;
            comboboxOrdered.IsEnabled = false;
            gridOrdered.Visibility = Visibility.Hidden;
            datagridOrdered.Visibility = Visibility.Hidden;
           
        }
        public void ActivatedRadioButtonUnordered()
        {
            
            gridUnordered.Visibility = Visibility.Hidden;
            radioButtonMin.IsChecked = true;
            datagridUnorderedDishes.Visibility = Visibility.Hidden;
            endDatePeroid.IsEnabled = false;
            radioButtonUnordered.IsChecked = true;
        }
      
        private void loadIngredients()
        {
            DataTable dataTable = new DataTable();
            ConnectionDataBase dataBase = new ConnectionDataBase();
            dataTable = dataBase.StoredProcedureNotParametr("loadProducts");
            datagridIngredients.ItemsSource = dataTable.DefaultView;


            DataTable dataTable1 = new DataTable();
            
            dataTable1 = dataBase.StoredProcedureNotParametr("loadProducts");
            comboxIngredient.ItemsSource = dataTable1.DefaultView;
            comboxIngredient.DisplayMemberPath = "nameProduct";
            comboxIngredient.SelectedValuePath = "";


        }

        private void checkboxDishes_Unchecked(object sender, RoutedEventArgs e)
        {
            comboboxOrdered.IsEnabled = false;
        }
        private void radioButtonOrdered_Checked(object sender, RoutedEventArgs e)
        {
            gridOrdered.Visibility = Visibility.Visible;
            gridUnordered.Visibility = Visibility.Hidden;
            datagridOrdered.Visibility = Visibility.Visible;
            datagridUnorderedDishes.Visibility = Visibility.Hidden;
        }
        private void radioButtonUnordered_Checked(object sender, RoutedEventArgs e)
        {
            gridOrdered.Visibility = Visibility.Hidden;
            gridUnordered.Visibility = Visibility.Visible;
            datagridUnorderedDishes.Visibility = Visibility.Visible;
            datagridOrdered.Visibility = Visibility.Hidden;
        }
        private void startCalendar_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            endCalendar.DisplayDateStart = startCalendar.SelectedDate.Value.Date;
            endCalendar.IsEnabled = true;
        }

        private void startDatePeriod_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            endDatePeroid.DisplayDateStart = startDatePeriod.SelectedDate.Value.Date;
            endDatePeroid.IsEnabled = true;
        }

        private void radioButtonAddProduct_Checked(object sender, RoutedEventArgs e)
        {
            gridAddProduct.Visibility = Visibility.Visible;
            gridChangeProduct.Visibility = Visibility.Hidden;
        }

        private void radioButtonUpdateProduct_Checked(object sender, RoutedEventArgs e)
        {
            gridChangeProduct.Visibility = Visibility.Visible;
            gridAddProduct.Visibility = Visibility.Hidden;
        }
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        private void StringValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            
            Regex regex = new Regex("[^[а-яА-Я]");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void buttonAddProduct_Click(object sender, RoutedEventArgs e)
        {
            ConnectionDataBase dataBase = new ConnectionDataBase();

            Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    {"productName", textboxNameProduct.Text},
                    {"countProduct",textboxCountProduct.Text},                    
                };
            DataTable dataTable1 = dataBase.StoredProcedureWithArray("insertProduct", parameters);
            textboxCountProduct.Text="";
            textboxNameProduct.Text="";
            loadIngredients();
            
        }

        private void buttonChangeProduct_Click(object sender, RoutedEventArgs e)
        {
            ConnectionDataBase dataBase = new ConnectionDataBase();

            DataRowView oDataRowView = comboxIngredient.SelectedItem as DataRowView;

            if(oDataRowView != null)
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    {"nameIngredients",  oDataRowView.Row["nameProduct"].ToString()},
                    {"countIngredients",textboxChange.Text},
                };
                DataTable dataTable1 = dataBase.StoredProcedureWithArray("updateProducts", parameters);
                textboxCountProduct.Text = "";
                textboxChange.Text = "";
                loadIngredients();
            }
                
        }
    }
}
