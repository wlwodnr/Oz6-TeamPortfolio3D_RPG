using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreManager : MonoBehaviour
{
    public static StoreManager Instance;

    [SerializeField] private StoreView _storeViewPrefab;
    [SerializeField] private Transform _storeCanvas;

    // npc id - StoreViewModel 구조
    private Dictionary<string,StoreViewModel> _cachedStoreVM = new Dictionary<string,StoreViewModel>();


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

 

    private void Start()
    {
        OpenStore("TestID_01");
    }

    public void OpenStore(string npcId)  //일단 테스트용으로 남겨둠!! UI매니저쪽으로 View관리를 넘길거임
    {
        if(ItemDataBase.StoreDic.ContainsKey(npcId) == false)
        {
            return;
        }
        
        if(_cachedStoreVM.ContainsKey(npcId) == false)
        {
            StoreModel sm = new StoreModel(ItemDataBase.StoreDic[npcId]);
            StoreViewModel svm = new StoreViewModel(sm);
            _cachedStoreVM.Add(npcId, svm);

            // UIManager.Instance.OpenStoreView(여기에 슬롯 뷰모델 전달)
            StoreView sv = Instantiate(_storeViewPrefab, _storeCanvas);
            sv.BindViewModel(svm);
        }
    }

    public void CloseStore(string npcId)
    {
        if(_cachedStoreVM.ContainsKey(npcId) == true)
        {
            
        }

    }
}
