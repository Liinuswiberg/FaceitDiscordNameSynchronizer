using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace FaceitDiscordNameSynchronizer
{
    public class DiscordApiHandler
    {
        
        private DiscordSocketClient _client;
        private SocketGuild _discordChannel;
        private bool _available;
        private List<IRole> _allRoles;
        private readonly string _token;

        public DiscordApiHandler(string token)
        {
            _token = token;
        }

        public async Task InitializeDiscordConnection()
        {
            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Info,
                AlwaysDownloadUsers = true,
            });

            await _client.LoginAsync(TokenType.Bot, _token);

            await _client.StartAsync();
            
            var service = new CommandService();

            service.AddModulesAsync(Assembly.GetEntryAssembly(), null).GetAwaiter();
            
            _client.GuildAvailable += GuildAvailable;
        }

        private async Task GuildAvailable(SocketGuild guild)
        {
            
            Console.WriteLine("Connected to Guild...");
            _discordChannel = guild;
            _available = true;
            _allRoles = new List<IRole>
            {
                _discordChannel.Roles.FirstOrDefault(x => x.Name == "Level 1 (1-800 ELO)"),
                _discordChannel.Roles.FirstOrDefault(x => x.Name == "Level 2 (801-950 ELO)"),
                _discordChannel.Roles.FirstOrDefault(x => x.Name == "Level 3 (951-1100 ELO)"),
                _discordChannel.Roles.FirstOrDefault(x => x.Name == "Level 4 (1101-1250 ELO)"),
                _discordChannel.Roles.FirstOrDefault(x => x.Name == "Level 5 (1251-1400 ELO)"),
                _discordChannel.Roles.FirstOrDefault(x => x.Name == "Level 6 (1401-1550 ELO)"),
                _discordChannel.Roles.FirstOrDefault(x => x.Name == "Level 7 (1551-1700 ELO)"),
                _discordChannel.Roles.FirstOrDefault(x => x.Name == "Level 8 (1701-1850 ELO)"),
                _discordChannel.Roles.FirstOrDefault(x => x.Name == "Level 9 (1851-2000 ELO)"),
                _discordChannel.Roles.FirstOrDefault(x => x.Name == "Level 10 (2001+ ELO)")
            };
        }

        public async Task UpdateUser(ulong userid, string roleName, Tuple<string, int, int> playerSkillsItem)
        {

            while (!_available)
            {
                Console.WriteLine("Guild not available yet, sleeping 5 seconds.");
                Thread.Sleep(5000);
            }

            Console.WriteLine("Attempting to give role..");

            var user = _discordChannel.GetUser(userid);
            if (user == null)
            {
                Console.WriteLine("User "+ userid +" has left the channel ;(");
                return;
            }
            var role = _discordChannel.Roles.FirstOrDefault(x => x.Name == roleName);

            UpdateName(user, playerSkillsItem);

            if (!user.Roles.Contains(role))
            {
                await ClearUserRoles(user);

                user.AddRoleAsync(role);
                
                Console.WriteLine("Giving role to "+user.Nickname);
            }
            else
            {
                await RemoveBadRoles(user, role);
            }

        }

        private static void UpdateName(IGuildUser user, Tuple<string, int, int> playerDetails)
        {
            
            Console.WriteLine("Attemping to update name...");
            
            var newName = "(" + playerDetails.Item3 + " ELO) " + playerDetails.Item1;

            //Cant change name of guild owner
            if (user.Id == 258650762169679872)
            {
                return;
            }
            
            if (user.Nickname == null)
            {
                user.ModifyAsync(p => p.Nickname = newName);
            }

            if (!user.Nickname.Contains("("+playerDetails.Item3+" ELO)"))
            {
                user.ModifyAsync(p => p.Nickname = newName);
            }

        }

        private async Task ClearUserRoles(IGuildUser user)
        {
            
            foreach (var roleId in user.RoleIds)
            {
                foreach (var role in _allRoles.Where(role => roleId == role.Id))
                {
                    user.RemoveRoleAsync(role);
                }
            }
        }

        private async Task RemoveBadRoles(IGuildUser user, SocketRole role)
        {
            foreach (var roleId in user.RoleIds)
            {
                foreach (var availableRole in _allRoles.Where(availableRoles => roleId == availableRoles.Id && roleId != role.Id))
                {
                    user.RemoveRoleAsync(availableRole);
                }
            }
        }

    }
}