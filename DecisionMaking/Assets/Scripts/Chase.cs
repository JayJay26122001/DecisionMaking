using UnityEngine;

public class Chase : Action
{
    private NPC npc;

    public Chase(NPC npcRef)
    {
        npc = npcRef;
    }

    public override void ActiveAction()
    {
        if (npc == null || npc.target == null) return;
        Vector3 direction = npc.target.transform.position - npc.transform.position;
    }
}
