using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "PatrolAroundAction", story: "[self] Patrol around", category: "Action", id: "5515001cf60fb5d11cfea54e7619258c")]
public partial class PatrolAroundAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;

    private NavMeshAgent _agent;
    private Vector3 _patrolPosition;
    private float _currentPatrolTime;
    private float _maxPatrolTime = 3.0f;

    protected override Status OnStart()
    {
        int moveAngleMin = 0;
        int moveAngleMax = 360;
        float patrolRadius = UnityEngine.Random.Range(2f, 3.5f);
        int patrolMoveAngle = UnityEngine.Random.Range(moveAngleMin, moveAngleMax);

        _patrolPosition = Self.Value.transform.position + GetPositionFromAngle(patrolRadius, patrolMoveAngle);
        _agent = Self.Value.GetComponent<NavMeshAgent>();
        _agent.SetDestination(_patrolPosition);
        _currentPatrolTime = Time.time;

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if((_patrolPosition - Self.Value.transform.position).sqrMagnitude < 1.0f || Time.time - _currentPatrolTime > _maxPatrolTime)
        {
            return Status.Success;
        }

        return Status.Running;
    }

    private Vector3 GetPositionFromAngle(float radius, float angle)
    {
        var position = Vector3.zero;
        angle = DegreeToRadian(angle);

        position.x = Mathf.Cos(angle) * radius;
        position.z = Mathf.Sin(angle) * radius;

        return position;
    }

    private float DegreeToRadian(float angle)
    {
        return Mathf.PI * angle / 180;
    }


    protected override void OnEnd()
    {
    }
}

