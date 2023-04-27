using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class ShurikenBehaviour : MonoBehaviour
{

    private bool moving;
    private Vector3 moveDirection;
    private float moveDistance;
    private float moveSpeed;

    private Vector3 startLocation;

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

    public void FireShuriken(Vector3 _direction, float _distance, float _speed)
    {
        moveDirection = _direction;
        moveDistance = _distance;
        moveSpeed = _speed;
        moving = true;
    }
}
