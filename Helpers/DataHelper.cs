using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading;

namespace DTNHistoryBridge.Helpers
{
    public static class DataHelper
    {

        private const string ConnectionFormat = "Data Source={0};Initial Catalog={1};User Id={2};Password={3};connection timeout={4}";
        private const string DataSource = "WORK\\HADASSQL";
        private const string Password = "m4ffCr113P3vqOGGtuTW";
        private const string UserId = "DevUser";
        private const string DefaultDB = "Dev";
        private const int connectionTimeout = 3600;
        //private const string ConnectionFormat = "Data Source={0};Initial Catalog={1};Integrated Security=True";

        private static SqlConnection _connection;
        private static bool _isConnected;

        public static bool IsConnected
        {
            get { return _isConnected; }
        }

        public static void Connect(string initialCatalog)
        {
            if (_isConnected) return;
            if (_connection != null && _connection.State == ConnectionState.Connecting)
            {
                return;
            }

            lock (new object())
            {
                _connection = new SqlConnection { ConnectionString = string.Format(ConnectionFormat, DataSource, initialCatalog, UserId, Password, connectionTimeout) };

                if (_connection.State != ConnectionState.Open)
                {
                    try
                    {
                        _connection.Open();
                        _isConnected = true;
                    }
                    catch (Exception e)
                    {
                        if (_connection.State != ConnectionState.Open)
                            _isConnected = false;
                    }
                }
            }
        }

        public static void Disconnect()
        {
            if (_isConnected)
            {
                try
                {
                    _connection.Close();
                    _isConnected = false;
                }
                catch (Exception e)
                {
                    if (_connection.State != ConnectionState.Open)
                        _isConnected = false;
                }
                finally
                {
                    _connection = null;
                }
            }
        }

        private static SqlConnection GetConnection()
        {
            if (_connection == null || _connection.State != ConnectionState.Open)
                Connect(DefaultDB);

            return _connection;
        }

        public static void TruncateMarketSnapshot()
        {
            string sql = StoredProcedures.TruncateMarketSnapshot();
            ExecuteSQL(sql);
        }

        public static void TruncateTable(string tableName)
        {
            string sql = string.Format(StoredProcedures.SqlTruncateTable, tableName);
            ExecuteSQL(sql);
        }

        public static void DeleteIntradayBarsForCurrentDay(DateTime date)
        {
            string sql = string.Format(StoredProcedures.DeleteIntradayBarsForCurrentDay, date.ToShortDateString());
            ExecuteSQL(sql);
        }

        public static void DeleteIntradayBarsForBacktesting(string tableName, DateTime startDate, DateTime endDate)
        {
            string sql = string.Format(StoredProcedures.DeleteIntradayBarsForBacktesting, tableName,
                                                                                          startDate.ToShortDateString(), 
                                                                                          endDate.AddDays(1).ToShortDateString());
            ExecuteSQL(sql);
        }

        public static void FixHistoryIntraday(DateTime date)
        {
            const string sql = "FixHistoryIntraday";
            List<SqlParameter> parameters = new List<SqlParameter> { new SqlParameter("@BeginDate", date) };
            ExecuteSQL(sql, CommandType.StoredProcedure, parameters);
        }

