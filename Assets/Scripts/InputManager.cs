using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] private float roadWidth;
    [SerializeField] private float sensibility;

    private Vector3 startTouchPosition;
    private Vector3 endTouchPosition;

    public event EventHandler<OnTouchEventArgs> OnTouchEvent;

    public class OnTouchEventArgs : EventArgs 
    {
        public Vector3 moveVector;
    }

    public static InputManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Update()
    {
        TouchesHandle();
    }

    private void TouchesHandle() 
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                startTouchPosition = touch.position;
            }

            if (touch.phase == TouchPhase.Moved)
            {
                endTouchPosition = touch.position;

                float deltaXTouchPosition = (endTouchPosition.x - startTouchPosition.x) / Screen.width;
                float deltaXMovePosition = deltaXTouchPosition * roadWidth * sensibility;

                OnTouchEvent?.Invoke(this, new OnTouchEventArgs { moveVector = Vector3.right * deltaXMovePosition });

                startTouchPosition = endTouchPosition;
            }
        }
    }

    public float GetRoadWidth() 
    {
        return roadWidth;
    }
}
