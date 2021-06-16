using System;
using UnityEngine;
using UnityEngine.UI;

public class ScorePad : MonoBehaviour{

	private static Text ScoreValue;

	[SerializeField]
	private int pointValue;

	private void Start() {

		ScoreValue = GameObject.Find( "ScoreValue" ).GetComponent<Text>();

	}

	private void OnTriggerEnter( Collider collidedObject ){

		if( collidedObject.gameObject.tag == "Structure" ){

			int score = 0;

			if( Int32.TryParse( ScoreValue.text, out score ) ){

				score += pointValue;
				ScoreValue.text = score.ToString();

			}else{

				ScoreValue.text = "0";

		}

			collidedObject.gameObject.SetActive( false ); // Hide structure object

		}

	}

}