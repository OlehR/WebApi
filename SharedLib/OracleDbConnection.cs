using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Data.Common;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace SharedLib
{
    public class OracleDbConnection:IDisposable
    {
        public bool isConnection()
        {
            
                return CreateConnection()!=null;
            
        }

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
        public string DataSource { get; set; }
        /// <summary>
        /// Имя БазыДаных
        /// </summary>
        

        /// <summary>
        /// Подключения к БД
        /// </summary>
        List<OracleConnection> _OracleConnectionList = new List<OracleConnection>();

        List<DbCommand> _DbCommandList = new List<DbCommand>();
        List<DbDataReader> _DbDataReaderList = new List<DbDataReader>();

        /// <summary>
        /// инициализация класа
        /// </summary>
        public OracleDbConnection()
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json");

            AppConfiguration = builder.Build();
            ProviderName = AppConfiguration["OracleDbConnection:ProviderName"];
            
            
            
            UserName = AppConfiguration["OracleDbConnection:UserName"];
            Password = AppConfiguration["OracleDbConnection:Password"];
            DataSource = AppConfiguration["OracleDbConnection:DataSource"];
        }

        /// <summary>
        /// получает строку подключения
        /// </summary>
        /// <returns></returns>
        public string GetConnectionString()
        {
            return String.Format("Data Source={0};User Id={1};Password={2};", DataSource, UserName, Password);
        }

        public OracleConnection CreateConnection()
        {
            OracleConnection _OracleConnection = new OracleConnection(GetConnectionString());
            try
            {
                _OracleConnection.Open();
                _OracleConnectionList.Add(_OracleConnection);
                return _OracleConnection;
            }
            catch(Exception Ex )
            {
                _OracleConnection.Dispose();
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
                Command = new OracleCommand(SQLString);
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
                OracleCommand Command = new OracleCommand(SQLString, CreateConnection());
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
            _OracleConnectionList.ForEach(delegate (OracleConnection _OracleConnection)
            {
                if (_OracleConnection != null)
                {
                    _OracleConnection.Close();
                    _OracleConnection.Dispose();
                    _OracleConnection = null;
                }
            });

        }
    }
}
