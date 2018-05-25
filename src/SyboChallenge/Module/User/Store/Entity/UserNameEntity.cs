using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace SyboChallenge.Module.User.Store.Entity
{
    public class UserNameEntity : TableEntity
    {
        [IgnoreProperty]
        public string Name
        {
            get => ParsePartitionKey(PartitionKey);
            set => PartitionKey = FormatPartitionKey(value);
        }

        public Guid Id { get; set; }

        public UserNameEntity()
        {
            RowKey = FormatRowKey();
        }

        public UserNameEntity(string name, Guid id) : this()
        {
            Name = name;
            Id = id;
        }

        public static string FormatPartitionKey(string name) => name;
        public static string ParsePartitionKey(string partitionKey) => partitionKey;

        public static string FormatRowKey() => "";
    }
}
