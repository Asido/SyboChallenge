using System;
using System.Collections.Generic;
using System.Text;

namespace SyboChallenge.Core.Setup
{
    public class InstallerException : Exception
    {
        public InstallerException(IReadOnlyCollection<string> errors)
            : base(FormatMessage(errors)) { }

        private static string FormatMessage(IReadOnlyCollection<string> errors)
        {
            var builder = new StringBuilder();
            builder.AppendLine($"Installer failed with the following {errors.Count} errors:");

            foreach (var error in errors)
                builder.AppendLine("  • " + error);

            return builder.ToString();
        }
    }
}
