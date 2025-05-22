using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using DG.Tweening;

public class Path : NetworkBehaviour
{

	public List<Transform> nodes = new List<Transform>();

	private void Awake() {

		// Transform[] pathTransforms = GetComponentsInChildren<Transform>();
		// for (int i = 0; i < pathTransforms.Length; i++)
		// {
		// 	if (pathTransforms[i] != transform)
		// 	{
		// 		nodes.Add(pathTransforms[i]);
		// 	}
		// }
	}
	void Start() {
		if (isServer)
		{
			ServerGizmos();
		}
	}
	void Update() {
		// if (isServer)
		// {
		// 	ServerGizmos();
		// }
	}

	[Server]
	private void ServerGizmos() {
		RpcGizmos();
	}
	[ClientRpc]
	private void RpcGizmos() {
		Transform[] pathTransforms = GetComponentsInChildren<Transform>();
		for (int i = 0; i < pathTransforms.Length; i++)
		{
			if (pathTransforms[i] != transform)
			{
				nodes.Add(pathTransforms[i]);
			}
		}
	}
}
