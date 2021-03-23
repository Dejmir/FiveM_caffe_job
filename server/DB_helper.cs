using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.IO;

//using Microsoft.Win32.TaskScheduler;
using MySql.Data.MySqlClient;
using System.Security.Cryptography;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json.Linq;

public class DB_helper
{

    public static string connect = $"server=localhost;userid=root;password=;database=essentialmode;Convert Zero Datetime=True";

    public bool CheckDatabaseConnection()
    {
        bool con = false;
        MySqlConnection sqlConnection = new MySqlConnection(connect);
        try
        {
            sqlConnection.Open();
            if (sqlConnection.State == System.Data.ConnectionState.Open) con = true;
        }
        catch
        {
            return false;
        }

        sqlConnection.Close();
        return con;
    }
    public void ExecuteNonQuery(string query)
    {
        var Connection = new MySqlConnection(connect);
        try { Connection.Open(); } catch { }
        var cmd = new MySqlCommand();
        cmd.Connection = Connection;
        cmd.CommandText = query;
        cmd.ExecuteNonQuery();
        Connection.Close();
    }

    public MySqlDataReader ExecuteQuery(string query, MySqlConnection Connection)
    {
        //var Connection = new MySqlConnection(connect);
        try { Connection.Open(); } catch { }
        var cmd = new MySqlCommand();
        cmd.Connection = Connection;
        cmd.CommandText = query;
        MySqlDataReader result = cmd.ExecuteReader();

        return result;
    }

}
