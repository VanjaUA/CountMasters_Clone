using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerLogic : MonoBehaviour
{
    private const int MAX_COUNT = 100;

    [SerializeField] private GameObject unitPrefab;
    private List<UnitLogic> unitsList = new List<UnitLogic>();

    [Range(0f, 1f)] [SerializeField] private float distanceFactor, radius;

    [SerializeField] private Transform unitsStorage;
    [SerializeField] private TextMeshPro countText;


    private void Awake()
    {
        for (int i = 0; i < unitsStorage.childCount; i++)
        {
            unitsList.Add(unitsStorage.GetChild(i).GetComponent<UnitLogic>());
            unitsList[i].OnDieEvent += NewUnit_OnDieEvent;
            unitsList[i].OnGateTriggerEvent += NewUnit_OnGateTriggerEvent;
            unitsList[i].OnCombatEvent += PlayerLogic_OnCombatEvent;
        }
        FormatUnits(true);

    }

    private void OnTriggerEnter(Collider other)
    {
        EnemyCenter enemyCenter;
        if (other.TryGetComponent<EnemyCenter>(out enemyCenter))
        {
            Debug.Log("trigger");
        }
    }

    private void PlayerLogic_OnCombatEvent(object sender, System.EventArgs e)
    {

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
        FormatUnits(false);
    }

    public void FormatUnits(bool setPositionImmediately, int startIndex = 1)
    {
        for (int i = startIndex; i < unitsList.Count; i++)
        {
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
    }

    private void UpdateCountText() 
    {
        countText.text = unitsList.Count.ToString();
    }

    private void ChangeColliderRadius() 
    {
        float volume = unitsList.Count;
        if (volume == 0)
        {
            GetComponent<SphereCollider>().radius = 0.33f;
            return;
        }
        float radiusCubed = volume / ((4f / 3f) * 3.14f);
        float radius = Mathf.Pow(radiusCubed, 1f / 3f);
        GetComponent<SphereCollider>().radius = radius + unitsList.Count/100;
    }
}
