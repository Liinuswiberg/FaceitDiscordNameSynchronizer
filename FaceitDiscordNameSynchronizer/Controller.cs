using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Microsoft.Extensions.Configuration;

namespace FaceitDiscordNameSynchronizer
{
    public class Controller
    {
        //This will control everything!

        private readonly DatabaseHandler _databaseHandler;
        private DiscordApiHandler _discordApiHandler;
        private FaceitAPIHandler _faceitApiHandler;

        private readonly IConfiguration _config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        public Controller()
        {

            Console.WriteLine("Attempting to initialize handlers...");
            _databaseHandler = new DatabaseHandler(_config["Secrets:DatabaseString"], _config["Secrets:TableName"]);
            Console.WriteLine("Database handler successfully initialized..");
            _discordApiHandler = new DiscordApiHandler(_config["Secrets:DiscordToken"]);
            _discordApiHandler.InitializeDiscordConnection();
            Console.WriteLine("DiscordApi handler successfully initialized..");
            _faceitApiHandler = new FaceitAPIHandler(_config["Secrets:FaceitToken"]);
            Console.WriteLine("FaceitApi handler successfully initialized..");
            Console.WriteLine("All handlers successfully initialized...");

            GetUsers();

        }

        private void GetUsers()
        {
            Console.WriteLine("Getting users...");
            var users = _databaseHandler.SelectUsers();
            Console.WriteLine(users.Count);
            if (users.Count > 0) ParseUsers(users);
        }

        private async void ParseUsers(Dictionary<string, string> users)
        {
            Console.WriteLine("Starting to parse users..");
            foreach (var key in users.Keys)
            {
                
                Console.WriteLine("Parsing " + key);
                
                var playerDetails = _faceitApiHandler.GetElo(key);

                if (playerDetails == null)
                {
                    continue;
                }
                    
                Console.WriteLine(playerDetails.Item1 + " | " + playerDetails.Item2 + " | " + playerDetails.Item3);

                var roleName = playerDetails.Item2 switch
                {
                    1 => "Level 1 (1-800 ELO)",
                    2 => "Level 2 (801-950 ELO)",
                    3 => "Level 3 (951-1100 ELO)",
                    4 => "Level 4 (1101-1250 ELO)",
                    5 => "Level 5 (1251-1400 ELO)",
                    6 => "Level 6 (1401-1550 ELO)",
                    7 => "Level 7 (1551-1700 ELO)",
                    8 => "Level 8 (1701-1850 ELO)",
                    9 => "Level 9 (1851-2000 ELO)",
                    10 => "Level 10 (2001+ ELO)",
                    _ => "Level 1 (1-800 ELO)"
                };

                Console.WriteLine("Attempting to update user...");
                try
                {
                    await _discordApiHandler.UpdateUser(Convert.ToUInt64(users[key]), roleName, playerDetails);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                Console.WriteLine(users[key] + " parse finished.");
                
                Thread.Sleep(1250);
            }
            
            Console.WriteLine("Intermission...");
            Thread.Sleep(10000);
            
            GetUsers();
            
        }
    }
}