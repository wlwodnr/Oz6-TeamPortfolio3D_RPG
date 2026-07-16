using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "NormalAttack", story: "[Self] Try NormalAttack", category: "Action", id: "8ff75793ada6f5cc51ee69f9a432a65a")]
public partial class NormalAttackAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;

    private EnemyAI _enemyAISelf;
    private float _attackCooldown = 3.0f;
    private float _lastAttackTime = -3.0f;


    protected override Status OnStart()
    {
        if ( _enemyAISelf == null && Self.Value != null)
        {
            _enemyAISelf = Self.Value.GetComponent<EnemyAI>();
        }

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (_enemyAISelf == null) return Status.Failure;

        if ( Time.time -  _lastAttackTime < _attackCooldown)
        {
            return Status.Failure;
            //return Status.Running // 공격 쿨타임 중에 추격불가능
        }
        
        _enemyAISelf.RequestAttack();
        _lastAttackTime = Time.time;

        return Status.Success;
    }


}

