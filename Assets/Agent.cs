using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour {
	public GameObject target;
	public GameObject avoid;
	

	[Range(1.0f, 10.0f)]
	public float maxSteering; // This changes the distance between the "wheels" (Back 2 verts); Bigger = slower turning.
	
	[Range(1f, 10f)]
	public float maxHealth; // This changes the agents scale and maxVelocity; more health = bigger & slower

	[Range(1f, 10f)]
	public float visionRange; // This is the radius of perception circle

	public AnimationCurve pickupHealthAttractionCurve;
	public AnimationCurve pickupDistanceAttractionCurve;
	private CircleCollider2D visionCollider;
	private List<GameObject> targetsInRange;
	private Vector3 velocity;
	private float maxVelocity;
	private float health;
	private float attractForceWeight;
	private float avoidForceWeight;

	// Use this for initialization
	void Start () {
		velocity = Vector3.zero;
		maxVelocity = (11.0f - maxHealth) / 1.25f; // TODO: Don't like the magic numbers here
		health = maxHealth;

		// TODO: This should be calculated with graph/bezier functions with health and distance as parameters
		// Look at AnimationCurves in the editor!
		attractForceWeight = 0.75f;
		avoidForceWeight = -0.25f; // The attraction force will be between -1 and 1

		// VVV Create agent polygon VVV
		MeshFilter meshFilter = GetComponent<MeshFilter>();
		Mesh agentMesh = new Mesh();
		meshFilter.mesh = agentMesh;

		Vector3[] verts = new Vector3[4];
		verts[0] = new Vector3((-maxSteering / 50.0f) * 5.0f, 0.15f, 0.0f);
		verts[1] = new Vector3(0.0f, (-maxHealth / 10.0f) - 0.15f, 0.0f);
		verts[2] = new Vector3(0.0f, 0.0f, 0.0f); // CENTER OF THE POLYGON
		verts[3] = new Vector3((maxSteering / 50.0f) * 5.0f, 0.15f, 0.0f);
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
		visionCollider.radius = visionRange / 2.0f;
		visionCollider.offset = new Vector2(0.0f, -maxHealth / 20.0f);

		targetsInRange = new List<GameObject>();

		pickupHealthAttractionCurve = new AnimationCurve();
		pickupDistanceAttractionCurve = new AnimationCurve();
		Keyframe[] keyframes = new Keyframe[5];
		for (int i = 0; i < keyframes.Length; i++) {
			pickupHealthAttractionCurve.AddKey(i * (1.0f / keyframes.Length), 0.5f);
			pickupDistanceAttractionCurve.AddKey(i * (1.0f / keyframes.Length), 0.5f);
		}
		 

		// TODO: Get all objects in visual range
		// Apply seek force to all of them?
		// Adjust force by weights
		// Need random wander behaviour, if no objects in visual range
		// SHOOTING?!?!
	}

	float forceCalc(float attribute, List<float> multipliers) {
		float force = 0;
		for (int i = 0; i < multipliers.Count; i++) {
			force += multipliers[i] * Mathf.Pow(attribute, i + 1.0f);
		}
		return force;
	}

	void OnTriggerEnter(Collider target) {
		if (!targetsInRange.Contains(target.gameObject)) {
			targetsInRange.Add(target.gameObject);
		}
	}

	void OnTriggerEXit(Collider target) {
		if (targetsInRange.Contains(target.gameObject)) {
			targetsInRange.Remove(target.gameObject);
		}
	}
	
	// Update is called once per frame
	void Update () {

		if (targetsInRange.Count > 0) {
			foreach (GameObject target in targetsInRange) {
				// Get Type of object to apply the correct force to it
			}
		}

		Vector3 steeringForce = Seek(target) * attractForceWeight;
		steeringForce += Seek(avoid) * avoidForceWeight;

		if (steeringForce.sqrMagnitude > maxSteering / 100.0f * maxSteering / 100.0f) {
			steeringForce.Normalize();
			steeringForce *= maxSteering / 100.0f;
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
}
