using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Ludiq.Dependencies
{
	[CreateAssetMenu]
	public class DependencyGroup : ScriptableObject
	{
		public List<Dependency> dependencies = new List<Dependency>();

		[Space]
		public bool overwrite = true;

		public void ImportAll()
		{
			foreach (var dependency in dependencies)
			{
				EditorUtility.DisplayProgressBar("Import Dependencies", "Importing " + dependency.name + "...", 0);
				dependency.Import(overwrite);
			}

			foreach (var dependency in dependencies)
			{
				EditorUtility.DisplayProgressBar("Import Dependencies", "Renaming namespaces for " + dependency.name + "...", 0);
				dependency.ReplaceNamespaces();
			}

			EditorUtility.ClearProgressBar();
		}
	}
}