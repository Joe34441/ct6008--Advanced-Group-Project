using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Grapple Hook")]
public class GrappleHook : Ability 
{

    [SerializeField] private bool offline = false;

    public float maxGrappleDistance = 20.0f;
    public float minGrappleDistance = 3.0f;
    public LayerMask hitList;
    public GameObject cablePrefab;
    private Vector3 connectionPoint;

    private bool connected = false;
    private PlayerCharacterController playerController;
    private CableComponent grappleCable;
    private GameObject currentCable;

    private Vector3 startPoint;
    public float grappleSpeed = 3.5f;
    private float travelTime;
    private float timeSinceStart;

    private GameObject raycastRef;
    private Animator animator;
    private GameObject grapplePoint;

    public override void Update()
    {
        if (connected)
        {
            if (grappleCable)
            {
                if(timeSinceStart >= 0.1f)
                {
                    if (animator.GetBool("StartGrapple") == true)
                    {
                        animator.SetBool("StartGrapple", false);
                    }
                }

                grappleCable.transform.position = grapplePoint.transform.position;
                timeSinceStart += Time.deltaTime;
                float travelPercent = timeSinceStart / travelTime;
                if (travelPercent >= 0.9f)
                {
                    connected = false;
                    DeactivateAbility();
                    return;
                }
                //playerController.transform.position = Vector3.Lerp(startPoint, connectionPoint, travelPercent);
                playerRef.transform.position = Vector3.Lerp(startPoint, connectionPoint, travelPercent);
                playerRef.transform.rotation = Quaternion.LookRotation(connectionPoint);
            }
        }
    }

    public override void ActivateAbility()
    {
        RaycastHit hit;
        GameObject obj = new GameObject();
        obj.transform.position = cameraReference.transform.position;
        obj.transform.LookAt(raycastRef.transform);
        //if (Physics.Raycast(playerCamera.transform.position, obj.transform.forward, out hit, maxGrappleDistance, hitList))
        if (Physics.Raycast(raycastRef.transform.position, obj.transform.forward, out hit, maxGrappleDistance, hitList))
        {
            connectionPoint = hit.point;
            //early out if the grapple is too close to the player
            if (Vector3.Distance(raycastRef.transform.position, connectionPoint) < minGrappleDistance) return;
            //currentCable is a gameobject, grappleCable is the actual cable class itself
            currentCable = Instantiate(cablePrefab);
            currentCable.transform.position = grapplePoint.transform.position;
            grappleCable = currentCable.GetComponentInChildren<CableComponent>();
            grappleCable.SetEndLocation(connectionPoint);

            if(connectionPoint.y < 2f)
            {
                connectionPoint.y = 2f;
            }

            playerController.movementDisabled = true;

            startPoint = playerRef.transform.position;

            float distanceToGrapple = Vector3.Distance(playerRef.transform.position, connectionPoint);
            travelTime = distanceToGrapple / grappleSpeed;

            timeSinceStart = 0;
            animator.SetBool("StartGrapple",true);
            
            animator.SetBool("Grappling", true);
            connected = true;
            shouldUpdate = true;
            activated = true;
        }
    }

    public override void Initialize(GameObject _playerRef, Camera _camera)
    {
        playerCamera = _camera;
        playerRef = _playerRef;
        //if(offline)
        //{
        //    _playerController = _playerRef.GetComponent<ThirdPersonMovement>();
        //}
        //else
        //{
        //    playerController = _playerRef.GetComponent<PlayerController>();
        //}
    }

    public void Initialize(GameObject _playerRef, Camera _camera, Transform _cameraRef, PlayerCharacterController _playerController, LayerMask _hitList, GameObject _cablePrefab, float _grappleSpeed, float _grappleDistance, Animator _animator, GameObject _grapplePoint)
    {
        playerRef = _playerRef;
        playerCamera = _camera;
        cameraReference = _cameraRef;
        hitList = _hitList;
        cablePrefab = _cablePrefab;
        grappleSpeed = _grappleSpeed;
        maxGrappleDistance = _grappleDistance;
        playerController = _playerController;
        raycastRef = playerRef.GetComponent<Character>().camRaycastReference;
        onCooldown = false;
        animator = _animator;
        grapplePoint = _grapplePoint;
    }

    public override void Released()
    {
        //DeactivateAbility();
    }

    public override void DeactivateAbility()
    {
        Destroy(currentCable);
        currentCable = null;
        grappleCable = null;

        playerController.movementDisabled = false;

        animator.SetBool("Grappling", false);
        shouldUpdate = false;
        onCooldown = true;
        activated = false;

    }
}