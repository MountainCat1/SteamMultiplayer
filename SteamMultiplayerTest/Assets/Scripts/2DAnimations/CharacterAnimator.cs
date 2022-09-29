using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class CharacterAnimator : SpriteAnimator
{
    [SerializeField] private string framesLocation = "";
    private bool _animationsLoaded = false;

    public enum Animation
    {
        Idle, Walk, Fall, Jump
    }

    public void PlayAnimation(Animation animation, bool loop = true, int startFrame = 0)
    {
        Play(animation.ToString().ToLower(), loop, startFrame);
    }

    public void SetAnimation(Animation animation)
    {
        if (currentAnimation.Name != animation.ToString().ToLower())
        {
            PlayAnimation(animation);
        }
    }

    public void ReloadAnimations()
    {
        LoadAnimations();
    }

    protected override void Awake()
    {
        base.Awake();

        playAnimationOnStart = "idle";

        if (!_animationsLoaded)
            LoadAnimations();
    }

    private void LoadAnimations()
    {
        var loadedAnimations = new List<SpriteAnimation>();

        var basePath = $"{BaseAnimationPath}/{framesLocation}";
        
        loadedAnimations.Add(new SpriteAnimation()
        {
            Frames = Resources.LoadAll<Sprite>($"{basePath}/idle").ToArray(),
            Name = "idle"
        });

        loadedAnimations.Add(new SpriteAnimation()
        {
            Frames = Resources.LoadAll<Sprite>($"{basePath}/walk").ToArray(),
            Name = "walk",
            Type = SpriteAnimation.AnimationType.PingPong
        });

        loadedAnimations.Add(new SpriteAnimation()
        {
            Frames = Resources.LoadAll<Sprite>($"{basePath}/jump").ToArray(),
            Name = "jump",
            Type = SpriteAnimation.AnimationType.Normal
        });

        loadedAnimations.Add(new SpriteAnimation()
        {
            Frames = Resources.LoadAll<Sprite>($"{basePath}/fall").ToArray(),
            Name = "fall",
            Type = SpriteAnimation.AnimationType.Normal
        });

        animations = loadedAnimations;
        _animationsLoaded = true;
    }
}
