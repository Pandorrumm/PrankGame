using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class MoveToInventory : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private float durationMove = 1;
    [SerializeField] private float durationFadeOut = 1.5f;
    private Inventory inventory;
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider2D;
    private const string NAME_AUDIOCLIP = "Right Choice";

    [Header("EventAtBeginningMovement")]
    public UnityEvent onStartMove;

    [Header("EventAtEndMovement")]
    public UnityEvent onEndMove;

    public delegate void FillingSlotInventory();
    public static event FillingSlotInventory FillingSlotInventoryEvent;

    public delegate void RightChoiceIcon();
    public static event RightChoiceIcon ShowIconEvent;

    public delegate void PlaySoundClip(string _name);
    public static event PlaySoundClip PlayClipEvent;

    public delegate void PlayVibration();
    public static event PlayVibration PlayVibrationEvent;

    private void OnEnable()
    {
        GameUI.KillDoTweenEvent += KillDoTween;
    }

    private void OnDisable()
    {
        GameUI.KillDoTweenEvent -= KillDoTween;
    }

    private void Awake()
    {
        gameObject.GetComponent<MovingObjects>().enabled = false;
        inventory = FindObjectOfType<Inventory>();

        if (gameObject.GetComponent<ReplaceObjects>() != null)
        {
            gameObject.GetComponent<ReplaceObjects>().enabled = false;
        }

        if (gameObject.GetComponent<AddCombinedSkin>() != null)
        {
            gameObject.GetComponent<AddCombinedSkin>().enabled = false;
        }
    }

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider2D = GetComponent<BoxCollider2D>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        GameObject icon = ObjectPool.sharedInstance.GetPoolObject();

        if (icon != null)
        {
            icon.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y);
            icon.SetActive(true);
        }

        boxCollider2D.enabled = false;
        PlayClipEvent(NAME_AUDIOCLIP);
        ShowIconEvent();
        PlayVibrationEvent();
        onStartMove.Invoke();

        if (spriteRenderer != null)
        {
            spriteRenderer.DOFade(0, durationFadeOut);            
        }

        foreach (var slotInventory in inventory.inventoryItem)
        {
            if (gameObject.name == slotInventory.needItem.name)
            {
                slotInventory.isFull = true;

                transform.DOMove(slotInventory.slot.transform.position, durationMove)
                .OnComplete(() => AddToInventory());

                void AddToInventory()
                {
                    onEndMove.Invoke();

                    for (int i = 0; i < slotInventory.slot.transform.childCount; i++)
                    {
                        slotInventory.slot.transform.GetChild(i).gameObject.SetActive(true);
                    }

                    FillingSlotInventoryEvent();
                    // item.slot.transform.GetChild(0).gameObject.SetActive(true);

                    if (spriteRenderer == null)
                    {
                        gameObject.SetActive(false);
                    }                   
                }
            }
        }
    }

    private void KillDoTween()
    {
        transform.DOKill();
        spriteRenderer.DOKill();
    }
}

