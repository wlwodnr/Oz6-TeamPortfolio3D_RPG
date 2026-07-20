using UnityEngine;

public class SkillObject : MonoBehaviour
{
    private float _leftTime = 0f;
    private float _currentTimer = 0f;

    public void Init(float poolLifeTime, Vector3 spawnPosition, Quaternion spawnRotation)
    {
        transform.SetPositionAndRotation(spawnPosition, spawnRotation);

        _leftTime = poolLifeTime;
        _currentTimer = 0f;

        gameObject.SetActive(true);
    }

    private void Update()
    {
        if (_leftTime <= 0f) return;

        _currentTimer += Time.deltaTime;
        if (_currentTimer >= _leftTime)
        {
            ReturnToPool();
        }
    }

    public void ReturnToPool()
    {
        _currentTimer = 0f;

        if (TryGetComponent<Rigidbody>(out var rb))
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        // 장판이나 투사체 관련으로 나중에 마법사 스킬 구현할 때 사용할 요소입니다.
    }
}