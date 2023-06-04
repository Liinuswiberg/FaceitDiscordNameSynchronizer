using System;

namespace FaceitDiscordNameSynchronizer
{
    class Program
    {
        
        static void Main(string[] args)
        {
            Console.WriteLine("Program starting...");

            try
            {
                new Controller();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            

        }
    }
}