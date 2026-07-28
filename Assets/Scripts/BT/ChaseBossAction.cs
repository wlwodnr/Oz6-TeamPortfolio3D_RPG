using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "ChaseBoss", story: "[Self] Navigate to [Target]", category: "Action", id: "9ab9e420fa6d9cc005c2969c4fe4ca9d")]
public partial class ChaseBossAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> Target;

    private NavMeshAgent _agent;

    protected override Status OnStart()
    {
        _agent = Self.Value.GetComponent<NavMeshAgent>();
        _agent.speed = UnityEngine.Random.Range(2.0f, 4.0f);
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

