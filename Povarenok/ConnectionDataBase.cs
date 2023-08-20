using System;
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
        MySqlConnection mySql = new MySqlConnection(@"Server=localhost;Port=3306;UserId=admin;Password=78451296;Database=db_amoll");

        public void openConnection()
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

        public DataTable StoredProcedureNotParametr(string nameProcedure)
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
            catch (Exception x)
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
        public DataTable StoredProcedure(string nameProcedure, string nameParametr,string nameValue)
        {
            MySqlCommand mySqlComm = new MySqlCommand();

            MySqlDataAdapter myadapter = new MySqlDataAdapter();
            DataTable table = new DataTable();
            try
            {
                mySqlComm = new MySqlCommand(nameProcedure, mySql);
                mySqlComm.CommandType = CommandType.StoredProcedure;
                mySqlComm.Parameters.AddWithValue(nameParametr,nameValue);
                mySqlComm.Connection.Open();
                mySqlComm.ExecuteNonQuery();
                myadapter.SelectCommand = mySqlComm;
                myadapter.Fill(table);
            }
            catch (Exception x)
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
        public DataTable StoredProcedureWithArray(string nameProcedure, Dictionary<string, object> parameters)
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
            catch (Exception x)
            {
                MessageBox.Show(x.GetBaseException().ToString(), "Error",
                           MessageBoxButton.OK, MessageBoxImage.Error);
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

