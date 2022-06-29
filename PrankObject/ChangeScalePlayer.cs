using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeScalePlayer : MonoBehaviour
{
    [SerializeField] private float _minimumDistance = 0;
    [SerializeField] private float _maximumDistance = 0;
    [SerializeField] private float _minimumDistanceScale = 0;
    [SerializeField] private float _maximumDistanceScale = 0;
    [SerializeField] private GameObject pointWithMinimumScale = null;
    private PrankObject _prankObject;

    private void Start()
    {
        _prankObject = FindObjectOfType<PrankObject>();
    }

    private void Update()
    {
        float distance = (_prankObject.transform.position.y - pointWithMinimumScale.transform.position.y)/*.magnitude*/;
        float norm = (distance - _minimumDistance) / (_maximumDistance - _minimumDistance);
        norm = Mathf.Clamp01(norm);

        var minScale = Vector2.one * _maximumDistanceScale;
        var maxScale = Vector2.one * _minimumDistanceScale;

        _prankObject.transform.localScale = Vector2.Lerp(maxScale, minScale, norm);
    }
}
