using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour{

	public GameObject LoadingScreen;
	public Slider     LoadingSlider;
	public Dropdown   SceneDropdown;
	public string     selectedScene = "";

	void Start(){

		setScene(); // Set selectedScene value

	}

	void Update(){

		if( Input.GetButtonDown( "Cancel" ) ){

			exitGame();

		}

	}

	public void setScene(){

		selectedScene = SceneDropdown.options[SceneDropdown.value].text;

	}

	public void loadScene(){

		LoadingScreen.SetActive( true );
		StartCoroutine( LoadAsync( selectedScene ) );

	}

	IEnumerator LoadAsync( string selectedLevel ){

		AsyncOperation asyncLoadOperation = SceneManager.LoadSceneAsync( selectedLevel );

		while( !asyncLoadOperation.isDone ){

			// Unity returns a value from 0f to 0.9f when loading assets. Values 0.9f to 1.0f is where previous objects get deleted
			float loadingProgress = Mathf.Clamp01( asyncLoadOperation.progress / 0.9f ); // Make loading values go from 0 to 1
			LoadingSlider.value = loadingProgress;

			yield return null;

		}

	}

	public void openWebsite( string URL ){

		Application.OpenURL( URL );

	}

	public void exitGame(){

		Application.Quit();

	}

}