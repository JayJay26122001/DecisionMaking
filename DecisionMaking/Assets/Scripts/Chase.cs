using UnityEngine;
using UnityEngine.AI;

public class Chase : Action
{
    private NavMeshAgent agent;
    private Transform target;
    public void ChaseRefs(NavMeshAgent agentRef, Transform targetRef)
    {
        agent = agentRef;
        target = targetRef;
    }
    public override void ActiveAction()
    {
        if (agent == null || target == null) return;

        agent.SetDestination(target.position);
        if (agent.GetComponent<NPC>() != null)
        {
            agent.GetComponent<NPC>().ChangeColor(Color.red);
        }
    }
}
