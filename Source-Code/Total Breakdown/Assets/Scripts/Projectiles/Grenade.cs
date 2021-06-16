using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour {

	public GameObject ExplosionParticles;
	private Controller Controller;
	private float  velocity        = 30;
	private string effectName      = "";
	private float  explosionRadius = 2.0f;
	private float  explosionForce  = 16000f;
	private float  explosionDelay  = 3.5f;
	private float  timer;

	private bool exploded = false;

	void Start(){

		Controller = GameObject.Find("Controller").GetComponent<Controller>();
		timer = explosionDelay;
		AudioSource WeaponAudio = gameObject.GetComponents<AudioSource>()[0];
		WeaponAudio.PlayOneShot( WeaponAudio.clip, 1.0f );
		effectName = ExplosionParticles.name;

	}

	void Update(){

		timer -= Time.deltaTime;

		if( timer <= 0.0f && !exploded ){

			explode();

		}

	}

	void explode(){

		GameObject explosion         = Controller.getPoolObject( effectName ); // Get explosion prefab from the object pool.
		explosion.transform.position = transform.position;
		explosion.transform.rotation = transform.rotation;
		explosion.SetActive( true ); // Activate object so it's visible in the world.
		// NOTE: The above object will be set to inactive by the particle system after it finishes playing.

		Collider[] BlastedObjects = Physics.OverlapSphere( transform.position, explosionRadius ); // Check for objects within the blast zone.

		explosion.GetComponent<ParticleSystem>().Play(); // Play explosion effect after blasted objects are calculated.

		AudioSource WeaponAudio = gameObject.GetComponents<AudioSource>()[1];
		WeaponAudio.PlayOneShot( WeaponAudio.clip, 1.0f );

		foreach( Collider hitObject in BlastedObjects ){

			Rigidbody objectBody = hitObject.GetComponent<Rigidbody>();

			if( objectBody != null ){

				objectBody.AddExplosionForce( explosionForce, transform.position, explosionRadius );

			}

		}

		MeshRenderer[] MeshRenderers =  gameObject.GetComponentsInChildren<MeshRenderer>();

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
