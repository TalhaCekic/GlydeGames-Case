using UnityEngine;
using System.Collections.Generic;
using Mirror;

public class NpcCarMovement : NetworkBehaviour
{
	[SyncVar] public string SelectPathString;
	private Rigidbody rb;
	[SyncVar] public float hiz;
	[SyncVar] public float speed;
	[SyncVar] public float maxSteerAngle = 50;
	[SyncVar] private float newSteer;
	[SyncVar] public float deleteTime;
	[SyncVar] public bool isStop;
	[SyncVar] public bool isFrontRayClose;
	[SyncVar] public bool isRightRayClose;
	[SyncVar] public bool isLeftRayClose;
	public WheelCollider wheelFl;
	public WheelCollider wheelFr;

	public List<GameObject> Path = new List<GameObject>();
	public Path SelectPath;
	public List<Vector3> VectorPos = new List<Vector3>();
	public List<Transform> nodes;
	public int currentNode = 0;
	//public Vector3 CurrentPos;

	public LayerMask layer;
	public LayerMask Backlayer;
	public Transform frontDetector;
	public Transform frontRightDetector;
	public Transform frontRightDetector1;
	public Transform frontLeftDetector;
	public Transform frontLeftDetector1;
	public Transform LeftDetector;
	public Transform rightDetector;

	private Ray ray;
	private Ray rayRight;
	private Ray rayLeft;
	private Ray rayRight1;
	private Ray rayLeft1;
	private Ray rayRight2;
	private Ray rayLeft2;

	private void Start() {
		rb = GetComponent<Rigidbody>();
		if (isServer)
		{
			ServerStart();
		}
	}
	[Server]
	private void ServerStart() {
		RpcStart();
	}
	[ClientRpc]
	private void RpcStart() {
		AppiranceValue();
	}
	void AppiranceValue() {
		GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag(SelectPathString);
		for (int i = 0; i < objectsWithTag.Length; i++)
		{
			Path.Add(objectsWithTag[i]);
		}
		SelectPath = Path[0].GetComponent<Path>();

		for (int i = 0; i < SelectPath.nodes.Count; i++)
		{
			VectorPos.Add(SelectPath.nodes[i].position);
		}
	}

	private void FixedUpdate() {
		if (isServer)
		{
			ApplySteer();
			CheckWayPointDistance();
			detector();
			ServerDistanceMove();
		}
	}
	[Server]
	private void ServerDistanceMove() {
		Vector3 lineerHiz = rb.velocity;
		hiz = lineerHiz.magnitude;
		if (Vector3.Distance(transform.position, VectorPos[currentNode]) < 8)
		{
			if (hiz > 2 || isStop)
			{
				stop();
			}
			else
			{
				slowDrive();
			}
		}
		else if (hiz < 5)
		{
			Drive();
		}

		if (isStop)
		{
			stop();
		}
		else
		{
			if (hiz < 0)
			{
				isStop = true;
				deleteTime -= Time.deltaTime;
			}
			else
			{
				deleteTime = 20;
			}
		}
	}
	[Server]
	private void ApplySteer() {
		Vector3 relativeVector = transform.InverseTransformPoint(VectorPos[currentNode]);
		newSteer = (relativeVector.x / relativeVector.magnitude) * maxSteerAngle;
		wheelFl.steerAngle = newSteer;
		wheelFr.steerAngle = newSteer;

		if (newSteer < -5)
		{
			isRightRayClose = true;
			isLeftRayClose = false;
			isFrontRayClose = true;
		}

		if (newSteer > 5)
		{
			isLeftRayClose = true;
			isRightRayClose = false;
			isFrontRayClose = true;
		}
		else if (newSteer < 5 && newSteer > -5)
		{
			isFrontRayClose = false;
			isLeftRayClose = false;
			isRightRayClose = false;
		}
	}
	[Server]
	private void Drive() {
		speed = rb.velocity.magnitude * 3.6f;
		wheelFl.motorTorque = 50;
		wheelFr.motorTorque = 50;
		wheelFl.brakeTorque = 0;
		wheelFr.brakeTorque = 0;
	}
	[Server]
	private void slowDrive() {
		speed = rb.velocity.magnitude * 3.6f;
		wheelFl.motorTorque = 20;
		wheelFr.motorTorque = 20;
		wheelFl.brakeTorque = 0;
		wheelFr.brakeTorque = 0;
	}

