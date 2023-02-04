using UnityEngine;
using UnityEngine.AI;

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
}
