using System;
using System.IO;
using System.Net.Http;
using Newtonsoft.Json;

namespace FaceitDiscordNameSynchronizer
{
    public class FaceitAPIHandler
    {

        private readonly string _token;
        public FaceitAPIHandler(string token)
        {
            _token = token;
        }

        public Tuple<string, int, int> GetElo(string faceitId)
        {
            try
            {
                var player = GetFaceitPlayer(faceitId);
                var playerDetails = new Tuple<string, int, int>(player.nickname, player.games.csgo.skill_level, player.games.csgo.faceit_elo);
                return playerDetails;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }
        
        private string FaceitApiQuery(Uri queryUri)
        {
            using var httpClient = new HttpClient();
            using var request = new HttpRequestMessage(new HttpMethod("GET"),
                queryUri);
            //Readies cURL get request using HttpClient
            request.Headers.TryAddWithoutValidation("Accept", "application/json");
            request.Headers.TryAddWithoutValidation("Authorization", "Bearer " + _token);

            //Saves response
            var response = httpClient.SendAsync(request).GetAwaiter().GetResult();
            //Reads response as a stream
            var stream = response.Content.ReadAsStreamAsync().Result;

            //Uses stream
            using var reader = new StreamReader(stream);
            //While stream has items(Should always exist & should be json)
            //If nothing exists, something has gone wrong.
            while (!reader.EndOfStream)
            {
                return reader.ReadLine();
            }

            throw new Exception("No JSON was received from Faceit API");
        }

        public PlayerDetails.Root GetFaceitPlayer(string faceitid)
        {
            
            //Asks Query method to query with specific Uri
            var queryResults = FaceitApiQuery(new Uri("https://open.faceit.com/data/v4/players/"+faceitid));
            
            //Serializes json to FaceitGames object.
            return JsonConvert.DeserializeObject<PlayerDetails.Root>(queryResults);
            
        }
    }
}