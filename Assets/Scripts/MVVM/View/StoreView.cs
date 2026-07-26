using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class StoreView : MonoBehaviour
{
    [SerializeField] ItemDetailView ItemDetailView;
    [SerializeField] GameObject SlotPrefab;
    [SerializeField] RectTransform EquipmentSlot;
    [SerializeField] RectTransform ConsumableSlot;
    [SerializeField] RectTransform FirstEmptySlot;
    [SerializeField] RectTransform SecondEmptySlot;
    [SerializeField] TextMeshProUGUI TextMesh_Gold;
    [SerializeField] Button Button_Exit;
    [SerializeField] Button Button_Buy;

    private StoreViewModel _vm;
    private Dictionary<long, SlotView> slotViewDic = new Dictionary<long, SlotView>();

    public void BindViewModel(StoreViewModel vm)
    {
        _vm = vm;
        _vm.OnGoldChanged += OnGoldChanged_View;
        _vm.OnSlotChanged += OnSlotChanged_View;
        _vm.OnSelectChanged += OnSelectChanged_View;
        _vm.InvokeOnceOnInit();
        RefreshCoins();
        Button_Buy.onClick.AddListener(OnBuyBtnClicked);
        Button_Exit.onClick.AddListener(OnExitBtnClicked);
    }

    private void OnDestroy()
    {
        if (_vm != null)
        {
            _vm.OnSlotChanged -= OnSlotChanged_View;
            _vm.OnGoldChanged -= OnGoldChanged_View;
            _vm.OnSelectChanged -= OnSelectChanged_View;
        }
    }

    public void OnSlotChanged_View(string state, long slotId)
    {
        switch(state)
        {
            case "AddSlot":
                CreateSlotView(slotId);
                break;
            case "RemoveSlot":
                RemoveSlot(slotId);
                break;
        }
    }

    public void OnGoldChanged_View(string state)
    {
        if(state == "Coins")
        {
            RefreshCoins();
        }
    }

    public void OnSelectChanged_View(string itemId)
    {
        ItemDetailView.Bind(itemId);
    }


    private void CreateSlotView(long slotId)
    {
        if (slotViewDic.ContainsKey(slotId)) return;

        SlotViewModel svm = _vm.GetSlotVM(slotId);
        if(svm != null)
        {
            SlotView sv;
            if(svm.IsType<ConsumableItem>())
            {
                CreateSlot(svm, ConsumableSlot, slotId);
            }
            if(svm.IsType<StatUpItem>() || svm.IsType<OnHitEffectItem>())
            {
                CreateSlot(svm, EquipmentSlot, slotId);
            }
        }

    }

    private void CreateSlot(SlotViewModel vm, RectTransform parent, long slotId)
    {
        var slot = Instantiate(SlotPrefab, parent).GetComponent<SlotView>();
        slot.BindViewModel(vm);
        slotViewDic.Add(slotId, slot);
    }

    private void RemoveSlot(long slotId)
    {
        if(slotViewDic.ContainsKey(slotId))
        {
            var slot = slotViewDic[slotId];
            slotViewDic.Remove(slotId);
            Destroy(slot.gameObject);
        }
    }

    private void RefreshCoins()
    {
        TextMesh_Gold.text = "Coins : " + _vm.Coins.ToString();
    }

    private void OnBuyBtnClicked()
    {
        _vm.OnBuyButtonClicked();
    }

    private void OnExitBtnClicked()
    {
        _vm.OnExitButtonClicked();

        Destroy(gameObject);
    }
}
