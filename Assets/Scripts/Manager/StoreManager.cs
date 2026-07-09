using System;
using System.Collections.Generic;
using UnityEngine;

public class StoreManager : MonoBehaviour
{
    public static StoreManager Instance;
    private Dictionary<string,StoreViewModel> _cachedStoreVM = new Dictionary<string,StoreViewModel>();

    private 

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
        }
        var view = Instantiate()
        
    }
}
