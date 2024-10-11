using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyCenter : MonoBehaviour
{
    [SerializeField] private int startCount;
    [SerializeField] private Transform unitsStorage;
    [SerializeField] private GameObject unitPrefab;
    private List<EnemyUnit> unitsList = new List<EnemyUnit>();
    [Range(0f, 1f)] [SerializeField] private float distanceFactor, radius;
    [SerializeField] private TextMeshPro countText;

    private PlayerLogic player;


    private void Update()
    {
        if (player == null)
        {
            return;
        }

        Vector3 movePoint = player.transform.position;
        for (int i = 0; i < unitsList.Count; i++)
        {
            unitsList[i].transform.position = Vector3.Lerp(unitsList[i].transform.position, movePoint, Time.deltaTime);

        }
    }

    private void Awake()
    {
        SpawnUnits(startCount);
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

    public void SpawnUnits(int count)
    {
        for (int i = 0; i < count; i++)
        {
            EnemyUnit newUnit = Instantiate(unitPrefab, unitsStorage).GetComponent<EnemyUnit>();
            newUnit.transform.localRotation = Quaternion.Euler(0,180f,0);
            unitsList.Add(newUnit);
            newUnit.OnDieEvent += NewUnit_OnDieEvent; ;
        }

        FormatUnits(true, unitsList.Count - count);
    }

    private void NewUnit_OnDieEvent(object sender, System.EventArgs e)
    {
        unitsList.Remove((EnemyUnit)sender);
        if (unitsList.Count == 0)
        {
            Destroy(this.gameObject);
        }
        UpdateCountText();
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
        GetComponent<SphereCollider>().radius = radius + unitsList.Count / 100;
    }

    public void Attack(PlayerLogic player) 
    {
        this.player = player;
    }

    public int GetUnitsCount() 
    {
        return unitsList.Count;
    }

    public EnemyUnit GetFirstEnemyUnit() 
    {
        if (unitsList.Count == 0)
        {
            return null;
        }
        return unitsList[unitsList.Count - 1];
    }
}
