using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour {

	public Vector3 velocity;

	// Use this for initialization
	void Start () {
		Destroy(gameObject, 2.5f);
	}
	
	// Update is called once per frame
	void Update () {
		transform.position += (velocity * Time.deltaTime);

		// TODO: HANDLE COLLISION
	}
}
