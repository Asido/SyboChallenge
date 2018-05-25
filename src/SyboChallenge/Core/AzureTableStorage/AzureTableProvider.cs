using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.Generic;

namespace SyboChallenge.Core.AzureTableStorage
{
    public class AzureTableProvider
    {
        public readonly AzureTableOptions options;
        public readonly CloudStorageAccount storageAccount;
        public readonly CloudTableClient client;

        public readonly CloudTable UserTable;
        public readonly CloudTable UserNameTable;

        public readonly IEnumerable<CloudTable> AllTables;

        public AzureTableProvider(AzureTableOptions options)
        {
            this.options = options;

            if (options.UseDevelopmentStorage)
                storageAccount = CloudStorageAccount.DevelopmentStorageAccount;
            else
                storageAccount = CloudStorageAccount.Parse(options.ConnectionString);

            client = storageAccount.CreateCloudTableClient();

            UserTable = client.GetTableReference(options.UserTableName);
            UserNameTable = client.GetTableReference(options.UserNameTableName);

            AllTables = new[] { UserTable, UserNameTable };
        }
    }
}
