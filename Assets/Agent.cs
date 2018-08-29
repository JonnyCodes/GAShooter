using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Agent : MonoBehaviour {

	[Range(0.01f, 1.0f)]
	public float maxSteering; // This changes the distance between the "wheels" (Back 2 verts); Bigger = slower turning.
	
	[Range(1f, 10f)]
	public float maxHealth; // This changes the agents scale and maxVelocity; more health = bigger & slower

	[Range(1f, 5f)]
	public float visionRadius; // This is the radius of perception circle

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
	}
	public AvoidForceData avoidForceData;

	private CircleCollider2D visionCollider;
	private Vector3 velocity;
	private float maxVelocity;
	private float health;

	private Vector3 minScreenBounds;
	private Vector3 maxScreenBounds;

	// Use this for initialization
	void Start () {

		maxSteering = Random.Range(0.01f, 1.0f);
		maxHealth = Random.Range(0.0f, 10.0f);
		visionRadius = Random.Range(1.0f, 5.0f);

		pickupForceData.pickupHealthForce.pickupHealthForceKey1 = Random.Range(0.0f, 1.0f);
		pickupForceData.pickupHealthForce.pickupHealthForceKey2 = Random.Range(0.0f, 1.0f);
		pickupForceData.pickupHealthForce.pickupHealthForceKey3 = Random.Range(0.0f, 1.0f);
		pickupForceData.pickupHealthForce.pickupHealthForceKey4 = Random.Range(0.0f, 1.0f);
		pickupForceData.pickupDistanceForce.pickupDistanceForceKey1 = Random.Range(0.0f, 1.0f);
		pickupForceData.pickupDistanceForce.pickupDistanceForceKey2 = Random.Range(0.0f, 1.0f);
		pickupForceData.pickupDistanceForce.pickupDistanceForceKey3 = Random.Range(0.0f, 1.0f);
		pickupForceData.pickupDistanceForce.pickupDistanceForceKey4 = Random.Range(0.0f, 1.0f);

		avoidForceData.avoidHealthForce.avoidHealthForceKey1 = Random.Range(0.0f, 1.0f);
		avoidForceData.avoidHealthForce.avoidHealthForceKey2 = Random.Range(0.0f, 1.0f);
		avoidForceData.avoidHealthForce.avoidHealthForceKey3 = Random.Range(0.0f, 1.0f);
		avoidForceData.avoidHealthForce.avoidHealthForceKey4 = Random.Range(0.0f, 1.0f);
		avoidForceData.avoidDistanceForce.avoidDistanceForceKey1 = Random.Range(0.0f, 1.0f);
		avoidForceData.avoidDistanceForce.avoidDistanceForceKey2 = Random.Range(0.0f, 1.0f);
		avoidForceData.avoidDistanceForce.avoidDistanceForceKey3 = Random.Range(0.0f, 1.0f);
		avoidForceData.avoidDistanceForce.avoidDistanceForceKey4 = Random.Range(0.0f, 1.0f);

		float randStartVelocity = Random.value;
		velocity = randStartVelocity < 0.25f ? Vector3.up : randStartVelocity < 0.5 ? Vector3.left : randStartVelocity < 0.75 ? Vector3.down : Vector3.right;
		maxVelocity = (11.0f - maxHealth) / 1.25f; // TODO: Don't like the magic numbers here
		health = maxHealth;

		// vvvvv Create agent polygon vvvvv
		MeshFilter meshFilter = GetComponent<MeshFilter>();
		Mesh agentMesh = new Mesh();
		meshFilter.mesh = agentMesh;

		Vector3[] verts = new Vector3[4];
		verts[0] = new Vector3(-maxSteering, 0.15f, 0.0f);
		verts[1] = new Vector3(0.0f, (-maxHealth / 10.0f) - 0.15f, 0.0f);
		verts[2] = new Vector3(0.0f, 0.0f, 0.0f); // CENTER OF THE POLYGON
		verts[3] = new Vector3(maxSteering, 0.15f, 0.0f);
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

		visionCollider = GetComponent<CircleCollider2D>();
		visionCollider.radius = visionRadius;
		visionCollider.offset = new Vector2(0.0f, -maxHealth / 20.0f);

		// vvvvv Setup force curves vvvvv

		float[] pickuphealthforces = new float[] {pickupForceData.pickupHealthForce.pickupHealthForceKey1, pickupForceData.pickupHealthForce.pickupHealthForceKey2, pickupForceData.pickupHealthForce.pickupHealthForceKey3, pickupForceData.pickupHealthForce.pickupHealthForceKey4};
		pickupForceData.pickupHealthForce.pickupHealthForceCurve = setupForceCurves(pickuphealthforces);
		float[] pickudistanceforces = new float[] {pickupForceData.pickupDistanceForce.pickupDistanceForceKey1, pickupForceData.pickupDistanceForce.pickupDistanceForceKey2, pickupForceData.pickupDistanceForce.pickupDistanceForceKey3, pickupForceData.pickupDistanceForce.pickupDistanceForceKey4};
		pickupForceData.pickupDistanceForce.pickupDistanceForceCurve = setupForceCurves(pickudistanceforces);

		float[] avoidhealthforces = new float[] {avoidForceData.avoidHealthForce.avoidHealthForceKey1, avoidForceData.avoidHealthForce.avoidHealthForceKey2, avoidForceData.avoidHealthForce.avoidHealthForceKey3, avoidForceData.avoidHealthForce.avoidHealthForceKey4};
		avoidForceData.avoidHealthForce.avoidHealthForceCurve = setupForceCurves(avoidhealthforces);
		float[] avoiddistanceforces = new float[] {avoidForceData.avoidDistanceForce.avoidDistanceForceKey1, avoidForceData.avoidDistanceForce.avoidDistanceForceKey2, avoidForceData.avoidDistanceForce.avoidDistanceForceKey3, avoidForceData.avoidDistanceForce.avoidDistanceForceKey4};
		avoidForceData.avoidDistanceForce.avoidDistanceForceCurve = setupForceCurves(avoiddistanceforces);
		// ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^

		minScreenBounds = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0));
		maxScreenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
	}

	AnimationCurve setupForceCurves(float[] keyframeForces) {
		AnimationCurve curve = new AnimationCurve();

		for (int i = 0; i < keyframeForces.Length; i++) {
			curve.AddKey(i * (1.0f / keyframeForces.Length), keyframeForces[i]);
		}

		return curve;
	}

	List<GameObject> GetTargetsInRange() {
		List<GameObject> targets = GameObject.FindGameObjectsWithTag("attract").Concat(GameObject.FindGameObjectsWithTag("avoid")).ToList();

		for (int i = targets.Count - 1; i >= 0; i--) {
			if ((targets[i].transform.position - transform.position).sqrMagnitude > visionRadius * visionRadius) {
				targets.RemoveAt(i);
			}
		}

		return targets;
	}
	
	// Update is called once per frame
	void Update () {

		List<GameObject> targetsInRange = GetTargetsInRange();

		Vector3 steeringForce = Vector3.zero;

		if (targetsInRange.Count > 0) {
			foreach (GameObject target in targetsInRange) {
				switch (target.tag) {
					case "avoid":
						float avoidForce = avoidForceData.avoidHealthForce.avoidHealthForceCurve.Evaluate(health / maxHealth); // return betweens 0 and 1
						float avoidDistance = (target.transform.position - transform.position).magnitude;
						avoidForce += avoidForceData.avoidDistanceForce.avoidDistanceForceCurve.Evaluate(avoidDistance / visionRadius); // return betweens 0 and 1
						avoidForce = avoidForce / 2; // get average
						avoidForce = avoidForce * 2 - 1; // Clamp between -1 & 1

						steeringForce += Seek(target) * avoidForce;
					break;

					case "attract":
						float attractForce = pickupForceData.pickupHealthForce.pickupHealthForceCurve.Evaluate(health / maxHealth); // returns between 0 and 1
						float attractDistance = (target.transform.position - transform.position).magnitude;
						attractForce += pickupForceData.pickupDistanceForce.pickupDistanceForceCurve.Evaluate(attractDistance / visionRadius); // return betweens 0 and 1
						attractForce = attractForce / 2; // get average
						attractForce = attractForce * 2 - 1; // Clamp between -1 & 1

						steeringForce += Seek(target) * attractForce;
					break;

					default:
						steeringForce += Seek(target);
					break;
				}
			}
		} else {
			steeringForce += RandomSeek();
		}

		// SHOOTING?!?!?!

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

		if (transform.position.x < minScreenBounds.x || transform.position.x > maxScreenBounds.x || transform.position.y < minScreenBounds.y || transform.position.y > maxScreenBounds.y) {
			// Head towards the center of the screen if outside the screen bounds
			desiredVelocity = Vector3.Normalize(Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 0)) - transform.position) * maxVelocity;
		} else {
			desiredVelocity = Vector3.Normalize((transform.position + (velocity * Time.deltaTime)) - transform.position) * maxVelocity;
			desiredVelocity = Quaternion.AngleAxis(Random.value >= 0.5f ? 35 : -35, Vector3.forward) * desiredVelocity;
		}

		desiredVelocity.z = 0.0f;

		Vector3 steering = desiredVelocity - velocity;
		steering.z = 0.0f;

		return steering;
	}
}
