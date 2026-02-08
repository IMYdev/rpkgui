using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using rpkGUI.Models;

namespace rpkGUI.Services
{
    public class AptService
    {
        public async Task<List<Package>> SearchPackagesAsync(string query)
        {
            var results = new List<Package>();

            if (string.IsNullOrWhiteSpace(query) || query.Contains(';')) 
                return results;

            var processStartInfo = new ProcessStartInfo
            {
                FileName = "/bin/bash",
                Arguments = $"-c \"apt-cache search {query}\"", 
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = new Process { StartInfo = processStartInfo };
            process.Start();

            while (await process.StandardOutput.ReadLineAsync() is { } line)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                var parts = line.Split(new[] { " - " }, 2, StringSplitOptions.None);

                results.Add(new Package { Name = parts[0], Description = parts[1] });
            }

            await process.WaitForExitAsync();
            return results;
        }
        public async Task<List<Package>> ListPackages()
        {
            var results = new List<Package>();

            var processStartInfo = new ProcessStartInfo
            {
                FileName = "/bin/bash",
                Arguments = $"-c \"apt-cache pkgnames\"", 
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = new Process { StartInfo = processStartInfo };
            process.Start();

            while (await process.StandardOutput.ReadLineAsync() is { } line)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                results.Add(new Package { Name = line.Trim() });
            }

            await process.WaitForExitAsync();
            return results;
        }
    }
}