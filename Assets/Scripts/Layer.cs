using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(Layer))]
public class LayerEditor : Editor {
	Layer targetObject;

	[MenuItem("SMW/ScriptableObject/LayerManager")]
	static void CreateLayerManager()
	{
		UnityEnhancements.ScriptableObjectUtility.CreateAsset<Layer> ();
	}

	void OnEnable()
	{
		targetObject = (Layer)target;
	}

	public override void OnInspectorGUI()
	{
		//base.OnInspectorGUI ();
		GUIElementsTop();

		DrawDefaultInspector();

		GUILayout.Space(10);
		GUIElementsBottom();
	}

	void GUIElementsTop () {
		if (GUILayout.Button ("Create Default Layers")) {
		}

		if (GUILayout.Button ("Update Layers")) {
			targetObject.Init();
		}

		if (GUILayout.Button ("Check")) {
		}
	}

	void GUIElementsBottom () {
	}
}
#endif

public class Layer : ScriptableObject {

	static Layer instance;

	public static Layer Instance {
		get
		{
			if (instance == null)
			{
				// singleton instance not defined
				instance = (Layer) FindObjectOfType<Layer>();
			}

			return instance;
				
		}
		private set { instance = value; }
	}
	
	// Physic Layer
	
	public LayerMask allPlayer;
	public LayerMask whatIsStaticGround;
	public LayerMask whatIsJumpOnPlatform;
	public LayerMask whatIsAllGround;
	public LayerMask whatIsWall;

	public int defaultLayer;
	public int deathLayer;
	public int superDeathLayer;
	public int player;

//	public int player1;
//	public int player2;
//	public int player3;
//	public int player4;
	
//	public int enemy;
	public int feet;
	public int head;
	public int body;
	public int item;

	public int ground;
	public int groundIcyLayer;
//	public int tagAble;
//	public int floor;
	public int block;
	public int jumpAblePlatform;
//	public int jumpAblePlatformSaveZone;
	
	public int powerUp;
//	public int bullet;
	
	public int groundStopper;
	
//	public int fader;


	public string defaultLayerName = "Default";
	public string deathLayerName = "Death";
	public string superDeathLayerName = "SuperDeath";
	public string playerLayerName = "Player";

//	public string player1LayerName = "Player1";
//	public string player2LayerName = "Player2";
//	public string player3LayerName = "Player3";
//	public string player4LayerName = "Player4";
//	
	public string feetLayerName = "Feet";
	public string headLayerName = "Head";
	public string bodyLayerName = "Body";
	public string itemLayerName = "Item";
	public string powerUpLayerName= "PowerUp";

//	public string enemyLayerName = "Enemy";

	public string groundLayerName = "Ground";
	public string groundIcyLayerName = "GroundIcy";
//	public string tagAbleLayerName = "TagAble";
//	public string floorLayerName = "Floor";
	public string blockLayerName = "Block";
	public string jumpAblePlatformLayerName = "JumpOnPlatform";
//	public string jumpAblePlatformSaveZoneLayerName = "JumpSaveZone";

//	public string bulletLayerName = "Bullet";
	public string groundStopperLayerName = "GroundStopper";
	
//	public string faderLayerName = "Fader";
	
	public void Init()
	{
		Debug.LogWarning(this.ToString() + ": Awake() - init public layer integers, scripts layer instantiation have to be AFTER this initialisation, NOT IN AWAKE!!!" );

		defaultLayer = LayerMask.NameToLayer(defaultLayerName);
		deathLayer = LayerMask.NameToLayer(deathLayerName);
		superDeathLayer = LayerMask.NameToLayer(superDeathLayerName);
		player = LayerMask.NameToLayer(playerLayerName);
//		player1 = LayerMask.NameToLayer(player1LayerName);
//		player2 = LayerMask.NameToLayer(player2LayerName);
//		player3 = LayerMask.NameToLayer(player3LayerName);
//		player4 = LayerMask.NameToLayer(player4LayerName);

		allPlayer = 1 << player;

//		allPlayer = 1 << player1;
//		allPlayer |= 1 << player2;
//		allPlayer |= 1 << player3;
//		allPlayer |= 1 << player4;
		
		feet = LayerMask.NameToLayer(feetLayerName);
		head = LayerMask.NameToLayer(headLayerName);
		body = LayerMask.NameToLayer(bodyLayerName);
		item = LayerMask.NameToLayer(itemLayerName);
		groundStopper = LayerMask.NameToLayer(groundStopperLayerName);
		powerUp = LayerMask.NameToLayer(powerUpLayerName);

//		bullet = LayerMask.NameToLayer(bulletLayerName);

		
//		enemy = LayerMask.NameToLayer(enemyLayerName);

		ground = LayerMask.NameToLayer(groundLayerName);
		groundIcyLayer = LayerMask.NameToLayer (groundIcyLayerName);
//		tagAble = LayerMask.NameToLayer(tagAbleLayerName);
//		floor = LayerMask.NameToLayer(floorLayerName);
		block = LayerMask.NameToLayer(blockLayerName);
		jumpAblePlatform = LayerMask.NameToLayer(jumpAblePlatformLayerName);
//		jumpAblePlatformSaveZone = LayerMask.NameToLayer(jumpAblePlatformSaveZoneLayerName);

		whatIsStaticGround = 1 << ground;
//		whatIsGround |= 1 << tagAble;
//		whatIsGround = 1 << floor;
		whatIsStaticGround |= 1 << block;

		whatIsJumpOnPlatform = 1 << jumpAblePlatform;

		whatIsAllGround = whatIsStaticGround;
		whatIsAllGround |= 1 << jumpAblePlatform;

		whatIsWall = whatIsStaticGround;
		

//		fader = LayerMask.NameToLayer(faderLayerName);
	}

//	void OnLevelWasLoaded()
//	{
//		Debug.LogWarning(this.ToString() + ": OnLevelWasLoaded()" );
//	}
//
//	void Start()
//	{
//		Debug.LogWarning(this.ToString() + ": Start()" );
//	}
	
}
