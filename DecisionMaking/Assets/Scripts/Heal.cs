using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class Heal : Action
{
    private NavMeshAgent agent;
    private Transform healingTarget;

    public void HealRefs(NavMeshAgent agentRef, Transform targetRef)
    {
        agent = agentRef;
        healingTarget = targetRef;
    }
    public override void ActiveAction()
    {
        if (agent == null || healingTarget == null) return;
        agent.SetDestination(healingTarget.position);
        if (Vector3.Distance(agent.transform.position, healingTarget.position) <= 0.1f)
        {
            NPC npc = agent.GetComponent<NPC>();
            npc.HealNPC();
        }

    }
}
