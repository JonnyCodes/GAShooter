﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Agent : MonoBehaviour {

	public GameObject bulletPrefab;
	public bool randInit = true;
	[Range(0.1f, 1.5f)]
	public float maxSteering; // This changes the distance between the "wheels" (Back 2 verts); Bigger = slower turning.
	
	[Range(1.0f, 10.0f)]
	public float maxHealth; // This changes the agents scale and maxVelocity; more health = bigger & slower

	[Range(1.0f, 10.0f)]
	public float visionRadius; // This is the radius of perception circle

	[Range(1.0f, 10.0f)]
	public float rateOfFire; // How quickly an agent can shoot

	[Range(0.2f, 1.0f)]
	public float bulletSize; // The size of the bullets

	[System.Serializable]
	public class PickupForceData {
		
		[System.Serializable]
		public class PickupHealthForce {
			public AnimationCurve pickupHealthForceCurve;

			[Range(0.0f, 1.0f)]
			public float pickupHealthForceKey1;
			[Range(0.0f, 1.0f)]
			public float pickupHealthForceKey2;
			[Range(0.0f, 1.0f)]
			public float pickupHealthForceKey3;
			[Range(0.0f, 1.0f)]
			public float pickupHealthForceKey4;
		}
		public PickupHealthForce pickupHealthForce;

		[System.Serializable]
		public class PickupDistanceForce {
			public AnimationCurve pickupDistanceForceCurve;

			[Range(0.0f, 1.0f)]
			public float pickupDistanceForceKey1;
			[Range(0.0f, 1.0f)]
			public float pickupDistanceForceKey2;
			[Range(0.0f, 1.0f)]
			public float pickupDistanceForceKey3;
			[Range(0.0f, 1.0f)]
			public float pickupDistanceForceKey4;
		}
		public PickupDistanceForce pickupDistanceForce;

		[System.Serializable]
		public class PickupDirectionForce {
			public AnimationCurve pickupDirectionForceCurve;

			[Range(0.0f, 1.0f)]
			public float pickupDirectionForceKey1;
			[Range(0.0f, 1.0f)]
			public float pickupDirectionForceKey2;
			[Range(0.0f, 1.0f)]
			public float pickupDirectionForceKey3;
			[Range(0.0f, 1.0f)]
			public float pickupDirectionForceKey4;
		}
		public PickupDirectionForce pickupDirectionForce;
	}
	public PickupForceData pickupForceData;
	
	[System.Serializable]
	public class AvoidForceData {

		[System.Serializable]
		public class AvoidHealthForce {
			public AnimationCurve avoidHealthForceCurve;

			[Range(0.0f, 1.0f)]
			public float avoidHealthForceKey1;
			[Range(0.0f, 1.0f)]
			public float avoidHealthForceKey2;
			[Range(0.0f, 1.0f)]
			public float avoidHealthForceKey3;
			[Range(0.0f, 1.0f)]
			public float avoidHealthForceKey4;
		}
		public AvoidHealthForce avoidHealthForce;

		[System.Serializable]
		public class AvoidDistanceForce {
			public AnimationCurve avoidDistanceForceCurve;

			[Range(0.0f, 1.0f)]
			public float avoidDistanceForceKey1;
			[Range(0.0f, 1.0f)]
			public float avoidDistanceForceKey2;
			[Range(0.0f, 1.0f)]
			public float avoidDistanceForceKey3;
			[Range(0.0f, 1.0f)]
			public float avoidDistanceForceKey4;
		}
		public AvoidDistanceForce avoidDistanceForce;

		[System.Serializable]
		public class AvoidDirectionForce {
			public AnimationCurve avoidDirectionForceCurve;

			[Range(0.0f, 1.0f)]
			public float avoidDirectionForceKey1;
			[Range(0.0f, 1.0f)]
			public float avoidDirectionForceKey2;
			[Range(0.0f, 1.0f)]
			public float avoidDirectionForceKey3;
			[Range(0.0f, 1.0f)]
			public float avoidDirectionForceKey4;
		}
		public AvoidDirectionForce avoidDirectionForce;
	}
	public AvoidForceData avoidForceData;

	[System.Serializable]
	public class AgentForceData {

		[System.Serializable]
		public class AgentHealthForce {
			public AnimationCurve agentHealthForceCurve;

			[Range(0.0f, 1.0f)]
			public float agentHealthForceKey1;
			[Range(0.0f, 1.0f)]
			public float agentHealthForceKey2;
			[Range(0.0f, 1.0f)]
			public float agentHealthForceKey3;
			[Range(0.0f, 1.0f)]
			public float agentHealthForceKey4;
		}
		public AgentHealthForce agentHealthForce;

		[System.Serializable]
		public class AgentDistanceForce {
			public AnimationCurve agentDistanceForceCurve;

			[Range(0.0f, 1.0f)]
			public float agentDistanceForceKey1;
			[Range(0.0f, 1.0f)]
			public float agentDistanceForceKey2;
			[Range(0.0f, 1.0f)]
			public float agentDistanceForceKey3;
			[Range(0.0f, 1.0f)]
			public float agentDistanceForceKey4;
		}
		public AgentDistanceForce agentDistanceForce;

		[System.Serializable]
		public class AgentDirectionForce {
			public AnimationCurve agentDirectionForceCurve;

			[Range(0.0f, 1.0f)]
			public float agentDirectionForceKey1;
			[Range(0.0f, 1.0f)]
			public float agentDirectionForceKey2;
			[Range(0.0f, 1.0f)]
			public float agentDirectionForceKey3;
			[Range(0.0f, 1.0f)]
			public float agentDirectionForceKey4;
		}
		public AgentDirectionForce agentDirectionForce;
	}
	public AgentForceData agentForceData;
	public int points;

	private CircleCollider2D collider;
	private Vector3 velocity;
	private float maxVelocity;
	private float health;
	private float shootTimer;

	private Vector3 minScreenBounds;
	private Vector3 maxScreenBounds;

	// Use this for initialization
	void Start () {

		if (randInit) {

			maxSteering = Random.Range(0.1f, 1.5f);
			maxHealth = Random.Range(1.0f, 10.0f);
			visionRadius = Random.Range(1.0f, 10.0f);
			rateOfFire = Random.Range(1.0f, 10.0f);
			bulletSize = Random.Range(0.2f, 1.0f);

			// TODO: This can be done with reflection!
			pickupForceData.pickupHealthForce.pickupHealthForceKey1 = Random.Range(0.0f, 1.0f);
			pickupForceData.pickupHealthForce.pickupHealthForceKey2 = Random.Range(0.0f, 1.0f);
			pickupForceData.pickupHealthForce.pickupHealthForceKey3 = Random.Range(0.0f, 1.0f);
			pickupForceData.pickupHealthForce.pickupHealthForceKey4 = Random.Range(0.0f, 1.0f);
			pickupForceData.pickupDistanceForce.pickupDistanceForceKey1 = Random.Range(0.0f, 1.0f);
			pickupForceData.pickupDistanceForce.pickupDistanceForceKey2 = Random.Range(0.0f, 1.0f);
			pickupForceData.pickupDistanceForce.pickupDistanceForceKey3 = Random.Range(0.0f, 1.0f);
			pickupForceData.pickupDistanceForce.pickupDistanceForceKey4 = Random.Range(0.0f, 1.0f);
			pickupForceData.pickupDirectionForce.pickupDirectionForceKey1 = Random.Range(0.0f, 1.0f);
			pickupForceData.pickupDirectionForce.pickupDirectionForceKey2 = Random.Range(0.0f, 1.0f);
			pickupForceData.pickupDirectionForce.pickupDirectionForceKey3 = Random.Range(0.0f, 1.0f);
			pickupForceData.pickupDirectionForce.pickupDirectionForceKey4 = Random.Range(0.0f, 1.0f);

			avoidForceData.avoidHealthForce.avoidHealthForceKey1 = Random.Range(0.0f, 1.0f);
			avoidForceData.avoidHealthForce.avoidHealthForceKey2 = Random.Range(0.0f, 1.0f);
			avoidForceData.avoidHealthForce.avoidHealthForceKey3 = Random.Range(0.0f, 1.0f);
			avoidForceData.avoidHealthForce.avoidHealthForceKey4 = Random.Range(0.0f, 1.0f);
			avoidForceData.avoidDistanceForce.avoidDistanceForceKey1 = Random.Range(0.0f, 1.0f);
			avoidForceData.avoidDistanceForce.avoidDistanceForceKey2 = Random.Range(0.0f, 1.0f);
			avoidForceData.avoidDistanceForce.avoidDistanceForceKey3 = Random.Range(0.0f, 1.0f);
			avoidForceData.avoidDistanceForce.avoidDistanceForceKey4 = Random.Range(0.0f, 1.0f);
			avoidForceData.avoidDirectionForce.avoidDirectionForceKey1 = Random.Range(0.0f, 1.0f);
			avoidForceData.avoidDirectionForce.avoidDirectionForceKey2 = Random.Range(0.0f, 1.0f);
			avoidForceData.avoidDirectionForce.avoidDirectionForceKey3 = Random.Range(0.0f, 1.0f);
			avoidForceData.avoidDirectionForce.avoidDirectionForceKey4 = Random.Range(0.0f, 1.0f);

			agentForceData.agentHealthForce.agentHealthForceKey1 = Random.Range(0.0f, 1.0f);
			agentForceData.agentHealthForce.agentHealthForceKey2 = Random.Range(0.0f, 1.0f);
			agentForceData.agentHealthForce.agentHealthForceKey3 = Random.Range(0.0f, 1.0f);
			agentForceData.agentHealthForce.agentHealthForceKey4 = Random.Range(0.0f, 1.0f);
			agentForceData.agentDistanceForce.agentDistanceForceKey1 = Random.Range(0.0f, 1.0f);
			agentForceData.agentDistanceForce.agentDistanceForceKey2 = Random.Range(0.0f, 1.0f);
			agentForceData.agentDistanceForce.agentDistanceForceKey3 = Random.Range(0.0f, 1.0f);
			agentForceData.agentDistanceForce.agentDistanceForceKey4 = Random.Range(0.0f, 1.0f);
			agentForceData.agentDirectionForce.agentDirectionForceKey1 = Random.Range(0.0f, 1.0f);
			agentForceData.agentDirectionForce.agentDirectionForceKey2 = Random.Range(0.0f, 1.0f);
			agentForceData.agentDirectionForce.agentDirectionForceKey3 = Random.Range(0.0f, 1.0f);
			agentForceData.agentDirectionForce.agentDirectionForceKey4 = Random.Range(0.0f, 1.0f);
		}


		float randStartVelocity = Random.value;
		velocity = randStartVelocity < 0.25f ? Vector3.up : randStartVelocity < 0.5 ? Vector3.left : randStartVelocity < 0.75 ? Vector3.down : Vector3.right;
		maxVelocity = (11.0f - maxHealth) / 1.25f; // TODO: Don't like the magic numbers
		health = maxHealth;
		shootTimer = 0.0f;

		// vvvvv Create agent polygon vvvvv
		MeshFilter meshFilter = GetComponent<MeshFilter>();
		Mesh agentMesh = new Mesh();
		meshFilter.mesh = agentMesh;

		Vector3[] verts = new Vector3[4];
		verts[0] = new Vector3(-maxSteering - 0.2f, 0.15f, 0.0f);
		verts[1] = new Vector3(0.0f, (-maxHealth / 10.0f) - 0.35f, 0.0f);
		verts[2] = new Vector3(0.0f, 0.0f, 0.0f); // CENTER OF THE POLYGON
		verts[3] = new Vector3(maxSteering + 0.2f, 0.15f, 0.0f);
		agentMesh.vertices = verts;

		int[] tris = new int[6];
		tris[0] = 0;
		tris[1] = 2;
		tris[2] = 1;

		tris[3] = 2;
		tris[4] = 3;
		tris[5] = 1;
		agentMesh.triangles = tris;

		Vector3[] normals = new Vector3[4];
		normals[0] = -Vector3.forward;
		normals[1] = -Vector3.forward;
		normals[2] = -Vector3.forward;
		normals[3] = -Vector3.forward;
		agentMesh.normals = normals;

		Vector2[] uvs = new Vector2[4];
		uvs[0] = new Vector2(0.0f, 0.0f);
		uvs[1] = new Vector2(1.0f, 0.0f);
		uvs[2] = new Vector2(0.0f, 1.0f);
		uvs[3] = new Vector2(1.0f, 1.0f);
		agentMesh.uv = uvs;
		// ^^^^^^^^^^^^^^^^^^^^^^

		// vvvvv Setup force curves vvvvv

		float[] pickuphealthforces = new float[] {pickupForceData.pickupHealthForce.pickupHealthForceKey1, pickupForceData.pickupHealthForce.pickupHealthForceKey2, pickupForceData.pickupHealthForce.pickupHealthForceKey3, pickupForceData.pickupHealthForce.pickupHealthForceKey4};
		pickupForceData.pickupHealthForce.pickupHealthForceCurve = setupForceCurves(pickuphealthforces);
		float[] pickudistanceforces = new float[] {pickupForceData.pickupDistanceForce.pickupDistanceForceKey1, pickupForceData.pickupDistanceForce.pickupDistanceForceKey2, pickupForceData.pickupDistanceForce.pickupDistanceForceKey3, pickupForceData.pickupDistanceForce.pickupDistanceForceKey4};
		pickupForceData.pickupDistanceForce.pickupDistanceForceCurve = setupForceCurves(pickudistanceforces);
		float[] pickudirectionforces = new float[] {pickupForceData.pickupDirectionForce.pickupDirectionForceKey1, pickupForceData.pickupDirectionForce.pickupDirectionForceKey2, pickupForceData.pickupDirectionForce.pickupDirectionForceKey3, pickupForceData.pickupDirectionForce.pickupDirectionForceKey4};
		pickupForceData.pickupDirectionForce.pickupDirectionForceCurve = setupForceCurves(pickudirectionforces);

		float[] avoidhealthforces = new float[] {avoidForceData.avoidHealthForce.avoidHealthForceKey1, avoidForceData.avoidHealthForce.avoidHealthForceKey2, avoidForceData.avoidHealthForce.avoidHealthForceKey3, avoidForceData.avoidHealthForce.avoidHealthForceKey4};
		avoidForceData.avoidHealthForce.avoidHealthForceCurve = setupForceCurves(avoidhealthforces);
		float[] avoiddistanceforces = new float[] {avoidForceData.avoidDistanceForce.avoidDistanceForceKey1, avoidForceData.avoidDistanceForce.avoidDistanceForceKey2, avoidForceData.avoidDistanceForce.avoidDistanceForceKey3, avoidForceData.avoidDistanceForce.avoidDistanceForceKey4};
		avoidForceData.avoidDistanceForce.avoidDistanceForceCurve = setupForceCurves(avoiddistanceforces);
		float[] avoiddirectionforces = new float[] {avoidForceData.avoidDirectionForce.avoidDirectionForceKey1, avoidForceData.avoidDirectionForce.avoidDirectionForceKey2, avoidForceData.avoidDirectionForce.avoidDirectionForceKey3, avoidForceData.avoidDirectionForce.avoidDirectionForceKey4};
		avoidForceData.avoidDirectionForce.avoidDirectionForceCurve = setupForceCurves(avoiddirectionforces);

		float[] agenthealthforces = new float[] {agentForceData.agentHealthForce.agentHealthForceKey1, agentForceData.agentHealthForce.agentHealthForceKey2, agentForceData.agentHealthForce.agentHealthForceKey3, agentForceData.agentHealthForce.agentHealthForceKey4};
		agentForceData.agentHealthForce.agentHealthForceCurve = setupForceCurves(agenthealthforces);
		float[] agentdistanceforces = new float[] {agentForceData.agentDistanceForce.agentDistanceForceKey1, agentForceData.agentDistanceForce.agentDistanceForceKey2, agentForceData.agentDistanceForce.agentDistanceForceKey3, agentForceData.agentDistanceForce.agentDistanceForceKey4};
		agentForceData.agentDistanceForce.agentDistanceForceCurve = setupForceCurves(agentdistanceforces);
		float[] agentdirectionforces = new float[] {agentForceData.agentDirectionForce.agentDirectionForceKey1, agentForceData.agentDirectionForce.agentDirectionForceKey2, agentForceData.agentDirectionForce.agentDirectionForceKey3, agentForceData.agentDirectionForce.agentDirectionForceKey4};
		agentForceData.agentDirectionForce.agentDirectionForceCurve = setupForceCurves(agentdirectionforces);
		// ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^

		points = 0;

		minScreenBounds = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0));
		maxScreenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));

		collider = GetComponent<CircleCollider2D>();
		collider.radius = visionRadius;
	}

	AnimationCurve setupForceCurves(float[] keyframeForces) {
		AnimationCurve curve = new AnimationCurve();

		for (int i = 0; i < keyframeForces.Length; i++) {
			curve.AddKey(i * (1.0f / keyframeForces.Length), keyframeForces[i]);
		}

		return curve;
	}

	// Update is called once per frame
	void Update () {

		List<Collider2D> targetsInRange = Physics2D.OverlapCircleAll(transform.position, visionRadius).ToList();

		Vector3 steeringForce = Vector3.zero;

		if (transform.position.x < minScreenBounds.x || transform.position.x > maxScreenBounds.x || transform.position.y < minScreenBounds.y || transform.position.y > maxScreenBounds.y) {
			// Head towards the center of the screen if outside the screen bounds
			Vector3 desiredVelocity = Vector3.Normalize(Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 0)) - transform.position) * maxVelocity;
			steeringForce = desiredVelocity - velocity;
			steeringForce.z = 0.0f;
		} else {

			// Own collider is always counted
			if (targetsInRange.Count > 1) {
				foreach (Collider2D targetCollider in targetsInRange) {

					if (targetCollider != collider) {

						GameObject target = targetCollider.gameObject;

						float healthPercentage = health / maxHealth; // Between 0 and 1
						Vector3 vectorToTarget = target.transform.position - transform.position;
						float distancePercentage = vectorToTarget.magnitude / visionRadius; // Between 0 and 1
						float direction =  Mathf.Abs(((((Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg) + 90) - ((Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg) + 90))) % 360) / 360; // Between 0 and 1

						switch (target.tag) {
							case "avoid":
								float avoidForce = avoidForceData.avoidHealthForce.avoidHealthForceCurve.Evaluate(healthPercentage); // return betweens 0 and 1
								avoidForce += avoidForceData.avoidDistanceForce.avoidDistanceForceCurve.Evaluate(distancePercentage); // return betweens 0 and 1
								avoidForce += avoidForceData.avoidDirectionForce.avoidDirectionForceCurve.Evaluate(direction); // return betweens 0 and 1
								avoidForce = avoidForce / 3; // get average
								avoidForce = avoidForce * 2 - 1; // Clamp between -1 & 1

								steeringForce += Seek(target) * avoidForce;

								if (vectorToTarget.magnitude <= 2) {
									target.GetComponent<RandomPoint>().ChangePosition();
									points -= 2;
								}
								break;

							case "attract":
								float attractForce = pickupForceData.pickupHealthForce.pickupHealthForceCurve.Evaluate(healthPercentage); // returns between 0 and 1
								attractForce += pickupForceData.pickupDistanceForce.pickupDistanceForceCurve.Evaluate(distancePercentage); // return betweens 0 and 1
								attractForce += pickupForceData.pickupDirectionForce.pickupDirectionForceCurve.Evaluate(direction); // return betweens 0 and 1
								attractForce = attractForce / 3; // get average
								attractForce = attractForce * 2 - 1; // Clamp between -1 & 1

								steeringForce += Seek(target) * attractForce;

								if (vectorToTarget.magnitude <= 2) {
									target.GetComponent<RandomPoint>().ChangePosition();
									points++;
								}
								break;

							case "agent":
								float agentForce = agentForceData.agentHealthForce.agentHealthForceCurve.Evaluate(healthPercentage); // returns between 0 and 1
								agentForce += agentForceData.agentDistanceForce.agentDistanceForceCurve.Evaluate(distancePercentage); // return betweens 0 and 1
								agentForce += agentForceData.agentDirectionForce.agentDirectionForceCurve.Evaluate(direction); // return betweens 0 and 1
								agentForce = agentForce / 3; // get average
								agentForce = agentForce * 2 - 1; // Clamp between -1 & 1

								steeringForce += Seek(target) * agentForce;
								break;
						}
					}

				}
			} else {
				steeringForce += RandomSeek();
			}
		}

		// shootTimer += Time.deltaTime;
		// if (shootTimer >= rateOfFire) {
		// 	shootTimer = 0;

		// 	// TODO: Need to check if agent should shoot, not just if the agent can shoot!

		// 	// SHOOT
		// 	GameObject bullet = Instantiate(bulletPrefab, transform.position + (Vector3.Normalize(velocity) * 1.5f), transform.rotation);
		// 	bullet.transform.localScale = Vector3.one * bulletSize;
		// 	bullet.GetComponent<BulletScript>().velocity = Vector3.ClampMagnitude(velocity, maxVelocity * 3);

		// 	// Decrease velocity by bullet size!
		// 	velocity -= velocity / (2.0f - bulletSize);
		// 	velocity.z = 0.0f;
		// 	// TODO: Agents should be able to travel backwards if shooting causes negative velocity??
		// }

		if (steeringForce.sqrMagnitude > maxSteering * maxSteering) {
			steeringForce.Normalize();
			steeringForce *= maxSteering;
		}

		velocity = Vector3.ClampMagnitude(velocity + steeringForce, maxVelocity);

		transform.position += (velocity * Time.deltaTime);

		float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
		Quaternion q = Quaternion.AngleAxis(angle + 90, Vector3.forward);
		transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * 10);
	}

	private Vector3 Seek(GameObject targetObj) {
		Vector3 desiredVelocity = Vector3.Normalize(targetObj.transform.position - transform.position) * maxVelocity;
		desiredVelocity.z = 0.0f;

		Vector3 steering = desiredVelocity - velocity;
		steering.z = 0.0f;

		return steering;
	}

	private Vector3 RandomSeek() {

		Vector3 desiredVelocity = Vector3.zero;

		desiredVelocity = Vector3.Normalize((transform.position + Vector3.ClampMagnitude(velocity * 3.0f, maxVelocity)) - transform.position) * maxVelocity;
		desiredVelocity = Quaternion.AngleAxis(Random.Range(-10.0f, 10.0f), Vector3.forward) * desiredVelocity;

		desiredVelocity.z = 0.0f;

		Vector3 steering = desiredVelocity - velocity;
		steering.z = 0.0f;

		return steering;
	}
}
