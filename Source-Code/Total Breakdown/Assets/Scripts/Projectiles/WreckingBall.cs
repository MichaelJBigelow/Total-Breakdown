using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WreckingBall : MonoBehaviour {

	private float velocity = 45;

	void Start(){

		AudioSource WeaponAudio = gameObject.GetComponent<AudioSource>();
		WeaponAudio.PlayOneShot( WeaponAudio.clip, 1.0f );
		Destroy( gameObject, 20.0f ); // Destroy cannonballs after x amount of time. Use if there is a performance issue.

	}

	void Update(){

	}

	public float getVelocity(){

		return velocity;

	}

}
