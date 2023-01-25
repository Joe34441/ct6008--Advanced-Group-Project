using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class NewPlayerController : NetworkBehaviour
{

    public float moveSpeed;
    public float jumpForce;

    public bool grounded;

    private string groundTag = "Ground";

    private Player _player;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    public override void Spawned()
    {
        _player = App.Instance.GetPlayer(Object.InputAuthority);
        //_name.text = _player.Name.Value;
        //_mesh.material.color = _player.Color;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag(groundTag)) grounded = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(groundTag)) grounded = false;
    }

    private void FixedUpdate()
    {
        if (_player && _player.InputEnabled &&  GetInput(out InputData data))
        {
            if (data.GetButton(ButtonFlag.LEFT))
                Debug.Log("left");
            if (data.GetButton(ButtonFlag.RIGHT))
                Debug.Log("right");
            if (data.GetButton(ButtonFlag.FORWARD))
                Debug.Log("forward");
            if (data.GetButton(ButtonFlag.BACKWARD))
                Debug.Log("backward");
        }

        float horizontal = Input.GetAxis("Horizontal");
        //Debug.Log(horizontal);

        rb.velocity = new Vector2(horizontal * moveSpeed, rb.velocity.y);
     }
}
