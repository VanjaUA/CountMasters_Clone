using System.Collections;
using UnityEngine;
using System;

public class PlayerUnit : UnitLogic
{
    public event EventHandler<OnCombat_EventArgs> OnCombatEvent;
    public class OnCombat_EventArgs
    {
        public EnemyCenter enemyCenter;
    }

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

    //private void OnCollisionEnter(Collision collision)
    //{
    //    EnemyUnit enemyUnit;
    //    if (collision.gameObject.TryGetComponent<EnemyUnit>(out enemyUnit))
    //    {
    //        OnCombatEvent?.Invoke(this, new OnCombat_EventArgs() { enemyCenter = enemyUnit.GetComponentInParent<EnemyCenter>() });
    //        enemyUnit.Kill();
    //        Kill();
    //    }
    //}

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
}
