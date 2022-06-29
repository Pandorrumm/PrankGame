using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour
{
    [System.Serializable]
    public class ShopData
    {
        public Button button;
        public ShopType shopType;
        public int price;
        public GameObject pricePanel;
        public bool isPurchased;
        public bool isStudioItem = false;
        public string audioDataKey;
        
        public enum ShopType
        {
            SHOPPING,
            VIEWING_ADS
        }
    }

    [SerializeField] private CanvasGroup shopToolTipNoLikes = null;
    [SerializeField] private CanvasGroup shopToolTipNoAds = null;

    [SerializeField] private List<ShopData> shopProducts = new List<ShopData>();   

    private int totalLikes = 0;

    private const string AUDIO_BLOCK_BUTTON = "BlockObjects";
    private const string AUDIO_CLOTHE = "Clothe";
    
    public delegate void PlaySoundClip(string _name);
    public static event PlaySoundClip PlayClipEvent;

    public delegate void OpenShoopToolTipNoLikes(CanvasGroup canvasGroup);
    public static event OpenShoopToolTipNoLikes ShoopToolTipNoLikesEvent;

    public delegate void CanBuyProduct();
    public static event CanBuyProduct CanBuyProductEvent;

    public static Action<int> UpdateTotalLikesEvent;

    private void OnEnable()
    {
        PurchasingManager.CheckLikeForShopEvent += FindPurchaseWithMinPrice;
    }

    private void OnDisable()
    {
        PurchasingManager.CheckLikeForShopEvent -= FindPurchaseWithMinPrice;
    }

    private void Awake()
    {       
        foreach (ShopData shopData in shopProducts)
        {
            if (shopData.shopType == ShopData.ShopType.SHOPPING)
            {
                if(shopData.pricePanel != null)
                {
                    shopData.pricePanel.GetComponentInChildren<TextMeshProUGUI>().text = "" + shopData.price;
                }              
            }

            if (!PlayerPrefs.HasKey("ShopController" + shopData.button))
            {
                PlayerPrefs.SetInt("ShopController" + shopData.button, 0);
                shopData.isPurchased = false;
            }
            else
            {
                if (PlayerPrefs.GetInt("ShopController" + shopData.button) == 1)
                {
                    if (shopData.pricePanel != null)
                    {
                        shopData.pricePanel.SetActive(false);
                    }

                    shopData.isPurchased = true;
                }
            }
        }
    }

    private void Start()
    {
        Utility.SetCanvasGroupEnabled(shopToolTipNoAds, false);
        Utility.SetCanvasGroupEnabled(shopToolTipNoLikes, false);
        FindPurchaseWithMinPrice();
        ShopItem();
    }

    private void ShopItem()
    {
        for (int i = 0; i < shopProducts.Count; i++)
        {
            int index = i;
            shopProducts[i].button.onClick.AddListener(() => ShopController(shopProducts[index]));
        }
    }

    public void ShopController(ShopData _data)
    {
        totalLikes = LevelManager.Instance.numberLikes;

        if (!_data.isPurchased)
        {
            if (_data.shopType == ShopData.ShopType.VIEWING_ADS)
            {
                bool isAvailable = AdsManager.IsRewardedAvailable();

                if (!isAvailable)
                {
                    PlayClipEvent(AUDIO_BLOCK_BUTTON);
                }

                AdsManager.ShowRewarded(ViewingADS);

                void ViewingADS()
                {
                    if (_data.pricePanel != null)
                    {
                        _data.pricePanel.SetActive(false);
                    }

                    if (_data.button.GetComponent<MixAndMatchSkinsButton>() != null)
                    {
                        _data.button.GetComponent<MixAndMatchSkinsButton>().ToClothe();

                        AnalyticsEventer.AddLog("" + _data.button.transform.parent.name);                 
                    }

                    if (_data.button.GetComponent<EnablingStudioItems>() != null)
                    {
                        _data.button.GetComponent<EnablingStudioItems>().EnablingItem();

                        AnalyticsEventer.AddLog("" + _data.button.transform.parent.name);
                    }

                    _data.isPurchased = true;
                    PlayClipEvent(AUDIO_CLOTHE);
                    PlayerPrefs.SetInt("ShopController" + _data.button, 1);
                }
            }

            if (_data.shopType == ShopData.ShopType.SHOPPING)
            {
                if (_data.price <= totalLikes)
                {
                    PurchaseForLikes(_data.price);

                    if (_data.pricePanel != null)
                    {
                        _data.pricePanel.SetActive(false);
                    }

                    if (_data.button.GetComponent<MixAndMatchSkinsButton>() != null)
                    {
                        _data.button.GetComponent<MixAndMatchSkinsButton>().ToClothe();

                        AnalyticsEventer.AddLog("" + _data.button.transform.parent.name);
                    }

                    if (_data.button.GetComponent<EnablingStudioItems>() != null)
                    {
                        _data.button.GetComponent<EnablingStudioItems>().EnablingItem();

                        AnalyticsEventer.AddLog("" + _data.button.transform.parent.name);
                    }

                    _data.isPurchased = true;
                    PlayClipEvent(AUDIO_CLOTHE);

                    PlayerPrefs.SetInt("ShopController" + _data.button, 1);
                }
                else
                {
                    // Debug.Log("Не хватает денег. у вас " + totalLikes + " A нужно " + _data.price);
                    ShoopToolTipNoLikesEvent(shopToolTipNoLikes);
                    PlayClipEvent(AUDIO_BLOCK_BUTTON);

                    return;
                }
            }
        }
        else
        {
            //  Debug.Log("Уже куплено");
            PlayClipEvent(AUDIO_CLOTHE);

            if (_data.button.GetComponent<MixAndMatchSkinsButton>() != null)
            {
                _data.button.GetComponent<MixAndMatchSkinsButton>().ToClothe();

                AnalyticsEventer.AddLog("" + _data.button.transform.parent.name);
            }

            if (_data.button.GetComponent<EnablingStudioItems>() != null)
            {
                _data.button.GetComponent<EnablingStudioItems>().EnablingItem();

                AnalyticsEventer.AddLog("" + _data.button.transform.parent.name);
            }
        }
    }

    private void PurchaseForLikes(int _price)
    {  
        int currentLike = totalLikes - _price;
        UpdateTotalLikesEvent?.Invoke(currentLike);
    }

    private void FindPurchaseWithMinPrice()
    {
        int likes = PlayerPrefs.GetInt("Likes");

        foreach (ShopData shopData in shopProducts)
        {
            if (likes >= shopData.price && !shopData.isPurchased && shopData.price != 0 && !shopData.isStudioItem)
            {
                CanBuyProductEvent();
            }     
        }
    }
}
