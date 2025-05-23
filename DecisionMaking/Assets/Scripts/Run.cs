using UnityEngine;
using UnityEngine.AI;

public class Run : Action
{
    private NavMeshAgent agent;
    private Transform target;
    public void RunRefs(NavMeshAgent agentRef, Transform targetRef)
    {
        agent = agentRef;
        target = targetRef;
    }
    public override void ActiveAction()
    {
        if (agent == null || target == null) return;
        Vector3 directionToTarget = agent.transform.position - target.position;
        Vector3 runPosition = agent.transform.position + directionToTarget;
        agent.SetDestination(runPosition);
        if (agent.GetComponent<NPC>() != null)
        {
            if (agent.GetComponent<NPC>().algorithm == DecisionMakingAlgorithm.FSMWithTree)
            {
                agent.GetComponent<NPC>().state = States.Run;
            }
            agent.GetComponent<NPC>().ChangeColor(Color.yellow);
        }
    }
}
