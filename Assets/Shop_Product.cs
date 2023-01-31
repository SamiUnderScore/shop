using Assets.Enums;
using UnityEditor;
using UnityEngine;

public class Shop_Product : MonoBehaviour
{
    public Shop_ProductInfo productInfo;
    
    public Shop_ProductInfo GetInfo()
    {
        return productInfo;
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}

[System.Serializable]
public class Shop_ProductInfo
{
    public ProductName productName;
    public ProductState productState;
    public int price;
    public int id;
    public int maxVideos;
    

    Shop_ProductInfo() { }

    public Shop_ProductInfo(ProductName productName, ProductState productState, int price, int maxVideos, int id)
    {
        this.productName = productName;
        this.productState = productState;
        this.maxVideos = maxVideos;
        this.price = price;
        this.id = id;
    }

}
