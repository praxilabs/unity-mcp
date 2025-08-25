using System;
using System.Diagnostics;

namespace UnityMcpBridge.Editor.Helpers
{
    /// <summary>
    /// Utility class for running command-line processes with proper error handling.
    /// </summary>
    public static class CommandRunner
    {
        /// <summary>
        /// Runs a command-line process and handles output/errors.
        /// </summary>
        /// <param name="command">The command to execute (e.g., "git", "uv")</param>
        /// <param name="arguments">The arguments to pass to the command</param>
        /// <param name="workingDirectory">The working directory for the command (optional)</param>
        /// <exception cref="Exception">Thrown when the command fails (non-zero exit code)</exception>
        public static void RunCommand(
            string command,
            string arguments,
            string workingDirectory = null
        )
        {
            Process process = new()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = command,
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WorkingDirectory = workingDirectory ?? string.Empty,
                },
            };
            
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();
            
            if (process.ExitCode != 0)
            {
                throw new Exception(
                    $"Command failed: {command} {arguments}\nOutput: {output}\nError: {error}"
                );
            }
        }
        
        /// <summary>
        /// Runs a command-line process and returns the output.
        /// </summary>
        /// <param name="command">The command to execute</param>
        /// <param name="arguments">The arguments to pass to the command</param>
        /// <param name="workingDirectory">The working directory for the command (optional)</param>
        /// <returns>The standard output of the command</returns>
        /// <exception cref="Exception">Thrown when the command fails (non-zero exit code)</exception>
        public static string RunCommandWithOutput(
            string command,
            string arguments,
            string workingDirectory = null
        )
        {
            Process process = new()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = command,
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WorkingDirectory = workingDirectory ?? string.Empty,
                },
            };
            
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();
            
            if (process.ExitCode != 0)
            {
                throw new Exception(
                    $"Command failed: {command} {arguments}\nOutput: {output}\nError: {error}"
                );
            }
            
            return output.Trim();
        }
        
        /// <summary>
        /// Runs a command-line process and returns both output and exit code.
        /// </summary>
        /// <param name="command">The command to execute</param>
        /// <param name="arguments">The arguments to pass to the command</param>
        /// <param name="workingDirectory">The working directory for the command (optional)</param>
        /// <returns>A tuple containing the output and exit code</returns>
        public static (string output, string error, int exitCode) RunCommandWithResult(
            string command,
            string arguments,
            string workingDirectory = null
        )
        {
            Process process = new()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = command,
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WorkingDirectory = workingDirectory ?? string.Empty,
                },
            };
            
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();
            
            return (output.Trim(), error.Trim(), process.ExitCode);
        }
    }
}
