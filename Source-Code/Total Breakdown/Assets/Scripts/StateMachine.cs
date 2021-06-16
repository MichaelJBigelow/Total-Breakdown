using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StateMachine : MonoBehaviour{

	private static bool created = false;

	public Controller  Controller; // Reference the Controller class

	public int        selectedWeaponIndex = 0;
	public string     selectedWeaponName  = "";
	public static int selectedGameMode;

	public static Dictionary<string, int> Inventory = new Dictionary<string, int>(); // Name of weapon, ammo

	public enum gameMode { CASUAL, CHALLENGE };

	private void Awake(){

		// Set default mode if the class has never been instantiated
		if( !created ){

			selectedGameMode = (int)gameMode.CASUAL; // Default to casual game mode during game launch
			created = true;

			//DontDestroyOnLoad( this.gameObject );

		}

	}

	void Start(){

		if( SceneManager.GetActiveScene().name == "Main Menu" ){

			GameObject CasualModeToggle	= GameObject.Find( "CasualModeToggle" );
			GameObject ChallengeModeToggle = GameObject.Find( "ChallengeModeToggle" );

			if( selectedGameMode == (int)gameMode.CASUAL ){

				CasualModeToggle.GetComponent<Toggle>().isOn	= true;
				ChallengeModeToggle.GetComponent<Toggle>().isOn = false;

			}else{

				CasualModeToggle.GetComponent<Toggle>().isOn	= false;
				ChallengeModeToggle.GetComponent<Toggle>().isOn = true;

			}

		}

	}

	public void setCasualMode(){

		selectedGameMode = (int)gameMode.CASUAL;

	}

	public void setChallengeMode(){

		selectedGameMode = (int)gameMode.CHALLENGE;

	}

	public int getCurrentWeaponAmmoCount(){

		if( Inventory.ContainsKey( selectedWeaponName ) ){

			return Inventory[selectedWeaponName];

		}else{

			return 0;

		}

	}

	public bool ammoAvailable(){

		if( Inventory.ContainsKey( selectedWeaponName ) && Inventory[selectedWeaponName] > 0 ){

			return true;

		}else{

			return false;

		}

	}

	public void useAmmo(){

		if( Inventory[selectedWeaponName] > 0 ){

			Inventory[selectedWeaponName]--;

		}

	}

	public void resetAmmo(){

		if( Controller != null ){

			foreach( string weapon in Controller.weapons ){

				Inventory[weapon] = 5;
				//Debug.Log( weapon + " ammo: " + Inventory[weapon] );

			}

		}

	}

}