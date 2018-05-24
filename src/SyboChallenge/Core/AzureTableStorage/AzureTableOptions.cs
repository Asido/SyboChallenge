﻿namespace SyboChallenge.Core.AzureTableStorage
{
    public class AzureTableOptions
    {
        public bool UseDevelopmentStorage { get; set; } = false;
        public string ConnectionString { get; set; }

        public int BatchOperationSize { get; set; } = 100;

        public string UserTableNme { get; set; } = "User";
    }
}
