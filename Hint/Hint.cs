using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.UI;

public class Hint : MonoBehaviour
{
    [SerializeField] private bool isADS = false;
    [SerializeField] private CanvasGroup noADSCanvasGroup = null;
    [Space]
    [SerializeField] private Inventory inventory = null;
    [SerializeField] private GameObject hint = null;   
    
    private bool isClick = true;
    private const string AUDIO_BLOCK_BUTTON = "BlockObjects";

    public delegate void PlayVibration();
    public static event PlayVibration PlayVibrationEvent;

    public delegate void PlaySoundClip(string _name);
    public static event PlaySoundClip PlayClipEvent;

    public static Action<CanvasGroup> OpenToolTipNoADS;

    public void ShowHint()
    {
        bool isAvailable = AdsManager.IsRewardedAvailable();

        if (!isAvailable)
        {
            PlayClipEvent(AUDIO_BLOCK_BUTTON);
        }

        AdsManager.ShowRewarded(ShowHint);

        void ShowHint()
        {
            if (isClick)
            {
                isClick = false;

                if (isADS)
                {
                    AnalyticsEventer.AddLog("Reward_Tip");

                    foreach (var slotInventory in inventory.inventoryItem)
                    {
                        if (!slotInventory.isFull && inventory.totalNumberFillSlots < 3)
                        {
                            hint.transform.position = slotInventory.needItem.transform.position;
                            hint.SetActive(true);

                            PlayVibrationEvent();

                            DOTween.Sequence()
                              .AppendInterval(3f)
                              .AppendCallback(() =>
                              {
                                  hint.SetActive(false);
                                  isClick = true;
                              });
                        }

                        if (slotInventory.isFull && inventory.totalNumberFillSlots > 2)
                        {
                            hint.transform.position = slotInventory.needItem.GetComponent<MovingObjects>()._destinationPosition.transform.position;
                            hint.SetActive(true);

                            PlayVibrationEvent();

                            ShakeSlot(inventory);

                            DOTween.Sequence()
                              .AppendInterval(3f)
                              .AppendCallback(() =>
                              {
                                  hint.SetActive(false);
                                  isClick = true;
                              });
                        }
                    }
                }
                else
                {
                    DOTween.Sequence()
                              .AppendInterval(3f)
                              .AppendCallback(() => {
                                  ;
                                  isClick = true;
                              });

                    OpenToolTipNoADS?.Invoke(noADSCanvasGroup);
                }
            }
            else
            {
                return;
            }
        }       
    }

    private void ShakeSlot(Inventory _inventory)
    {
        for (int i = _inventory.inventoryItem.Count-1; i > -1; i--)
        {
            if (_inventory.inventoryItem[i].isFull)
            {
                var position = _inventory.inventoryItem[i].slot.transform.position;
                _inventory.inventoryItem[i].slot.GetComponent<RectTransform>().DOShakeAnchorPos(3, new Vector3(35, 35, 35))
                    .OnComplete(() => _inventory.inventoryItem[i].slot.transform.DOMove(position, 0.5f));

                return;
            }                   
        }        
    }
}
