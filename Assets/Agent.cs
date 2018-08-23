using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour {

	[Range(0.01f, 1.0f)]
	public float maxSteering; // This changes the distance between the "wheels" (Back 2 verts); Bigger = slower turning.
	
	[Range(1f, 10f)]
	public float maxHealth; // This changes the agents scale and maxVelocity; more health = bigger & slower

	[Range(1f, 10f)]
	public float visionRange; // This is the radius of perception circle

	[Range(-5.0f, 5.0f)]
	public float attactMultiplier1;
	[Range(-5.0f, 5.0f)]
	public float attactMultiplier2;
	[Range(-5.0f, 5.0f)]
	public float attactMultiplier3;

	[Range(-5.0f, 5.0f)]
	public float avoidMultiplier1;
	[Range(-5.0f, 5.0f)]
	public float avoidMultiplier2;
	[Range(-5.0f, 5.0f)]
	public float avoidMultiplier3;

	public AnimationCurve pickupHealthAttractionCurve;
	public AnimationCurve pickupDistanceAttractionCurve;
	private CircleCollider2D visionCollider;
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
		visionCollider.radius = visionRange / 2.0f;
		visionCollider.offset = new Vector2(0.0f, -maxHealth / 20.0f);

		pickupHealthAttractionCurve = new AnimationCurve();
		pickupDistanceAttractionCurve = new AnimationCurve();
		Keyframe[] keyframes = new Keyframe[5];
		for (int i = 0; i < keyframes.Length; i++) {
			// TODO: Evolve the values!
			pickupHealthAttractionCurve.AddKey(i * (1.0f / keyframes.Length), 0.5f);
			pickupDistanceAttractionCurve.AddKey(i * (1.0f / keyframes.Length), 0.5f);
		}
	}

	float ForceCalc(float attribute, List<float> multipliers) {
		float force = 0;
		for (int i = 0; i < multipliers.Count; i++) {
			force += multipliers[i] * Mathf.Pow(attribute, i);
		}
		return force;
	}

	List<GameObject[]> GetTargetsInRange() {
		List<GameObject[]> targets = new List<GameObject[]>();

		// TODO: FILTER BY RANGE!!!
		targets.Add(GameObject.FindGameObjectsWithTag("attract"));
		targets.Add(GameObject.FindGameObjectsWithTag("avoid"));

		return targets;
	}
	
	// Update is called once per frame
	void Update () {

		List<GameObject[]> targetsInRange = GetTargetsInRange();

		Vector3 steeringForce = Vector3.zero;

		if (targetsInRange.Count > 0) {
			foreach (GameObject[] list in targetsInRange) {
				foreach (GameObject target in list) {
					switch (target.tag) {
						case "avoid":
							steeringForce += Seek(target) * ForceCalc(health, new List<float> { avoidMultiplier1, avoidMultiplier2, avoidMultiplier3 });
						break;

						case "attract":
							steeringForce += Seek(target) * ForceCalc(health, new List<float> { attactMultiplier1, attactMultiplier2, attactMultiplier3 });
						break;

						default:
							steeringForce += Seek(target);
						break;
					}
				}
			}
		} else {
			// TODO: Improve this!
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
		Vector3 newPos = Camera.main.ScreenToWorldPoint(
				new Vector3(
					Random.Range(0, Screen.width),
					Random.Range(0, Screen.height),
					10.0f
				)
			);
		newPos.z = 0;
		Vector3 desiredVelocity = Vector3.Normalize(newPos - transform.position) * maxVelocity;
		desiredVelocity.z = 0.0f;

		Vector3 steering = desiredVelocity - velocity;
		steering.z = 0.0f;

		return steering;
	}
}
