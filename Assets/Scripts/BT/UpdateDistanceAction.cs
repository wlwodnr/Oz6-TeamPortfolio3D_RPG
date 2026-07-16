using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "UpdateDistance", story: "Update [Self] and [Target] [CurrentDist]", category: "Action", id: "3dc203458c91443e131dc9564baabe8f")]
public partial class UpdateDistanceAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<float> CurrentDist;


    protected override Status OnUpdate()
    {
        CurrentDist.Value = Vector2.Distance(Self.Value.transform.position, Target.Value.transform.position);
        
        return Status.Success;
    }

}

