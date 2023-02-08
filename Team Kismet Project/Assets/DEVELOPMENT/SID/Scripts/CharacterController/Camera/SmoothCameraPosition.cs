using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothCameraPosition : MonoBehaviour
{
	//the target transform, whose position values will be copied and smoothed
	public Transform target;
	Transform currentTransform;

	Vector3 currentPosition;

	public float lerpSpeed = 20f;

	public float smoothDampTime = 0.02f;

	//local position offset at the start of the game
	Vector3 localPositionOffset;

	Vector3 refVelocity;
	void Awake()
	{

		//if no target has been selected, set parent as the target
		if (target == null)
        {
			target = this.transform.parent;
		}
			
		currentTransform = transform;
		currentPosition = transform.position;

		localPositionOffset = currentTransform.localPosition;
	}

	void OnEnable()
	{
		ResetCurrentPosition();
	}

	void Update()
	{
		//smooth current position
		currentPosition = Smooth(currentPosition, target.position);

		//set position
		currentTransform.position = currentPosition;
	}
	Vector3 Smooth(Vector3 startPos, Vector3 targetPos)
	{
		//convert local position offset to world coordinates
		Vector3 offset = currentTransform.localToWorldMatrix * localPositionOffset;

		//add local position offset to target
		targetPos += offset;

		return Vector3.SmoothDamp(startPos, targetPos, ref refVelocity, smoothDampTime);
	}

	//reset stored position and move this gameobject to the targets position
	public void ResetCurrentPosition()
	{
		//convert local position offset to world coordinates
		Vector3 offset = currentTransform.localToWorldMatrix * localPositionOffset;
		currentPosition = target.position + offset;
	}
}
