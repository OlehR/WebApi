using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Data.SqlClient;
using System.Data.Common;
using System.Data;

namespace SharedLib
{
    public class MsDbConnection
    {
        public static IConfigurationRoot AppConfiguration { get; set; }
        /// <summary>
        /// Имя провайдера
        /// </summary>
        public string ProviderName { get; set; }
        /// <summary>
        /// Имя пользователя
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// Пароль пользователя
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// Адрес БД
        /// </summary>
        public string Host { get; set; }
        /// <summary>
        /// Имя БазыДаных
        /// </summary>
        public string DataBase { get; set; }
        /// <summary>
        /// Порт БазыДаных
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// наименование схемы БД
        /// </summary>
        public string Schema { get; set; }

        /// <summary>
        /// Подключения к БД
        /// </summary>
        List<SqlConnection> _SqlConnectionList = new List<SqlConnection>();

        List<DbCommand> _DbCommandList = new List<DbCommand>();
        List<DbDataReader> _DbDataReaderList = new List<DbDataReader>();

        /// <summary>
        /// инициализация класа
        /// </summary>
        public MsDbConnection()
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json");

            AppConfiguration = builder.Build();
            ProviderName = AppConfiguration["MsDbConnection:ProviderName"];
            DataBase = AppConfiguration["MsDbConnection:Db"];
            Port = Convert.ToInt32(AppConfiguration["MsDbConnection:Port"]);
            Host = AppConfiguration["MsDbConnection:Host"];
            UserName = AppConfiguration["MsDbConnection:UserName"];
            Password = AppConfiguration["MsDbConnection:Password"];
            Schema = AppConfiguration["MsDbConnection:Schema"];
        }

        /// <summary>
        /// получает строку подключения
        /// </summary>
        /// <returns></returns>
        public string GetConnectionString()
        {
            return String.Format("Server={0};Database={1};Trusted_Connection=True;"/*User Id={2};Password={3}"*/, Host,  DataBase/*, UserName, Password*/);
        }

        SqlConnection _SqlConnection = null;
        public SqlConnection CreateConnection()
        {
            if (_SqlConnection != null)
                return _SqlConnection;

            _SqlConnection = new SqlConnection(GetConnectionString());

            try
            {
                _SqlConnection.Open();
                _SqlConnectionList.Add(_SqlConnection);
                return _SqlConnection;
            }
            catch(Exception Ex)
            {
                _SqlConnection.Dispose();
                return null;
            }
        }

        /// <summary>
        /// Создает комманду к или изменяет запрос БД
        /// </summary>
        /// <param name="Command"> команда</param>
        /// <param name="SQLString"> Sql запрос </param>
        /// <returns>получилось ли создать</returns>
        private bool RepairCommand(DbCommand Command, string SQLString)
        {
            if (Command == null)
            {
                Command = new SqlCommand(SQLString);
                return true;
            }
            else
                Command.CommandText = SQLString;
            return true;
        }

        /// <summary>
        /// Создает команду
        /// </summary>
        /// <param name="SQLString"> Sql запрос </param>
        /// <returns>команда БД</returns>
        public DbCommand CreateCommand(string SQLString)
        {
            try
            {
                SqlCommand Command = new SqlCommand(SQLString, CreateConnection());
                _DbCommandList.Add(Command);
                return Command;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Добавляет параметр
        /// </summary>
        /// <param name="Command">ссылка на команду</param>
        /// <param name="Param_name"> наименование параметра </param>
        /// <param name="Param_value"> значение параметра </param>
        /// <param name="Type"> тип параметра </param>
        public void AddCommandParam(ref DbCommand Command, string Param_name, object Param_value, DbType Type)
        {
            DbParameter param = Command.CreateParameter();
            param.ParameterName = Param_name;
            param.Value = Param_value;
            param.DbType = Type;
            Command.Parameters.Add(param);
        }

        /// <summary>
        /// Выполняет команду
        /// </summary>
        /// <param name="Command"> команда БД</param>
        /// <param name="SQLString"> Sql запрос </param>
        /// <returns></returns>
        public bool CommandExecute(string SQLString = "")
        {
            try
            {
                DbCommand Command = CreateCommand(SQLString);
                Command.ExecuteNonQuery();
                Command.Dispose();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Выполняет команду
        /// </summary>
        /// <param name="Command"> команда БД</param>
        /// <param name="SQLString"> Sql запрос </param>
        /// <returns></returns>
        public DataTable Select(string SQLString = "")
        {
            try
            {
                DbCommand Command = CreateCommand(SQLString);
                var res=Command.ExecuteReader();
                Command.Dispose();

                while (res.Read())
                {
                    var myString = res.GetString(0); //The 0 stands for "the 0'th column", so the first column of the result.
                                                     // Do somthing with this rows string, for example to put them in to a list
                    Console.WriteLine(myString);
                }

                //return res;
                DataTable dt = new DataTable();
                dt.Load(res);
                return dt;

            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public bool CommandExecute(DbCommand Command)
        {
            if (Command != null)
            {
                try
                {
                    Command.ExecuteNonQuery();
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            return false;
        }

        /// <summary>
        /// Возвращает датаридер
        /// </summary>
        /// <param name="Command"> комманда БД</param>
        /// <returns> Датаридер </returns>
        public DbDataReader CreateReader(DbCommand Command)
        {
            var reader = Command.ExecuteReader();
            _DbDataReaderList.Add(reader);
            return reader;
        }

        /// <summary>
        /// Возвращает датаридер
        /// </summary>
        /// <param name="SQLString">Sql запрос</param>
        /// <returns>ДатаРидер</returns>
        public DbDataReader CreateReader(string SQLString)
        {
            var Command = CreateCommand(SQLString);
            return CreateReader(Command);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Command"></param>
        /// <param name="Param_name"></param>
        /// <param name="Type"></param>
        /// <param name="SourceColumn"></param>
        /// <param name="dataRowVersion"></param>
        public void AddCommandParam(ref DbCommand Command, string Param_name, DbType Type, string SourceColumn, DataRowVersion dataRowVersion)
        {
            DbParameter param = Command.CreateParameter();
            param.ParameterName = Param_name;
            param.DbType = Type;
            param.SourceColumn = SourceColumn;
            param.SourceVersion = dataRowVersion;
            Command.Parameters.Add(param);
        }

        /// <summary>
        /// Освобождаем память
        /// </summary>
        public void Dispose()
        {
            //освобождаем память от Ридеров
            _DbDataReaderList.ForEach(delegate (DbDataReader _DbDataReader)
            {
                if (_DbDataReader != null)
                {
                    _DbDataReader.Dispose();
                    _DbDataReader = null;
                }
            });

            //освобождаем память от Командов
            _DbCommandList.ForEach(delegate (DbCommand _DbCommand)
            {
                if (_DbCommand != null)
                {
                    _DbCommand.Dispose();
                    _DbCommand = null;
                }
            });



            //освобождаем память от Конекшенов
            _SqlConnectionList.ForEach(delegate (SqlConnection _SqlConnection)
            {
                if (_SqlConnection != null)
                {
                    _SqlConnection.Close();
                    _SqlConnection.Dispose();
                    _SqlConnection = null;
                }
            });

        }
    }
}
