using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "ChangeAnimation", story: "[Self] to [StateEnum]", category: "Action", id: "89fec3416f74ef723afc5dc9c934f75b")]
public partial class ChangeAnimationAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<EnemyAIState> ChangeStateEnum;
    protected override Status OnStart()
    {
        var battleAgentSelf = Self.Value.GetComponent<EnemyAI>();
        if (battleAgentSelf != null)
        {
            battleAgentSelf.ChangeState(ChangeStateEnum);
        }

        return Status.Success;
    }



    protected override void OnEnd()
    {
    }
}

