﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data;
using System.Windows;

namespace Povarenok
{


    class ConnectionDataBase
    {
        MySqlConnection mySql = new MySqlConnection(@"Server=localhost;Port=3306;UserId=root;Password=123456;Database=db_amoll");

        public void openConnection()// Открывает канал для подключения к бд 
        {
            if (mySql.State == System.Data.ConnectionState.Closed)
            {
                mySql.Open();
            }
        }

        public void closeConnection()
        {
            if (mySql.State == System.Data.ConnectionState.Open)
            {
                mySql.Close();
            }
        }

        public MySqlConnection GetNpgsqlConnection()
        {
            return mySql;
        }

        public DataTable StoredProcedureNotParametr(string nameProcedure)// получения данных из БД без параметров
        {
            MySqlCommand mySqlComm = new MySqlCommand();

            MySqlDataAdapter myadapter = new MySqlDataAdapter();
            DataTable table = new DataTable();
            try
            {
                mySqlComm = new MySqlCommand(nameProcedure, mySql);
                mySqlComm.CommandType = CommandType.StoredProcedure;
                myadapter.SelectCommand = mySqlComm;
                myadapter.Fill(table);
            }
            catch (MySqlException x)
            {
                MessageBox.Show(x.GetBaseException().ToString(), "Error",
                           MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                mySqlComm.Dispose();
                mySql.Close();
            }
            return table;
        }      
        public DataTable StoredProcedureWithArray(string nameProcedure, Dictionary<string, object> parameters)//Отравка данных в Mysql c n-количество параметром
        {

            MySqlCommand mysqlCommand = new MySqlCommand();

            MySqlDataAdapter mySqlAdapter = new MySqlDataAdapter();
            DataTable table = new DataTable();
            try
            {
                mysqlCommand = new MySqlCommand(nameProcedure, mySql);
                mysqlCommand.CommandType = CommandType.StoredProcedure;
                foreach (var parameter in parameters)
                {
                    mysqlCommand.Parameters.AddWithValue(parameter.Key, parameter.Value);
                }
                mysqlCommand.Connection.Open();
                mysqlCommand.ExecuteNonQuery();
                mySqlAdapter.SelectCommand = mysqlCommand;
                mySqlAdapter.Fill(table);
            }
            catch (MySqlException x)
            {
                if (x.Number == 1062)
                {
                    MessageBox.Show("Заказ сформирован успешно");
                }
                else
                {
                    MessageBox.Show(x.GetBaseException().ToString(), "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }


             
            }
            finally
            {
                mysqlCommand.Dispose();
                mySql.Close();
            }           
            return table;
        }

    }
}

