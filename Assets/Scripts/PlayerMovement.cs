using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] Rigidbody rb;
    [SerializeField] float speed = 5;
    [SerializeField] float turnSpeed = 360;
    Vector3 input;

    [SerializeField] Animator animator;

    private void Update()
    {
        GatherInput();
        Look();
    }

    private void FixedUpdate()
    {
        Move();
    }

    void GatherInput()
    {
        input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
    }

    void Look()
    {
        if (input != Vector3.zero)
        {
            var matrix = Matrix4x4.Rotate(Quaternion.Euler(0, 45, 0));

            var skewedInput = matrix.MultiplyPoint3x4(input);

            var relative = (transform.position + skewedInput) - transform.position;
            var rot = Quaternion.LookRotation(relative, Vector3.up);

            transform.rotation = /*Quaternion.RotateTowards(transform.rotation, rot, turnSpeed * Time.deltaTime);*/ rot;
        }
    }

    void Move()
    {
        if (input.magnitude > 0)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                animator.SetBool("isRunning", true);
                speed = 8;
            }
            else
            {
                animator.SetBool("isRunning", false);
                speed = 5;
            }

            rb.MovePosition(transform.position + /*(*/transform.forward /** input.magnitude)*/ * speed * Time.deltaTime);
            animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isWalking", false);
            animator.SetBool("isRunning", false);
            speed = 5;
        }
    }
}
