using System;
using System.Collections.Generic;
using System.IO;
using Ludiq.Dependencies.Internal;
using UnityEditor;
using UnityEngine;

namespace Ludiq.Dependencies
{
	public abstract class Dependency : ScriptableObject
	{
		private const string progressTitle = "Import Dependency";
		private const int progressSteps = 8;
		private static int progressIndex = 0;

		[Header("Paths")]
		public string sourcePath;
		public string destinationPath;
		public List<string> preserveFiles = new List<string>();

		[Header("Replace Namespaces"), Space]
		public string sourceNamespace;
		public string destinationNamespace;

		protected abstract void ImportFiles(string destinationDirectory);

		protected void UpdateProgress(string message, bool increment = true)
		{
			EditorUtility.DisplayProgressBar(progressTitle, message, ((float)progressIndex / progressSteps));

			if (increment)
			{
				progressIndex++;
			}
		}

		public virtual void Import(bool overwrite = false)
		{
			var temporaryPath = Path.Combine(Path.GetTempPath(), "ImportDependency");
			var preservationPath = Path.Combine(Path.GetTempPath(), "ImportDependencyPreservation");

			try
			{
				var sourcePath = Path.Combine(temporaryPath, this.sourcePath);
				var destinationPath = Path.Combine(Application.dataPath, this.destinationPath);

				UpdateProgress("Creating temporary folders...");

				// Clean the temporary directories

				FileSystemUtilities.CleanDirectory(temporaryPath);
				FileSystemUtilities.CleanDirectory(preservationPath);

				// Copy source files to the temporary directory

				UpdateProgress("Importing files...");

				ImportFiles(temporaryPath);

				if (!Directory.Exists(sourcePath))
				{
					throw new InvalidOperationException(string.Format("Source folder '{0}' does not exist.", this.sourcePath));
				}

				// Copy preserved file patterns

				UpdateProgress("Copying preserved files...");

				foreach (var preservationPattern in preserveFiles)
				{
					foreach (var destinationFile in Directory.GetFiles(destinationPath, preservationPattern, SearchOption.AllDirectories))
					{
						FileSystemUtilities.CopyFileRelative(destinationFile, destinationPath, preservationPath);
					}
				}

				// Clean destination folder

				if (Directory.Exists(destinationPath) && !overwrite)
				{
					var confirmationMessage = "Are you sure you want to overwrite the existing destination folder?";

					if (preserveFiles.Count > 0)
					{
						confirmationMessage += "The following file patterns will be preserved: \n\n";

						foreach (var preservationPattern in preserveFiles)
						{
							confirmationMessage += preservationPattern + "\n";
						}
					}

					if (overwrite || EditorUtility.DisplayDialog("Import Dependency", confirmationMessage, "Overwrite", "Cancel"))
					{
						FileSystemUtilities.DeleteDirectory(destinationPath);
					}
					else
					{
						return;
					}
				}

				UpdateProgress("Cleaning destination folder...");

				Directory.CreateDirectory(destinationPath);

				// Copy source files to the destination path

				UpdateProgress("Copying dependency files to destination folder...");

				FileSystemUtilities.CopyDirectory(sourcePath, destinationPath);

				// Copy the license files to the destination path

				UpdateProgress("Copying license file to destination folder...");

				foreach (var licenseFile in Directory.GetFiles(sourcePath, "LICENSE*", SearchOption.AllDirectories))
				{
					FileSystemUtilities.CopyFileRelative(licenseFile, sourcePath, destinationPath);
				}

				// Overwrite them with preserved files

				UpdateProgress("Copying preserved files to destination folder...");

				FileSystemUtilities.CopyDirectory(preservationPath, destinationPath);

				// Replace namespaces in C# files

				UpdateProgress("Renaming namespaces in source files...");

				ReplaceNamespaces();

				Debug.LogFormat("Import of {0} dependency successful.", name);

				progressIndex = 0;
			}
			catch (Exception ex)
			{
				EditorUtility.DisplayDialog(progressTitle, "Error while importing dependency: \n\n" + ex.Message, "OK");
				Debug.LogError(ex);
			}
			finally
			{
				// Clean the temporary folders

				UpdateProgress("Cleaning temporary folders...");

				FileSystemUtilities.DeleteDirectory(temporaryPath);
				FileSystemUtilities.DeleteDirectory(preservationPath);

				EditorUtility.ClearProgressBar();
			}
		}

		public void ReplaceNamespaces()
		{
			foreach (var sourceFile in Directory.GetFiles(Application.dataPath, "*.cs", SearchOption.AllDirectories))
			{
				var source = File.ReadAllText(sourceFile);
				bool replace = source.Contains("namespace " + sourceNamespace) || source.Contains("using " + sourceNamespace);

				if (replace)
				{
					source = source.Replace(string.Format("namespace {0}", sourceNamespace), string.Format("namespace {0}", destinationNamespace));
					source = source.Replace(string.Format("using {0}", sourceNamespace), string.Format("using {0}", destinationNamespace));
					File.WriteAllText(sourceFile, source);
				}
			}
		}
	}
}
