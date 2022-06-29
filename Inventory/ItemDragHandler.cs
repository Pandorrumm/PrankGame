using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class ItemDragHandler : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [SerializeField] private Canvas myCanvas = null;
    [SerializeField] private CanvasGroup slotCanvasGroup = null;

    private Inventory inventory;   
    private Vector2 positionFinger;
    private bool isDrag = false;

    public delegate void ActionWithItems();
    public static event ActionWithItems ActionWithItemsEvent;

    public delegate void GetDressed();
    public static event GetDressed GetDressedEvent;

    public delegate void RightChoice();
    public static event RightChoice AddPointEvent;

    public delegate void RightChoiceIcon();
    public static event RightChoiceIcon ShowIconEvent;

    public delegate void PlayVibration();
    public static event PlayVibration PlayVibrationEvent;

    private void OnEnable()
    {
        Inventory.AllowDragEvent += AllowDrags;
        GameUI.KillDoTweenEvent += KillDoTween;
    }

    private void OnDisable()
    {
        Inventory.AllowDragEvent -= AllowDrags;
        GameUI.KillDoTweenEvent -= KillDoTween;
    }

    private void Start()
    {
        inventory = GetComponentInParent<Inventory>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!isDrag)
        {
            return;
        }
        else
        {
            foreach (var slotInventory in inventory.inventoryItem)
            {
                if (slotCanvasGroup.name == slotInventory.slot.name)
                {
                    if (slotInventory.needItem.GetComponent<SpriteRenderer>() != null)
                    {
                        slotInventory.needItem.GetComponent<SpriteRenderer>().DOFade(1, 0);
                    }
                    else
                    {
                        slotInventory.needItem.SetActive(true);
                    }                       

                    slotInventory.isFull = false;

                    for (int i = 0; i < slotInventory.slot.transform.childCount; i++)
                    {
                        slotInventory.slot.transform.GetChild(i).GetComponent<Image>().DOFade(0, 0.5f);
                    }
                }             
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {     
        if (!isDrag)
        {
            return;
        }
        else
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(myCanvas.transform as RectTransform, Input.mousePosition, myCanvas.worldCamera, out positionFinger);

            foreach (var slotInventory in inventory.inventoryItem)
            {
                if (slotCanvasGroup.name == slotInventory.slot.name)
                {
                    slotInventory.needItem.SetActive(true);
                    slotInventory.needItem.transform.position = myCanvas.transform.TransformPoint(positionFinger);                                    
                }
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDrag)
        {
            return;
        }
        else
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(myCanvas.transform as RectTransform, Input.mousePosition, myCanvas.worldCamera, out positionFinger);
            foreach (var slotInventory in inventory.inventoryItem)
            {
                if (slotCanvasGroup.name == slotInventory.slot.name)
                {
                    slotInventory.needItem.transform.position = myCanvas.transform.TransformPoint(positionFinger);
                    RaycastHit2D hit = Physics2D.Raycast(myCanvas.transform.TransformPoint(positionFinger), Vector2.zero);

                    if (hit.collider == slotInventory.rightPlace)
                    {
                        GameObject icon = ObjectPool.sharedInstance.GetPoolObject();

                        if (icon != null)
                        {
                            icon.transform.position = new Vector3(slotInventory.needItem.transform.position.x, slotInventory.needItem.transform.position.y);
                            icon.SetActive(true);
                        }

                        PlayVibrationEvent();
                        ShowIconEvent();
                                              
                        if (slotInventory.needItem.GetComponent<ReplaceObjects>() != null)
                        {
                            slotInventory.needItem.GetComponent<ReplaceObjects>().enabled = true;
                            slotInventory.needItem.GetComponent<ReplaceObjects>().Swap(slotInventory.needItem.GetComponent<ReplaceObjects>()._firstObject, slotInventory.needItem.GetComponent<ReplaceObjects>()._secondObject);
                        }

                        slotInventory.needItem.GetComponent<MovingObjects>().enabled = true;
                        ActionWithItemsEvent();
                        AddPointEvent();

                        if (slotInventory.needItem.GetComponent<AddCombinedSkin>() != null)
                        {
                            slotInventory.needItem.GetComponent<AddCombinedSkin>().enabled = true;
                            GetDressedEvent();
                        }                      

                        slotCanvasGroup.DOFade(0, 1);
                        slotCanvasGroup.gameObject.SetActive(false);
                    }
                    else
                    {
                        slotInventory.isFull = true;
                        slotInventory.needItem.transform.DOPunchRotation(new Vector3(30, 30, 30), 0.5f)
                            .OnComplete(() => BackToSlot());

                        void BackToSlot()
                        {
                            slotInventory.needItem.transform.DOMove(slotInventory.slot.transform.position, 0f)
                                .OnComplete(() => DisableItem());
                           
                            void DisableItem()
                            {
                                if (slotInventory.needItem.GetComponent<SpriteRenderer>() == null)
                                {
                                    slotInventory.needItem.SetActive(false);
                                }

                                if (slotInventory.needItem.GetComponent<SpriteRenderer>() != null)
                                {
                                    slotInventory.needItem.GetComponent<SpriteRenderer>().DOFade(0, 0f);
                                }
                            }
                          
                            for (int i = 0; i < slotInventory.slot.transform.childCount; i++)
                            {
                                slotInventory.slot.transform.GetChild(i).GetComponent<Image>().DOFade(1, 0f);
                            }
                        }
                    }                  
                }
            }
        }
    }

    private void AllowDrags()
    {
        isDrag = true;
    }

    private void KillDoTween()
    {
        transform.DOKill();
        slotCanvasGroup.DOKill();

        foreach (var slotInventory in inventory.inventoryItem)
        {
            slotInventory.needItem.GetComponent<SpriteRenderer>().DOKill();
        }
    }
}
