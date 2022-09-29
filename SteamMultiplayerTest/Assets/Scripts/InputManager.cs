using System;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public event Action OnInputRight;
    public event Action OnInputLeft;
    public event Action OnInputDown;
    public event Action OnInputUp;
    
    private void Update()
    {
        var horizontalAxis = Input.GetAxis("Horizontal");
        var verticalAxis = Input.GetAxis("Vertical");
        
        if(horizontalAxis > 0)
            OnInputRight?.Invoke();
        else if(horizontalAxis < 0)
            OnInputLeft?.Invoke();
        
        if(verticalAxis > 0)
            OnInputUp?.Invoke();
        else if(verticalAxis < 0)
            OnInputDown?.Invoke();
    }
}