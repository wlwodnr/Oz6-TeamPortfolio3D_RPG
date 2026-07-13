using UnityEngine;


// 첫빌드 초기화용 임시 매니저입니다 !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
public class FirstBuildTestManager : MonoBehaviour    
{

    [SerializeField] private GameObject _player_Prefab;
    [SerializeField] private CameraController _main_CameraController;

    private void Awake()
    {
        Transform playerTrans = Instantiate(_player_Prefab, Vector3.zero, Quaternion.identity).transform;

        _main_CameraController = Camera.main.GetComponent<CameraController>();
        _main_CameraController.target = playerTrans;
    }

}
