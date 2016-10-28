using UnityEditor;
using UnityEngine;

namespace Ludiq.Dependencies
{
	[CustomEditor(typeof(Dependency), true)]
	public class DependencyEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			var dependency = (Dependency)target;

			EditorGUILayout.Space();

			if (GUILayout.Button("Import"))
			{
				dependency.Import();
			}
		}
	}
}