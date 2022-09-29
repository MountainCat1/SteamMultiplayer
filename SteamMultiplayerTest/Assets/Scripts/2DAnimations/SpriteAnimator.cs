using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(SpriteRenderer))]
public abstract class SpriteAnimator : MonoBehaviour
{
    protected const string BaseAnimationPath = "Animations";
    
    [SerializeField] public float speedMultiplier = 1f;
    [HideInInspector] public string playAnimationOnStart;

    [HideInInspector] private SpriteRenderer _spriteRenderer;

    [HideInInspector] public List<SpriteAnimation> animations;
    [HideInInspector] public SpriteAnimation currentAnimation;
    [HideInInspector] public bool playing = false;
    [HideInInspector] public bool Flipped { get => _spriteRenderer.flipX; set => _spriteRenderer.flipX = value; }

    [HideInInspector] private int _currentFrame = 0;
    [HideInInspector] private bool _loop = true;

    public event Action<SpriteAnimation> AnimationFinishedEvent;

    public int RendererSortingOrder { get => _spriteRenderer.sortingOrder; set => _spriteRenderer.sortingOrder = value; }

    protected virtual void Awake()
    {
        if (!_spriteRenderer)
            _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected virtual void OnEnable()
    {
        if (playAnimationOnStart != "")
            Play(playAnimationOnStart);
    }
    protected virtual void OnDisable()
    {
        playing = false;
        currentAnimation = null;
    }

    public void Play(string name, bool loop = true, int startFrame = 0)
    {
        var animation = GetAnimation(name);
        if (animation != null && animation.Frames.Length > 0)
        {
            if (animation != currentAnimation)
            {
                ForcePlay(name, loop, startFrame);
            }
        }
        else
        {
            Debug.LogWarning("could not find animation, or frames are empty: " + name);
        }
    }

    public void ForcePlay(string name, bool loop = true, int startFrame = 0)
    {
        var animation = GetAnimation(name);
        if (animation != null)
        {
            this._loop = loop;
            currentAnimation = animation;
            playing = true;
            _currentFrame = startFrame;
            _spriteRenderer.sprite = animation.Frames[_currentFrame];
            StopAllCoroutines();
            StartCoroutine(PlayAnimation(currentAnimation));
        }
    }

    public SpriteAnimation GetAnimation(string name)
    {
        return animations.Find(x => x.Name == name);
    }

    IEnumerator PlayAnimation(SpriteAnimation animation)
    {
        var timer = 0f;
        
        var direction = true;

        while (_loop || _currentFrame < animation.Frames.Length - 1)
        {
            var delay = animation.Time / speedMultiplier;
            while (timer < delay) // wait to match fps
            {
                timer += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            while (timer > delay) // display frames
            {
                timer -= delay;

                NextFrame(animation, ref direction);
            }

            _spriteRenderer.sprite = animation.Frames[_currentFrame];
        }

        AnimationFinishedEvent?.Invoke(animation);
        currentAnimation = null;
        playing = false;
    }

    void NextFrame(SpriteAnimation animation, ref bool direction)
    {
        if (!_loop)
        {
            _currentFrame++;

            if (_currentFrame >= animation.Frames.Length)
            {
                _currentFrame = animation.Frames.Length - 1;
            }
            return;
        }


        switch (animation.Type)
        {
            case SpriteAnimation.AnimationType.Normal:
                _currentFrame++;

                if (_currentFrame >= animation.Frames.Length)
                {
                    _currentFrame = 0;
                    AnimationFinishedEvent?.Invoke(animation);
                }
                break;
            case SpriteAnimation.AnimationType.PingPong:
                if (direction)
                {
                    _currentFrame += 1;

                    if (_currentFrame >= animation.Frames.Length - 1)
                    {
                        direction = false;
                        AnimationFinishedEvent?.Invoke(animation);
                    }
                }
                else
                {
                    _currentFrame -= 1;

                    if (_currentFrame <= 0)
                    {
                        direction = true;
                        AnimationFinishedEvent?.Invoke(animation);
                    }
                }
                break;
            default:
                throw new NotImplementedException();
        }
    }
}

[System.Serializable]
public class SpriteAnimation
{
    public enum AnimationType
    {
        Normal, PingPong
    }

    public string Name { get; set; }
    public float Speed { get; private set; } = 1;
    public float Time { get => 1f / Speed; }
    public AnimationType Type { get; set; }
    public Sprite[] Frames { get; set; }


    public SpriteAnimation(string name, float speed, Sprite[] frames)
    {
        Name = name;
        Speed = speed;
        Frames = frames;
    }

    public SpriteAnimation()
    {
    }
}