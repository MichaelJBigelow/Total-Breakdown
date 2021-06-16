using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Controller : MonoBehaviour{

	// Script References
	private StateMachine   StateMachine;
	private UniversalLevel UniversalLevel;

	// Object References
	public  GameObject  Cannonball;
	public  GameObject  WreckingBall;
	public  GameObject  Grenade;
	public  GameObject  ImpactGrenade;
	public  GameObject  ImpactBomb;
	public  GameObject  TNT;
	public  GameObject  MegaBomb;
	public  GameObject  Imploder;
	public  GameObject  Vaporizer;
	private GameObject  DestructionWarehouse;
	private GameObject  WeaponObject; // Stores reference to currently selected weapon
	public  Camera      MainCamera;
	private Button      MenuButton;
	private Button      ResetButton;
	private Button      PauseButton;
	private Text        PauseButtonText;
	private Button      NextWeaponButton;
	private Button      PreviousWeaponButton;
	private Toggle      ZeroGravityToggle;
	private Slider      ZoomSlider;
	private Slider      RotationSlider;
	private Text        WeaponText;
	public GameObject[] ObjectsForPooling;

	// General Properties
	private float          lastZoomSliderValue;
	private float          lastRotationSliderValue;
	private float          projectileVelocity = 0;
	public  Color          pauseColor;
	public  Color          playColor;
	private string         pauseString = "| |";
	private string         playString  = ">";
	public static string[] weapons = new string[]{
			"Cannonball",
			"Wrecking Ball",
			"Grenade",
			"TNT",
			"Mega Bomb",
			"Impact Grenade",
			"Impact Bomb",
			"Imploder",
			"Vaporizer"
			};

	// Object pool setup
	public static Dictionary< string, Queue<GameObject> > ObjectPool;
	private int poolSize = 90;

	void Awake(){

		pauseColor = new Color( 0.8196079f, 0.454902f,  0.454902f,  0.7215686f );
		playColor  = new Color( 0.2039216f, 0.9568627f, 0.2196078f, 0.7215686f );

	}

	void Start(){

		MenuButton           = GameObject.Find( "MenuButton"           ).GetComponent<Button>();
		ResetButton          = GameObject.Find( "ResetButton"          ).GetComponent<Button>();
		PauseButton          = GameObject.Find( "PauseButton"          ).GetComponent<Button>();
		PauseButtonText      = GameObject.Find( "PauseText"            ).GetComponent<Text>();
		NextWeaponButton     = GameObject.Find( "NextWeaponButton"     ).GetComponent<Button>();
		PreviousWeaponButton = GameObject.Find( "PreviousWeaponButton" ).GetComponent<Button>();
		ZeroGravityToggle    = GameObject.Find( "ZeroGravityToggle"    ).GetComponent<Toggle>();
		ZoomSlider           = GameObject.Find( "ZoomSlider"           ).GetComponent<Slider>();
		RotationSlider       = GameObject.Find( "RotationSlider"       ).GetComponent<Slider>();
		StateMachine         = GameObject.Find( "StateMachine"         ).GetComponent<StateMachine>();
		UniversalLevel       = GameObject.Find( "Level"                ).GetComponent<UniversalLevel>();
		WeaponText           = GameObject.Find( "WeaponText"           ).GetComponent<Text>();
		DestructionWarehouse = GameObject.Find( "DestructionWarehouse" );

		lastZoomSliderValue     = ZoomSlider.value;
		lastRotationSliderValue = RotationSlider.value;

		// Assign event handlers for the controller interface
		MenuButton.onClick.AddListener( exitToMenu );
		PauseButton.onClick.AddListener( UniversalLevel.togglePauseState );
		NextWeaponButton.onClick.AddListener( nextWeapon );
		PreviousWeaponButton.onClick.AddListener( previousWeapon );
		ZeroGravityToggle.onValueChanged.AddListener( delegate{ UniversalLevel.toggleGravity(); } );
		ZoomSlider.onValueChanged.AddListener( zoomCamera );
		RotationSlider.onValueChanged.AddListener( rotateCamera );
		ResetButton.onClick.AddListener( UniversalLevel.resetLevel );

		StateMachine.selectedWeaponName = weapons[ StateMachine.selectedWeaponIndex ];
		WeaponText.text                 = StateMachine.selectedWeaponName;

		// Initialize and fill object pool
		ObjectPool = new Dictionary<string, Queue<GameObject>>();

		int objectPoolCount = ObjectsForPooling.Length;

		for( int nameIndex = 0; nameIndex < objectPoolCount; nameIndex++ ){

			Queue<GameObject> poolQueue = new Queue<GameObject>();

			for( int poolIndex = 0; poolIndex < poolSize; poolIndex++ ){

				// Initialize, set tranform properties, and set to inactive
				GameObject poolObject         = Instantiate( ObjectsForPooling[nameIndex] );
				poolObject.transform.position = Vector3.zero;
				poolObject.transform.rotation = Quaternion.Euler( 0, 0, 0 );
				poolObject.SetActive( false );

				// Add object to the queue
				poolQueue.Enqueue( poolObject );

			}

			// Add queue to the object pool
			ObjectPool.Add( ObjectsForPooling[nameIndex].name, poolQueue );

		}

	}

	void Update(){

		if( Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && !UniversalLevel.isPaused ){ // Touchscreen

			if( !EventSystem.current.IsPointerOverGameObject( Input.GetTouch(0).fingerId ) ){

				storeProjectileReference(); // Store reference to the currently selected game object
				fireWeapon();               // Fire weapon at finger/mouse position

			}

		}else if( Input.GetMouseButtonDown( 0 ) && !EventSystem.current.IsPointerOverGameObject() && !UniversalLevel.isPaused ){ // Desktop

			storeProjectileReference(); // Store reference to the currently selected game object
			fireWeapon();               // Fire weapon at finger/mouse position

		}

	}

	void fireWeapon(){

		// Check for ammo
		if( StateMachine.ammoAvailable() || StateMachine.selectedGameMode == (int)StateMachine.gameMode.CASUAL ){

			// Reduce ammo count if in challenge mode
			if( StateMachine.selectedGameMode == (int)StateMachine.gameMode.CHALLENGE ){

				StateMachine.useAmmo();
				UniversalLevel.updateAmmoDisplay();

			}

			Ray fireRay = MainCamera.GetComponent<Camera>().ScreenPointToRay( Input.mousePosition );

			if( WeaponObject != null ){ // Projectile

				GameObject WeaponInstance = Instantiate( WeaponObject, fireRay.origin + ( fireRay.direction * 1.1f ), WeaponObject.transform.rotation );
				WeaponInstance.GetComponent<Rigidbody>().velocity = fireRay.direction * projectileVelocity;

			}else{ // Non-physical Weapon

				RaycastHit laserHit;

				if( Physics.Raycast( fireRay.origin, fireRay.direction, out laserHit ) ){

					if( laserHit.collider.gameObject.tag == "Structure" ){

						laserHit.collider.gameObject.SetActive( false );

					}

				}

			}

		}

	}

	public void exitToMenu(){

		SceneManager.LoadScene( "Main Menu" );

	}

	public void nextWeapon(){

		int weaponCount = weapons.Length;

		if( ( StateMachine.selectedWeaponIndex + 1 ) >= weaponCount ){

			StateMachine.selectedWeaponIndex = 0;
			StateMachine.selectedWeaponName = weapons[ StateMachine.selectedWeaponIndex ];

		}else{

			StateMachine.selectedWeaponIndex++;
			StateMachine.selectedWeaponName = weapons[ StateMachine.selectedWeaponIndex ];

		}

		WeaponText.text = StateMachine.selectedWeaponName;

		UniversalLevel.updateAmmoDisplay();

	}

	public void previousWeapon(){

		int weaponCount = weapons.Length;

		if( StateMachine.selectedWeaponIndex <= 0 ){

			StateMachine.selectedWeaponIndex = weaponCount - 1;
			StateMachine.selectedWeaponName  = weapons[ StateMachine.selectedWeaponIndex ];

		}else{

			StateMachine.selectedWeaponIndex--;
			StateMachine.selectedWeaponName = weapons[ StateMachine.selectedWeaponIndex ];

		}

		WeaponText.text = StateMachine.selectedWeaponName;

		UniversalLevel.updateAmmoDisplay();

	}

	public void removeProjectiles(){

		GameObject[] projectiles         = GameObject.FindGameObjectsWithTag( "Projectile" );
		GameObject[] expendedProjectiles = GameObject.FindGameObjectsWithTag( "ExpendedProjectile" );

		for( int index = 0; index < projectiles.Length; index++ ){

			Destroy( projectiles[index] );

		}

		for( int index = 0; index < expendedProjectiles.Length; index++ ){

			Destroy( expendedProjectiles[index] );

		}

	}

	void storeProjectileReference(){

		// Store reference to the currently selected game object
		switch( StateMachine.selectedWeaponName ){

			case "Cannonball":
				WeaponObject       = Cannonball;
				projectileVelocity = WeaponObject.GetComponent<Cannonball>().getVelocity();
				break;
			case "Wrecking Ball":
				WeaponObject       = WreckingBall;
				projectileVelocity = WeaponObject.GetComponent<WreckingBall>().getVelocity();
				break;
			case "Grenade":
				WeaponObject       = Grenade;
				projectileVelocity = WeaponObject.GetComponent<Grenade>().getVelocity();
				break;
			case "TNT":
				WeaponObject       = TNT;
				projectileVelocity = WeaponObject.GetComponent<TNT>().getVelocity();
				break;
			case "Mega Bomb":
				WeaponObject       = MegaBomb;
				projectileVelocity = WeaponObject.GetComponent<MegaBomb>().getVelocity();
				break;
			case "Impact Grenade":
				WeaponObject       = ImpactGrenade;
				projectileVelocity = WeaponObject.GetComponent<ImpactGrenade>().getVelocity();
				break;
			case "Impact Bomb":
				WeaponObject       = ImpactBomb;
				projectileVelocity = WeaponObject.GetComponent<ImpactBomb>().getVelocity();
				break;
			case "Imploder":
				WeaponObject       = Imploder;
				projectileVelocity = WeaponObject.GetComponent<Imploder>().getVelocity();
				break;
			case "Vaporizer":
				WeaponObject       = Vaporizer;
				projectileVelocity = WeaponObject.GetComponent<Vaporizer>().getVelocity();
				break;
			default:
				WeaponObject       = Cannonball;
				projectileVelocity = WeaponObject.GetComponent<Cannonball>().getVelocity();
				break;

		}

	}

	public GameObject getPoolObject( string prefabName ){

		// Pull game object from the queue
		GameObject poolObject = ObjectPool[ prefabName ].Dequeue();

		// Add the game object back into the queue for later reference
		ObjectPool[ prefabName ].Enqueue( poolObject );

		return poolObject;

	}

	public void setButtonPauseStyle(){

		ColorBlock colors = PauseButton.colors;

		colors.normalColor      = pauseColor;
		colors.highlightedColor = pauseColor;

		PauseButton.colors   = colors;
		PauseButtonText.text = pauseString;

	}

	public void setButtonPlayStyle(){

		ColorBlock colors = PauseButton.colors;

		colors.normalColor      = playColor;
		colors.highlightedColor = playColor;

		PauseButton.colors   = colors;
		PauseButtonText.text = playString;

	}

	private void zoomCamera( float sliderValue ){

		float distanceChange = sliderValue - lastZoomSliderValue;

		MainCamera.transform.position = Vector3.MoveTowards( MainCamera.transform.position, DestructionWarehouse.transform.position, distanceChange );
		lastZoomSliderValue           = sliderValue;

	}

	private void rotateCamera( float sliderValue ){

		float angleChange = sliderValue - lastRotationSliderValue;

		MainCamera.transform.RotateAround( DestructionWarehouse.transform.position, Vector3.up, angleChange );

		lastRotationSliderValue = sliderValue;

	}

}