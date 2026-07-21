using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;
public enum EnemyAIState
{
    Idle,
    Attack,
    Dead,
    Walk
}

public interface IEnemyAIState
{
    void EnterState(EnemyAI entity);
    void UpdateState(EnemyAI entity);
    void ExitState(EnemyAI entity);
}


public class EnemyAIState_Idle : IEnemyAIState
{
    public void EnterState(EnemyAI entity)
    {
        var animator = entity.GetEntityAnimator();
        if (animator == null)
        {
            return;
        }

        animator.SetBool("IsRun", false);
        animator.SetBool("IsWalk", false);
    }

    public void UpdateState(EnemyAI entity)
    {

    }

    public void ExitState(EnemyAI entity)
    {

    }

}


public class EnemyAIState_Attack : IEnemyAIState
{
    public void EnterState(EnemyAI entity)
    {
        var animator = entity.GetEntityAnimator();
        if (animator == null)
        {
            return;
        }

        animator.SetTrigger("IsAttack");
    }

    public void UpdateState(EnemyAI entity)
    {

    }

    public void ExitState(EnemyAI entity)
    {

    }

}


public class EnemyAIState_Dead : IEnemyAIState
{
    public void EnterState(EnemyAI entity)
    {
        var animator = entity.GetEntityAnimator();
        if (animator == null)
        {
            return;
        }

        animator.SetBool("IsDead", true);
    }

    public void UpdateState(EnemyAI entity)
    {

    }

    public void ExitState(EnemyAI entity)
    {

    }
}


public class EnemyAIState_Walk : IEnemyAIState
{
    public void EnterState(EnemyAI entity)
    {
        ToggleWalkAnimation(entity, true);
    }

    public void UpdateState(EnemyAI entity)
    {

    }

    public void ExitState(EnemyAI entity)
    {
        ToggleWalkAnimation(entity, false);
    }

    private void ToggleWalkAnimation(EnemyAI entity, bool isActive)
    {
        var animator = entity.GetEntityAnimator();
        if (animator == null)
        {
            return;
        }

        animator.SetBool("IsWalk", isActive);
    }
}