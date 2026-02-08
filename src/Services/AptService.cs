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
                Arguments = $"-c \"apt-cache search {query} | grep -i {query} | sort\"", 
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

                    public async Task RunPackageActionAsync(string name, string action)

                    {

                        string aptCommand = action == "install" ? "install" : "remove";

                        string command = $"sudo apt-get {aptCommand} '{name}'; echo; echo 'Process finished. Press Enter to close...'; read";

                        string terminal = "";

                        string terminalArgs = "";

                        if (System.IO.File.Exists("/usr/bin/x-terminal-emulator"))

                        {

                            terminal = "/usr/bin/x-terminal-emulator";

                            terminalArgs = $"-e bash -c \"{command}\"";

                        }

                        else if (System.IO.File.Exists("/usr/bin/konsole"))

                        {

                            terminal = "/usr/bin/konsole";

                            terminalArgs = $"-e bash -c \"{command}\"";

                        }

                        else if (System.IO.File.Exists("/usr/bin/gnome-terminal"))

                        {

                            terminal = "/usr/bin/gnome-terminal";

                            terminalArgs = $"-- bash -c \"{command}\"";

                        }

                        else if (System.IO.File.Exists("/usr/bin/xfce4-terminal"))

                        {

                            terminal = "/usr/bin/xfce4-terminal";

                            terminalArgs = $"-e \"bash -c '{command}'\"";

                        }

                        else if (System.IO.File.Exists("/usr/bin/xterm"))

                        {

                            terminal = "/usr/bin/xterm";

                            terminalArgs = $"-e \"bash -c '{command}'\"";

                        }

                        else if (System.IO.File.Exists("/usr/bin/alacritty"))

                        {

                            terminal = "/usr/bin/alacritty";

                            terminalArgs = $"-e bash -c \"{command}\"";

                        }

                        else if (System.IO.File.Exists("/usr/bin/kitty"))

                        {

                            terminal = "/usr/bin/kitty";

                            terminalArgs = $"-e bash -c \"{command}\"";

                        }

                        else

                        {

                            terminal = "/usr/bin/bash";

                            terminalArgs = $"-c \"{command}\"";

                        }

            

                        var startInfo = new ProcessStartInfo

                        {

                            FileName = terminal,

                            Arguments = terminalArgs,

                            UseShellExecute = false,

                            CreateNoWindow = false

                        };

            

                        try

                        {

                            using var process = Process.Start(startInfo);

                            if (process != null)

                            {

                                await process.WaitForExitAsync();

                            }

                        }

                        catch (Exception ex)

                        {

                            Console.WriteLine($"Error launching terminal: {ex.Message}");

                        }

                    }

                }

            }

            