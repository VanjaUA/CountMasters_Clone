using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UnitLogic : MonoBehaviour
{
    [SerializeField] private float speed;

    public Vector3 Destination { get; set; }

    private const float MIN_DISTANCE = 0.1f;

    public event EventHandler OnDieEvent;

    public event EventHandler OnCombatEvent;

    public event EventHandler<OnGateTrigger_EventArgs> OnGateTriggerEvent;
    public class OnGateTrigger_EventArgs
    {
        public Gate.GateType gateType;
        public uint value;
    }

    private void Awake()
    {
        StartCoroutine(CheckIfAliveCoroutine());

        Destination = transform.localPosition;
    }

    private void Update()
    {
        MoveUnit();
    }

    private void OnCollisionEnter(Collision collision)
    {
        EnemyUnit enemyUnit;
        if (collision.gameObject.TryGetComponent<EnemyUnit>(out enemyUnit))
        {
            OnCombatEvent?.Invoke(this,EventArgs.Empty);
            enemyUnit.Kill();
            Kill();
        }
    }

    private void MoveUnit() 
    {
        if (Vector3.Distance(Destination, transform.localPosition) < MIN_DISTANCE)
        {
            return;
        }
        transform.localPosition = Vector3.MoveTowards(transform.localPosition, Destination, speed * Time.deltaTime);
    }

    private IEnumerator CheckIfAliveCoroutine() 
    {
        while (true)
        {
            if (Physics.Raycast(transform.position, Vector3.down, 2f) == false)
            {
                Kill(1f);
                yield break;
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void EnterGateTrigger(OnGateTrigger_EventArgs eventArgs) 
    {
        OnGateTriggerEvent?.Invoke(this, eventArgs);
    }

    public void Kill(float delay = 0f) 
    {
        GetComponent<Collider>().isTrigger = true;
        OnDieEvent?.Invoke(this, EventArgs.Empty);
        transform.parent = null;
        Destroy(this.gameObject, delay);
    }
}