	[Server]
	private void stop() {
		wheelFl.motorTorque = 0;
		wheelFr.motorTorque = 0;
		wheelFl.brakeTorque = 5000;
		wheelFr.brakeTorque = 5000;

		if (deleteTime < 0)
		{
			Destroy(this.gameObject);
		}
		else
		{
			deleteTime -= Time.deltaTime;
		}
	}
	[Server]
	private void CheckWayPointDistance() {
		if (Vector3.Distance(transform.position, VectorPos[currentNode]) < 2)
		{
			currentNode++;
			if (currentNode >= VectorPos.Count)
			{
				currentNode = 0;
			}
		}
	}
	[Server]
	private void detector() {
		if (!isFrontRayClose)
		{
			ray = new Ray(frontDetector.position, frontDetector.forward);
			Debug.DrawLine(ray.origin, ray.origin + ray.direction * 6, Color.yellow);
		}

		if (!isRightRayClose)
		{
			rayRight = new Ray(frontRightDetector.position, frontRightDetector.forward);
			Debug.DrawLine(rayRight.origin, rayRight.origin + rayRight.direction * 4, Color.yellow);

			rayRight1 = new Ray(frontRightDetector1.position, frontRightDetector1.forward);
			Debug.DrawLine(rayRight1.origin, rayRight1.origin + rayRight1.direction * 4, Color.yellow);
			Debug.DrawLine(rayRight1.origin, rayRight1.origin + rayRight1.direction * 4, Color.white);
		}

		if (!isLeftRayClose)
		{
			rayLeft = new Ray(frontLeftDetector.position, frontLeftDetector.forward);
			Debug.DrawLine(rayLeft.origin, rayLeft.origin + rayLeft.direction * 4, Color.yellow);

			rayLeft1 = new Ray(frontLeftDetector1.position, frontLeftDetector1.forward);
			Debug.DrawLine(rayLeft1.origin, rayLeft1.origin + rayLeft1.direction * 4, Color.yellow);
			Debug.DrawLine(rayLeft1.origin, rayLeft1.origin + rayLeft1.direction * 4, Color.white);
		}

		rayLeft2 = new Ray(LeftDetector.position, LeftDetector.forward);
		Debug.DrawLine(rayLeft2.origin, rayLeft2.origin + rayLeft2.direction * 1, Color.white);

		rayRight2 = new Ray(rightDetector.position, rightDetector.forward);
		Debug.DrawLine(rayRight2.origin, rayRight2.origin + rayRight2.direction * 1, Color.white);

		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, 6, layer) || Physics.Raycast(rayRight, out hit, 4, layer) ||
		    Physics.Raycast(rayLeft, out hit, 4, layer) || Physics.Raycast(rayRight1, out hit, 4, layer) ||
		    Physics.Raycast(rayLeft1, out hit, 4, layer) || Physics.Raycast(rayRight, out hit, 4, Backlayer) ||
		    Physics.Raycast(rayLeft, out hit, 4, Backlayer) ||
		    Physics.Raycast(rayLeft1, out hit, 4, Backlayer) || Physics.Raycast(rayRight1, out hit, 4, Backlayer) ||
		    Physics.Raycast(rayLeft1, out hit, 4, Backlayer) || Physics.Raycast(rayRight2, out hit, 1, Backlayer) ||
		    Physics.Raycast(rayLeft2, out hit, 1, Backlayer))
		{
			if (hit.transform == this.transform)
			{
				isStop = false;
			}
			else
			{
				isStop = true;
			}
		}
		else
		{
			isStop = false;
		}
	}

	[Server]
	public void ServerCustomerSpawn(GameObject obj, List<Transform> NpcCarSpawnPos) {
		int index = Random.Range(0, NpcCarSpawnPos.Count);
		obj.transform.position = NpcCarSpawnPos[index].position;
		//obj.transform.rotation = Quaternion.LookRotation(VectorPos[currentNode]);
	}

}
