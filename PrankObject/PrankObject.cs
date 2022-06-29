using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;
using Spine.Unity;
using System;
using DG.Tweening;
using UnityEngine.Events;

public class PrankObject : MonoBehaviour
{
    [System.Serializable]
    public class AnimationData
    {
        public string key;
        public string name;
        public bool isLoop;       
        public string audioDataKey;
    }

    [System.Serializable]
    public class ActionData
    {
        public ActionType actionType;
        public string key;
        public Vector3 targetPosition;
        public Vector3 currentPosition;
        public float ScaleX;
        public UnityEvent onStart;

        public enum ActionType
        {
            ANIMATION,
            MOVE_TO_TARGET
        }
    }

    [Header("AnimationData")]
    public List<AnimationData> animations = new List<AnimationData>();

    [Header("ActionData")]
    public List<ActionData> movingToTargetPositions = new List<ActionData>();

    private Action onCompleted;
    private Vector3 targetPosition;
    private SkeletonAnimation _playerAnimation;
    private bool isWalk;
    private bool isStopMove;

    [SerializeField] private Vector3 centerPosition = new Vector3(0, -3, 0);
    [SerializeField] private float _moveSpeed = 0;
    [SerializeField] private float delayEndGame = 2f;

    public delegate void EnableRewindTimeButton();
    public static event EnableRewindTimeButton EnableRewindTimeButtonEvent;

    public delegate void PlaySoundClip(string _name);
    public static event PlaySoundClip PlayClipEvent;

    private void OnEnable()
    {
        GameController.StartAnimationPlayerEvent += NextStep;
        TimeBody.StopMovePrankObjectEvent += StopMove;
    }

    private void OnDisable()
    {
        GameController.StartAnimationPlayerEvent -= NextStep;
        TimeBody.StopMovePrankObjectEvent -= StopMove;
    }

    private void Start()
    {
        _playerAnimation = GetComponent<SkeletonAnimation>();       
    }

    private void Update()
    {
        if (isWalk)
        {
            Move();
        }
    }

    private AnimationData SearchAnimation(string _key)
    {
        foreach (AnimationData animationData in animations)
        {
            if (animationData.key == _key)
            {
                return animationData;
            }
        }
        return null;
    }

    public void PlayAnimation(string _key, Action _onComplete)
    {
        AnimationData animationData = SearchAnimation(_key);
        _playerAnimation.loop = animationData.isLoop;

        if (_playerAnimation.AnimationName != animationData.name)
        {
            // Debug.Log("Началась анимация " + animationData.key);
            _playerAnimation.AnimationName = animationData.name;
        }
        else
        {
            _playerAnimation.AnimationName = "";
            _playerAnimation.AnimationName = animationData.name;
        }

        PlayClipEvent(animationData.audioDataKey);

        _playerAnimation.AnimationState.Complete += AnimationComplete;

        void AnimationComplete(TrackEntry track)
        {
            _playerAnimation.AnimationState.Complete -= AnimationComplete;

           DOTween.Sequence()
                .AppendInterval(0.2f)
                .AppendCallback(() => _onComplete?.Invoke());
        }
    }

    public void StartMove(Vector3 _targetPosition, string _key, Action _onComplete)
    {
        isStopMove = false;
        Flip(_targetPosition.x, transform.position.x);
        isWalk = true;
        PlayAnimation(_key, null);
        onCompleted = _onComplete;
        targetPosition = _targetPosition;
    }

    private void Flip(float _targetX, float _currentX)
    {
        if (_targetX < _currentX)
        {
            _playerAnimation.Skeleton.ScaleX = -1;
        }
        else
        {
            _playerAnimation.Skeleton.ScaleX = 1;
        }
    }

    private void Move()
    {
        if(!isStopMove)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, _moveSpeed * Time.deltaTime);

            if (transform.position == targetPosition)
            {
                onCompleted?.Invoke();
                isWalk = false;
            }
        }       
    }

    public void NextStep(int _stepNumber)
    {
        if (_stepNumber <= movingToTargetPositions.Count - 1)
        {
            if (movingToTargetPositions[_stepNumber].actionType == ActionData.ActionType.ANIMATION)
            {
                movingToTargetPositions[_stepNumber].onStart.Invoke();
                movingToTargetPositions[_stepNumber].currentPosition = gameObject.transform.position;
                movingToTargetPositions[_stepNumber].ScaleX = _playerAnimation.Skeleton.ScaleX;

                PlayAnimation(movingToTargetPositions[_stepNumber].key, () => NextStep(_stepNumber + 1));
            }
            else if (movingToTargetPositions[_stepNumber].actionType == ActionData.ActionType.MOVE_TO_TARGET)
            {
                movingToTargetPositions[_stepNumber].onStart.Invoke();
                movingToTargetPositions[_stepNumber].currentPosition = gameObject.transform.position;
                StartMove(movingToTargetPositions[_stepNumber].targetPosition, movingToTargetPositions[_stepNumber].key, () => NextStep(_stepNumber + 1));
            }
        }
        else
        {
            DOTween.Sequence()
                .AppendInterval(delayEndGame)
                .AppendCallback(() => EndGame());
        }       
    }

    private void StopMove()
    {
        isStopMove = true;
    }

    private void EndGame()
    {
        EnableRewindTimeButtonEvent();
    }

    public void MoveToCenter()
    {
        transform.position = centerPosition;
    }

    public void FlipX(bool _flipX)
    {
        if (_flipX)
        {
            if (_playerAnimation.Skeleton.ScaleX == -1)
            {
                _playerAnimation.Skeleton.ScaleX = 1;
            }
            else if (_playerAnimation.Skeleton.ScaleX == 1)
            {
                _playerAnimation.Skeleton.ScaleX = -1;
            }
        }
    }
}
