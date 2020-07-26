using System;
using System.Net.Http;
using TwitchLib.Client;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;

namespace SuaveBot
{
    class Program
    {
        static void Main(string[] args)
        {
            Bot bot = new Bot();
            Console.ReadLine();
        }
    }

    class Bot
    {
        TwitchClient client;
        int newUserCount = 0;
        public Bot()
        {
            var credentials = new ConnectionCredentials(Keys.TwitchAccount, Keys.TwitchToken);
            var clientOptions = new ClientOptions
            {
                MessagesAllowedInPeriod = 750,
                ThrottlingPeriod = TimeSpan.FromSeconds(30)
            };
            var customClient = new WebSocketClient(clientOptions);
            client = new TwitchClient(customClient);
            client.Initialize(credentials, Keys.TwitchChannel);

            client.OnLog += Client_OnLog;
            client.OnJoinedChannel += Client_OnJoinedChannel;
            client.OnMessageReceived += Client_OnMessageReceived;
            client.OnNewSubscriber += Client_OnNewSubscriber;
            client.OnConnected += Client_OnConnected;
            client.OnUserJoined += Client_OnUserJoined;
            client.Connect();
        }

        private void Client_OnUserJoined(object sender, OnUserJoinedArgs e)
        {
            Console.WriteLine($"USER JOINED: {e.Username}");
            newUserCount++;
            if(newUserCount % 5 == 0) // every 5 new folks, lets let them know whats up
            {
                client.SendMessage(e.Channel, "Welcome to Suave_Pirate's Suave Stream! We're building cool stuff like voice powered apps, virtual assistants, and more! We also do some gaming and guitar playing :)");
            }
        }

        private void Client_OnLog(object sender, OnLogArgs e)
        {
            //Console.WriteLine($"{e.DateTime.ToString()}: {e.BotUsername} - {e.Data}");
        }

        private void Client_OnConnected(object sender, OnConnectedArgs e)
        {
            Console.WriteLine($"Connected to {e.AutoJoinChannel}");
        }

        private void Client_OnJoinedChannel(object sender, OnJoinedChannelArgs e)
        {
            var message = "Hey everyone! I am a bot that lets you ask about the channel. Try something like !project to learn about the current project we're working on.";
            Console.WriteLine(message);
            client.SendMessage(e.Channel, message);
        }

        private void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            if(e.ChatMessage?.Message?.ToLower() == "!project")
                client.SendMessage(e.ChatMessage.Channel, "We're currently working on Project Suave Keys - a voice powered controller and keyboard to let you use your virtual assistants and voice to control your games and other software. Checkout the github here: https://github.com/suavepirate/suavekeys");
            if (e?.ChatMessage?.Message?.ToLower() == "!discord")
                client.SendMessage(e.ChatMessage.Channel, "https://discord.gg/uMyJDug");

        }


        private void Client_OnNewSubscriber(object sender, OnNewSubscriberArgs e)
        {
            client.SendMessage(e.Channel, $"Welcome {e.Subscriber.DisplayName} to the Pirate Crew! You just earned 500 points!");
        }
    }
}