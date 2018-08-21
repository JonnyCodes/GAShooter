using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour {
	public GameObject target;
	public GameObject avoid;
	
	[Range(0.02f, 0.2f)]
	public float maxSteering; // This changes the distance between the "wheels" (Back 2 verts); Bigger = slower turning.
	
	[Range(0.1f, 5f)]
	public float maxHealth; // This changes the agents scale and maxVelocity; more health = bigger & slower

	private Vector3 velocity;
	private float maxVelocity;
	private float health;
	private float attractForceWeight;
	private float avoidForceWeight;

	// Use this for initialization
	void Start () {
		velocity = Vector3.zero;
		maxVelocity = (6.0f - maxHealth) / 1.25f; // TODO: Don't like the magic numbers here
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
		verts[0] = new Vector3((-maxSteering / 2.0f) * 10.0f, 0.25f, 0.0f);
		verts[1] = new Vector3(0.0f, -0.75f, 0.0f);
		verts[2] = new Vector3(0.0f, 0.0f, 0.0f); // CENTER OF THE POLYGON
		verts[3] = new Vector3((maxSteering / 2.0f) * 10.0f, 0.25f, 0.0f);
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

		transform.localScale = new Vector3(maxHealth, maxHealth, 1.0f);
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 steeringForce = Seek(target) * attractForceWeight;
		steeringForce += Seek(avoid) * avoidForceWeight;

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
}
