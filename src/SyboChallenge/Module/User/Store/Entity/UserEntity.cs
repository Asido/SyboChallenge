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

        public string Name { get; set; }
        public int GamesPlayed { get; set; }
        public int Highscore { get; set; }

        public List<Guid> Friends
        {
            get => JsonConvert.DeserializeObject<List<Guid>>(FriendsJson);
            set => FriendsJson = FormatFriendsJson(value);
        }
        public string FriendsJson { get; set; }

        public UserEntity()
        {
            RowKey = FormatRowKey();
        }

        public UserEntity(Guid key, string name, int gamesPlayed, int highscore) : this()
        {
            Key = key;
            Name = name;
            GamesPlayed = gamesPlayed;
            Highscore = highscore;
            Friends = new List<Guid>();
        }

        public static string FormatPartitionKey(Guid key) => key.ToString("D");
        public static Guid ParsePartitionKey(string partitionKey) => Guid.ParseExact(partitionKey, "D");

        public static string FormatRowKey() => "";

        public static string FormatFriendsJson(IEnumerable<Guid> keys) => JsonConvert.SerializeObject(keys);
    }
}
