using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class UIMenuScreen : MonoBehaviour
{
    /// <summary>
    /// Event invoked whenever <see cref="UIMenuScreen"/> is shown
    /// </summary>
    public event Action OnScreenShow;
    
    /// <summary>
    /// Event invoked whenever <see cref="UIMenuScreen"/> is hidden
    /// </summary>
    public event Action OnScreenHide;
    
    public virtual void Hide()
    {
        gameObject.SetActive(false);
        OnScreenHide?.Invoke();
    }

    public virtual void Show()
    {
        gameObject.SetActive(true);
        OnScreenShow?.Invoke();
    }
    
    
}
