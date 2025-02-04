﻿using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

class Rabbit : MonoBehaviour
{
	const float ScatterSpeed = 400.0f;
	
	Malisse parent;
	tk2dAnimatedSprite sprite;
	
	public float DistanceToMalisse;
	
	public RoadWalker Walker { get; private set; }
	public bool Stunned { get; set; }
	public bool Scattering { get; private set; }
	
	Vector3 ScatterDestination;

	string lastAnimCheck;
	float sinceLastAnimChanged;
	
	string[] PerAngleAnimationMap =
	{
		"b", 
		"rb", 
		"rs", 
		"rf", 
		"f", 
		"lf", 
		"ls", 
		"lb", 
	};
	
	void Start()
	{
		transform.localRotation = Quaternion.identity;
		
		sprite = GetComponent<tk2dAnimatedSprite>();
		parent = transform.parent.GetComponent<Malisse>();
		Walker = GetComponent<RoadWalker>();
		
		Walker.RoadToWalk = GameUtils.FindAssociatedLevel(transform).transform.Find("Road").GetComponent<Spline>();
		
		Walker.Stop();
	}
	
	Vector3 lastPosition;
	Vector3 lastDirection;
	
	public void Scatter()
	{
		FlowManager.Instance.TriggerAction("LOSE_RABBIT");
		
		transform.parent = transform.parent.parent;
		
		Scattering = true;
		Walker.enabled = false;
		
		var r = Random.insideUnitCircle.normalized;
		ScatterDestination = new Vector3(r.x, 0, r.y);
	}
	
	void Update()
	{
		if (Scattering)
			transform.position += ScatterDestination * Time.deltaTime * ScatterSpeed;
		else
		{
			Walker.DistanceFromStart = parent.Walker.DistanceFromStart - DistanceToMalisse;
			foreach (var r in GetComponentsInChildren<Renderer>())
                r.enabled = Walker.DistanceFromStart > 0 && !Walker.Done;
		}

		if (!Stunned)
		{
			sinceLastAnimChanged += Time.deltaTime;
			if (lastAnimCheck != null && sinceLastAnimChanged > 0.25f)
				lastAnimCheck = null;
			UpdateDirection();
		}

		{
			// readjust height based on ground raycast
			RaycastHit hit;
			if (Physics.SphereCast(new Ray(transform.position + Vector3.up * 1080, Vector3.down), 25, out hit, 2160,
			                       1 << LayerMask.NameToLayer("WalkableObject")))
			{
				parent.RefreshWalkableCollider(hit.collider);
				Walker.GroundHeight = hit.point.y;
			}
			else
				Walker.GroundHeight = Mathf.Max(Walker.GroundHeight - Time.deltaTime * 500.0f, 0.0f);
		}
	}
	
	void UpdateDirection()
	{
		lastDirection = transform.position - lastPosition;
		lastPosition = transform.position;
		
		float angle = Mathf.Atan2(lastDirection.x, lastDirection.z);
		if (angle < 0) angle += Mathf.PI * 2;
		int index = Mathf.RoundToInt(angle / (Mathf.PI * 2) * 8);
		if (index == 8) index = 0;
		//Debug.Log("Angle : " + angle + " | Index = " + index);
		
		var animName = PerAngleAnimationMap[index];
		var lastName = sprite.CurrentClip == null ? " " : sprite.CurrentClip.name;
		if (lastName != animName && (lastAnimCheck == null || lastAnimCheck != animName))
		{
			sprite.Play(animName);
			
			if (Scattering)
				sprite.ClipFps = 19;
			
			if ((animName.StartsWith("r") && !lastName.StartsWith("r")) || (lastName.StartsWith("r") && !animName.StartsWith("r")))
			{
				sprite.FlipX();
			}

			lastAnimCheck = lastName;
			sinceLastAnimChanged = 0;
		}
	}
}