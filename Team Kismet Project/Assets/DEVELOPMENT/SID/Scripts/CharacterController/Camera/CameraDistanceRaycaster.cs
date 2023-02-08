using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDistanceRaycaster : MonoBehaviour
{

	public Transform cameraTransform;
	
	public Transform cameraTargetTransform;

	Transform currentTransform;

	//layermask used for raycasting
	public LayerMask layerMask = 0;

	//layer number for Ignore Raycast layer
	int ignoreRaycastLayer;

	//list of colliders to ignore when raycasting
	public Collider[] ignoreList;

	//array to store layers of colliders in ignore list
	int[] ignoreListLayers;

	float currentDistance;

	//additional distance which is added to the raycast's length to stop the camera from clipping into the level
	public float minimumDistanceFromObstacles = 0.1f;

	public float smoothingFactor = 25f;

	void Awake()
	{
		currentTransform = transform;

		//setup array to store ignore list layers
		ignoreListLayers = new int[ignoreList.Length];

		//store ignore layer number for later
		ignoreRaycastLayer = LayerMask.NameToLayer("Ignore Raycast");

		if (layerMask == (layerMask | (1 << ignoreRaycastLayer)))
		{
			layerMask ^= (1 << ignoreRaycastLayer);
		}

		if (cameraTransform == null)
        {
			Debug.LogWarning("No camera transform has been assigned.", this);
		}
			
		if (cameraTargetTransform == null)
        {
			Debug.LogWarning("No camera target transform has been assigned.", this);
		}

		if (cameraTransform == null || cameraTargetTransform == null)
		{
			this.enabled = false;
			return;
		}

		//set intial starting distance
		currentDistance = (cameraTargetTransform.position - currentTransform.position).magnitude;
	}

	void LateUpdate()
	{

		//move all objects in ignore list to 'Ignore Raycast' layer and store their layer value for later
		for (int i = 0; i < ignoreList.Length; i++)
		{
			ignoreListLayers[i] = ignoreList[i].gameObject.layer;
			ignoreList[i].gameObject.layer = ignoreRaycastLayer;
		}

		//calculate current distance by casting a raycast
		float _distance = GetCameraDistance();

		//reset layers
		for (int i = 0; i < ignoreList.Length; i++)
		{
			ignoreList[i].gameObject.layer = ignoreListLayers[i];
		}

		currentDistance = Mathf.Lerp(currentDistance, _distance, Time.deltaTime * smoothingFactor);

		//set new position of cameraTransform
		cameraTransform.position = currentTransform.position + (cameraTargetTransform.position - currentTransform.position).normalized * currentDistance;

	}

	//calculate maximum distance by casting a ray from this transform to the camera target transform
	float GetCameraDistance()
	{
		RaycastHit outHit;

		//calculate cast direction
		Vector3 castDirection = cameraTargetTransform.position - currentTransform.position;
		//cast ray
		if (Physics.Raycast(new Ray(currentTransform.position, castDirection), out outHit, castDirection.magnitude + minimumDistanceFromObstacles, layerMask, QueryTriggerInteraction.Ignore))
		{
			if (outHit.distance - minimumDistanceFromObstacles < 0f)
            {
				return outHit.distance;
			}
			else
            {
				return outHit.distance - minimumDistanceFromObstacles;
			}	
		}
		//if no obstacle was hit, return full distance
		return castDirection.magnitude;
	}
}
