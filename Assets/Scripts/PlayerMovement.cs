using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float forwardSpeed;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Vector3 cameraOffset;
    [SerializeField] private float cameraSensitivity;


    private PlayerLogic playerLogic;
    private bool combat = false;

    private void Start()
    {
        InputManager.Instance.OnTouchEvent += InputManager_OnTouchEvent;

        if (TryGetComponent<PlayerLogic>(out playerLogic))
        {
            playerLogic.OnStateChangedEvent += PlayerLogic_OnStateChangedEvent;
        }
    }

    private void PlayerLogic_OnStateChangedEvent(object sender, PlayerLogic.OnStateChanged_EventArgs e)
    {
        switch (e.state)
        {
            case PlayerLogic.State.Moving:
                combat = false;
                break;
            case PlayerLogic.State.Combat:
                combat = true;
                break;
            default:
                break;
        }
    }

    private void InputManager_OnTouchEvent(object sender, InputManager.OnTouchEventArgs e)
    {
        SidewayMoving(e.moveVector);
    }

    private void Update()
    {
        ForwardMoving();
    }

    private void LateUpdate()
    {
        CameraMoving();
    }

    private void ForwardMoving() 
    {
        if (combat)
        {
            return;
        }
        transform.position += Vector3.forward * forwardSpeed * Time.deltaTime;
    }

    private void SidewayMoving(Vector3 moveVector) 
    {
        if (combat)
        {
            return;
        }
        float roadWidth = InputManager.Instance.GetRoadWidth();
        float newXPosition = (transform.position + moveVector).x;
        newXPosition = Mathf.Clamp(newXPosition,(roadWidth/2f) * -1,(roadWidth/2f));
        Vector3 newPosition = new Vector3(newXPosition, transform.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position,newPosition,0.5f);
    }

    private void CameraMoving() 
    {
        float cameraNewXValue = Mathf.Lerp(cameraTransform.position.x, (transform.position + cameraOffset).x, cameraSensitivity * Time.deltaTime);
        Vector3 cameraNewPosition = transform.position + cameraOffset;
        cameraNewPosition.x = cameraNewXValue;
        cameraTransform.position = cameraNewPosition;
    }
}
