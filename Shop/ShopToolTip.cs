using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ShopToolTip : MonoBehaviour
{
    [SerializeField] private float delayDisableToolTip = 2f;
    private bool isClick = true;

    private void OnEnable()
    {
        ShopManager.ShoopToolTipNoLikesEvent += ShowToolTip;
        Hint.OpenToolTipNoADS += ShowToolTip;
        LoadingLevel.OpenToolTipNoADS += ShowToolTip;
    }

    private void OnDisable()
    {
        ShopManager.ShoopToolTipNoLikesEvent -= ShowToolTip;
        Hint.OpenToolTipNoADS -= ShowToolTip;
        LoadingLevel.OpenToolTipNoADS -= ShowToolTip;
    }

    private void ShowToolTip(CanvasGroup _canvasGroup)
    {
        if (isClick)
        {
            isClick = false;
            StartCoroutine(Show(_canvasGroup));
        }
        else
        {
            return;
        }      
    }

    private IEnumerator Show(CanvasGroup _canvasGroup)
    {
        Utility.SetCanvasGroupEnabled(_canvasGroup, true);
        yield return new WaitForSeconds(delayDisableToolTip);
        Utility.SetCanvasGroupEnabled(_canvasGroup, false);
        isClick = true;
    }
}
