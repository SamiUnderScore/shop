using Assets.Enums;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.UI;

public class Shop_Manager : MonoBehaviour
{
    #region singleton definitin
    public static Shop_Manager instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }
    #endregion

    [SerializeField] private List<Shop_ProductsManager> shopProductManagers;
    public Color[] colors;
    public int totalProductsManagersExpected;
    public Shop_ProductInfo currentSelectedProduct;
    [SerializeField] private Button buyByCurrencyButton, buyByAdsButton, equipButton;


    public ProductName currentSelectedProductName
    {
        get
        {
            return (ProductName)Enum.GetValues(typeof(ProductName)).GetValue(PlayerPrefs.GetInt("currentSelectedProductType"));
        }
        set
        {
            PlayerPrefs.SetInt("currentSelectedProductType", Array.IndexOf(Enum.GetValues(typeof(ProductName)), value));
        }
    }

    private void OnEnable()
    {
        Shop_ProductButton.productSelected += SelectProduct;
        Shop_ProductPanel.buttonsInformationCompleted += ButtonsInformed;

        equipButton.onClick.AddListener(EquipProduct);
        buyByAdsButton.onClick.AddListener(WatchAdsToBuyProduct);
        buyByCurrencyButton.onClick.AddListener(PayCurrencyToBuyProduct);
    }

    private void OnDisable()
    {
        Shop_ProductButton.productSelected -= SelectProduct;
        Shop_ProductPanel.buttonsInformationCompleted -= ButtonsInformed;

        equipButton.onClick.RemoveListener(EquipProduct);
        buyByAdsButton.onClick.RemoveListener(WatchAdsToBuyProduct);
    }

    private void Start()
    {
        var productPanels = FindObjectsOfType<Shop_ProductPanel>();
        var productRacks = FindObjectsOfType<Shop_ProductRack>();

        if (productPanels.Length == 0 || productRacks.Length == 0)
        {
            Debug.LogError("There should be at least one shop panel and shop rack in order of shop to work");
        }
        else if(productPanels.Length != productRacks.Length)
        {
            Debug.LogError("Shop Racks and Shop Panels do not match the count");
        }

        PopulateShopProductsManagersInShop(productPanels, productRacks);
    }


    public void ButtonsInformed()
    {
        SelectSelectedProduct();
    }

    void PopulateShopProductsManagersInShop(Shop_ProductPanel[] shopPanels, Shop_ProductRack[] shopRacks)
    {
        for(int i = 0; i < shopPanels.Length; i++)
        {
             shopProductManagers.Insert(i, new Shop_ProductsManager
                (
                    shopPanels[i], 
                    GetMatchingShopRack(shopRacks, shopPanels[i].productName), 
                    shopPanels[i].productName
                ));

            SetButtonsInformation(shopProductManagers[i], shopProductManagers[i].GetDefaultProductStates());
        }
    }

    public void SetButtonsInformation(Shop_ProductsManager productManager, string productStates)
    {
        productManager.productPanel.SetButtonsInfo(productManager.productName, productStates);
        SyncRacksWithPanels();
        ActionOnSelection(currentSelectedProduct);
    }

    public void SyncRacksWithPanels()
    {
        for(int i =0; i< shopProductManagers.Count; i++)
        {
            shopProductManagers[i].SyncData();
        }
    }

    ProductName GetProductTypeSelectedByDefault()
    {
        currentSelectedProductName = shopProductManagers[0].productName;
        return currentSelectedProductName;
    }

    void SelectSelectedProduct()
    {
        SelectProduct(GetManagerOfProduct(GetProductTypeSelectedByDefault()).SelectedProduct, GetProductTypeSelectedByDefault());
    }

    Shop_ProductRack GetMatchingShopRack(Shop_ProductRack[] productRacks, ProductName matchingName)
    {
        for(int i = 0; i <productRacks.Length; i++)
        {
            if (productRacks[i].productName == matchingName) return productRacks[i];
        }
        Debug.LogError("The shop items type mismatches in your shops");
        return null;
    }

    public Shop_ProductsManager GetManagerOfProduct(ProductName _productName)
    {
        for (int i = 0; i < shopProductManagers.Count; i++)
        {
            if (shopProductManagers[i].productName == _productName)
            {
                return shopProductManagers[i];
            }
        }
        //Debug.LogError("No matching products manager exists");
        return null;
    }

    public void SelectProduct(int id, ProductName _productName)
    {
        GetManagerOfProduct(_productName).SelectProduct(id);
        currentSelectedProduct = GetManagerOfProduct(_productName).GetSelectedProduct().productInfo;
        currentSelectedProductName = currentSelectedProduct.productName;
        ActionOnSelection(currentSelectedProduct);

        //just to check states being changed;
        //GetManagerOfProduct(_productName).UpdateState(id, ProductState.unlockedEquiped);
    }

    public Shop_ProductInfo GetProductInfo(int id, ProductName _productName)
    {
        return GetManagerOfProduct(_productName).GetProductInfo(id);
    }

    public void ActionOnSelection(Shop_ProductInfo product)
    {
        HideAllButtons();
        if (product.productState == ProductState.unlockedEquiped)
        {
            return;
        }
        else if(product.productState == ProductState.unlocked)
        {
            ShowButton(equipButton);
        }
        else if (product.productState == ProductState.lockedCoins)
        {
            ShowButton(buyByCurrencyButton);
        }
        else if (product.productState == ProductState.lockedAds)
        {
            ShowButton(buyByAdsButton);
        }
    }

    void HideAllButtons()
    {
        equipButton.gameObject.SetActive(false);
        buyByCurrencyButton.gameObject.SetActive(false);
        buyByAdsButton.gameObject.SetActive(false);
    }

    void ShowButton(Button buttonToShow)
    {
        buttonToShow.gameObject.SetActive(true);
    }

    void EquipProduct()
    {
        if(currentSelectedProduct.productState == ProductState.unlocked)
        {
            GetManagerOfProduct(currentSelectedProduct.productName).EquipProduct(currentSelectedProduct.id);
        }
    }

    void WatchAdsToBuyProduct()
    {
        if(currentSelectedProduct.productState == ProductState.lockedAds)
        {
            // Increment to watch videos Counter of the selected product
            GetManagerOfProduct(currentSelectedProduct.productName).IncrementWatchedVideosCounter(currentSelectedProduct.id);
        }
    }

    void PayCurrencyToBuyProduct()
    {

    }

    public int GetVideosWatchedCountForProduct(int id, ProductName productName)
    {
        GetManagerOfProduct(productName).GetVideosWatchedCountForProduct(id);
        return 0;
    }
}

