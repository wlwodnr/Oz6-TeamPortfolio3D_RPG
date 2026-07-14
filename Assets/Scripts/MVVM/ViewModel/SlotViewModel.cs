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
    }
    
    public void InvokeOnceOnInit()
    {
        OnPropertyChanged(nameof(ItemId));
        OnPropertyChanged(nameof(Count));
    }

    public string ItemId
    {
        get => _slotModel.ItemId;
        set
        {
            if(_slotModel.ItemId != value)
            {
                _slotModel.ItemId = value;
                OnPropertyChanged(nameof(ItemId));
            }
        }
    }

    public int Count
    {
        get => _slotModel.Count;
        set
        {
            if(_slotModel.Count != value && value <= ItemData.MaxCount)
            {
                _slotModel.Count = value;
                OnPropertyChanged(nameof(Count));
            }
        }
    }

    private bool _isSelected;
    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            _isSelected = value;
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

}
