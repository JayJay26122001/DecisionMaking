using UnityEngine;
using UnityEngine.AI;

public class Heal : Action
{
    private NavMeshAgent agent;
    private Transform target;
    public void HealRefs(NavMeshAgent agentRef, Transform targetRef)
    {
        agent = agentRef;
        target = targetRef;
    }
    public override void ActiveAction()
    {
        if (agent == null || target == null) return;
        agent.SetDestination(target.position);
        
    }
}
