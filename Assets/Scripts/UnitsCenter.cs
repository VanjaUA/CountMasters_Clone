using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class UnitsCenter : MonoBehaviour
{
    [SerializeField] protected int startCount;
    [SerializeField] protected Transform unitsStorage;
    [SerializeField] protected GameObject unitPrefab;

    [Range(0f, 1f)] [SerializeField] protected float distanceFactor, radius;
    [SerializeField] protected TextMeshPro countText;

    protected List<UnitLogic> unitsList = new List<UnitLogic>();
    protected List<UnitLogic> sortedUnits;

    protected UnitsCenter enemyCenter;

    public float Radius { get; set; }

    public virtual void FormatUnits(bool setPositionImmediately, int startIndex = 0)
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
    }

    protected void UpdateCountText()
    {
        countText.text = unitsList.Count.ToString();
    }

    protected void ChangeColliderRadius()
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

    public int GetUnitsCount()
    {
        return unitsList.Count;
    }

    public UnitLogic RemoveFirstUnit()
    {
        if (sortedUnits.Count < 1)
        {
            return null;
        }
        UnitLogic unitToRemove = sortedUnits[0];
        sortedUnits.RemoveAt(0);
        unitToRemove.Kill();
        return unitToRemove;
    }

    public void SetEnemy(UnitsCenter enemyCenter) 
    {
        this.enemyCenter = enemyCenter;
        SetCombatDestination();
    }

    public virtual void SpawnUnits(int count) { }

    public virtual void SetCombatDestination() 
    {
        for (int i = 0; i < unitsList.Count; i++)
        {
            float xDelta = enemyCenter.transform.position.x - transform.position.x;
            float newXPosition = (transform.localPosition.x / Radius) * enemyCenter.Radius + xDelta;

            float zDelta = (enemyCenter.transform.transform.position.z + transform.position.z) / 2;
            float newZPosition = zDelta - Mathf.Abs(transform.position.z);

            Vector3 newPosition = new Vector3(newXPosition, transform.localPosition.y, newZPosition);

            unitsList[i].Destination = newPosition;
        }
    }
}
