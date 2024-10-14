using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Linq;

public class PlayerCenter : UnitsCenter
{
    public enum State
    {
        Moving,
        Combat,
    }

    private const int MAX_COUNT = 100;

    private float fightTime = 0.05f;
    private float maxFightTimeModifier = 2f, fightTimeModifier = 1f;
    private float fightTimer;

    private State playerState;

    public State PlayerState
    {
        get { return playerState; }
        private set 
        {
            playerState = value;
            OnStateChangedEvent?.Invoke(this,new OnStateChanged_EventArgs {state = playerState });
        }
    }

    public event EventHandler<OnStateChanged_EventArgs> OnStateChangedEvent;

    public class OnStateChanged_EventArgs : EventArgs
    {
        public State state;
    }

    private void Awake()
    {
        for (int i = 0; i < unitsStorage.childCount; i++)
        {
            unitsList.Add(unitsStorage.GetChild(i).GetComponent<PlayerUnit>());
            PlayerUnit unit = (PlayerUnit)unitsList[i];
            unit.OnDieEvent += NewUnit_OnDieEvent;
            unit.OnGateTriggerEvent += NewUnit_OnGateTriggerEvent;
        }
        FormatUnits(true);
    }

    private void Update()
    {
        if (PlayerState == State.Combat)
        {
            HandleFight();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<UnitsCenter>(out enemyCenter))
        {
            PlayerState = State.Combat;
            enemyCenter.SetEnemy(this);
            SetCombatDestination();
        }
    }

    private void DespawnUnits(int count) 
    {
        count = Mathf.Clamp(count, 0, unitsList.Count);

        for (int i = 0; i < count; i++)
        {
            unitsList[unitsList.Count - 1].Kill();
        }
    }

    private void NewUnit_OnGateTriggerEvent(object sender, PlayerUnit.OnGateTrigger_EventArgs e)
    {
        switch (e.gateType)
        {
            case Gate.GateType.Addition:
                SpawnUnits((int)e.value);
                break;
            case Gate.GateType.Subrtactoin:
                DespawnUnits((int)e.value);
                break;
            case Gate.GateType.Multiplication:
                SpawnUnits((unitsList.Count * (int)e.value) - unitsList.Count);
                break;
        }
    }

    private void NewUnit_OnDieEvent(object sender, System.EventArgs e)
    {
        unitsList.Remove((PlayerUnit)sender);
        if (playerState == State.Combat)
        {
            UpdateCountText();
            return;
        }
        FormatUnits(false);
    }

    public override void FormatUnits(bool setPositionImmediately, int startIndex = 0)
    {
        base.FormatUnits(setPositionImmediately, startIndex);

        sortedUnits = unitsList.OrderByDescending(x => x.transform.position.z).ToList();
    }

    public override void SpawnUnits(int count)
    {
        count = Mathf.Clamp(count, 0, MAX_COUNT - unitsList.Count);

        for (int i = 0; i < count; i++)
        {
            PlayerUnit newUnit = Instantiate(unitPrefab, unitsStorage).GetComponent<PlayerUnit>();
            unitsList.Add(newUnit);
            newUnit.OnDieEvent += NewUnit_OnDieEvent;
            newUnit.OnGateTriggerEvent += NewUnit_OnGateTriggerEvent;
        }

        FormatUnits(true, unitsList.Count - count);
    }

    private void HandleFight() 
    {
        if (enemyCenter == null || enemyCenter.GetUnitsCount() <= 0)
        {
            PlayerState = State.Moving;
            fightTimeModifier = 1f;
            FormatUnits(false);
            return;
        }

        fightTimeModifier = Mathf.Lerp(fightTimeModifier, maxFightTimeModifier, Time.deltaTime);
        fightTimer += Time.deltaTime * fightTimeModifier;
        if (fightTimer >= fightTime)
        {
            fightTimer -= fightTime;
            sortedUnits[0].Kill();
            sortedUnits.RemoveAt(0);

            enemyCenter.RemoveFirstUnit();
        }

        if (unitsList.Count == 0)
        {
            enemyCenter.FormatUnits(false);
            Debug.Log("Game over");
            Destroy(this.gameObject);
        }
    }
}
