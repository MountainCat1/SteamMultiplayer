using System;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public event Action InputRight;
    public event Action InputLeft;
    public event Action InputDown;
    public event Action InputUp;
    
    private void Update()
    {
        var horizontalAxis = Input.GetAxis("Horizontal");
        var verticalAxis = Input.GetAxis("Vertical");
        
        if(horizontalAxis > 0)
            InputRight?.Invoke();
        else if(horizontalAxis < 0)
            InputLeft?.Invoke();
        
        if(verticalAxis > 0)
            InputUp?.Invoke();
        else if(verticalAxis < 0)
            InputDown?.Invoke();
    }
}