[System.Serializable]
public class Shop_ProductsManager
{
    public int SelectedProduct
    {
        get
        {
            return PlayerPrefs.GetInt($"SelectedProduct{productName}");
        }
        set
        {
            PlayerPrefs.SetInt($"SelectedProduct{productName}", value);
        }
    }
    public int EquipedProduct
    {
        get
        {
            return PlayerPrefs.GetInt($"EquipedProduct{productName}");
        }
        set
        {
            PlayerPrefs.SetInt($"EquipedProduct{productName}", value);
        }
    }
    public Shop_ProductPanel productPanel;
    public Shop_ProductRack productRack;
    public ProductName productName;

    public Shop_ProductsManager(Shop_ProductPanel _productPanel, Shop_ProductRack _productRack, ProductName _productName)
    {
        productName  = _productName;
        productRack  = _productRack;
        productPanel = _productPanel;
        productPanel.InstantiateButtons(productRack.products.Length);
        if(SelectedProduct != EquipedProduct)
        {
            SelectedProduct = EquipedProduct;
        }
    }

    public void UpdateState(int _productId, ProductState changedState)
    {
        int[] statesArray = Shop_DataSaveSystem.ConvertToIntArray(GetDefaultProductStates());
        Debug.Log(statesArray.ToCommaSeparatedString());
        statesArray[_productId] = Shop_DataSaveSystem.ConvertEnumToIntRepresentation(changedState);
        Debug.Log(statesArray.ToCommaSeparatedString());
        SaveStatesArray(Shop_DataSaveSystem.Stringify(statesArray));
        SyncData();
        Shop_Manager.instance.SetButtonsInformation(this, GetProductStatesArray());
    }


