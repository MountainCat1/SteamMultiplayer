using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SimpleSpriteAnimation : SpriteAnimator
{
    [SerializeField] private string framesLocation = "";
    private bool _animationsLoaded = false;

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

        animations = loadedAnimations;
        _animationsLoaded = true;
    }
}
