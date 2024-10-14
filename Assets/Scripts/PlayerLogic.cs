using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Linq;

public class PlayerLogic : MonoBehaviour
{
    public enum State
    {
        Moving,
        Combat,
    }

    private const int MAX_COUNT = 100;

    [SerializeField] private GameObject unitPrefab;
    private List<UnitLogic> unitsList = new List<UnitLogic>();
    private List<UnitLogic> sortedUnits;

    private float fightTime = 0.05f;
    private float maxFightTimeModifier = 1.5f, fightTimeModifier = 1f;
    private float fightTimer;

    [Range(0f, 1f)] [SerializeField] private float distanceFactor, radius;

    [SerializeField] private Transform unitsStorage;
    [SerializeField] private TextMeshPro countText;

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

    public float Radius { get; set; }

    public event EventHandler<OnStateChanged_EventArgs> OnStateChangedEvent;

    public class OnStateChanged_EventArgs : EventArgs
    {
        public State state;
    }

    private EnemyCenter currentEnemy;

    private void Awake()
    {
        for (int i = 0; i < unitsStorage.childCount; i++)
        {
            unitsList.Add(unitsStorage.GetChild(i).GetComponent<UnitLogic>());
            unitsList[i].OnDieEvent += NewUnit_OnDieEvent;
            unitsList[i].OnGateTriggerEvent += NewUnit_OnGateTriggerEvent;
        }
        FormatUnits(true);
    }

    private void Update()
    {

        if (PlayerState == State.Combat)
        {
            if (currentEnemy == null || currentEnemy.GetUnitsCount() <= 0)
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

                currentEnemy.RemoveFirstUnit();
            }

            for (int i = 0; i < unitsList.Count; i++)
            {
                float xDelta = currentEnemy.transform.position.x - transform.position.x;
                float newXPosition = (transform.localPosition.x / Radius) * currentEnemy.Radius + xDelta * 2;
                float newZPosition = currentEnemy.transform.position.z - transform.position.z;
                Vector3 newPosition = new Vector3(newXPosition,transform.localPosition.y,newZPosition);

                unitsList[i].Destination = newPosition;
            }

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<EnemyCenter>(out currentEnemy))
        {
            Debug.Log("trigger");
            PlayerState = State.Combat;
            currentEnemy.Attack(this);
        }
    }

    public void SpawnUnits(int count) 
    {
        count = Mathf.Clamp(count,0, MAX_COUNT - unitsList.Count);

        for (int i = 0; i < count; i++)
        {
            UnitLogic newUnit = Instantiate(unitPrefab, unitsStorage).GetComponent<UnitLogic>();
            unitsList.Add(newUnit);
            newUnit.OnDieEvent += NewUnit_OnDieEvent;
            newUnit.OnGateTriggerEvent += NewUnit_OnGateTriggerEvent;
        }

        FormatUnits(true,unitsList.Count - count);
    }

    private void DespawnUnits(int count) 
    {
        count = Mathf.Clamp(count, 0, unitsList.Count);

        for (int i = 0; i < count; i++)
        {
            unitsList[unitsList.Count - 1].Kill();
        }
    }

    private void NewUnit_OnGateTriggerEvent(object sender, UnitLogic.OnGateTrigger_EventArgs e)
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
        unitsList.Remove((UnitLogic)sender);
        if (playerState == State.Combat)
        {
            UpdateCountText();
            return;
        }
        FormatUnits(false);
    }

    public void FormatUnits(bool setPositionImmediately, int startIndex = 0)
    {
        for (int i = startIndex; i < unitsList.Count; i++)
        {
            if (i == 0)
            {
                unitsList[i].Destination = Vector3.zero;
                if (setPositionImmediately)
                {
                    unitsList[i].transform.localPosition = Vector3.zero;
                }

                continue;
            }
            var x = distanceFactor * Mathf.Sqrt(i + 2) * Mathf.Cos(i * radius);
            var z = distanceFactor * Mathf.Sqrt(i + 2) * Mathf.Sin(i * radius);

            var newPosition = new Vector3(x, 0f, z);

            unitsList[i].Destination = newPosition;
            if (setPositionImmediately)
            {
                unitsList[i].transform.localPosition = newPosition;
            }
        }

        UpdateCountText();
        ChangeColliderRadius();

        sortedUnits = unitsList.OrderByDescending(x => x.transform.position.z).ToList();
    }

    private void UpdateCountText() 
    {
        countText.text = unitsList.Count.ToString();
    }

    private void ChangeColliderRadius() 
    {
        float radiusModifier = 1.1f;
        float volume = unitsList.Count;
        if (volume == 0)
        {
            GetComponent<SphereCollider>().radius = 0.33f;
            return;
        }
        float radiusCubed = volume / ((4f / 3f) * 3.14f);
        float radius = Mathf.Pow(radiusCubed, 1f / 3f);
        Radius = radius * radiusModifier;
        GetComponent<SphereCollider>().radius = Radius;
    }

    public UnitLogic GetFirstUnit()
    {
        if (unitsList.Count == 0)
        {
            return null;
        }
        return unitsList[unitsList.Count - 1];
    }
}
