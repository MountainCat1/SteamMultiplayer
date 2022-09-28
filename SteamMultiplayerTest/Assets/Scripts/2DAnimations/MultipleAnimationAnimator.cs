using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MultipleAnimationAnimator : SpriteAnimator
{
    [SerializeField] private List<string> framesLocations = new List<string>();
    private bool _animationsLoaded = false;

    protected override void Awake()
    {
        base.Awake();

        if (!_animationsLoaded)
            LoadAnimations();
    }

    private void LoadAnimations()
    {
        var loadedAnimations = new List<SpriteAnimation>();
        
        foreach (var animationPath in framesLocations)
        {
            loadedAnimations.Add(new SpriteAnimation()
            {
                Frames = Resources.LoadAll<Sprite>($"{BaseAnimationPath}/{animationPath}").ToArray(),
                Name = animationPath
            });
        }

        animations = loadedAnimations;
        _animationsLoaded = true;
    }
}
