using UnityEngine;

public class ActionNode : TreeNode
{
    public Action action;
    public ActionNode(Action a)
    {
        action = a;
    }
    public override ActionNode MakeDecision()
    {
        return this;
    }

    public void Execute()
    {
        action?.ActiveAction();
    }
}
