using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothCameraRotation : MonoBehaviour
{

	//target rotation values will be copied and smoothed
	public Transform target;
	Transform currentTransform;

	Quaternion currentRotation;

	public float smoothSpeed = 20f;

	void Awake()
	{
		if (target == null)
			target = this.transform.parent;

		currentTransform = transform;
		currentRotation = transform.rotation;
	}

	void OnEnable()
	{
		ResetCurrentRotation();
	}

	void Update()
	{
		SmoothUpdate();
	}

	void SmoothUpdate()
	{
		//smooth current rotation
		currentRotation = Smooth(currentRotation, target.rotation, smoothSpeed);

		//set rotation
		currentTransform.rotation = currentRotation;
	}
	Quaternion Smooth(Quaternion currentRot, Quaternion targetRot, float smoothSpeed)
	{
		//slerp rotation and return
		return Quaternion.Slerp(currentRot, targetRot, Time.deltaTime * smoothSpeed);
	}

	//reset stored rotation and rotate this gameobject to macth the target's rotation
	public void ResetCurrentRotation()
	{
		currentRotation = target.rotation;
	}
}
