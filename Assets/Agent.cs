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
		public AnimationCurve pickupForceCurve;

		[Range(0.0f, 1.0f)]
		public float pickupForceKey1;
		[Range(0.0f, 1.0f)]
		public float pickupForceKey2;
		[Range(0.0f, 1.0f)]
		public float pickupForceKey3;
		[Range(0.0f, 1.0f)]
		public float pickupForceKey4;
	}
	public PickupForceData pickupForceData;
	
	[System.Serializable]
	public class AvoidForceData {
		public AnimationCurve avoidForceCurve;

		[Range(0.0f, 1.0f)]
		public float avoidForceKey1;
		[Range(0.0f, 1.0f)]
		public float avoidForceKey2;
		[Range(0.0f, 1.0f)]
		public float avoidForceKey3;
		[Range(0.0f, 1.0f)]
		public float avoidForceKey4;
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
		velocity = Vector3.zero;
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
		float[] pickupforces = new float[] {pickupForceData.pickupForceKey1, pickupForceData.pickupForceKey2, pickupForceData.pickupForceKey3, pickupForceData.pickupForceKey4};
		pickupForceData.pickupForceCurve = setupForceCurves(pickupforces);

		float[] avoidforces = new float[] {avoidForceData.avoidForceKey1, avoidForceData.avoidForceKey2, avoidForceData.avoidForceKey3, avoidForceData.avoidForceKey4};
		avoidForceData.avoidForceCurve = setupForceCurves(avoidforces);
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
						float avoidHealthForce = avoidForceData.avoidForceCurve.Evaluate(health / maxHealth);
						avoidHealthForce = avoidHealthForce * 2 - 1; // Clamp between -1 & 1

						steeringForce += Seek(target) * avoidHealthForce;
					break;

					case "attract":
						float attractHealthForce = pickupForceData.pickupForceCurve.Evaluate(health / maxHealth);
						attractHealthForce = attractHealthForce * 2 - 1; // Clamp between -1 & 1

						steeringForce += Seek(target) * attractHealthForce;
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
			desiredVelocity = Vector3.Normalize(velocity - transform.position) * maxVelocity;
			desiredVelocity = Quaternion.AngleAxis(Random.value > 0.5 ? 25 : -25, Vector3.forward) * desiredVelocity;
		}

		desiredVelocity.z = 0.0f;

		Vector3 steering = desiredVelocity - velocity;
		steering.z = 0.0f;

		return steering;
	}
}
