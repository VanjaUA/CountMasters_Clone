using System;
using UnityEngine;

public class UnitLogic : MonoBehaviour
{
    [SerializeField] private float speed;
    public Vector3 Destination { get; set; }
    public event EventHandler OnDieEvent;

    protected const float MIN_DISTANCE = 0.2f;

    [SerializeField] protected Animator animator;

    public void Kill(float delay = 0f)
    {
        GetComponent<Collider>().isTrigger = true;
        OnDieEvent?.Invoke(this, EventArgs.Empty);
        transform.parent = null;
        Destroy(this.gameObject, delay);
    }

    protected void MoveUnit()
    {
        if (Vector3.Distance(Destination, transform.localPosition) < MIN_DISTANCE)
        {
            return;
        }
        transform.localPosition = Vector3.MoveTowards(transform.localPosition, Destination, speed * Time.deltaTime);
    }
}
