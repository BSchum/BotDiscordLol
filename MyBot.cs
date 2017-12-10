using Discord;
using Discord.Commands;
using RiotApi.Net.RestClient;
using RiotApi.Net.RestClient.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot
{
    class MyBot
    {
        DiscordClient discord;
        CommandService commands;
        IRiotClient riotClient = new RiotClient("RGAPI-98426dc4-fbcc-414e-94b7-fd758437fee7");
        public MyBot()
        {
            discord = new DiscordClient(x =>
            {
                x.LogLevel = LogSeverity.Info;
                x.LogHandler = Log;

            });

            discord.UsingCommands(x =>
            {
                x.PrefixChar = '!';
                x.AllowMentionPrefix = true;

            });


            commands = discord.GetService<CommandService>();
            OpggFunc();
            PedroFunc();
            GetCurrentGameInfo();
            discord.ExecuteAndWait(async () =>
            {
                await discord.Connect("",TokenType.Bot);
            });
        }
        private void OpggFunc()
        {
            commands.CreateCommand("opgg")
            .Parameter("SummonerName")
            .Do(async (e) =>
            {
                 await e.Channel.SendMessage("http://euw.op.gg/summoner/userName=" + e.GetArg("SummonerName"));
            }
            );
        }

        private void GetCurrentGameInfo()
        {
            commands.CreateCommand("currentgame")
            .Parameter("SummonerName")
            .Do(async (e) =>
            {
                
                await e.Channel.SendMessage("GameInfo :");
                var summoners = riotClient.Summoner.GetSummonersByName(RiotApiConfig.Regions.EUW, e.GetArg("SummonerName"));
                var summonersId = summoners[e.GetArg("SummonerName")].Id;
                var currentGame = riotClient.CurrentGame.GetCurrentGameInformationForSummonerId(RiotApiConfig.Platforms.EUW1,summonersId);
                string message = "Banned Champ: ";
                foreach(var i in currentGame.BannedChampions)
                {
                    var champ = riotClient.LolStaticData.GetChampionById(RiotApiConfig.Regions.EUW,(int)i.ChampionId);
                    message += champ.Name+" \n";
                }
                int x = 0;
                message += "TEAM BLUE\n";
                foreach(var i in currentGame.Participants)
                {
                    if(x == 5)
                    {
                        message += "\nTEAM RED\n";
                    }
                    message += i.SummonerName+"  \n";
                    var actualSumm = riotClient.Summoner.GetSummonersById(RiotApiConfig.Regions.EUW,i.SummonerId.ToString());
                    message += actualSumm[i.SummonerId.ToString()].SummonerLevel.ToString();
                    x++;
                }
                await e.Channel.SendMessage(message);
            });

        }
        private void PedroFunc()
        {
            commands.CreateCommand("Pedro").Do(async (e) =>
            {
                await e.Channel.SendMessage("Paul, Léo , et Brice sont dans un bateau, léo tombe a l'eau ... Mais ou est pedro??");
            }   
            );
        }
        private void Log(object sender, LogMessageEventArgs e)
        {
            Console.WriteLine(e.Message);
        }
        private void disconnect()
        {
            commands.CreateCommand("disconnect").Do( async (e) => {
                await discord.Disconnect();
            });
        }
    }
}
