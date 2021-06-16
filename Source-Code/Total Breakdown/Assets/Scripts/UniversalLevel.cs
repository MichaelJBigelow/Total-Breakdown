using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UniversalLevel : MonoBehaviour{

	private Controller   Controller; // Reference the Fire script aka class
	private StateMachine StateMachine; // Reference the StateMachine class
	private Toggle       ZeroGravityToggle;
	private Text         ScoreValue;
	private Text         AmmoValue;

	private Vector3 standardGravity = new Vector3( 0, -9.8f, 0 ); // Values based on Debug.Log( Physics.gravity );
	private Vector3 zeroGravity     = new Vector3( 0, 0, 0 );
	public bool isPaused            = false;
	private float originalTimeScale;
	//private bool gravityEnabled = true;
	private Vector3[]    InitialPositions;
	private Quaternion[] InitialRotations;
	private GameObject[] StructureObjects;
	private float        startingPositionX;
	private float        startingPositionY;
	private float        startingPositionZ;
	private const bool   INITIALIZE = true;
	private const bool   DONT_INITIALIZE = false;

	void Awake() {

		originalTimeScale = Time.timeScale; // Set default time

	}

	void Start(){

		// Slow motion debug
		//Time.timeScale = 0.2f;
		//Time.fixedDeltaTime = 0.02f * Time.timeScale; // Slow down fixed updates to match new time scale. .02 is recommended by Unity

		ZeroGravityToggle = GameObject.Find( "ZeroGravityToggle" ).GetComponent<Toggle>();

		// Set up level based on game mode
		Controller   = GameObject.Find( "Controller" ).GetComponent<Controller>();
		StateMachine = GameObject.Find( "StateMachine" ).GetComponent<StateMachine>();
		ScoreValue   = GameObject.Find( "ScoreValue" ).GetComponent<Text>();
		AmmoValue    = GameObject.Find( "AmmoValue" ).GetComponent<Text>();

		if( StateMachine.selectedGameMode == (int)StateMachine.gameMode.CHALLENGE ){

			StateMachine.resetAmmo();
			updateAmmoDisplay();

		}else{

			// Disable score display
			GameObject.Find( "ScoreCanvas" ).SetActive( false );

			// Disable ammo display
			GameObject.Find( "AmmoCanvas" ).SetActive( false );

			// Disable score pads
			GameObject[] ScorePads = GameObject.FindGameObjectsWithTag( "ScorePad" );

			foreach( GameObject ScorePad in ScorePads ){

				ScorePad.SetActive( false );

			}

		}

		// Store structure object references
		StructureObjects = GameObject.FindGameObjectsWithTag( "Structure" );
		int objectCount  = StructureObjects.Length;

		//Debug.Log( "Structure Blocks: " + objectCount );
		//Debug.Log( "Max Score: " + ( objectCount * 5 ) );

		// Store initial structure object positions and rotations
		InitialPositions = new Vector3[objectCount];
		InitialRotations = new Quaternion[objectCount];

		for( int i = 0; i < objectCount; i++ ){

			InitialPositions[i] = StructureObjects[i].transform.position; // Store structure positions
			InitialRotations[i] = StructureObjects[i].transform.rotation; // Store structure rotations
			StructureObjects[i].GetComponent<Rigidbody>().Sleep();	// Force objects to sleep after initial scene load

		}

		// Set score to 0
		ScoreValue.text = "0";

		// Force initial gravity settings
		toggleGravity();

	}

	void Update(){

		if( Input.GetButtonDown( "Cancel" ) ){

			SceneManager.LoadScene( "Main Menu" );

		}

	}

	public void updateAmmoDisplay(){

		if( StateMachine.selectedGameMode == (int)StateMachine.gameMode.CHALLENGE ){

			AmmoValue.text = StateMachine.getCurrentWeaponAmmoCount().ToString();

		}

	}

	public void togglePauseState(){

		if( !isPaused ){ // Pause game

			isPaused = true;
			Time.timeScale = 0;

			Controller.setButtonPlayStyle();

		}else{ // Resume game

			isPaused = false;
			Time.timeScale = originalTimeScale;

			Controller.setButtonPauseStyle();

		}

	}

	public void toggleGravity(){

		if( ZeroGravityToggle != null ){

			bool objectsAlreadyMoving = false;

			// Determine if any of the blocks in the scene are moving
			for( int index = 0; index < StructureObjects.Length; index++ ){

				if( !StructureObjects[index].GetComponent<Rigidbody>().IsSleeping() ){

					objectsAlreadyMoving = true;
					break;

				}

			}

			if( ZeroGravityToggle.isOn ){

				//gravityEnabled = false;

				if( objectsAlreadyMoving ){ // If objects in the scene are already moving just disable gravity

					Physics.gravity = zeroGravity;

				}else{ // If objects in the scene are not moving, force all objects to sleep after changing gravity. Blocks will float up otherwise
					
					Physics.gravity = zeroGravity;

					for( int index = 0; index < StructureObjects.Length; index++ ){

						StructureObjects[index].GetComponent<Rigidbody>().Sleep();

					}

				}

			}else{

				//gravityEnabled = true;

				if( objectsAlreadyMoving ){ // If objects in the scene are already moving just enable gravity

					Physics.gravity = standardGravity;

				}else{ // If objects in the scene are not moving, force all objects to sleep after changing gravity. Blocks will float up otherwise

					Physics.gravity = standardGravity;

					for( int index = 0; index < StructureObjects.Length; index++ ){

						StructureObjects[index].GetComponent<Rigidbody>().Sleep();

					}

				}

			}

		}

	}

	public void resetLevel(){

		Controller.removeProjectiles();

		StateMachine.resetAmmo();
		updateAmmoDisplay();

		for( int index = 0; index < StructureObjects.Length; index++ ){

			StructureObjects[index].SetActive( false ); // disable blocks to prevent further movement during the rebuild process
			StructureObjects[index].GetComponent<Rigidbody>().velocity = Vector3.zero;
			StructureObjects[index].GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
			StructureObjects[index].transform.position = InitialPositions[index]; // Reset structure positions
			StructureObjects[index].transform.rotation = InitialRotations[index]; // Reset structure rotations

		}

		for( int index = 0; index < StructureObjects.Length; index++ ){

			StructureObjects[index].SetActive( true ); // Re-enable "destroyed" blocks
			StructureObjects[index].GetComponent<Rigidbody>().Sleep(); // MUST BE CALLED AFTER RE-ENABLING THE OBJECTS!

		}

		// Reset score
		ScoreValue.text = "0";

	}

}