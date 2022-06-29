using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Spine.Unity;

public class TimeBody : MonoBehaviour
{
    [SerializeField] private bool isHeroInCenter = true;
    [SerializeField] private bool isNeedForwardButton = true;
    [SerializeField] private bool isNeedThirdAnimationButton = true;
    [SerializeField] private PrankObject prankObject = null;
    private SkeletonAnimation _playerAnimation;
    [SerializeField] private float delayEnableTimeButton = 2;

    private int previousStep = -1;
    private GameUI gameUI;

    [Header("FirstAnimation")]
    [SerializeField] private List<PrankObject.ActionData> startingAnimationRewindingBack = new List<PrankObject.ActionData>();

    [Header("SecondAnimation")]
    [SerializeField] private List<PrankObject.ActionData> startingAnimationRewindingForward = new List<PrankObject.ActionData>();

    [Header("ThirdAnimation")]
    [SerializeField] private List<PrankObject.ActionData> thirdAnimationList = new List<PrankObject.ActionData>();

    [Header("Index Starting Prank Object Animation")]
    [SerializeField] private int indexStartingBackAnimation = 0;
    [SerializeField] private int indexStartingForwardAnimation = 0;
    [SerializeField] private int indexStartingThirdAnimation = 0;

    public delegate void DisableButton(CanvasGroup _canvasGroup);
    public static event DisableButton DisableButtonEvent;

    public delegate void EnableButton(CanvasGroup _canvasGroup);
    public static event EnableButton EnableButtonEvent;

    public delegate void MoveHeroToCenter();
    public static event MoveHeroToCenter MoveHeroToCenterEvent;

    public delegate void EndLevel();
    public static event EndLevel EndGameEvent;

    public delegate void StopMovePrankObject();
    public static event StopMovePrankObject StopMovePrankObjectEvent;

    private void Awake()
    {
        gameUI = FindObjectOfType<GameUI>();
        _playerAnimation = prankObject.GetComponent<SkeletonAnimation>();
    }

    private void Start()
    {
        if (isNeedForwardButton)
        {
            startingAnimationRewindingForward.InsertRange(1, prankObject.movingToTargetPositions.GetRange(indexStartingForwardAnimation, prankObject.movingToTargetPositions.Count - indexStartingForwardAnimation));
        }
        else
        {
            DisableButtonEvent(gameUI.forwardButton);
        }

        if (indexStartingBackAnimation != 0)
        {
            startingAnimationRewindingBack.InsertRange(1, prankObject.movingToTargetPositions.GetRange(indexStartingBackAnimation, prankObject.movingToTargetPositions.Count - indexStartingBackAnimation));
        }

        if(isNeedThirdAnimationButton)
        {
            thirdAnimationList.InsertRange(1, prankObject.movingToTargetPositions.GetRange(indexStartingThirdAnimation, prankObject.movingToTargetPositions.Count - indexStartingThirdAnimation));
        }
        else
        {
            DisableButtonEvent(gameUI.thirdButton);
        }        
    }

    public void OpenWinPanel()
    {
        if (isHeroInCenter)
        {
            MoveHeroToCenterEvent();         
            prankObject.gameObject.SetActive(false);
        }
        else
        {
            StopMovePrankObjectEvent();
            prankObject.MoveToCenter();
        }

        EndGameEvent();
    }

    public void BackTimeButton()
    {
        previousStep = -1;
        StopMovePrankObjectEvent();
        // prankObject.transform.position = startingAnimationRewindingBack[0].currentPosition;
        prankObject.transform.position = prankObject.movingToTargetPositions[0].currentPosition;
        
        if (indexStartingBackAnimation > 0)
        {
            _playerAnimation.Skeleton.ScaleX = prankObject.movingToTargetPositions[indexStartingBackAnimation - 1].ScaleX;
        }

        StepByStepAnimation(startingAnimationRewindingBack, 0);

        if (isNeedForwardButton)
        {
            EnableButtonEvent(gameUI.forwardButton);
        }

        DisableButtonEvent(gameUI.backButton);

        DOTween.Sequence()
             .AppendInterval(delayEnableTimeButton)
             .AppendCallback(() => EnableButtonEvent(gameUI.backButton));
    }

    public void ForwardTimeButton()
    {
        RewindAction(indexStartingForwardAnimation, startingAnimationRewindingForward);
        DisableButtonEvent(gameUI.forwardButton);

        DOTween.Sequence()
             .AppendInterval(delayEnableTimeButton)
             .AppendCallback(() => EnableButtonEvent(gameUI.forwardButton));
    }

    public void ThirdAnimationButton()
    {
        RewindAction(indexStartingThirdAnimation, thirdAnimationList);
        DisableButtonEvent(gameUI.thirdButton);

        DOTween.Sequence()
           .AppendInterval(delayEnableTimeButton)
           .AppendCallback(() => EnableButtonEvent(gameUI.thirdButton));
    }

    private void RewindAction(int _startIndexAnimation, List<PrankObject.ActionData> _animationList)
    {
        previousStep = -1;
        StopMovePrankObjectEvent();
        prankObject.transform.position = prankObject.movingToTargetPositions[_startIndexAnimation - 1].currentPosition;
        _playerAnimation.Skeleton.ScaleX = prankObject.movingToTargetPositions[_startIndexAnimation - 1].ScaleX;
        StepByStepAnimation(_animationList, 0);
    }

    private void StepByStepAnimation(List<PrankObject.ActionData> _startingAnimation, int _step)
    {
       // Debug.Log("previousStep:  " + previousStep + " _step:  " + _step);

        if (previousStep == 0 && _step != 1 || previousStep == 1 && _step != 2 || previousStep == 1 && _step == 1)
            return;

        previousStep = _step;

        if (_step <= _startingAnimation.Count - 1)
        {
            if (_startingAnimation[_step].actionType == PrankObject.ActionData.ActionType.ANIMATION)
            {
                _startingAnimation[_step].onStart.Invoke();
                prankObject.PlayAnimation(_startingAnimation[_step].key, () => StepByStepAnimation(_startingAnimation, _step + 1));
            }
            else if (_startingAnimation[_step].actionType == PrankObject.ActionData.ActionType.MOVE_TO_TARGET)
            {
                _startingAnimation[_step].onStart.Invoke();
                prankObject.StartMove(_startingAnimation[_step].targetPosition, _startingAnimation[_step].key, () => StepByStepAnimation(_startingAnimation, _step + 1));
            }
        }
    }
}
