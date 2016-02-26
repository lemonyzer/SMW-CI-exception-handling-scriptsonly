using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(Hazards))]
public class HazardEditor : Editor {

	Hazards targetObject;

	void OnEnable()
	{
		targetObject = (Hazards)target;
	}
	
	public override void OnInspectorGUI()
	{
		//base.OnInspectorGUI ();
		GUIElementsTop();

		DrawDefaultInspector();

		GUILayout.Space(10);
		GUIElementsBottom();
	}

	void GUIElementsTop()
	{
		GUILayout.BeginVertical();
		{
			if (GUILayout.Button("Init Hazards"))
			{
				targetObject.InitHazards();
			}


		}
		GUILayout.EndVertical();

	}

	void GUIElementsBottom()
	{
		GUILayout.BeginVertical();
		{
			if (GUILayout.Button("Check"))
			{
				targetObject.InitNaming();
			}
		}
		GUILayout.EndVertical();
	}
}
