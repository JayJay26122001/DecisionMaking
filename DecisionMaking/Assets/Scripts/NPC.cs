using UnityEngine;

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
    public int health;
    public GameObject target;


    private void Awake()
    {
        NPCBehaviour();
    }

    public void NPCBehaviour()
    {
        switch (algorithm)
        {
            case DecisionMakingAlgorithm.FSM:

                break;
            case DecisionMakingAlgorithm.DecisionTree:

                break;
            case DecisionMakingAlgorithm.NFSM:

                break;
            case DecisionMakingAlgorithm.PFSM:

                break;
        }
    }
}
