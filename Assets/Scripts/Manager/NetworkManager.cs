using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


public class NetworkManager : MonoBehaviour
{
    public static NetworkManager Inst { get; set; }

    public NetworkPlayerService LocalPlayerService { get; private set; }
    public NetworkInventoryService InventoryService { get; private set; }
    public NetworkSkillService SkillService { get; private set; }
    public NetworkSaveService SaveService { get; private set; }

    public PlayerModel LocalPlayerModel; // 테스트용 임시 변수
    [SerializeField] private string _playerStatDataId = "stat_dummy";

    private void Awake()
    {
        if (Inst != null && Inst != this)
        {
            Debug.LogWarning($"NetworkManager: 중복된 NetworkManager를 제거합니다. Object: {gameObject.name}", this);
            Destroy(this);
            return;
        }

        Inst = this;
        InitNetworkService();
    }

    private void OnDestroy()
    {
        if (Inst == this)
        {
            Inst = null;
        }
    }

    private void InitNetworkService()
    {
        // 앞으로 네트워크 매니저에서 사용할 다양한 서비스를 생성
        LocalPlayerService = new NetworkPlayerService();
        InventoryService = new NetworkInventoryService();
        SkillService = new NetworkSkillService();
        SaveService = new NetworkSaveService();

        var localPlayerModel = new PlayerModel();
        LocalPlayerService.Initialize(localPlayerModel);
        SkillService.Init(localPlayerModel);

        //아래는 임시로 만든거!!! 나중에 합치면 지워야함
        LocalPlayerModel = localPlayerModel;
    }

    private void Start()
    {
        if (GameDataManager.Instance == null)
        {
            Debug.LogWarning($"NetworkManager: GameDataManager가 없어 PlayerStatData를 적용할 수 없습니다. PlayerStatDataId: {_playerStatDataId}");
            return;
        }

        PlayerStatData playerStatData = GameDataManager.Instance.GetPlayerStatData(_playerStatDataId);

        if (playerStatData == null)
        {
            Debug.LogWarning($"NetworkManager: PlayerStatData를 찾을 수 없습니다. PlayerStatDataId: {_playerStatDataId}");
            return;
        }

        LocalPlayerModel.InitializeStats(playerStatData);
    }

    //public void RequestCreateLocalPlayer()
    //{
    //    // 게임 시작이나, 맵 진입 시 로컬 플레이어를 서버에 생성하는 요청
    //    var localPlayerVm = LocalPlayerService.CreateLocalPlayerViewModel();

    //    // 응답 받았다고 가정한다 = 추후 실제 서버 통신시에는 람다나 비동기 로직으로 받아온다
    //    OnRecvCreateLocalPlayer(localPlayerVm);
    //}

    //public void OnRecvCreateLocalPlayer(LocalPlayerViewModel localPlayerVm)
    //{
    //    DaniTechGameObjectManager.Inst.CreateLocalPlayer(localPlayerVm);
    //}

    // 파일 저장 경로 설정 (C:/Users/이름/.../projectName/save.json)


}
