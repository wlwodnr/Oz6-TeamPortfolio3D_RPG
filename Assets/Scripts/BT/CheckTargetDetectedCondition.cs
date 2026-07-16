using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "CheckTargetDetected", story: "Compare values of [CurrentDist] and [ChaseDist]", category: "Conditions", id: "8ead50c48c70037b58dcd8dc2730e4b8")]
public partial class CheckTargetDetectedCondition : Condition
{
    [SerializeReference] public BlackboardVariable<float> CurrentDist;
    [SerializeReference] public BlackboardVariable<float> ChaseDist;

    public override bool IsTrue()
    {
        bool ConditionResult = CurrentDist.Value <= ChaseDist.Value;
        return true;
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
