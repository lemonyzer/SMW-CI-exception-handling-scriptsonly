using UnityEngine;
using System;
using SMW;

[Serializable]
public class MapTile
{
	[SerializeField]
	public TileType iType;
	[SerializeField]
	public int iFlags;
};