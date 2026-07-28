using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "SpecialAttack", story: "[Self] try SpecialAttack", category: "Action", id: "f78d2f4cd53cf45149a27fa65a105ac4")]
public partial class SpecialAttackAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<float> AttackDist;

    private EnemyAI _enemyAISelf;
    private EnemyStatus _enemyStatus;
    private float _attackCooldown = 3.0f;
    private float _lastAttackTime = -3.0f;

    protected override Status OnStart()
    {
        if (_enemyAISelf == null && Self.Value != null)
        {
            _enemyAISelf = Self.Value.GetComponent<EnemyAI>();
        }

        if (_enemyStatus == null)
        {
            _enemyStatus = Self.Value.GetComponent<EnemyStatus>();
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

        _enemyStatus.ReinforceAttack();
        _enemyAISelf.RequestAttack();
        _lastAttackTime = Time.time;


        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

