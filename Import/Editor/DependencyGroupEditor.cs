using UnityEditor;
using UnityEngine;

namespace Ludiq.Dependencies
{
	[CustomEditor(typeof(DependencyGroup), true)]
	public class DependencyGroupEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			var dependencyGroup = (DependencyGroup)target;

			EditorGUILayout.Space();

			if (GUILayout.Button("Import All"))
			{
				dependencyGroup.ImportAll();
			}
		}
	}
}