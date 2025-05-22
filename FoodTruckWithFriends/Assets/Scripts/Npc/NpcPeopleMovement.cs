using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Mirror;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class NpcPeopleMovement : NetworkBehaviour
{
	NavMeshAgent _agent;
	Animator anims;
	[SyncVar] public int AppearanceValue;
	public List<GameObject> Appearance = new List<GameObject>();

	public List<GameObject> Path = new List<GameObject>();
	public Path SelectPath;
	public List<Vector3> VectorPos = new List<Vector3>();
	public Vector3 CurrentPos;
	[SyncVar] public int currentNode;

	void Start() {
		anims = GetComponent<Animator>();
		_agent = GetComponent<NavMeshAgent>();
		if (isServer)
		{
			ServerStart();
		}
	}
	[Server]
	private void ServerStart() {
		anims.SetBool("isWalk", true);
		AppearanceValue = Random.Range(1, Appearance.Count);
		//print(AppearanceValue);
		RpcStart(AppearanceValue);
	}
	[ClientRpc]
	private void RpcStart(int appearanceValue) {
		AppiranceValue(appearanceValue);

		GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag("Path");
		for (int i = 0; i < objectsWithTag.Length; i++)
		{
			Path.Add(objectsWithTag[i]);
		}
		int selectPathIndex = Random.Range(0, Path.Count);
		SelectPath = Path[selectPathIndex].GetComponent<Path>();

		for (int i = 0; i < SelectPath.nodes.Count; i++)
		{
			VectorPos.Add(SelectPath.nodes[i].position);
			CurrentPos = VectorPos[currentNode];
		}
	}
	void AppiranceValue(int value) {
		//AppearanceValue = Random.Range(0, Appearance.Count);
		Appearance[value].SetActive(true);
	}
	void Update() {
		if (isServer)
		{
			ServerMovementNpc();
		}
	}
	[Server]
	private void ServerMovementNpc() {
		_agent.destination = CurrentPos;

		Vector3 direction = CurrentPos - transform.position;
		Quaternion targetRotation = Quaternion.LookRotation(direction);

		float rotationSpeed = 5f;
		transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

		if (Vector3.Distance(transform.position, CurrentPos) < 2f)
		{
			currentNode++;
			if (currentNode >= VectorPos.Count)
			{
				currentNode = 0;
			}
			CurrentPos = VectorPos[currentNode];
		}
	}
	[Server]
	public void ServerCustomerSpawn(GameObject obj,List<Transform> NpcSpawnPos) {
		if (obj!=null)
		{
			int index = Random.Range(0, NpcSpawnPos.Count);
			//RpcCustomerSpawn(obj,index,NpcSpawnPos);
			//obj.transform.position = NpcSpawnPos[index].position;
		}
	}

	[ClientRpc]
	public void RpcCustomerSpawn(GameObject obj,int index,List<Transform> NpcSpawnPos)
	{
		//obj.transform.position = NpcSpawnPos[0].position;
	}
}
