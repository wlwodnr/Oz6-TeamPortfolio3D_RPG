using UnityEngine;

public abstract class BTNode
{
    protected EnemyAI AgentEnemy;

    public BTNode(EnemyAI enemy)
    {
        AgentEnemy = enemy;
    }

    public abstract bool Execute();
}


public class Node_CheckHasTarget : BTNode
{
    public Node_CheckHasTarget(EnemyAI enemy) : base(enemy) { }

    public override bool Execute()
    {
        if (AgentEnemy.CurrnetTarget != null)
        {
            return true;
        }

        return AgentEnemy.SearchTarget();
    }
}

public class Node_CheckSpawnLimit : BTNode
{
    public Node_CheckSpawnLimit(EnemyAI enemy) : base(enemy) { }

    public override bool Execute()
    {
        if (AgentEnemy.CheckExceededSpawnLimit() == true)
        {
            Debug.LogWarning($"[{AgentEnemy.gameObject.name}] 집에서 너무 멉니다!");
            AgentEnemy.ClearTarget();
            return true;

        }
        return false;

    }
}

public class Node_CheckInAttackRange : BTNode
{
    public Node_CheckInAttackRange(EnemyAI enemy) : base(enemy) { }

    public override bool Execute()
    {
        return AgentEnemy.CheckAttackRange();
    }

}

public class Node_ActionAttack: BTNode
{
    public Node_ActionAttack(EnemyAI enemy) : base(enemy) { }

    public override bool Execute()
    {
        AgentEnemy.RequestAttack();
        return true;
    }
}


