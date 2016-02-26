using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
using UnityEnhancements;
public class HazardManagerHelper
{
	[MenuItem("SMW/ScriptableObject/HazardManager")]
	static Hazards CreateAsset ()
	{

		return ScriptableObjectUtility.CreateAsset<Hazards>("Assets","HazardsManager",true);

	}
}
#endif

public class Hazards : ScriptableObject {
	
	public List<Hazard> list;


	public void SetAmount (int num)
	{
		list = new List<Hazard> ();
	}

	public int GetAmount ()
	{
		return list.Count;
	}

	public void Init ()
	{
		for (int i=0; i< GetAmount (); i++)
		{
			if (list[i] == null)
				list[i] = new Hazard ();
		}
	}

	public Hazard GetHazard (HazardType type)
	{
		int index = (int)type;
		if (index >=0 && index < list.Count)
			return list[index];
		return null;
	}

	public void InitHazards ()
	{
		list = new List<Hazard> ();
		for (int i = 0; i < (int)HazardType.count; i++)
		{
			Hazard newHazard = new Hazard ();
			newHazard.Name = "" + (HazardType)i;
			newHazard.type = (HazardType)i;
			list.Add (newHazard);
		}
		EditorUtility.SetDirty (this);
	}

	public void InitNaming ()
	{
		for (int i = 0; i < GetAmount (); i++)
		{
			Hazard current = list [i];
			if (current != null)
				current.Name = "" + current.type;
		}
		EditorUtility.SetDirty (this);
	}
}
