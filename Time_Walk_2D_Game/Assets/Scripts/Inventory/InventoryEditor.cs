#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Inventory))]

public class InventoryEditor : Editor {

	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		Inventory e = (Inventory)target;
		GUILayout.Label("Build Grid:", EditorStyles.boldLabel);
		if(GUILayout.Button("Create / Update"))
		{
			e.BuildGrid();
		}
	}
}
#endif
