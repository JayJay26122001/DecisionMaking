using UnityEngine;

public class Example : MonoBehaviour
{
    public bool ViuPlayer, ViuCura;
    public int vida = 10;
    DecisionNode Root;

    void Awake()
    {
        InstantiateTree();
    }
    public void InstantiateTree()
    {
        ActionNode run = new ActionNode(new Run());
        ActionNode patrol = new ActionNode(new Patrol());
        ActionNode chase = new ActionNode(new Chase());
        ActionNode heal = new ActionNode(new Heal());

        DecisionNode layer2num1 = new DecisionNode(() =>ViuPlayer, run, patrol);
        DecisionNode layer1num1 = new DecisionNode(() =>ViuPlayer, chase, patrol);
        DecisionNode layer1num2 = new DecisionNode(() =>ViuCura, heal, layer2num1);
        Root = new DecisionNode(() => (vida <= 5), layer1num2, layer1num1);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Root.MakeDecision().Execute();
        }
    }
}
