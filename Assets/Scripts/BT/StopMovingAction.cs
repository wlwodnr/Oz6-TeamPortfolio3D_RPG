using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using UnityEngine.AI;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "StopMoving", story: "Update [Self] Stop [StopDist]", category: "Action", id: "0fb82e0cecc33e4491d62903edb25d34")]
public partial class StopMovingAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<float> StopDist;
    [SerializeReference] public BlackboardVariable<float> CurrentDist;

    private NavMeshAgent _agent;

    protected override Status OnStart()
    {
        _agent = Self.Value.GetComponent<NavMeshAgent>();
        

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (_agent != null && StopDist >= CurrentDist)
        {
            _agent.isStopped = true;
            _agent.ResetPath();
        }


        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

