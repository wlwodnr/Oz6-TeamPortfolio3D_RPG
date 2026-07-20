using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreManager : MonoBehaviour
{
    public static StoreManager Instance;

    [SerializeField] private StoreView _storeViewPrefab;
    [SerializeField] private Transform _storeCanvas;

    private PlayerModel _playerModel;

    private Dictionary<string, StoreModel> _cachedModel = new Dictionary<string, StoreModel>();

    // npc id - StoreViewModel 구조  ... 나중에 바꿀예정, VM은 재사용하지않고 VIEW를 재사용하기로 결정
    private StoreViewModel _cachedStoreVM;

    public NetworkStoreService StoreService { get; private set; }


    private void Awake()
    {
        if (Instance == null)
        {
            ItemDataBase.LoadAllData();
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    //임시 초기화
    public void InitNetworkService()
    {
        StoreService = new NetworkStoreService();
        _playerModel = NetworkManager.Inst.LocalPlayerModel;
    }



    private void Start()
    {
        InitNetworkService();
        OpenStore("TestID_01");
    }


    public void OpenStore(string npcId)  //일단 테스트용으로 남겨둠!! UI매니저쪽으로 View관리를 넘길거임
    {
        if(ItemDataBase.StoreDic.ContainsKey(npcId) == false)
        {
            return;
        }

        InputManager.Instance.UnlockCursor();
        InputManager.Instance.DisableMove();
        InputManager.Instance.DisableCamera();

        if(_cachedModel.ContainsKey(npcId) == false)
        {
            StoreModel sm = new StoreModel(ItemDataBase.StoreDic[npcId]);
            StoreViewModel svm = new StoreViewModel(sm, _playerModel);
            _cachedStoreVM = svm;
            _cachedModel.Add(npcId, sm);

            // UIManager.Instance.OpenStoreView(여기에 슬롯 뷰모델 전달)
            StoreView sv = Instantiate(_storeViewPrefab, _storeCanvas);
            sv.BindViewModel(svm);

            StoreService.Initialize(_playerModel, sm);
        }
        else
        {
            StoreViewModel svm = new StoreViewModel(_cachedModel[npcId], _playerModel);
            _cachedStoreVM = svm;

            StoreView sv = Instantiate(_storeViewPrefab, _storeCanvas);
            sv.BindViewModel(svm);

            StoreService.Initialize(_playerModel, _cachedModel[npcId]);
        }
    }

    public void CloseStore()
    {
        if(_cachedStoreVM != null)
        {
            InputManager.Instance.LockCursor();
            InputManager.Instance.ActiveMove();
            InputManager.Instance.ActiveCamera();

            _cachedStoreVM.Dispose();
            _cachedStoreVM = null;
        }
    }

    [ContextMenu("Open Store")]
    public void TestOpenStore()
    {
        OpenStore("TestID_01");
    }
}
