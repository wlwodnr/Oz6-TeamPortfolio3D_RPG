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
        if (Self == null)
        {
            Debug.LogWarning("[UpdateDistanceAction] Self Blackboard 변수가 연결되지 않았습니다.");
            return Status.Failure;
        }
        if (Target == null)
        {
            Debug.LogWarning("[UpdateDistanceAction] Target Blackboard 변수가 연결되지 않았습니다.");
            return Status.Failure;
        }
        if (CurrentDist == null)
        {
            Debug.LogWarning("[UpdateDistanceAction] CurrentDist Blackboard 변수가 연결되지 않았습니다.");
            return Status.Failure;
        }

        if (Self.Value == null)
        {
            Debug.LogWarning("[UpdateDistanceAction] Self.Value가 설정되지 않았거나 파괴되었습니다.");
            return Status.Failure;
        }

        if (Target.Value == null)
        {
            Debug.LogWarning("[UpdateDistanceAction] Target.Value가 설정되지 않았거나 파괴되었습니다.");
            return Status.Failure;
        }

        CurrentDist.Value = Vector2.Distance(Self.Value.transform.position, Target.Value.transform.position);
        
        return Status.Success;
    }

}

