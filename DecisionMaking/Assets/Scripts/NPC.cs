using UnityEngine;
using UnityEngine.AI;

public enum DecisionMakingAlgorithm
{
    FSM,
    DecisionTree,
    NFSM,
    PFSM
}

public class NPC : MonoBehaviour
{
    public DecisionMakingAlgorithm algorithm;
    public int health, maxHealth;
    public GameObject target, healingTarget;
    public bool isViewingPlayer, isViewingHealing;
    public float visionRange, fieldOfViewAngle;
    public LayerMask visionMask;
    public Transform[] patrolWaypoints;
    private DecisionNode Root;
    private NavMeshAgent agent;
    private Chase chaseAction;
    private Run runAction;
    private Patrol patrolAction;
    private Heal healAction;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        InitializeActions();
        NPCBehaviour();
        Debug.Log(algorithm.ToString() + " criada");
        health = maxHealth;
    }

    void Update()
    {
        CheckForTargets();
        if (Input.GetKeyDown(KeyCode.X))
        {
            health--;
        }
        Root.MakeDecision().Execute();
    }

    void CheckForTargets()
    {
        if (CheckForTargetInCone(target))
        {
            isViewingPlayer = true;
        }
        else
        {
            isViewingPlayer = false;
        }
        if (CheckForTargetInCone(healingTarget))
        {
            isViewingHealing = true;
        }
        else
        {
            isViewingHealing = false;
        }
    }

    public bool CheckForTargetInCone(GameObject targetObject)
    {
        RaycastHit hit;
        Vector3 directionToTarget = (targetObject.transform.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, directionToTarget);
        if (angle <= fieldOfViewAngle / 2)
        {
            if (Physics.Raycast(transform.position, directionToTarget, out hit, visionRange, visionMask))
            {
                if (hit.collider.gameObject == targetObject)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "HealingOrb")
        {
            this.health += 3;
            if(health > maxHealth)
            {
                health = maxHealth;
            }
        }
    }

    public void InitializeActions()
    {
        runAction = new Run();
        runAction.RunRefs(agent, target.transform);
        patrolAction = new Patrol();
        patrolAction.PatrolRefs(agent, patrolWaypoints);
        chaseAction = new Chase();
        chaseAction.ChaseRefs(agent, target.transform);
        healAction = new Heal();
    }

    public void DecisionTree()
    {
        ActionNode run = new ActionNode(runAction);
        ActionNode patrol = new ActionNode(patrolAction);
        ActionNode chase = new ActionNode(chaseAction);
        ActionNode heal = new ActionNode(new Heal());
        DecisionNode layer2num1 = new DecisionNode(() => isViewingPlayer, run, patrol);
        DecisionNode layer1num1 = new DecisionNode(() => isViewingPlayer, chase, patrol);
        DecisionNode layer1num2 = new DecisionNode(() => isViewingHealing, heal, layer2num1);
        Root = new DecisionNode(() => (health <= 5), layer1num2, layer1num1);
    }
    public void NPCBehaviour()
    {
        switch (algorithm)
        {
            case DecisionMakingAlgorithm.FSM:

                break;
            case DecisionMakingAlgorithm.DecisionTree:
                DecisionTree();
                break;
            case DecisionMakingAlgorithm.NFSM:

                break;
            case DecisionMakingAlgorithm.PFSM:

                break;
        }
    }
}