        public static void InsertDataTable(DataTable table, string storedProcedureName, string tableValueParameterName)
        {
            // Configure the SqlCommand and SqlParameter.
            SqlCommand insertCommand = new SqlCommand(storedProcedureName, GetConnection()) { CommandType = CommandType.StoredProcedure };

            SqlParameter tvpParam = insertCommand.Parameters.AddWithValue(tableValueParameterName, table);
            tvpParam.SqlDbType = SqlDbType.Structured;

            // Execute the command.
            try
            {
                insertCommand.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static void InsertToMarketSnapshot(string instrument, DateTime date, double open, double high, double low, double last, double volume)
        {
            string sql = StoredProcedures.InsertToMarketSnapshot();
            sql = string.Format(sql, instrument, date, open, high, low, last, volume);
            ExecuteSQL(sql);
        }

        public static DataTable GetSymbolsForBackTesting()
        {
            return ExecuteSqlForData(StoredProcedures.GetAllSymbolsForBackTesting) ?? new DataTable();
        }

        public static DataTable GetAllSymbols(bool EOD)
        {
            DataTable dataTable = new DataTable("dataTable");
            DataColumn tickerColumn = new DataColumn("ticker", typeof(string));

            dataTable.Columns.Add(tickerColumn);

            //string sql = EOD ? StoredProcedures.GetAllSymbols : StoredProcedures.GetAllSymbols;
            string sql = EOD ? StoredProcedures.GetAllSymbolsForEOD : StoredProcedures.GetAllSymbols;

            SqlCommand command;
            SqlDataReader reader = null;

            try
            {
                command = new SqlCommand(sql, GetConnection()) { CommandType = CommandType.Text };
            }
            catch (Exception e)
            {
                Console.WriteLine(@"Possilbe Connection Error :" + e.Message);
                Disconnect();
                Connect(DefaultDB);
                command = new SqlCommand(sql, GetConnection()) { CommandType = CommandType.Text };
            }

            try
            {
                reader = command.ExecuteReader();
                if (reader != null)
                    while (reader.Read())
                    {
                        DataRow row = dataTable.NewRow();
                        row[tickerColumn] = reader.GetString(0);
                        dataTable.Rows.Add(row);
                    }
            }
            catch (Exception e)
            {
                Console.WriteLine(@"Error :" + e.Message);
                Thread.Sleep(1000);
            }

            if (reader != null)
                reader.Close();

            command.Dispose();
            return dataTable;
        }

        public static bool ExecuteSQL(string sql)
        {
            return ExecuteSQL(sql, CommandType.Text, null);
        }

        public static bool ExecuteSQL(string sql, CommandType commandType, List<SqlParameter> parameters)
        {
            SqlCommand command;

            if(!IsConnected)
                Connect(DefaultDB);

            try
            {
                command = new SqlCommand(sql, GetConnection()) { CommandType = commandType };

                if (parameters != null && parameters.Count > 0)
                {
                    foreach (var sqlParameter in parameters)
                    {
                        command.Parameters.Add(sqlParameter);
                    }
                }

                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        public static bool UpdateSnapShot(string sql)
        {
            string sp = "UpdateSnapshot";
            SqlCommand command = new SqlCommand(sp, GetConnection()) { CommandType = CommandType.StoredProcedure };
            command.Parameters.Add(new SqlParameter("@sql",SqlDbType.NVarChar,-1));
            command.Parameters[0].Value = sql;

            try
            {
                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        public static DataTable ExecuteSqlForData(string sql)
        {
            if (!IsConnected)
                Connect(DefaultDB);

            DataTable result = null;
            SqlCommand command = null;
            SqlDataReader reader = null;
            DataRow row;
            try
            {
                command = new SqlCommand(sql, GetConnection()) { CommandType = CommandType.Text };
                reader = command.ExecuteReader();
                if (reader != null)
                    while (reader.Read())
                    {
                        if (result == null)
                        {
                            result = CreateResultTable(reader);
                        }
                        row = result.NewRow();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            // if it is null and is not of type string, then, initialize to zero
                            if (    reader.IsDBNull(i) 
                                && !(reader.GetFieldType(i) == typeof(string))
                                && !(reader.GetFieldType(i) == typeof(DateTime)))
                            {
                                row[i] = 0;
                            }
                            else
                            {
                                row[i] = reader.GetValue(i);
                            }
                        }
                        result.Rows.Add(row);
                    }

                return result;
            }
            catch (SqlException e)
            {
                return result;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                if (command != null)
                    command.Dispose();
            }
        }

        private static DataTable CreateResultTable(IDataRecord reader)
        {
            DataTable dataTable = new DataTable();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                DataColumn dataColumn = new DataColumn(reader.GetName(i), reader.GetFieldType(i));
                dataTable.Columns.Add(dataColumn);
            }

            return dataTable;
        }

    }
}
