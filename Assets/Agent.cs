using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour {
	public GameObject target;
	private float maxVelocity;
	private float maxSteering;
	private Vector3 velocity;

	// Use this for initialization
	void Start () {
		velocity = Vector3.zero;
		maxVelocity = 4.0f;
		maxSteering = 0.1f;
	}
	
	// Update is called once per frame
	void Update () {
		Seek();
	}

	private void Seek() {
		Vector3 desiredVelocity = Vector3.Normalize(target.transform.position - transform.position) * maxVelocity;
		desiredVelocity.z = 0.0f;

		Vector3 steering = desiredVelocity - velocity;
		steering.z = 0.0f;
		if (steering.sqrMagnitude > maxSteering * maxSteering) {
			steering.Normalize();
			steering *= maxSteering;
		}

		velocity += steering;

		transform.position += (velocity * Time.deltaTime);
	}
}
