using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;

public class EnemyCenter : MonoBehaviour
{
    [SerializeField] private int startCount;
    [SerializeField] private Transform unitsStorage;
    [SerializeField] private GameObject unitPrefab;
    private List<EnemyUnit> unitsList = new List<EnemyUnit>();
    private List<EnemyUnit> sortedUnits;
    [Range(0f, 1f)] [SerializeField] private float distanceFactor, radius;
    [SerializeField] private TextMeshPro countText;

    public float Radius { get; set; }

    private PlayerLogic player;


    private void Update()
    {
        if (player == null)
        {
            return;
        }

        for (int i = 0; i < unitsList.Count; i++)
        {
            float xDelta = player.transform.position.x - transform.position.x;
            float newXPosition = (transform.localPosition.x / Radius) * player.Radius + xDelta;
            float newZPosition = player.transform.position.z - transform.position.z;
            Vector3 newPosition = new Vector3(newXPosition, transform.localPosition.y, newZPosition);

            unitsList[i].Destination = newPosition;
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

        sortedUnits = unitsList.OrderBy(x => x.transform.position.z).ToList();
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

    public EnemyUnit RemoveFirstUnit() 
    {
        if (sortedUnits.Count < 1)
        {
            return null;
        }
        EnemyUnit unitToRemove = sortedUnits[0];
        sortedUnits.RemoveAt(0);
        unitToRemove.Kill();
        return unitToRemove;
    }
}
