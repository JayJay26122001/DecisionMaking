using UnityEngine;
using UnityEngine.AI;
using TMPro;
using System.Collections;

public enum DecisionMakingAlgorithm
{
    FSM,
    FSMWithTree,
    DecisionTree,
    NFSM,
    PFSM
}

public enum States
{
    Patrol,
    Chase,
    Heal,
    Run
}

public class NPC : MonoBehaviour
{
    public DecisionMakingAlgorithm algorithm;
    public States state;
    public int health, maxHealth;
    public GameObject target, healingTarget;
    public bool isPlayerNear, isHealingNear;
    public float safeDistance, distanceToPlayer, distanceToHeal;
    public LayerMask visionMask;
    public Transform[] patrolWaypoints;
    private DecisionNode Root;
    private NavMeshAgent agent;
    private Chase chaseAction;
    private Run runAction;
    private Patrol patrolAction;
    private Heal healAction;
    public TextMeshProUGUI healthText;
    int rand;
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
            if(health > 0)
            {
                health--;
            }
            UpdateHealthText();
        }
        CheckForTargets();
        switch(algorithm)
        {
            case DecisionMakingAlgorithm.DecisionTree:
                Root.MakeDecision().Execute();
                break;

            default:
                switch(state)
                {
                    case States.Patrol:
                        patrolAction.ActiveAction();
                        break;

                    case States.Chase:
                        chaseAction.ActiveAction();
                        break;
                        
                    case States.Heal:
                        healAction.ActiveAction();
                        break;

                    case States.Run:
                        runAction.ActiveAction();
                        break;
                }
                break;
        }
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
        distanceToPlayer = Vector3.Distance(transform.position, target.transform.position);
        isPlayerNear = distanceToPlayer < safeDistance;
        distanceToHeal = Vector3.Distance(transform.position, healingTarget.transform.position);
        isHealingNear = distanceToHeal < safeDistance;
        /*if (Vector3.Distance(transform.position, target.transform.position) < safeDistance)
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
        }*/
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
                state = States.Patrol;
                StartCoroutine(FSMThink());
                break;
            case DecisionMakingAlgorithm.FSMWithTree:
                state = States.Patrol;
                FSMDecisionTree();
                StartCoroutine(FSMWithTreeThink());
                break;
            case DecisionMakingAlgorithm.DecisionTree:
                DecisionTree();
                break;
            case DecisionMakingAlgorithm.NFSM:
                state = States.Patrol;
                StartCoroutine(NFSMThink());
                break;
            case DecisionMakingAlgorithm.PFSM:
                state = States.Patrol;
                StartCoroutine(PFSMThink());
                break;
        }
    }

    public IEnumerator FSMThink()
    {
        if(state == States.Patrol)
        {
            if(isPlayerNear)
            {
                state = States.Chase;
            }
        }
        else if(state == States.Chase)
        {
            if(health < maxHealth / 2)
            {
                state = States.Run;
            }
        }
        else
        {
            if(!isPlayerNear)
            {
                state = States.Patrol;
            }
        }
        
        yield return new WaitForSeconds(0.25f);
        if(algorithm == DecisionMakingAlgorithm.FSM)
        {
            StartCoroutine(FSMThink());
        }
    }
    public IEnumerator FSMWithTreeThink()
    {
        if(state == States.Patrol)
        {
            if(isPlayerNear)
            {
                state = States.Chase;
            }
        }
        else if(state == States.Chase)
        {
            if(health < maxHealth / 2)
            {
                Root.MakeDecision().Execute();
            }
        }
        else if(state == States.Run)
        {
            if(!isPlayerNear)
            {
                state = States.Patrol;
            }
        }
        else
        {
            if(health > maxHealth / 2)
            {
                state = States.Patrol;
            }
        }
        
        yield return new WaitForSeconds(0.25f);
        if(algorithm == DecisionMakingAlgorithm.FSMWithTree)
        {
            StartCoroutine(FSMWithTreeThink());
        }
    }

    public void FSMDecisionTree()
    {
        ActionNode run = new ActionNode(runAction);
        ActionNode heal = new ActionNode(healAction);
        Root = new DecisionNode(() => isHealingNear, heal, run);
    }

    public IEnumerator NFSMThink()
    {
        if(state == States.Patrol)
        {
            if(isPlayerNear)
            {
                rand = Random.Range(0, 2);
                if(rand == 0)
                {
                    state = States.Chase;
                }
                else
                {
                    state = States.Run;
                }
            }
        }
        else if(state == States.Chase)
        {
            if(health < maxHealth / 2)
            {
                rand = Random.Range(0, 2);
                if (rand == 0)
                {
                    state = States.Heal;
                }
                else
                {
                    state = States.Run;
                }
            }
        }
        else if(state == States.Run)
        {
            if(!isPlayerNear)
            {
                rand = Random.Range(0, 2);
                if (rand == 0)
                {
                    state = States.Patrol;
                }
                else
                {
                    state = States.Heal;
                }
            }
        }
        else
        {
            if(health > maxHealth / 2)
            {
                state = States.Patrol;
            }
        }
        
        yield return new WaitForSeconds(0.25f);
        if(algorithm == DecisionMakingAlgorithm.NFSM)
        {
            StartCoroutine(NFSMThink());
        }
    }
    public IEnumerator PFSMThink()
    {
        if(state == States.Patrol)
        {
            if(isPlayerNear)
            {
                rand = Random.Range(1, 11);
                if(rand <= health)
                {
                    state = States.Chase;
                }
                else
                {
                    state = States.Run;
                }
            }
        }
        else if(state == States.Chase)
        {
            if(health < maxHealth / 2)
            {
                float nearHealDistance = Mathf.Clamp(distanceToHeal - safeDistance, 0, 10);
                rand = Random.Range(1, 11);
                if (rand < nearHealDistance)
                {
                    state = States.Run;
                }
                else
                {
                    state = States.Heal;
                }
            }
        }
        else if(state == States.Run)
        {
            if(!isPlayerNear)
            {
                float nearPlayerDistance = Mathf.Clamp(distanceToPlayer - safeDistance, 0, 10);
                rand = Random.Range(1, 11);
                if(rand < nearPlayerDistance)
                {
                    rand = Random.Range(1, 11);
                    if (rand <= health)
                    {
                        state = States.Patrol;
                    }
                    else
                    {
                        state = States.Heal;
                    }
                }
            }
        }
        else
        {
            if(health > maxHealth / 2)
            {
                state = States.Patrol;
            }
        }
        
        yield return new WaitForSeconds(0.25f);
        if(algorithm == DecisionMakingAlgorithm.PFSM)
        {
            StartCoroutine(PFSMThink());
        }
    }

    public void ChangeColor(Color32 col)
    {
        this.GetComponent<MeshRenderer>().material.color = col;
    }
}
