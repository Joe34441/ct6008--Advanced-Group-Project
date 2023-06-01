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
                //stop the animation after a small amount of time to ensure it starts playing,
                //will have exit time on the anim blueprint
                if(timeSinceStart >= 0.1f)
                {
                    if (animator.GetBool("StartGrapple") == true)
                    {
                        animator.SetBool("StartGrapple", false);
                    }
                }

                grappleCable.transform.position = grapplePoint.transform.position;
                timeSinceStart += Time.deltaTime;
                //get a number between 0 and 1 to apply to the vector lerp for smooth travel along the zipline
                float travelPercent = timeSinceStart / travelTime;
                //end the zipline slightly early to avoid the player getting stuck
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
        //use this obj to perfrom the raycast and avoid issues with the current camera setup
        GameObject obj = new GameObject();
        obj.transform.position = cameraReference.transform.position;
        obj.transform.LookAt(raycastRef.transform);
        
        //perform a raycast to find the end point of the zipline
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

            //ensure the connection point is atleast at 2 on the y axis to avoid the player sinking into the floor
            if(connectionPoint.y < 2f)
            {
                connectionPoint.y = 2f;
            }

            //stop the player moving when ziplining
            playerController.movementDisabled = true;

            startPoint = playerRef.transform.position;
            //calculate how long it will take to travel the length of the zipline
            float distanceToGrapple = Vector3.Distance(playerRef.transform.position, connectionPoint);
            travelTime = distanceToGrapple / grappleSpeed;

            timeSinceStart = 0;
            //start animations
            animator.SetBool("StartGrapple",true);
            
            animator.SetBool("Grappling", true);
            connected = true;
            shouldUpdate = true;
            activated = true;
        }
    }

    //old initialize, ignore
    public override void Initialize(GameObject _playerRef, Camera _camera)
    {
        playerCamera = _camera;
        playerRef = _playerRef;
    }

    public void Initialize(GameObject _playerRef, Camera _camera, Transform _cameraRef, PlayerCharacterController _playerController, LayerMask _hitList, GameObject _cablePrefab, float _grappleSpeed, float _grappleDistance, Animator _animator, GameObject _grapplePoint)
    {
        //setup all the components
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
        
    }

    public override void DeactivateAbility()
    {
        Destroy(currentCable);
        currentCable = null;
        grappleCable = null;
        //re-enable movement
        playerController.movementDisabled = false;
        //stop animations
        animator.SetBool("Grappling", false);
        shouldUpdate = false;
        onCooldown = true;
        activated = false;

    }
}