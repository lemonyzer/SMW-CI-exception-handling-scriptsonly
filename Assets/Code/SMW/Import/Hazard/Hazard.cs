using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// TODO ScriptableObject

[System.Serializable]
public class Hazard {

	[SerializeField]
	private string name;

	[SerializeField]
	public HazardType type;
	[SerializeField]
	public List<Sprite> projectile;

	[SerializeField]
	public Sprite previewSprite;

	public string Name {
		get { return name; }
		set { name = value; }
	}


}
