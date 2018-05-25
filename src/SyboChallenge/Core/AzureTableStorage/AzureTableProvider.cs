using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.Generic;

namespace SyboChallenge.Core.AzureTableStorage
{
    public class AzureTableProvider
    {
        private readonly AzureTableOptions options;
        private readonly CloudStorageAccount storageAccount;
        private CloudTableClient client => storageAccount.CreateCloudTableClient();

        public CloudTable UserTable => client.GetTableReference(options.UserTableName);
        public CloudTable UserNameTable => client.GetTableReference(options.UserNameTableName);

        public IEnumerable<CloudTable> AllTables => new[] { UserTable, UserNameTable };

        public AzureTableProvider(AzureTableOptions options)
        {
            this.options = options;

            if (options.UseDevelopmentStorage)
                storageAccount = CloudStorageAccount.DevelopmentStorageAccount;
            else
                storageAccount = CloudStorageAccount.Parse(options.ConnectionString);
        }
    }
}
