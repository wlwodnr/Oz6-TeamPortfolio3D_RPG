using UnityEngine;

public enum EnemyAnimState
{
    Idle,
    Walk,
    Attack,
    Run
}




public class EnemyAnimatorController : MonoBehaviour
{
    [SerializeField] private Animator Animator_Enemy;

    private EnemyAnimState _currentAnimState;

    public void SetState(EnemyAnimState newState)
    {
        if (newState == EnemyAnimState.Idle && _currentAnimState == EnemyAnimState.Idle)
        {
            return;
        }

        _currentAnimState = newState;

        switch (_currentAnimState)
        {
            case EnemyAnimState.Idle:
                ResetAllAnimParameters();
                break;
            case EnemyAnimState.Walk:
                Animator_Enemy.SetBool("IsWalk", true);
                break;
            case EnemyAnimState.Attack:
                Animator_Enemy.SetBool("IsAttack", true);
                break;
            default:
                ResetAllAnimParameters();
                break;
        }
    }

    private void ResetAllAnimParameters()
    {
        Animator_Enemy.SetBool("IsWalk", false);
        Animator_Enemy.SetBool("IsDead", false);
    }
}
