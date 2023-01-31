using Assets.Enums;
using UnityEngine;

public class Shop_ProductRack : MonoBehaviour
{
    public ProductName productName;

    public Shop_Product[] products;

    public Shop_Product GetShopProduct(int id)
    {
        return products[id];
    }

}
