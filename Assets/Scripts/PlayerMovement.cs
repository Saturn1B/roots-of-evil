using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    enum WalkState
    {
        NORMAL,
        CAPTURE,
        SACRIFICE
    }

    [SerializeField] Rigidbody rb;
    [SerializeField] float speed = 5;
    [SerializeField] float turnSpeed = 360;
    Vector3 input;

    [SerializeField] Animator animator;

    WalkState walkState = WalkState.NORMAL;

    bool captured;
    [SerializeField] GameObject animalInRange = null;
    [SerializeField] Transform capturedPoints;

    bool canSacrifice;
    [SerializeField] ParticleSystem blood01;
    [SerializeField] ParticleSystem blood02;

    private void Update()
    {
        GatherInput();
        Look();
        Capture();
        Sacrifice();
    }

    private void FixedUpdate()
    {
        switch (walkState)
        {
            case WalkState.NORMAL:
                MoveNormal();
                break;
            case WalkState.CAPTURE:
                MoveCapture();
                break;
            case WalkState.SACRIFICE:
                break;
            default:
                break;
        }
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

    void Capture()
    {
        if (Input.GetKeyDown(KeyCode.E) && animalInRange != null && !captured)
        {
            //TO DO Capture
            captured = true;

            animalInRange.GetComponent<Animals>().Capture();
            animalInRange.transform.SetParent(transform);
            animalInRange.transform.position = capturedPoints.position;
            animalInRange.transform.eulerAngles = capturedPoints.eulerAngles;

            walkState = WalkState.CAPTURE;
            animator.SetBool("isRunning", false);
            animator.SetBool("isCapturing", true);
        }
    }

    void Sacrifice()
    {
        if(Input.GetKeyDown(KeyCode.E) && captured && canSacrifice)
        {
            StartCoroutine(SacrificePath());
        }
    }

    IEnumerator SacrificePath()
    {
        walkState = WalkState.SACRIFICE;

        animator.SetBool("isWalking", false);
        animator.SetBool("isRunning", false);
        animator.SetBool("isCapturing", false);

        blood01.Play();
        blood02.Play();

        Destroy(animalInRange);
        animalInRange.SetActive(false);
        captured = false;

        yield return new WaitForSeconds(1);

        walkState = WalkState.NORMAL;
    }

    void MoveNormal()
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

    void MoveCapture()
    {
        if (input.magnitude > 0)
        {
            speed = 4;
            rb.MovePosition(transform.position + /*(*/transform.forward /** input.magnitude)*/ * speed * Time.deltaTime);
            animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<Animals>() != null && !captured)
        {
            animalInRange = other.gameObject;
        }
        else if (other.CompareTag("Altar"))
        {
            canSacrifice = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject == animalInRange)
        {
            animalInRange = null;
        }

        if (other.CompareTag("Altar"))
        {
            canSacrifice = false;
        }
    }
}
