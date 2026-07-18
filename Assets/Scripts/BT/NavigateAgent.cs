using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class NavigateAgent : MonoBehaviour
{
    [SerializeField] private Transform Transform_TargetObject;

    [SerializeField] private bool IsAlwaysTargetPlayer;
    [SerializeField] private bool IsReturnToOrigin;
    [SerializeField] private float PathUpdatePeriod = 0.1f;
    [SerializeField] private float TargetCheckPeriod = 0.5f;


    private NavMeshAgent _agent;
    private Vector3 _originPosition;
    private Coroutine _mainCoroutine;
    private Coroutine _followCoroutine;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        _mainCoroutine = StartCoroutine(CheckTargetRoutine());
        _originPosition = transform.position;
    }

    public void SetTarget(Transform NewTarget)
    {
        Transform_TargetObject = NewTarget;
    }

    private IEnumerator CheckTargetRoutine()
    {
        WaitForSeconds checkWait = new WaitForSeconds(TargetCheckPeriod);

        while (true)
        {
            if (Transform_TargetObject != null)
            {
                if (_followCoroutine == null)
                {
                    _followCoroutine = StartCoroutine(FollowTargetRoutine());
                }
            }
            else
            {
                if (_followCoroutine == null)
                {
                    StopCoroutine(_followCoroutine);
                    _followCoroutine = null;

                    if (_agent.isOnNavMesh)
                    {
                        _agent.ResetPath();
                    }

                }

            }

            if (IsAlwaysTargetPlayer == true)
            {
                //
            }


            yield return checkWait;
        }
    }

    private IEnumerator FollowTargetRoutine()
    {
        WaitForSeconds updateWait = new WaitForSeconds(PathUpdatePeriod);

        while (Transform_TargetObject != null)
        {
            if (_agent.isOnNavMesh)
            {
                _agent.SetDestination(Transform_TargetObject.position);
                //ChangeAnimation();
            }

            yield return updateWait;
        }

        _followCoroutine = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") == true)
        {
            SetTarget(other.transform);
        }


    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") == true)
        {
            if (IsReturnToOrigin == true)
            {
                ReturnToOrigin();
            }
            else
            {
                if (_agent.isOnNavMesh == true)
                {
                    _agent.ResetPath(); // 목적지 초기화 (제자리 정지)
                    //ChangeAnimation();
                }
            }

            Transform_TargetObject = null; // 타겟 오브젝트 초기화
        }
    }

    private void ReturnToOrigin()
    {
        //ChangeAnimation();
        if (_agent.isOnNavMesh)
        {
            _agent.ResetPath();
           // SetDestinationWithCallback(_originPosition, OnDestinationArrived);
        }
    }

}
