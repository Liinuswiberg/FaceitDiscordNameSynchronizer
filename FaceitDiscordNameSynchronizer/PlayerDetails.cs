using System.Collections.Generic;
using Newtonsoft.Json;

namespace FaceitDiscordNameSynchronizer
{
    public class PlayerDetails
    {
       
        
        
    public class Csgo    {
        [JsonProperty("game_profile_id")]
        public string game_profile_id; 
        [JsonProperty("region")]
        public string region;
        [JsonProperty("skill_level_label")]
        public string skill_level_label; 
        [JsonProperty("game_player_id")]
        public string game_player_id; 
        [JsonProperty("skill_level")]
        public int skill_level; 
        [JsonProperty("faceit_elo")]
        public int faceit_elo; 
        [JsonProperty("game_player_name")]
        public string game_player_name; 

    }
    
    public class Games    {
        
        [JsonProperty("csgo")]
        public Csgo csgo;

    }

    public class Root    {
        [JsonProperty("player_id")]
        public string player_id; 
        [JsonProperty("nickname")]
        public string nickname; 
        [JsonProperty("avatar")]
        public string avatar; 
        [JsonProperty("country")]
        public string country; 
        [JsonProperty("cover_image")]
        public string cover_image; 
        [JsonProperty("cover_featured_image")]
        public string cover_featured_image;
        [JsonProperty("games")]
        public Games games;
        [JsonProperty("friends_ids")]
        public List<string> friends_ids; 
        [JsonProperty("bans")]
        public List<object> bans; 
        [JsonProperty("new_steam_id")]
        public string new_steam_id; 
        [JsonProperty("steam_id_64")]
        public string steam_id_64; 
        [JsonProperty("steam_nickname")]
        public string steam_nickname; 
        [JsonProperty("membership_type")]
        public string membership_type; 
        [JsonProperty("memberships")]
        public List<string> memberships; 
        [JsonProperty("faceit_url")]
        public string faceit_url; 

    }

    }

}