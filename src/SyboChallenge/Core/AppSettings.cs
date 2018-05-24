using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace SyboChallenge.Core
{
    public class AppSettings
    {
        public readonly ConnectionStrings ConnectionStrings;

        public AppSettings(ConnectionStrings connectionStrings)
        {
            ConnectionStrings = connectionStrings;
        }

        public static AppSettings From(IConfiguration section)
        {
            var config = new AppSettings(new ConnectionStrings(section.GetSection(nameof(ConnectionStrings))));
            return config;
        }
    }

    public class ConnectionStrings
    {
        public readonly string User;

        public ConnectionStrings(IConfiguration section)
        {
            User = section[nameof(User)];
        }
    }
}
