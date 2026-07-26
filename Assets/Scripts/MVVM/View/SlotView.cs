using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class SlotView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI TextMesh_Count;
    [SerializeField] private Image Image_ItemIcon;
    [SerializeField] private Image Image_CheckIcon;
    [SerializeField] private Button Button_Select;

    private SlotViewModel _vm;

    public void BindViewModel(SlotViewModel vm)
    {
        _vm = vm;
        _vm.PropertyChanged += OnPropChanged_View;
        OnceOnInit();
        Button_Select.onClick.AddListener(OnButtonClicked);
    }

    private void OnDestroy()
    {
        if(_vm != null)
        {
            _vm.PropertyChanged -= OnPropChanged_View;
        }
    }

    private void OnceOnInit()
    {
        Image_ItemIcon.sprite = _vm.GetItemIconImage();
        TextMesh_Count.text = _vm.Count.ToString();
        if (_vm.Count <= 0)
        {
            Button_Select.interactable = false;
        }
    }

    private void OnPropChanged_View(object sender, PropertyChangedEventArgs e)
    {
        switch(e.PropertyName)
        {
            case nameof(SlotViewModel.ItemId):
                Image_ItemIcon.sprite = _vm.GetItemIconImage();
                break;
            case nameof(SlotViewModel.Count):
                TextMesh_Count.text = _vm.Count.ToString();

                if(_vm.Count <= 0)
                {
                    Button_Select.interactable = false;
                    _vm.ButtonClicked();
                }
                break;
            case nameof(SlotViewModel.IsSelected):
                Image_CheckIcon.gameObject.SetActive(_vm.IsSelected);
                break;
        }
    }

    private void OnButtonClicked()
    {
        _vm.ButtonClicked();
    }
}
