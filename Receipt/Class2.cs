using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Microsoft.Data.Sqlite;

namespace Receipt
{

    public class Parameter
    {
       public string ColumnName { get; set; }
       public object Value { get; set; }
       public SqliteType Type { get; set; }
    }

    public class SQLite
    {
        SqliteConnection connection = null;
        public SQLite(String varConectionString)
        {
            connection = new SqliteConnection(varConectionString);
            connection.Open();
        }

        /// <summary>
        /// Выполняет переданный запрос в виде строки.
        /// </summary>
        /// <param name="query">Строка запроса</param>
        /// <param name="parameters">Коллекция параметров</param>
        /// <returns>Таблица с данными</returns>
        public DataTable Execute(string query, Parameter[] parameters = null)
        {
            DataTable dt = new DataTable();

            var command = connection.CreateCommand();
            command.CommandText = query;
            if (parameters != null)
                foreach (var iparam in parameters)
                    command.Parameters.AddWithValue(iparam.ColumnName, iparam.Value);

            using (var reader = command.ExecuteReader())
                return dt;
        }

        public void ExecuteNonQuery(string query, Parameter[] parameters = null)
        {
            using (var transaction = connection.BeginTransaction())
            {

                DataTable dt = new DataTable();
                var command = connection.CreateCommand();
                command.CommandText = query;
                command.Transaction = transaction;
                if (parameters != null)
                    foreach (var iparam in parameters)
                        command.Parameters.AddWithValue(iparam.ColumnName, iparam.Value);
                command.ExecuteNonQuery();
                transaction.Commit();
            }
        }
        public object ExecuteScalar(string query, Parameter[] parameters = null)
        {
            DataTable dt = new DataTable();
            var command = connection.CreateCommand();
            command.CommandText = query;
            if (parameters != null)
                foreach (var iparam in parameters)
                    command.Parameters.AddWithValue(iparam.ColumnName, iparam.Value);
            return command.ExecuteScalar();

        }

    }

        class Class2
    {
        
    
        public void Run()
        {



            using (var connection = new SqliteConnection("" +
    new SqliteConnectionStringBuilder
    {
        DataSource = "hello.db"
    }))
            {
                connection.Open();                
                using (var transaction = connection.BeginTransaction())
                {
                    var insertCommand = connection.CreateCommand();
                    insertCommand.Transaction = transaction;
                    insertCommand.CommandText = "INSERT INTO message ( text ) VALUES ( $text )";
                    insertCommand.Parameters.AddWithValue ("$text", "Hello, World!");
                    insertCommand.ExecuteNonQuery();

                    var selectCommand = connection.CreateCommand();
                    selectCommand.Transaction = transaction;
                    selectCommand.CommandText = "SELECT text FROM message";                    
                    using (var reader = selectCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var message = reader.GetString(0);
                            Console.WriteLine(message);
                        }
                    }

                    transaction.Commit();
                }
            }
        }
    }
}
