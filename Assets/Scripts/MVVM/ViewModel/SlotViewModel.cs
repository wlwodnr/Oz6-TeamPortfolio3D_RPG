using System;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;

public class SlotViewModel : INotifyPropertyChanged
{
    private SlotModel _slotModel;
    public ItemBase ItemData => ItemDataBase.GetItemData(ItemId);

    public event PropertyChangedEventHandler PropertyChanged;
    public event Action<SlotViewModel, bool> OnSelected;

    public SlotViewModel(SlotModel slotmodel)
    {
        _slotModel = slotmodel;
        _slotModel.OnSlotChanged += OnPropertyChanged;
    }
    
    public void InvokeOnceOnInit()
    {
        OnPropertyChanged(nameof(ItemId));
        OnPropertyChanged(nameof(Count));
    }

    public string ItemId
    {
        get => _slotModel.ItemId;
    }

    public int Count
    {
        get => _slotModel.Count;
    }

    private bool _isSelected;
    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            _isSelected = value;
            OnPropertyChanged(nameof(IsSelected));
        }
    }

    public bool IsCanStock => Count < ItemData.MaxCount;

    public Sprite GetItemIconImage()
    {
        return ItemDataBase.GetItemIcon(ItemId);
    }

    public void ButtonClicked()
    {
        OnSelected?.Invoke(this, IsSelected);
    }

    public long GetSlotId()
    {
        return _slotModel.SlotId;
    }

    public bool IsType<T>() where T : ItemBase
    {
        return ItemData is T;
    }
    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public void Dispose()
    {
        _slotModel.OnSlotChanged -= OnPropertyChanged;
        _slotModel = null;
    }

}
