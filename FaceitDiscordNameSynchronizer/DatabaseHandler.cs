using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;

namespace FaceitDiscordNameSynchronizer
{
    public class DatabaseHandler
    {
        
        private static MySqlConnection _con;
        private readonly string _tableName;

        public DatabaseHandler(string cs, string tn)
        {
            _tableName = tn;
            CreateConnection(cs);
        }
        
        /**
         * Creates connection
         */
        public static void CreateConnection(string cs)
        {
            _con = new MySqlConnection(cs);
            Console.WriteLine("DatabaseConnection created");
        }
        
        /**
         * Query database
         */
        public Dictionary<string, string> SelectUsers()
        {
            if (_tableName == null) return null;
       
            if (_con.State != ConnectionState.Open)
            {
                _con.Open();
            }

            var cmd = new MySqlCommand("SELECT Faceitid, Discordid FROM "+_tableName) {Connection = _con};
            
            var reader = cmd.ExecuteReader();

            var dataDict = new Dictionary<string, string>();

            while (reader.Read())
            {
                dataDict.Add(reader.GetString("Faceitid"), reader.GetString("Discordid"));
            }
            
            _con.Close();

            return dataDict;
            
        }
        
    }
}