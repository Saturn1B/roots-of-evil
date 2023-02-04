using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;
using UnityEngine.UI;

public class Animals : MonoBehaviour
{
    [HideInInspector] enum AIStates
    {
        Idle,
        Wandering,
        Captured
    }

    [SerializeField] NavMeshAgent agent = null;
    [SerializeField] LayerMask floorMask = 0;

    [SerializeField] CapsuleCollider collider;

    [HideInInspector] AIStates curStates = AIStates.Idle;
    float waitTimer = 0.0f;

    float shakeTimer = 1.0f;
    public float escapePercent;

    // Update is called once per frame
    void Update()
    {

        switch (curStates)
        {
            case AIStates.Idle:
                DoIdle();
                break;
            case AIStates.Wandering:
                DoWander();
                break;
            case AIStates.Captured:
                DoEscape();
                break;
            default:
                break;
        }
    }

    void DoIdle()
    {
        if(waitTimer > 0)
        {
            waitTimer -= Time.deltaTime;
            return;
        }

        agent.SetDestination(RandomNavSphere(transform.position, 10.0f, floorMask));
        curStates = AIStates.Wandering;
    }

    void DoWander()
    {
        if (agent.pathStatus != NavMeshPathStatus.PathComplete)
            return;

        waitTimer = Random.Range(1.0f, 4.0f);
        curStates = AIStates.Idle;
    }

    void DoEscape()
    {
        escapePercent += Time.deltaTime * 10;

        if(escapePercent > 100)
        {
            Escape();
            return;
        }

        if (shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;
            return;
        }

        transform.DOShakeRotation(.5f, 20, 5, 60);
        shakeTimer = Random.Range(.5f, 2.0f);
    }

    void Escape()
    {
        escapePercent = 0;

        transform.parent = null;

        collider.enabled = true;
        agent.enabled = true;

        agent.SetDestination(RandomNavSphere(transform.position, 20.0f, floorMask));
        curStates = AIStates.Wandering;

        GameObject.FindObjectOfType<PlayerMovement>().LooseAnimal();
    }

    Vector3 RandomNavSphere(Vector3 origin, float distance, LayerMask layerMask)
    {
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * distance;

        randomDirection += origin;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randomDirection, out navHit, distance, layerMask);

        return navHit.position;
    }

    public void Capture()
    {
        curStates = AIStates.Captured;
        collider.enabled = false;
        agent.enabled = false;
    }

    public void Block()
    {
        if (escapePercent < 0)
        {
            escapePercent = 0;
            return;
        }

        escapePercent -= 3.5f;
    }
}