    public void SelectProduct(int _productId)
    {
        if(SelectedProduct != _productId)
        {
            SelectedProduct = _productId;
        }

        HideAllProducts();
        SelectProductFromRack(_productId);
        SelectButtonFromPanel(_productId);
    }

    public void SyncData()
    {
        for(int i = 0; i < productRack.products.Length; i++)
        {
            productRack.products[i].productInfo = productPanel.GetShopProductButton(i).productInfo;
        }
    }

    public void EquipProduct(int _productId)
    {
        UpdateState(EquipedProduct, ProductState.unlocked);
        EquipedProduct = _productId;
        UpdateState(EquipedProduct, ProductState.unlockedEquiped);
        productPanel.GetShopProductButton(_productId).ReCheckInfo();
    }

    public void BuyProduct(int _productId)
    {

    }

    void SelectButtonFromPanel(int _productId)
    {
        SelectedProduct = _productId;
        productPanel.SelectProductButton(_productId);
    }

    void SelectProductFromRack(int _productId)
    {
        HideAllProducts();
        GetProductFromRack(_productId).Show();
    }

    public void HideAllProducts()
    {
        foreach (var product in productRack.products)
        {
            product.Hide();
        }
    }

    public Shop_Product GetEquipedProduct()
    {
        return productRack.products[EquipedProduct];
    }

    public Shop_Product GetSelectedProduct()
    {
        return productRack.products[SelectedProduct];
    }

    Shop_Product GetProductFromRack(int _productId)
    {
        return productRack.GetShopProduct(_productId);
    }

    Shop_ProductButton GetButtonFromPanel(int _productId)
    {
        return productPanel.GetShopProductButton(_productId);
    }
    
    public Shop_ProductInfo GetProductInfo(int _productId)
    {
        return GetProductFromRack(_productId).productInfo;
    }

    public string GetDefaultProductStates()
    {
        /*{
         * States Setting Map
         * 
         *  0 = unlockedEquiped
         *  1 = unlocked
         *  2 = lockedCoins
         *  3 = lockedAds
         *  
         * }*/
        return GetProductStatesArray();
    }

    string GetProductStatesArray()
    {
        return PlayerPrefs.GetString($"{productName}states", Shop_DataSaveSystem.GetSequencedDefaultProductStates(productRack.products.Length));
    }

    void SaveStatesArray(string statesStringifiedArray)
    {
        PlayerPrefs.SetString($"{productName}states", statesStringifiedArray);
    }

    public int GetVideosWatchedCountForProduct(int _productId)
    {
        string watchedVideosOfAllProducts = GetWatchedVideosOfAllProducts();
        Debug.Log("videosToWatch" + watchedVideosOfAllProducts);
        Debug.Log($"{productName}videosToWatch");

        return Shop_DataSaveSystem.ConvertToIntArray(watchedVideosOfAllProducts)[_productId];
    }

    public void IncrementWatchedVideosCounter(int _productId)
    {
        int[] incrementedArray = Shop_DataSaveSystem.ConvertToIntArray(GetWatchedVideosOfAllProducts());
        incrementedArray[_productId] = incrementedArray[_productId] + 1;
        SetWatchedVideosCountForAllProducts(incrementedArray);
    }

    public string GetWatchedVideosOfAllProducts()
    {
        return
            PlayerPrefs.GetString(

                $"{productName}videosToWatch",
                Shop_DataSaveSystem.Stringify(GetDefaultVideosToWatch(ref productRack.products))
            );
    }

    public void SetWatchedVideosCountForAllProducts(string watchedVideosString)
    {
        PlayerPrefs.GetString(

                $"{productName}videosToWatch",
                watchedVideosString
            );
    }

    public void SetWatchedVideosCountForAllProducts(int[] watchedVideosString)
    {
        PlayerPrefs.SetString(
            
                $"{productName}videosToWatch",
                Shop_DataSaveSystem.Stringify(watchedVideosString)
            );
    }

    int[] GetDefaultVideosToWatch(ref Shop_Product[] products)
    {
        int[] videosCount = new int[products.Length];
        for (int i = 0; i < videosCount.Length; i++)
        {
            videosCount[i] = 0;
        }
        return videosCount;
    }


}
