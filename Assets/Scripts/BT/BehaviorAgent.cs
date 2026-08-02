using UnityEngine;
using Unity.Behavior;
using NUnit.Framework;
using System.Collections.Generic;

public class BehaviorAgent : MonoBehaviour
{
    [SerializeField] private BehaviorGraphAgent BehaviorAgent_Self;
    [SerializeField] private List<GameObject> PatrolSpotGameObjectList;
    [SerializeField] private EnemyStatus EnemyStatus_Self;


    private void Start()
    {       
        int playerInstanceId = GameObjectManager.Instance.PlayerInstanceId;
        GameObject playerObject = GameObjectManager.Instance.GetGameObjectCanBeNull(playerInstanceId);

        if (playerObject != null )
        {
            BehaviorAgent_Self.SetVariableValue("Target",playerObject);
        }
        
        if (EnemyStatus_Self != null)
        {
            BehaviorAgent_Self.SetVariableValue("AttackDist",EnemyStatus_Self.AttackRange);
            BehaviorAgent_Self.SetVariableValue("ChaseDist", EnemyStatus_Self.DetectRange);
            BehaviorAgent_Self.SetVariableValue("StopDist",EnemyStatus_Self.StopDistance);
            BehaviorAgent_Self.SetVariableValue("HalfHp",EnemyStatus_Self.MaxHp/2);
            BehaviorAgent_Self.SetVariableValue("CurrentHp",EnemyStatus_Self.CurrentHp);

        }


    }

}
