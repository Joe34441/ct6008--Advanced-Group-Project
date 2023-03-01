using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonCameraController : MonoBehaviour
{

	public CameraDistanceRaycaster camRaycaster;
	public Transform camParent;

	float currentXAngle = 0f;
	float currentYAngle = 0f;

	//values clamp how high the camera can look up and down
	[Range(0f, 90f)]
	public float upperVerticalLimit = 60f;
	[Range(0f, 90f)]
	public float lowerVerticalLimit = 60f;

	//variables to store old rotation values for interpolation purposes
	float oldHorizontalInput = 0f;
	float oldVerticalInput = 0f;

	public float cameraSpeed = 5.0f;

	//controls how smoothly the old camera rotation angles will be interpolated toward the new camera rotation angles
	[Range(1f, 50f)]
	public float cameraSmoothingFactor = 25f;

	Vector3 facingDirection;
	Vector3 upwardsDirection;

	//transform and camera components
	private Transform tr;
	private Camera cam;

	//input variables
	protected Vector2 mouseVector;

	void Awake()
	{
		tr = transform;
		cam = GetComponent<Camera>();

		//If no camera component has been attached to this gameobject, search the transform's children
		if (cam == null)
			cam = GetComponentInChildren<Camera>();

		//Set angle variables to current rotation angles of this transform
		currentXAngle = tr.localRotation.eulerAngles.x;
		currentYAngle = tr.localRotation.eulerAngles.y;

		//Execute camera rotation code once to calculate facing and upwards direction
		RotateCamera(0f, 0f);

		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

	public void SetupCameras(Camera _camera)
    {
		if(cam != _camera)
        {
			cam = _camera;
        }
		camRaycaster.cameraTransform = _camera.transform;
		cam.transform.parent = camParent;
		cam.transform.position = new Vector3(0, 0, 0);
    }

	void Update()
	{
		HandleCameraRotation();
	}

	public void NetworkedLookInput(Vector2 input)
    {
		RotateCamera(input.x, -input.y);
    }

	protected virtual void HandleCameraRotation()
	{
		return; //see above

		//get input values
		float _inputHorizontal = mouseVector.x;
		float _inputVertical = -mouseVector.y;

		RotateCamera(_inputHorizontal, _inputVertical);
	}

	protected void RotateCamera(float _newHorizontalInput, float _newVerticalInput)
	{
		//replace old input directly
		oldHorizontalInput = _newHorizontalInput;
		oldVerticalInput = _newVerticalInput;

		//add input to camera angles
		currentXAngle += oldVerticalInput * (cameraSpeed / 5);
		currentYAngle += oldHorizontalInput * (cameraSpeed / 5);

		//clamp vertical rotation
		currentXAngle = Mathf.Clamp(currentXAngle, -upperVerticalLimit, lowerVerticalLimit);

		UpdateRotation();
	}

	//update camera rotation based on x and y angles
	protected void UpdateRotation()
	{
		tr.localRotation = Quaternion.Euler(new Vector3(0, currentYAngle, 0));

		//save facingDirection and upwardsDirection for later
		facingDirection = tr.forward;
		upwardsDirection = tr.up;

		tr.localRotation = Quaternion.Euler(new Vector3(currentXAngle, currentYAngle, 0));
	}

	public void SetFOV(float _fov)
	{
		if (cam)
        {
			cam.fieldOfView = _fov;
		}
	}

	public void SetRotationAngles(float _xAngle, float _yAngle)
	{
		currentXAngle = _xAngle;
		currentYAngle = _yAngle;

		UpdateRotation();
	}

	public void RotateTowardPosition(Vector3 _position, float _lookSpeed)
	{
		//calculate target look vector
		Vector3 _direction = (_position - tr.position);

		RotateTowardDirection(_direction, _lookSpeed);
	}

	public void RotateTowardDirection(Vector3 _direction, float _lookSpeed)
	{
		//normalize direction
		_direction.Normalize();

		//transform target look vector to this transform's local space
		_direction = tr.parent.InverseTransformDirection(_direction);

		//calculate (local) current look vector
		Vector3 _currentLookVector = GetAimingDirection();
		_currentLookVector = tr.parent.InverseTransformDirection(_currentLookVector);

		//calculate x angle difference
		float _xAngleDifference = GetAngle(new Vector3(0f, _currentLookVector.y, 1f), new Vector3(0f, _direction.y, 1f), Vector3.right);

		//calculate y angle difference
		_currentLookVector.y = 0f;
		_direction.y = 0f;
		float _yAngleDifference = GetAngle(_currentLookVector, _direction, Vector3.up);

		//turn angle values into Vector2 variables for better clamping
		Vector2 _currentAngles = new Vector2(currentXAngle, currentYAngle);
		Vector2 _angleDifference = new Vector2(_xAngleDifference, _yAngleDifference);

		//calculate normalized direction
		float _angleDifferenceMagnitude = _angleDifference.magnitude;
		if (_angleDifferenceMagnitude == 0f)
			return;
		Vector2 _angleDifferenceDirection = _angleDifference / _angleDifferenceMagnitude;

		//check for overshooting
		if (_lookSpeed * Time.deltaTime > _angleDifferenceMagnitude)
		{
			_currentAngles += _angleDifferenceDirection * _angleDifferenceMagnitude;
		}
		else
			_currentAngles += _angleDifferenceDirection * _lookSpeed * Time.deltaTime;

		//set new angles
		currentYAngle = _currentAngles.y;
		//clamp vertical rotation
		currentXAngle = Mathf.Clamp(_currentAngles.x, -upperVerticalLimit, lowerVerticalLimit);

		UpdateRotation();
	}

	public static float GetAngle(Vector3 _vector1, Vector3 _vector2, Vector3 _planeNormal)
	{
		//calculate angle and sign
		float _angle = Vector3.Angle(_vector1, _vector2);
		float _sign = Mathf.Sign(Vector3.Dot(_planeNormal, Vector3.Cross(_vector1, _vector2)));

		//combine angle and sign
		float _signedAngle = _angle * _sign;

		return _signedAngle;
	}

	public float GetCurrentXAngle()
	{
		return currentXAngle;
	}

	public float GetCurrentYAngle()
	{
		return currentYAngle;
	}

	public Vector3 GetFacingDirection()
	{
		return facingDirection;
	}

	public Vector3 GetAimingDirection()
	{
		return tr.forward;
	}
	public Vector3 GetStrafeDirection()
	{
		return tr.right;
	}

	public Vector3 GetUpDirection()
	{
		return upwardsDirection;
	}

	public void OnMouseMove(InputAction.CallbackContext context)
    {
		mouseVector = context.ReadValue<Vector2>();
    }

}
