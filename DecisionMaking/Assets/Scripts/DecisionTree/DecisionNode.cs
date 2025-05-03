using System;

public class DecisionNode : TreeNode
{
    public TreeNode True, False;
    Func<bool> Condition;

    public DecisionNode(Func<bool> condition, TreeNode caseTrue, TreeNode caseFalse)
    {
        True = caseTrue;
        False = caseFalse;
        Condition = condition;
    }

    protected virtual TreeNode GetNextNode()
    {
        if (Condition())
        {
            return True;
        }
        else
        {
            return False;
        }
    }
    public override ActionNode MakeDecision()
    {
        TreeNode next = GetNextNode();
        if (next != null)
        {
            return GetNextNode().MakeDecision();
        }
        else{ return null; }
    }
}
