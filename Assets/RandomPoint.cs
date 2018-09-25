using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomPoint : MonoBehaviour {
	public float initialChangeTime = 5.5f;
	private float changeTime;

	public void ChangePosition() {
		// changeTime = initialChangeTime;

		Vector3 newPos = Camera.main.ScreenToWorldPoint(
			new Vector3(
				Random.Range(0, Screen.width),
				Random.Range(0, Screen.height),
				10.0f
			)
		);

		newPos.z = 0;

		gameObject.transform.position = newPos;
	}

	// Use this for initialization
	void Start () {
		// changeTime = initialChangeTime;
	}
	
	// Update is called once per frame
	void Update () {
		// changeTime -= Time.deltaTime;

		// if (changeTime <= 0) {
			// ChangePosition();
		// }
	}
}
