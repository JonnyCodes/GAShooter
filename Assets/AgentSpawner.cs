using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentSpawner : MonoBehaviour {

	public GameObject agentPrefab;
	public int maxNumAgents = 5;
	public float reproduceInteval = 30.0f;
	public float mutatePercetage = 3.0f;
	public List<GameObject> agents;

	private float reproduceTimer;
	private int agentCount;

	// Use this for initialization
	void Start () {
		agents = new List<GameObject>();
		agentCount = 0;
		reproduceTimer = reproduceInteval;

		for(int i = 0; i < maxNumAgents; i++) {
			SpawnAgent();
		}
	}

	void SpawnAgent() {
		Vector3 spawnPos = Camera.main.ScreenToWorldPoint(
			new Vector3(
				Random.Range(0, Screen.width),
				Random.Range(0, Screen.height),
				10.0f
			)
		);
		spawnPos.z = 0.0f;

		GameObject agent = Instantiate(agentPrefab, spawnPos, Quaternion.Euler(0, 0, Random.Range(0, 360)));
		agent.name = "Agent: " + agentCount;
		agents.Add(agent);
		agentCount++;
	}

	void Reproduce() {
		agents.Sort(SortAgentsByPoints);

		// remove the worst agent!
		Destroy(agents[0]);
		agents.RemoveAt(0);

		agents.Reverse();

		Agent agentA = agents[0].GetComponent<Agent>();
		Agent agentB = agents[1].GetComponent<Agent>();

		// CROSSOVER
		// Take a average of both agents values, weighted towards the better agent

		Agent agentScript = agentPrefab.GetComponent<Agent>();
		agentScript.randInit = false;

		agentScript.maxHealth = DoCrossover(agentA.maxHealth, agentB.maxHealth, 1.0f, 10.0f);
		agentScript.maxSteering = DoCrossover(agentA.maxSteering, agentB.maxSteering, 0.1f, 1.5f);
		agentScript.visionRadius = DoCrossover(agentA.visionRadius, agentB.visionRadius, 1.0f, 10.0f);
		agentScript.rateOfFire = DoCrossover(agentA.rateOfFire, agentB.rateOfFire, 1.0f, 10.0f);
		agentScript.bulletSize = DoCrossover(agentA.bulletSize, agentB.bulletSize, 0.2f, 1.0f);

		agentScript.pickupForceData.pickupHealthForce.pickupHealthForceKey1 = DoCrossover(agentA.pickupForceData.pickupHealthForce.pickupHealthForceKey1, agentB.pickupForceData.pickupHealthForce.pickupHealthForceKey1);
		agentScript.pickupForceData.pickupHealthForce.pickupHealthForceKey2 = DoCrossover(agentA.pickupForceData.pickupHealthForce.pickupHealthForceKey2, agentB.pickupForceData.pickupHealthForce.pickupHealthForceKey2);
		agentScript.pickupForceData.pickupHealthForce.pickupHealthForceKey3 = DoCrossover(agentA.pickupForceData.pickupHealthForce.pickupHealthForceKey3, agentB.pickupForceData.pickupHealthForce.pickupHealthForceKey3);
		agentScript.pickupForceData.pickupHealthForce.pickupHealthForceKey4 = DoCrossover(agentA.pickupForceData.pickupHealthForce.pickupHealthForceKey4, agentB.pickupForceData.pickupHealthForce.pickupHealthForceKey4);
		agentScript.pickupForceData.pickupDistanceForce.pickupDistanceForceKey1 = DoCrossover(agentA.pickupForceData.pickupDistanceForce.pickupDistanceForceKey1, agentB.pickupForceData.pickupDistanceForce.pickupDistanceForceKey1);
		agentScript.pickupForceData.pickupDistanceForce.pickupDistanceForceKey2 = DoCrossover(agentA.pickupForceData.pickupDistanceForce.pickupDistanceForceKey2, agentB.pickupForceData.pickupDistanceForce.pickupDistanceForceKey2);
		agentScript.pickupForceData.pickupDistanceForce.pickupDistanceForceKey3 = DoCrossover(agentA.pickupForceData.pickupDistanceForce.pickupDistanceForceKey3, agentB.pickupForceData.pickupDistanceForce.pickupDistanceForceKey3);
		agentScript.pickupForceData.pickupDistanceForce.pickupDistanceForceKey4 = DoCrossover(agentA.pickupForceData.pickupDistanceForce.pickupDistanceForceKey4, agentB.pickupForceData.pickupDistanceForce.pickupDistanceForceKey4);
		agentScript.pickupForceData.pickupDirectionForce.pickupDirectionForceKey1 = DoCrossover(agentA.pickupForceData.pickupDirectionForce.pickupDirectionForceKey1, agentB.pickupForceData.pickupDirectionForce.pickupDirectionForceKey1);
		agentScript.pickupForceData.pickupDirectionForce.pickupDirectionForceKey2 = DoCrossover(agentA.pickupForceData.pickupDirectionForce.pickupDirectionForceKey2, agentB.pickupForceData.pickupDirectionForce.pickupDirectionForceKey2);
		agentScript.pickupForceData.pickupDirectionForce.pickupDirectionForceKey3 = DoCrossover(agentA.pickupForceData.pickupDirectionForce.pickupDirectionForceKey3, agentB.pickupForceData.pickupDirectionForce.pickupDirectionForceKey3);
		agentScript.pickupForceData.pickupDirectionForce.pickupDirectionForceKey4 = DoCrossover(agentA.pickupForceData.pickupDirectionForce.pickupDirectionForceKey4, agentB.pickupForceData.pickupDirectionForce.pickupDirectionForceKey4);

		agentScript.avoidForceData.avoidHealthForce.avoidHealthForceKey1 = DoCrossover(agentA.avoidForceData.avoidHealthForce.avoidHealthForceKey1, agentB.avoidForceData.avoidHealthForce.avoidHealthForceKey1);
		agentScript.avoidForceData.avoidHealthForce.avoidHealthForceKey2 = DoCrossover(agentA.avoidForceData.avoidHealthForce.avoidHealthForceKey2, agentB.avoidForceData.avoidHealthForce.avoidHealthForceKey2);
		agentScript.avoidForceData.avoidHealthForce.avoidHealthForceKey3 = DoCrossover(agentA.avoidForceData.avoidHealthForce.avoidHealthForceKey3, agentB.avoidForceData.avoidHealthForce.avoidHealthForceKey3);
		agentScript.avoidForceData.avoidHealthForce.avoidHealthForceKey4 = DoCrossover(agentA.avoidForceData.avoidHealthForce.avoidHealthForceKey4, agentB.avoidForceData.avoidHealthForce.avoidHealthForceKey4);
		agentScript.avoidForceData.avoidDistanceForce.avoidDistanceForceKey1 = DoCrossover(agentA.avoidForceData.avoidDistanceForce.avoidDistanceForceKey1, agentB.avoidForceData.avoidDistanceForce.avoidDistanceForceKey1);
		agentScript.avoidForceData.avoidDistanceForce.avoidDistanceForceKey2 = DoCrossover(agentA.avoidForceData.avoidDistanceForce.avoidDistanceForceKey2, agentB.avoidForceData.avoidDistanceForce.avoidDistanceForceKey2);
		agentScript.avoidForceData.avoidDistanceForce.avoidDistanceForceKey3 = DoCrossover(agentA.avoidForceData.avoidDistanceForce.avoidDistanceForceKey3, agentB.avoidForceData.avoidDistanceForce.avoidDistanceForceKey3);
		agentScript.avoidForceData.avoidDistanceForce.avoidDistanceForceKey4 = DoCrossover(agentA.avoidForceData.avoidDistanceForce.avoidDistanceForceKey4, agentB.avoidForceData.avoidDistanceForce.avoidDistanceForceKey4);
		agentScript.avoidForceData.avoidDirectionForce.avoidDirectionForceKey1 = DoCrossover(agentA.avoidForceData.avoidDirectionForce.avoidDirectionForceKey1, agentB.avoidForceData.avoidDirectionForce.avoidDirectionForceKey1);
		agentScript.avoidForceData.avoidDirectionForce.avoidDirectionForceKey2 = DoCrossover(agentA.avoidForceData.avoidDirectionForce.avoidDirectionForceKey2, agentB.avoidForceData.avoidDirectionForce.avoidDirectionForceKey2);
		agentScript.avoidForceData.avoidDirectionForce.avoidDirectionForceKey3 = DoCrossover(agentA.avoidForceData.avoidDirectionForce.avoidDirectionForceKey3, agentB.avoidForceData.avoidDirectionForce.avoidDirectionForceKey3);
		agentScript.avoidForceData.avoidDirectionForce.avoidDirectionForceKey4 = DoCrossover(agentA.avoidForceData.avoidDirectionForce.avoidDirectionForceKey4, agentB.avoidForceData.avoidDirectionForce.avoidDirectionForceKey4);

		agentScript.agentForceData.agentHealthForce.agentHealthForceKey1 = DoCrossover(agentA.agentForceData.agentHealthForce.agentHealthForceKey1, agentB.agentForceData.agentHealthForce.agentHealthForceKey1);
		agentScript.agentForceData.agentHealthForce.agentHealthForceKey2 = DoCrossover(agentA.agentForceData.agentHealthForce.agentHealthForceKey2, agentB.agentForceData.agentHealthForce.agentHealthForceKey2);
		agentScript.agentForceData.agentHealthForce.agentHealthForceKey3 = DoCrossover(agentA.agentForceData.agentHealthForce.agentHealthForceKey3, agentB.agentForceData.agentHealthForce.agentHealthForceKey3);
		agentScript.agentForceData.agentHealthForce.agentHealthForceKey4 = DoCrossover(agentA.agentForceData.agentHealthForce.agentHealthForceKey4, agentB.agentForceData.agentHealthForce.agentHealthForceKey4);
		agentScript.agentForceData.agentDistanceForce.agentDistanceForceKey1 = DoCrossover(agentA.agentForceData.agentDistanceForce.agentDistanceForceKey1, agentB.agentForceData.agentDistanceForce.agentDistanceForceKey1);
		agentScript.agentForceData.agentDistanceForce.agentDistanceForceKey2 = DoCrossover(agentA.agentForceData.agentDistanceForce.agentDistanceForceKey2, agentB.agentForceData.agentDistanceForce.agentDistanceForceKey2);
		agentScript.agentForceData.agentDistanceForce.agentDistanceForceKey3 = DoCrossover(agentA.agentForceData.agentDistanceForce.agentDistanceForceKey3, agentB.agentForceData.agentDistanceForce.agentDistanceForceKey3);
		agentScript.agentForceData.agentDistanceForce.agentDistanceForceKey4 = DoCrossover(agentA.agentForceData.agentDistanceForce.agentDistanceForceKey4, agentB.agentForceData.agentDistanceForce.agentDistanceForceKey4);
		agentScript.agentForceData.agentDirectionForce.agentDirectionForceKey1 = DoCrossover(agentA.agentForceData.agentDirectionForce.agentDirectionForceKey1, agentB.agentForceData.agentDirectionForce.agentDirectionForceKey1);
		agentScript.agentForceData.agentDirectionForce.agentDirectionForceKey2 = DoCrossover(agentA.agentForceData.agentDirectionForce.agentDirectionForceKey2, agentB.agentForceData.agentDirectionForce.agentDirectionForceKey2);
		agentScript.agentForceData.agentDirectionForce.agentDirectionForceKey3 = DoCrossover(agentA.agentForceData.agentDirectionForce.agentDirectionForceKey3, agentB.agentForceData.agentDirectionForce.agentDirectionForceKey3);
		agentScript.agentForceData.agentDirectionForce.agentDirectionForceKey4 = DoCrossover(agentA.agentForceData.agentDirectionForce.agentDirectionForceKey4, agentB.agentForceData.agentDirectionForce.agentDirectionForceKey4);

		SpawnAgent();
	}
	int SortAgentsByPoints(GameObject agentA, GameObject agentB) {
		return agentA.GetComponent<Agent>().points.CompareTo(agentB.GetComponent<Agent>().points);
	}

	float DoCrossover(float agentAValue, float agentBValue, float minValue = 0.0f, float maxValue = 1.0f) {
		float mutate = Random.Range(0.0f, 100.0f);
		if (mutate <= mutatePercetage) {
			return Random.Range(minValue, maxValue);
		}
		return (agentAValue + agentAValue + agentBValue) / 3;
	}
	
	// Update is called once per frame
	void Update () {
		reproduceTimer -= Time.deltaTime;

		if (reproduceTimer <= 0) {
			reproduceTimer = reproduceInteval;

			Reproduce();
		}
	}
}
