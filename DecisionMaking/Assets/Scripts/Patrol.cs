using UnityEngine;
using UnityEngine.AI;

public class Patrol : Action
{
    private NavMeshAgent agent;
    private Transform[] waypoints;
    private int currentWaypointIndex = 0;
    public void PatrolRefs(NavMeshAgent agentRef, Transform[] waypointsRef)
    {
        agent = agentRef;
        waypoints = waypointsRef;
    }
    public override void ActiveAction()
    {
        if (agent == null || waypoints.Length == 0) return;
        if (Vector3.Distance(agent.transform.position, waypoints[currentWaypointIndex].position) < 1f)
        {
            currentWaypointIndex++;
            if (currentWaypointIndex >= waypoints.Length)
                currentWaypointIndex = 0;
        }
        agent.SetDestination(waypoints[currentWaypointIndex].position);
    }
}
