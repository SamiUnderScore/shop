using Assets.Enums;
using System;
using UnityEngine;
using UnityEngine.UI;

public class Shop_ProductButton : MonoBehaviour
{
    public Shop_ProductInfo productInfo;
    [SerializeField] private Button productButton;
    [SerializeField] private Text price;
    [SerializeField] private Text name;
    [SerializeField] private GameObject lockedCoinsOverlay, lockedAdsOverlay, equipedOverlay, selectionOverlay;
    public bool isSelected = false;
    public static event Action<int, ProductName> productSelected;
    private void OnEnable()
    {
        productButton = GetComponent<Button>();
        productButton.onClick.AddListener(SelectProductButton);
    }

    private void OnDisable()
    {
        productButton.onClick.RemoveListener(SelectProductButton);
    }

    public void SelectProductButton()
    {
        productSelected?.Invoke(productInfo.id, productInfo.productName);
    }

    //public void CheckState()
    //{
    //    if (productInfo.productState == ProductState.lockedCoins)
    //    {
    //        print("can unlock using Coins");
    //    }
    //    else if (productInfo.productState == ProductState.lockedAds)
    //    {
    //        print("can unlock using Ads");
    //    }
    //}

    public void SetSelectionStatus(bool status)
    {
        isSelected = status;
        CheckSelectionStatus();
    }

    public void DeselectProductButton()
    {
        this.productInfo.productState = ProductState.unlockedEquiped;
    }

    public void SetInfo(ProductName productName, ProductState productState, int price, int id)
    {
        GetComponent<Image>().color = Shop_Manager.instance.colors[id];
        productInfo = new Shop_ProductInfo(productName, productState, price, id);
        this.price.text = price.ToString();
        name.text = productName.ToString();
        ReCheckInfo();
    }

    public void CheckEquiptionStatus()
    {
        equipedOverlay.SetActive(productInfo.productState == ProductState.unlockedEquiped);
    }

    void CheckSelectionStatus()
    {
        selectionOverlay.SetActive(isSelected);
    }

    public void ReCheckInfo()
    {
        if(productInfo.productState == ProductState.lockedAds || productInfo.productState == ProductState.lockedCoins)
        {
            price.text = "";
        }
        lockedCoinsOverlay.SetActive(productInfo.productState == ProductState.lockedCoins);
        lockedAdsOverlay.SetActive(productInfo.productState == ProductState.lockedAds);
        CheckEquiptionStatus();
        CheckSelectionStatus();
    }
}
