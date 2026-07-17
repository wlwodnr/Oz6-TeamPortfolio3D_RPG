using Cysharp.Threading.Tasks;
using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{
    [SerializeField] private Text Text_StackCount;
    [SerializeField] private UIButton Button_Slot;
    [SerializeField] private Image Image_Icon;
    [SerializeField] private Image Image_Frame;
    [SerializeField] private Image Image_Selected;

    private SlotViewModel _vm;

    public long SlotItemUniqueId { get; private set; }
    public bool IsUsableItem { get; private set; }

    private void OnEnable()
    {
        Image_Selected.gameObject.SetActive(false);
        Button_Slot.BindOnClickButtonEvent(OnClick_SelectItem);
    }

    private void OnDisable()
    {
        if (Button_Slot != null)
        {
            Button_Slot.UnBindAllOnClickButtonEvent();
        }
        UnbindViewModel();
    }

    public void BindSlotViewModel(SlotViewModel slotVm)
    {
        UnbindViewModel();

        if (slotVm == null)
        {
            ClearSlot();
            return;
        }

        gameObject.SetActive(true);
        _vm = slotVm;
        SlotItemUniqueId = _vm.GetSlotId();
        _vm.PropertyChanged += OnPropChanged_View;
        _vm.InvokeOnceOnInit();
    }

    private void UnbindViewModel()
    {
        if (_vm != null)
        {
            _vm.PropertyChanged -= OnPropChanged_View;
            _vm = null;
        }
    }

    private void OnPropChanged_View(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(SlotViewModel.ItemId):
                SetIcon(_vm.ItemId);
                break;
            case nameof(SlotViewModel.Count):
                if (Text_StackCount != null)
                {
                    Text_StackCount.text = _vm.Count > 1 ? $"{_vm.Count}" : string.Empty;
                }
                break;
            case nameof(SlotViewModel.IsSelected):
                if (Image_Selected != null)
                {
                    Image_Selected.gameObject.SetActive(_vm.IsSelected);
                }
                break;
        }
    }

    public void SetIcon(string itemDataId)
    {
        if (string.IsNullOrEmpty(itemDataId))
        {
            IsUsableItem = false;
            if (Image_Icon != null) Image_Icon.sprite = null;
            return;
        }

        var itemData = GameDataManager.Instance.GetItemData(itemDataId);
        if (itemData == null)
        {
            Debug.LogWarning($"Item 데이터를 불러올 수 없습니다! 경로:{itemDataId}");
            return;
        }

        string iconPath = itemData.IconPath;
        if (string.IsNullOrEmpty(iconPath) == true)
        {
            Debug.LogWarning($"Item 데이터에 아이콘 경로가 존재하지 않습니다.");
            return;
        }
        IsUsableItem = (itemData is IUseable);

        // + Addressable을 적용하면서 비동기로 바뀌었다
        //ResourceManager.Inst.LoadSprite(iconPath, (sprite) => {
        //    Image_Icon.sprite = sprite;
        //});
        //GameUtil.LoadAndSetSpriteImage(Image_Icon, iconPath).Forget();
        //var sprite = GameUtil.LoadSpriteCanBeNull(iconPath);
        //if (sprite == null)
        //{
        //    Debug.LogWarning($"Sprite를 불러올 수 없습니다! 경로:{iconPath}");
        //    return;
        //}
        //Image_Icon.sprite = sprite;
    }

    public void OnClick_SelectItem()
    {
        _vm?.ButtonClicked();
        Debug.Log($"{SlotItemUniqueId} 슬롯 클릭됨");
    }

    public void ClearSlot()
    {
        if (_vm != null)
        {
            _vm.IsSelected = false;
        }
        UnbindViewModel();

        SlotItemUniqueId = 0;
        IsUsableItem = false;

        if (Image_Icon != null)
        {
            Image_Icon.sprite = null;
        }
        if (Text_StackCount != null)
        {
            Text_StackCount.text = string.Empty;
        } 
        gameObject.SetActive(false);
    }
}
