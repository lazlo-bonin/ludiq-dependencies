using System;
using System.IO;

namespace Ludiq.Dependencies.Internal
{
	internal static class FileSystemUtilities
	{
		public static string GetRelativePath(string file, string directory)
		{
			var pathUri = new Uri(file);

			// Folders must end in a slash
			if (!directory.EndsWith(Path.DirectorySeparatorChar.ToString()))
			{
				directory += Path.DirectorySeparatorChar;
			}

			var folderUri = new Uri(directory);

			return Uri.UnescapeDataString(folderUri.MakeRelativeUri(pathUri).ToString().Replace('/', Path.DirectorySeparatorChar));
		}

		public static void CopyFileRelative(string sourceFile, string sourceDirectory, string targetDirectory)
		{
			var relativePath = GetRelativePath(sourceFile, sourceDirectory);
			var targetFile = targetDirectory + "/" + relativePath;
			var targetSubDirectory = Directory.GetParent(targetFile).FullName;

			if (!Directory.Exists(targetSubDirectory))
			{
				Directory.CreateDirectory(targetSubDirectory);
			}

			File.Copy(sourceFile, targetFile);
		}

		public static void CopyDirectory(string sourceDirectory, string targetDirectory)
		{
			CopyDirectory(new DirectoryInfo(sourceDirectory), new DirectoryInfo(targetDirectory));
		}

		public static void CopyDirectory(DirectoryInfo source, DirectoryInfo target)
		{
			Directory.CreateDirectory(target.FullName);

			foreach (FileInfo file in source.GetFiles())
			{
				file.CopyTo(Path.Combine(target.FullName, file.Name), true);
			}

			foreach (DirectoryInfo subDirectory in source.GetDirectories())
			{
				DirectoryInfo nextTargetSubDir = target.CreateSubdirectory(subDirectory.Name);
				CopyDirectory(subDirectory, nextTargetSubDir);
			}
		}

		public static void ResetDirectoryAttributes(DirectoryInfo directory)
		{
			directory.Attributes = FileAttributes.Normal;

			foreach (var subDirectory in directory.GetDirectories())
			{
				ResetDirectoryAttributes(subDirectory);
			}

			foreach (var file in directory.GetFiles())
			{
				file.Attributes = FileAttributes.Normal;
			}
		}

		public static void ResetDirectoryAttributes(string directory)
		{
			ResetDirectoryAttributes(new DirectoryInfo(directory));
		}

		public static void DeleteDirectory(DirectoryInfo directory)
		{
			foreach (var file in directory.GetFiles())
			{
				file.Attributes = FileAttributes.Normal;
				file.Delete();
			}

			foreach (var subDirectory in directory.GetDirectories())
			{
				DeleteDirectory(subDirectory);
            }

			directory.Attributes = FileAttributes.Normal;
			directory.Delete(false);
		}

		public static void DeleteDirectory(string directory)
		{
			DeleteDirectory(new DirectoryInfo(directory));
		}

		public static void CleanDirectory(string directory)
		{
			if (Directory.Exists(directory))
			{
				DeleteDirectory(directory);
			}

			Directory.CreateDirectory(directory);
		}
	}
}