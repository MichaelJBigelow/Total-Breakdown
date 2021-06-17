using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vaporizer : MonoBehaviour {

	public GameObject ExplosionParticles;
	private float  velocity   = 40;
	//private string effectName = "";
	private float  timer      = 5.0f; // Number of seconds before the projectile will be destroyed.
	private bool   exploded   = false;

	void Start(){

		AudioSource WeaponAudio = gameObject.GetComponents<AudioSource>()[0];
		WeaponAudio.PlayOneShot( WeaponAudio.clip, 1.0f );
		//effectName = ExplosionParticles.name;

	}

	void Update(){

		timer -= Time.deltaTime;

		if( timer <= 0.0f && !exploded ){

			Destroy( gameObject );

		}

	}

	void OnCollisionEnter( Collision collision ){

		if( !exploded && collision.gameObject.tag == "Structure" ){

			explode( collision );

		}else{

			Destroy( gameObject );

		}

	}

	void explode( Collision collision ){

		collision.gameObject.SetActive( false );

		MeshRenderer[] MeshRenderers = gameObject.GetComponentsInChildren<MeshRenderer>();

		foreach( MeshRenderer meshRenderer in MeshRenderers ){

			meshRenderer.enabled = false;

		}

		Destroy( gameObject, 7.0f );

		exploded = true;

	}

	public float getVelocity(){

		return velocity;

	}

}
