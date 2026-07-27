using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "RangeAttack", story: "[Self] Try RangeAttack", category: "Action", id: "ff2fa288273075216d9dc4d74ca1eb1a")]
public partial class RangeAttackAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<float> AttackDist;

    private EnemyAI _enemyAISelf;
    private float _attackCooldown = 3.0f;
    private float _lastAttackTime = -3.0f;


    protected override Status OnStart()
    {
        if (_enemyAISelf == null && Self.Value != null)
        {
            _enemyAISelf = Self.Value.GetComponent<EnemyAI>();
        }

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (_enemyAISelf == null) return Status.Failure;

        if (Time.time - _lastAttackTime < _attackCooldown)
        {
            return Status.Failure;
        }

        _enemyAISelf.RequestAttack();
        _lastAttackTime = Time.time;

        return Status.Running;
    }


}

