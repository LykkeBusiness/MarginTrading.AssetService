﻿using System;
using System.Text;
using System.Threading.Tasks;
using Lykke.SlackNotifications;
using MarginTrading.SettingsService.Core.Domain;
using MarginTrading.SettingsService.Core.Services;

namespace MarginTrading.SettingsService.Services
{
    public class MtSlackNotificationsSender : ISlackNotificationsSender
    {
        private readonly ISlackNotificationsSender _sender;
        private readonly string _appName;
        private readonly string _env;

        public MtSlackNotificationsSender(ISlackNotificationsSender sender, string appName, string env)
        {
            _sender = sender;
            _appName = appName;
            _env = env;
        }

        public async Task SendAsync(string type, string sender, string message)
        {
            if (type.Equals(ChannelTypes.Monitor, StringComparison.InvariantCultureIgnoreCase))
            {
                await _sender.SendAsync(type, sender, message);
                return;
            }

            await _sender.SendAsync(ChannelTypes.MarginTrading, sender, GetSlackMsg(message));
        }

        public async Task SendAsync(DateTime moment, string type, string sender, string message)
        {
            if (type.Equals(ChannelTypes.Monitor, StringComparison.InvariantCultureIgnoreCase))
            {
                await _sender.SendAsync(moment, type, sender, message);
                return;
            }

            await _sender.SendAsync(moment, ChannelTypes.MarginTrading, sender, GetSlackMsg(message));
        }

        private string GetSlackMsg(string message)
        {
            var sb = new StringBuilder();
            sb.Append(_appName);
            sb.Append(":");
            sb.AppendLine(_env);
            sb.AppendLine("\n====================================");
            sb.AppendLine(message);
            sb.AppendLine("====================================\n");

            return sb.ToString();
        }
    }
}
