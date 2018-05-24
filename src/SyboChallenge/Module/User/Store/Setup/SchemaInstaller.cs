using SyboChallenge.Core.AzureTableStorage;
using SyboChallenge.Core.Setup;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SyboChallenge.Module.User.Store.Setup
{
    public class SchemaInstaller : IInstallStep
    {
        private readonly AzureTableProvider provider;

        public string Name { get; } = "Create Azure Storage Tables";
        public InstallPhase Phase { get; } = InstallPhase.Install;
        public int Priority { get; } = 0;

        public SchemaInstaller(AzureTableProvider provider)
        {
            this.provider = provider;
        }

        public async Task<bool> ShouldRun()
        {
            var tasks = provider.AllTables.Select(x => x.ExistsAsync());
            await Task.WhenAll(tasks);

            return tasks.Any(x => !x.Result);
        }

        public async Task<IEnumerable<string>> Run()
        {
            var tasks = provider.AllTables.Select(x => x.CreateIfNotExistsAsync());

            await Task.WhenAll(tasks);

            if (tasks.All(x => x.Result))
                return null;

            var errors = new List<string>();
            foreach (var table in provider.AllTables)
            {
                if (!await table.ExistsAsync())
                    errors.Add($"Failed to create table {table.Name}.");
            }

            return errors;
        }
    }
}
