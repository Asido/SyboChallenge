using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace SyboChallenge.Module.User.Store.Entity
{
    public class UserEntity : TableEntity
    {
        [IgnoreProperty]
        public Guid Key
        {
            get => ParsePartitionKey(PartitionKey);
            set => PartitionKey = FormatPartitionKey(value);
        }

        [IgnoreProperty]
        public string Name
        {
            get => ParseRowKey(RowKey);
            set => RowKey = FormatRowKey(value);
        }

        public int GamesPlayed { get; set; }
        public int Highscore { get; set; }

        public List<Guid> Friends
        {
            get => JsonConvert.DeserializeObject<List<Guid>>(FriendsJson);
            set => FriendsJson = JsonConvert.SerializeObject(value);
        }
        public string FriendsJson { get; set; }

        public UserEntity() { }
        public UserEntity(Guid key, string name, int gamesPlayed, int highscore)
        {
            Key = key;
            Name = name;
            GamesPlayed = gamesPlayed;
            Highscore = highscore;
            Friends = new List<Guid>();
        }

        public static string FormatPartitionKey(Guid key) => key.ToString("D");
        public static Guid ParsePartitionKey(string partitionKey) => Guid.ParseExact(partitionKey, "D");
        public static string FormatRowKey(string name) => name;
        public static string ParseRowKey(string rowKey) => rowKey;
    }
}
