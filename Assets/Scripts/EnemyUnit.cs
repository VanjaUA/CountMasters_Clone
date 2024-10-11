using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUnit : MonoBehaviour
{
    [SerializeField] private float speed;

    public event EventHandler OnDieEvent;
    public Vector3 Destination { get; set; }
    private const float MIN_DISTANCE = 0.1f;

    private void Update()
    {
        MoveUnit();
    }

    public void Kill(float delay = 0f)
    {
        GetComponent<Collider>().isTrigger = true;
        OnDieEvent?.Invoke(this, EventArgs.Empty);
        transform.parent = null;
        Destroy(this.gameObject, delay);
    }

    private void MoveUnit()
    {
        if (Vector3.Distance(Destination, transform.localPosition) < MIN_DISTANCE)
        {
            return;
        }
        transform.localPosition = Vector3.MoveTowards(transform.localPosition, Destination, speed * Time.deltaTime);
    }
}
