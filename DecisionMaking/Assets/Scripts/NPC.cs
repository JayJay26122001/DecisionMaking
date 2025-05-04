using UnityEngine;
using UnityEngine.AI;
using TMPro;
using System.Collections;

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
    public bool isPlayerNear, isHealingNear;
    public float safeDistance;
    public LayerMask visionMask;
    public Transform[] patrolWaypoints;
    private DecisionNode Root;
    private NavMeshAgent agent;
    private Chase chaseAction;
    private Run runAction;
    private Patrol patrolAction;
    private Heal healAction;
    public TextMeshProUGUI healthText;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        InitializeActions();
        NPCBehaviour();
        Debug.Log(algorithm.ToString() + " criada");
    }
    void Start()
    {
        health = maxHealth;
        UpdateHealthText();
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            health--;
            UpdateHealthText();
        }
        CheckForTargets();
        if(algorithm == DecisionMakingAlgorithm.DecisionTree) 
            Root.MakeDecision().Execute();
    }

    public void UpdateHealthText()
    {
        if (healthText != null)
        {
            healthText.text = $"NPC Health : {health} / {maxHealth}";
        }
    }

    public void CheckForTargets()
    {
        if(Vector3.Distance(transform.position, target.transform.position) < safeDistance)
        {
            isPlayerNear = true;
        }
        else
        {
            isPlayerNear = false;
        }
        if(Vector3.Distance(transform.position, healingTarget.transform.position) < safeDistance)
        {
            isHealingNear = true;
        }
        else
        {
            isHealingNear = false;
        }
    }

    public void HealNPC()
    {
        health += 3;
        if (health > maxHealth)
            health = maxHealth;
        UpdateHealthText();
        healingTarget.gameObject.SetActive(false);
        healingTarget.gameObject.GetComponent<Collider>().enabled = false;
        StartCoroutine(ReactivateHealingTarget());
    }

    public IEnumerator ReactivateHealingTarget()
    {
        yield return new WaitForSeconds(5f);
        healingTarget.gameObject.SetActive(true);
        healingTarget.gameObject.GetComponent<Collider>().enabled = true;
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
        healAction.HealRefs(agent, healingTarget.transform);
    }

    public void DecisionTree()
    {
        ActionNode run = new ActionNode(runAction);
        ActionNode patrol = new ActionNode(patrolAction);
        ActionNode chase = new ActionNode(chaseAction);
        ActionNode heal = new ActionNode(healAction);
        DecisionNode layer2num1 = new DecisionNode(() => isPlayerNear, run, patrol);
        DecisionNode layer1num1 = new DecisionNode(() => isPlayerNear, chase, patrol);
        DecisionNode layer1num2 = new DecisionNode(() => isHealingNear, heal, layer2num1);
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
