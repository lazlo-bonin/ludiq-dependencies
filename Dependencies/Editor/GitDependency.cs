using System;
using System.Diagnostics;
using System.IO;
using Ludiq.Dependencies.Internal;
using UnityEngine;

namespace Ludiq.Dependencies
{
	[CreateAssetMenu]
	public class GitDependency : Dependency
	{
		[Header("Git"), Space]
		public string repositoryPath;

		protected override void ImportFiles(string destinationDirectory)
		{
			ProcessStartInfo gitCloneStart = new ProcessStartInfo("git.exe");

			gitCloneStart.UseShellExecute = false;
			gitCloneStart.RedirectStandardInput = true;
			gitCloneStart.RedirectStandardOutput = true;
			gitCloneStart.RedirectStandardError = true;
			gitCloneStart.CreateNoWindow = true;
			gitCloneStart.Arguments = string.Format("clone \"{0}\" \"{1}\" -q", repositoryPath, destinationDirectory);

			var gitClone = Process.Start(gitCloneStart);

			string output;

			while ((output = gitClone.StandardOutput.ReadLine()) != null)
			{
				UpdateProgress("Git: " + output, false);
			}

			gitClone.WaitForExit();

			if (gitClone.ExitCode != 0)
			{
				throw new InvalidOperationException("Git error: " + gitClone.StandardError.ReadToEnd());
			}

			FileSystemUtilities.ResetDirectoryAttributes(destinationDirectory);

			var gitDirectories = Directory.GetDirectories(destinationDirectory, ".git*");

			foreach (var gitDirectory in gitDirectories)
			{
				FileSystemUtilities.DeleteDirectory(gitDirectory);
			}

			var gitFiles = Directory.GetFiles(destinationDirectory, ".git*");

			foreach (var gitFile in gitFiles)
			{
				File.Delete(gitFile);
			}
		}
	}
}