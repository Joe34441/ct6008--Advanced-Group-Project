using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IcePillarBehaviour : MonoBehaviour
{

    private Vector3 upPosition;
    private Vector3 downPosition;
    private bool started;

    public float pillarRaiseTime;
    public float pillarLifeTime;
    private float startTimeRef;

    public LayerMask whatIsGround;

    // Start is called before the first frame update
    void Start()
    {
        //RaycastHit hit;
        //bool hitFloor = Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, whatIsGround);
        //if(hitFloor)
        //{
        //    upPosition = transform.position;
        //    downPosition = new Vector3(hit.point.x, hit.point.y - 1.5f, hit.point.z);
        //    transform.position = downPosition;
        //    startTimeRef = Time.time;
        //    started = true;
        //    Destroy(gameObject, pillarRaiseTime + pillarLifeTime);
        //}

        upPosition = transform.position;
        downPosition = new Vector3(transform.position.x, transform.position.y - 3.5f, transform.position.z);
        transform.position = downPosition;
        startTimeRef = Time.time;
        started = true;
        Destroy(gameObject, pillarRaiseTime + pillarLifeTime);
    }

    // Update is called once per frame
    void Update()
    {
        if(started)
        {
            float progress = (Time.time - startTimeRef) / pillarRaiseTime;
            transform.position = Vector3.Lerp(downPosition, upPosition, progress);
        }
    }
}
