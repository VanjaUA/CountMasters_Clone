using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitLogic : MonoBehaviour
{
    [SerializeField] private float speed;
    private Rigidbody rigidbody;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }
    private void FixedUpdate()
    {
        MoveUnit(Vector3.zero);
        //transform.localPosition = Vector3.MoveTowards(transform.localPosition, Vector3.zero, speed * Time.deltaTime);
    }

    private void MoveUnit(Vector3 direction) 
    {
        direction.y += rigidbody.velocity.y;
        rigidbody.velocity = direction * speed;
    }
}
