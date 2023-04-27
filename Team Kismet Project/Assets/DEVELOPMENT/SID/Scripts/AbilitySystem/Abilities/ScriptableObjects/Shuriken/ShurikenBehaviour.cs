using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class ShurikenBehaviour : MonoBehaviour
{

    private bool moving;
    private Vector3 moveDirection;
    private float lifetime = 10.0f;
    private float moveSpeed;



    private Vector3 startLocation;

    private BoxCollider collision;
    private GameObject whoFired;

    // Start is called before the first frame update
    void Start()
    {
        startLocation = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(moving)
        {
            transform.position += moveDirection * moveSpeed * App.Instance.GetPlayer().Runner.DeltaTime;
        }
    }

    public void FireShuriken(Vector3 _direction, float _distance, float _speed, GameObject _whoFired)
    {
        moveDirection = _direction;
        moveSpeed = _speed;
        moving = true;
        whoFired = _whoFired;
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "player" && other.gameObject != whoFired)
        {
            other.gameObject.GetComponent<PlayerCharacterController>().movementDisabled = true;
            other.gameObject.GetComponent<PlayerCharacterController>().Invoke("ResetMovement", 3.0f);
        }
    }
}
