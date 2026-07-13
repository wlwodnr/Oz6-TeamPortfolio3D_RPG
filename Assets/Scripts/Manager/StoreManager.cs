using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreManager : MonoBehaviour
{
    public static StoreManager Instance;

    [SerializeField] private StoreView _storeViewPrefab;
    [SerializeField] private Transform _storeCanvas;

    private Dictionary<string,StoreViewModel> _cachedStoreVM = new Dictionary<string,StoreViewModel>();


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public void OpenStore(string npcId)
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

            StoreView sv = Instantiate(_storeViewPrefab, _storeCanvas);
        }
        
    }
}
