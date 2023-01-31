using Assets.Enums;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Shop_ProductPanel : MonoBehaviour
{
    public ProductName productName;
    [SerializeField] private GameObject productButtonPrefab;
    [SerializeField] private Transform buttonsParent;
    [SerializeField] private List<Shop_ProductButton> buttons = new List<Shop_ProductButton>(); 
    [Space]
    [SerializeField] private Sprite[] TotalProducts;

    public static event Action<Shop_ProductInfo> productInfoChanged;
    public static event Action buttonsInstanitationCompleted, buttonsInformationCompleted;


    public void SelectProductButton(int id)
    {
        SetSelectStatuses(id);
    }

    public void SetSelectStatuses(int id)
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            buttons[i].SetSelectionStatus(id == i);
        }
        //buttons[id].CheckState();
    }

    public void InstantiateButtons(int productsCount)
    {
        for(int i = 0; i < productsCount; i++)
        {
            var productButtonInfo = Instantiate(productButtonPrefab, buttonsParent);
            buttons.Insert(i, productButtonInfo.GetComponent<Shop_ProductButton>());
        }
    }

    public void SetButtonsInfo(ProductName productName, string productStates)
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            buttons[i].SetInfo(
                productName,
                Shop_DataSaveSystem.GetStateFromIndex<ProductState>(productStates, i),
                Shop_Manager.instance.GetManagerOfProduct(productName).GetProductInfo(i).price,
                Shop_Manager.instance.GetManagerOfProduct(productName).GetProductInfo(i).maxVideos,
                i);
        }
        buttonsInformationCompleted?.Invoke();
    }

    public Shop_ProductButton GetShopProductButton(int id)
    {
        return buttons[id];
    }
}
