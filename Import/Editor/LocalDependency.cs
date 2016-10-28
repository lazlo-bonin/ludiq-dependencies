using Ludiq.Dependencies.Internal;
using UnityEngine;

namespace Ludiq.Dependencies
{
	[CreateAssetMenu]
	public class LocalDependency : Dependency
	{
		[Header("Local"), Space]
		public string dependencyPath;

		protected override void ImportFiles(string destinationDirectory)
		{
			FileSystemUtilities.CopyDirectory(dependencyPath, destinationDirectory);
		}
	}
}