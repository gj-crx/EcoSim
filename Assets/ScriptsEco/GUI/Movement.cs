using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float Speed = 5;
    public Rigidbody2D rb;
    public Animator animator;

    private Vector2 move;

    public bool PhoneControlling = false;
    public Joystick joystick;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (PhoneControlling == false)
        {
            move.x = Input.GetAxisRaw("Horizontal");
            move.y = Input.GetAxisRaw("Vertical");
        }
        else
        {
            move.x = joystick.Horizontal;
            move.y = joystick.Vertical;
        }
        if (animator != null)
        {
            if (move.x == 0 && move.y == 0)
            {
                animator.SetFloat("Stopped", 1); //приходится передавать отдельный сигнал на переход обратно в айдл

            }
            else
            {
                animator.SetFloat("Stopped", 0);
                animator.SetFloat("Horizontal", move.x);
                animator.SetFloat("Vertical", move.y);
            }
        }
    }
    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + move * Speed * Time.fixedDeltaTime);
    }
}
