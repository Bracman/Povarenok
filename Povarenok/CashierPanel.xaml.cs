﻿using System;
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
using System.IO.Packaging;

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
        public class TotalAmountItem
        {
            public string Name { get; set; }
            public decimal TotalAmount { get; set; }
        }
        private decimal CalculateTotalAmount()
        {
            decimal totalAmount = DishDataSets.Sum(item => item.priceDishDataSet * item.countDishDataSet);
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
            timeTextBlock.Text = DateTime.Now.ToLongTimeString();
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
       
        private ObservableCollection<TotalAmountItem> TotalAmountItems = new ObservableCollection<TotalAmountItem>();
        private void AddTotalAmountRowToDataGrid()
        {           
            decimal totalAmount = CalculateTotalAmount(); 
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
                dataTable = singleStreamFilling("find_dishes", "input", message);
                dataGrid.ItemsSource = dataTable.DefaultView;
            }
            if (ProductRadioButton.IsChecked==true)
            {                
                dataTable = singleStreamFilling("withdrawProducts", "dish", message);
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
                loadDataForCombobox(comboxTypeDish, "selectTypeDish", "", "nameTypeDish", "codeTypeDish");
                loadDataForCombobox(comboxTypeOfDishOrderClient, "selectTypeDish", "", "nameTypeDish", "codeTypeDish");
                loadDataForCombobox(comboboxTypeDishAddMenu, "selectTypeDish", "", "nameTypeDish", "codeTypeDish");  
            }
            catch (Exception x)
            {
                MessageBox.Show(x.GetBaseException().ToString(), "Error",
                           MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        DataTable  singleStreamFilling(string nameProcedure,string nameParametr,string inputDate)
        {
            DataTable dataTable = new DataTable();
            ConnectionDataBase dataBase = new ConnectionDataBase();
            Dictionary<string, object> parameters = new Dictionary<string, object>
             {
                {nameParametr,  inputDate}              

             };
            dataTable = dataBase.StoredProcedureWithArray(nameProcedure, parameters);
            return dataTable;
        }

        private void comboxTypeDish_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                dates.Clear();
                DataRowView oDataRowView = comboxTypeDish.SelectedItem as DataRowView;
                if (oDataRowView != null)     
                   
                {
                    DataTable dateTable;

                    dateTable =singleStreamFilling("selectDish", "namet", oDataRowView.Row["nameTypeDish"].ToString());                  
                    
                    foreach (DataRow dr in dateTable.Rows)
                    {
                        dates.Add(new Dates
                        {
                            inputDates = dr[1].ToString()
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
                loadDataForCombobox(comboboxOrdered, "selectTypeDish", "", "nameTypeDish", "codeTypeDish");               
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
                      
                        existingItem.countDishDataSet++;
                        CollectionViewSource.GetDefaultView(DishDataSets).Refresh();
                        AddTotalAmountRowToDataGrid();                       
                    }
                    else
                    {
                       
                        MessageBox.Show("Нельзя добавить больше двух порций.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else
                {
                    
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
                    DataTable dataTable = singleStreamFilling("selectDish", "namet", oDataRowView.Row["nameTypeDish"].ToString());                  

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

            
            
                List<int> codeDishList = new List<int>();
                List<int> countDishList = new List<int>();

                foreach (var item in datagridOrder.Items)
                {
                    DishDataSet dataSet = (DishDataSet)item;

                    int codeValues = dataSet.codeDishDataSet;
                    int countValues = dataSet.countDishDataSet;
                    countDishList.Add(countValues);
                    codeDishList.Add(codeValues);
                }
                string pointCodeList = string.Join(";", codeDishList);
                string pointCountList = string.Join(";", countDishList);
                if (codeDishList.Count > 0 & countDishList.Count > 0)
                {
                    pointCodeList += ";";
                    pointCountList += ";";
                }

                decimal totalAmount = CalculateTotalAmount();
                StringBuilder stringBuilder = new StringBuilder();
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    {"orderedDishes", pointCodeList},
                    {"numOrder",textblockKeyOrder.Text.ToString()},
                    {"dishCount",pointCountList },
                    {"totalAmount",totalAmount.ToString()},
                    {"dateOrder",dateTextBlock.Text },
                    {"timeOrder",timeTextBlock.Text }

                };
                DataTable dataTable = new DataTable();
                ConnectionDataBase dataBase = new ConnectionDataBase();
                dataTable = dataBase.StoredProcedureWithArray("insertOrders", parameters);

                calculateAmountTextBlock.Text = "0" + " " + "рублей";
                DishDataSets.Clear();
                loadKeyOrder();
            
           
           

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
            loadDataForElement();
            gridAddProduct.Visibility = Visibility.Hidden;
            ActivateMenu();
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
        private void loadDataForCombobox(ComboBox comboBox,string nameProcedure,string nameParam,string nameDisplayValue,string nameSelectedvalue)
        {
            DataTable dataTable = new DataTable();
            dataTable = singleStreamFilling(nameProcedure, nameParam, "");

            comboBox.ItemsSource = dataTable.DefaultView;
            comboBox.DisplayMemberPath = nameDisplayValue;
            comboBox.SelectedValuePath = nameSelectedvalue;
        }
        private void loadDataForElement()
        {
            DataTable dataTable = new DataTable();
            ConnectionDataBase dataBase = new ConnectionDataBase();
            dataTable = dataBase.StoredProcedureNotParametr("loadProducts");
            datagridIngredients.ItemsSource = dataTable.DefaultView;        

            DataTable dataTable1 = new DataTable();
           
            loadDataForCombobox(comboxIngredient, "loadProducts", "", "nameProduct", "codeDish");
           
            loadDataForCombobox(comboxProductAddMenu, "loadProducts", "", "nameProduct", "codeDish");
           
            loadDataForCombobox(comboxDishes, "loadDishForProducts", "", "nameDish", "codeDish");         

            loadDataForCombobox(comboxChangeDish, "loadDishForProducts", "", "nameDish", "codeDish");

            loadDataForCombobox(comboxDishes, "loadDishForProducts", "", "nameDish", "codeDish");

            dataTable1 = dataBase.StoredProcedureNotParametr("loadDishWithProduct");
            gridProduct.ItemsSource = dataTable1.DefaultView;

            dataTable1 = dataBase.StoredProcedureNotParametr("loadDishWithPrice");
            dataGridBoardDishes.ItemsSource = dataTable1.DefaultView;      
                       
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
            loadDataForElement();            
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
                loadDataForElement();
            }                
        }
        private void ActivateMenu()
        {
            gridAddMenuDishes.Visibility = Visibility.Hidden;
            gridchangeMenuDishes.Visibility = Visibility.Hidden;
            gridAddProductsInDish.Visibility = Visibility.Hidden;
            gridAddDish.Visibility = Visibility.Hidden;
            gridDeleteDish.Visibility = Visibility.Hidden;
            gridChangeDish.Visibility = Visibility.Hidden;
        }

        private void addMenuDishes_Checked(object sender, RoutedEventArgs e)
        {
            gridAddMenuDishes.Visibility = Visibility.Visible;
            radioButtonAddProductDishes.Visibility = Visibility.Visible;
            radioButtonAddDishes.Visibility = Visibility.Visible;
            radioButtonDeleteDish.Visibility = Visibility.Hidden;
            radioButtonChangeDish.Visibility = Visibility.Hidden;
            gridchangeMenuDishes.Visibility = Visibility.Hidden;
        }

        private void radioButtonAddProductDishes_Checked(object sender, RoutedEventArgs e)
        {
            gridAddProductsInDish.Visibility = Visibility.Visible;
            gridAddDish.Visibility = Visibility.Hidden;
        }

        private void radioButtonAddDishes_Checked(object sender, RoutedEventArgs e)
        {
            gridAddProductsInDish.Visibility = Visibility.Hidden;
            gridAddDish.Visibility = Visibility = Visibility.Visible;
        }

        private void addDishInMenu_Click(object sender, RoutedEventArgs e)
        {
            try {
                  ConnectionDataBase dataBase = new ConnectionDataBase();
                
                if(textboxDishWeight.Text!="" & textboxPriceAddMenu.Text!="" & textboxNameAddMenu.Text!="")
                {
                    string codeTypeDish = comboboxTypeDishAddMenu.SelectedValue.ToString();

                    Dictionary<string, object> parameters = new Dictionary<string, object>
                    {
                     {"dishName",  textboxNameAddMenu.Text},
                     {"dishPrice",textboxPriceAddMenu.Text},
                     {"weightDish",textboxDishWeight.Text },
                     {"typeDishCode",codeTypeDish}
                    };
                    DataTable dataTable1 = dataBase.StoredProcedureWithArray("insertDishInMenu", parameters);

                    string message = dataTable1.Rows[0][0].ToString();

                    MessageBox.Show(message, "Уведомление",
                                   MessageBoxButton.OK, MessageBoxImage.Information);
                }

                
            }
            catch (Exception x)
            {
                
            } 
        }

        private void buttonAddProductInDish_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if(comboxProductAddMenu.Text!="" && comboxDishes.Text!="")
                {
                    ConnectionDataBase dataBase = new ConnectionDataBase();

                    string codeProduct = comboxProductAddMenu.SelectedValue.ToString();
                    string codeDish = comboxDishes.SelectedValue.ToString();

                    Dictionary<string, object> parameters = new Dictionary<string, object>
                 {
                     {"dishCode",  codeDish},
                     {"productNum",codeProduct},
                     {"productCount",textboxProductAddMenu.Text}
                 };
                    DataTable dataTable1 = dataBase.StoredProcedureWithArray("insertProductInDish", parameters);

                    string messageDatebase = dataTable1.Rows[0][0].ToString();

                    MessageBox.Show(messageDatebase, "Уведомление",
                                   MessageBoxButton.OK, MessageBoxImage.Information);
                    loadDataForElement();
                }
                
            }
            catch (Exception x)
            {

            }

        }

        private void buttonChangeDish_Click(object sender, RoutedEventArgs e)
        {
            if(textboxChangePrice.Text!="" && comboxChangeDish.Text!="")
            {
                ConnectionDataBase dataBase = new ConnectionDataBase();
               
                string codeDish = comboxChangeDish.SelectedValue.ToString();
                string priceDish = textboxChangePrice.Text;

                Dictionary<string, object> parameters = new Dictionary<string, object>
                 {
                     {"dishCode",  codeDish},
                     {"dishPrice",textboxChangePrice.Text},
                     
                 };
                DataTable dataTable1 = dataBase.StoredProcedureWithArray("updateDishInMenu", parameters);

                string messageDatebase = dataTable1.Rows[0][0].ToString();

                MessageBox.Show(messageDatebase, "Уведомление",
                               MessageBoxButton.OK, MessageBoxImage.Information);

                loadDataForElement();
            }
        }

        private void changeMenuDishes_Checked(object sender, RoutedEventArgs e)
        {
            gridchangeMenuDishes.Visibility = Visibility.Visible;
            radioButtonAddDishes.Visibility = Visibility.Hidden;
            radioButtonAddProduct.Visibility = Visibility.Visible;
            radioButtonAddProductDishes.Visibility = Visibility.Hidden;
            radioButtonChangeDish.Visibility = Visibility.Visible;
            radioButtonDeleteDish.Visibility = Visibility.Visible;
            gridAddMenuDishes.Visibility = Visibility.Hidden;
        }

        private void radioButtonChangeDish_Checked(object sender, RoutedEventArgs e)
        {
            gridDeleteDish.Visibility = Visibility.Hidden;
            gridChangeDish.Visibility = Visibility.Hidden;
            gridChangeDish.Visibility = Visibility.Visible;
        }

        private void buttonDeleteDish_Click(object sender, RoutedEventArgs e)
        {
            if(comboboxDishDelete.Text!="")
            {
                ConnectionDataBase dataBase = new ConnectionDataBase();


                string codeDish = comboxChangeDish.SelectedValue.ToString();
                string priceDish = textboxChangePrice.Text;

                Dictionary<string, object> parameters = new Dictionary<string, object>
                 {
                     {"dishCode",  codeDish},
                     {"dishPrice",textboxChangePrice.Text},

                 };
                DataTable dataTable1 = dataBase.StoredProcedureWithArray("updateDishInMenu", parameters);

                string messageDatebase = dataTable1.Rows[0][0].ToString();

                MessageBox.Show(messageDatebase, "Уведомление",
                               MessageBoxButton.OK, MessageBoxImage.Information);

                loadDataForElement();
            }
        }
    }
}
