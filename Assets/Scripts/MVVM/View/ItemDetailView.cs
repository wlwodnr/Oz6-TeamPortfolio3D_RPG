using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemDetailView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _descText;
    [SerializeField] private TextMeshProUGUI _priceText;

    public void Bind(string itemId)
    {
        if(itemId == null)
        {
            gameObject.SetActive(false);
        }
        else
        {
            var item = ItemDataBase.GetItemData(itemId);
            _nameText.text = "Name : " + item.Name;
            _descText.text = "desc : " + item.ItemDesc;

            if(item is ITradeable tradeable)
            {
                _priceText.text = tradeable.BuyPrice.ToString();
                _priceText.gameObject.SetActive(true);
            }
            else
            {
                _priceText.gameObject.SetActive(false);
            }

            gameObject.SetActive(true);
        }
    }
}
