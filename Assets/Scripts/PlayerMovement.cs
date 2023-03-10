using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    [SerializeField] Slider escapePercentSlider;
    [SerializeField] GameObject canvas;

    bool firstSacrifice;
    [SerializeField] GameObject sacrificeText;

    bool firstBlock;
    [SerializeField] GameObject blockText;

    bool firstCapture;
    [SerializeField] GameObject captureText;

    [SerializeField] AudioClip sacrifice;
    [SerializeField] AudioClip capture;
    [SerializeField] AudioClip escape;

    Altar altar;

    private void Awake()
    {
        altar = FindObjectOfType<Altar>();
    }

    private void Update()
    {
        if (altar.movementEnd)
            return;

        GatherInput();
        Look();
        Capture();
        Sacrifice();
        Block();
        UpdateSlider();
    }

    private void FixedUpdate()
    {
        if (altar.movementEnd)
            return;

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
            captured = true;
            canvas.SetActive(true);
            captureText.SetActive(false);
            if(!firstCapture)
                firstCapture = true;

            animalInRange.GetComponent<Animals>().Capture();
            animalInRange.transform.SetParent(transform);
            animalInRange.transform.position = capturedPoints.position;
            animalInRange.transform.eulerAngles = capturedPoints.eulerAngles;

            walkState = WalkState.CAPTURE;
            animator.SetBool("isWalking", false);
            animator.SetBool("isRunning", false);
            animator.SetBool("isCapturing", true);

            SoundManager.Instance.Play(capture);

            if (!firstBlock)
            {
                blockText.SetActive(true);
                firstBlock = true;
            }
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
        Debug.LogWarning("sacrifice animal");

        SoundManager.Instance.Play(sacrifice);

        sacrificeText.SetActive(false);

        walkState = WalkState.SACRIFICE;

        animator.SetBool("isWalking", false);
        animator.SetBool("isRunning", false);
        animator.SetBool("isCapturing", false);

        FindObjectOfType<Altar>().Corrupt();

        Destroy(animalInRange);
        animalInRange.SetActive(false);
        captured = false;

        canvas.SetActive(false);

        yield return new WaitForSeconds(.5f);

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
            speed = 3;
            rb.MovePosition(transform.position + /*(*/transform.forward /** input.magnitude)*/ * speed * Time.deltaTime);
            animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isWalking", false);
            if (Input.GetKeyDown(KeyCode.Q))
            {
                animalInRange.GetComponent<Animals>().Block();
            }
        }
    }

    void Block()
    {
        if (input.magnitude <= 0 && captured)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                blockText.SetActive(false);

                animalInRange.GetComponent<Animals>().Block();
            }
        }
    }

    void UpdateSlider()
    {
        if (captured)
        {
            escapePercentSlider.value = 1 - (animalInRange.GetComponent<Animals>().escapePercent / 100);
        }
    }

    public void LooseAnimal()
    {
        SoundManager.Instance.Play(escape);
        blockText.SetActive(false);
        canvas.SetActive(false);
        animator.SetBool("isCapturing", false);
        captured = false;
        animalInRange = null;
        walkState = WalkState.NORMAL;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<Animals>() != null && !captured)
        {
            animalInRange = other.gameObject;
            if(!firstCapture)
            {
                captureText.SetActive(true);
            }
        }
        else if (other.CompareTag("Altar"))
        {
            canSacrifice = true;

            if (captured && !firstSacrifice)
            {
                blockText.SetActive(false);
                sacrificeText.SetActive(true);
                firstSacrifice = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject == animalInRange)
        {
            animalInRange = null;
            captureText.SetActive(false);
        }

        if (other.CompareTag("Altar"))
        {
            canSacrifice = false;
        }
    }
}
