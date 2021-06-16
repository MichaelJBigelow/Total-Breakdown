using System;
using UnityEngine;
using UnityEngine.UI;

public class Target : MonoBehaviour{

	private Text ScoreValue;
	public AudioClip targetGenericHit;
	public AudioClip targetBullseyeHit;

	[SerializeField]
	private int pointValue;

	private void Start(){

		ScoreValue = GameObject.Find( "ScoreValue" ).GetComponent<Text>();

	}

	private void OnCollisionEnter( Collision collidedObject ){

		if( collidedObject.gameObject.tag == "Projectile" ){

			collidedObject.gameObject.tag = "ExpendedProjectile";

			int score = 0;

			if( Int32.TryParse( ScoreValue.text, out score ) ){

				score += pointValue;
				ScoreValue.text = score.ToString();

			}else{

				ScoreValue.text = "0";

			}

			// Play impact sound.
			AudioSource targetAudio = this.transform.parent.gameObject.GetComponent<AudioSource>();

			if( pointValue >= 10 ){

				targetAudio.PlayOneShot( targetBullseyeHit );

			}else{

				targetAudio.PlayOneShot( targetGenericHit );

			}

		}

	}

}