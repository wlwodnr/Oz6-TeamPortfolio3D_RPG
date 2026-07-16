using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Chase", story: "[Self] Navigate To [Target]", category: "Action", id: "c72629ee6affc55658a69a3eb9911bce")]
public partial class ChaseAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> Target;

    private NavMeshAgent _agent;

    protected override Status OnStart()
    {
        _agent = Self.Value.GetComponent<NavMeshAgent>();
        _agent.speed = 5.0f;
        _agent.SetDestination(Target.Value.transform.position);

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

