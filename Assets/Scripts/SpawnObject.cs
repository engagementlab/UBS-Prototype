﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SpawnObject : MonoBehaviour
{

	public Transform waypointStart;
	public Transform waypointEnd;
	
	public bool wizardMode;

	public float MoveSpeed {
		get { return _MoveSpeed; }
		set { _MoveSpeed = value; }
	}

	public bool isEnemy {
		get { return spawnType == "enemy"; }
	}
	public bool isFly {
		get { return spawnType == "fly"; }
	}
	public bool moveEnabled = true;
	public bool isDestroyed;

	[HideInInspector]
	public int _spawnTypeIndex = 0;
	[HideInInspector]
	public int _direction= 0;
	[HideInInspector]
	public string spawnType;
	
	[HideInInspector]
	public string movementDir;
	
	[HideInInspector]
	public Vector3[] movementPoints = new Vector3[20];
	[HideInInspector]
	public float currentPathPercent = 0.0f; //min 0, max 1

//	[HideInInspector]
	public float _MoveSpeed;
	
	[Range(0, 0.5f)]
	public float localMoveSpeed = .1f;
	
	MeshRenderer rend;

	GameObject parent;
	bool moveToEnd = true;

  public Vector3 ClampToScreen(Vector3 vector) {

  	Vector3 pos = Camera.main.ScreenToWorldPoint(vector);
		pos.z = 0;

  	return pos;

  }
	
	void OnDrawGizmosSelected() {

		if(waypointStart != null && waypointEnd != null)
		{
			Gizmos.color = Color.cyan;
			Gizmos.DrawLine(waypointStart.position, waypointEnd.position);
			
			Gizmos.color = Color.green;
			Gizmos.DrawCube(waypointStart.position, new Vector3(.3f, .3f, .3f));
			Gizmos.color = Color.red;
			Gizmos.DrawCube(waypointEnd.position, new Vector3(.3f, .3f, .3f));
		}
	}

	public void Awake()
	{

		parent = new GameObject("Parent");
		parent.transform.position = transform.position;
		transform.parent = parent.transform;
		transform.localPosition = Vector3.zero;

		if(waypointStart != null && waypointEnd != null)
		{
			waypointEnd.parent = parent.transform;
			waypointStart.parent = parent.transform;
		}
	}

	// Update is called once per frame
	public void Update () {

		if(!moveEnabled)
			return;

		if(_MoveSpeed == 0)
			return;
		
		if(parent == null)
			return;

		if(spawnType != "fly") {

			Vector3 target = parent.transform.position;
			
			if(movementDir == "up")
				target.y += _MoveSpeed;
			else if(movementDir == "right")
				target.x += _MoveSpeed;
			else if(movementDir == "left")
				target.x -= _MoveSpeed;
			else
				target.y -= _MoveSpeed;

			parent.transform.position = Vector3.Lerp(parent.transform.position, target, .2f);

			if(waypointStart != null && waypointEnd != null)
			{
				if(moveToEnd)
				{
					transform.localPosition = Vector3.Lerp(transform.localPosition, waypointEnd.localPosition, localMoveSpeed);
					if(Vector3.Distance(transform.position, waypointEnd.position) < .1f)
						moveToEnd = false;

				} 
				else
				{
					
					transform.localPosition = Vector3.Lerp(transform.localPosition, waypointStart.localPosition, localMoveSpeed);
					if(Vector3.Distance(transform.position, waypointStart.position) < .1f)
						moveToEnd = true;

				}
			}

			if(Camera.main.WorldToViewportPoint(parent.transform.position).y < 0) {

				if(!isDestroyed)
					Events.instance.Raise (new ScoreEvent(1, ScoreEvent.Type.Bad));

				Destroy(gameObject);
			}

		}
		
	}

}