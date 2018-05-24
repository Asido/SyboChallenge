using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SyboChallenge.Core.Setup
{
    public class Installer
    {
        private readonly Dictionary<InstallPhase, IEnumerable<IInstallStep>> providerInstallers;
        private static readonly InstallPhase[] stepExecutionOrder = new[]
        {
                InstallPhase.Install,
                InstallPhase.Upgrade,
                InstallPhase.Initialize,
                InstallPhase.Seed,
                InstallPhase.Validate
        };

        public Installer(IEnumerable<IInstallStep> providerInstallers)
        {
            this.providerInstallers = providerInstallers
                .GroupBy(x => x.Phase)
                .ToDictionary(x => x.Key, x => x.AsEnumerable());
        }

        public async Task Execute()
        {
            var errors = new List<string>();

            foreach (var installers in stepExecutionOrder.Select(getInstallersForStep))
            {
                foreach (var installer in installers.OrderBy(x => x.Priority))
                {
                    if (!await installer.ShouldRun())
                        continue;

                    var result = await installer.Run();
                    if (result != null && result.Any())
                        errors.AddRange(result);
                }

                if (errors.Any())
                    break;
            }

            if (errors.Any())
                throw new InstallerException(errors);
        }

        private IEnumerable<IInstallStep> getInstallersForStep(InstallPhase step) =>
            providerInstallers.TryGetValue(step, out var installers) ? installers : Enumerable.Empty<IInstallStep>();
    }
}
