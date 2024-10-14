using UnityEngine;
using System.Linq;

public class EnemyCenter : UnitsCenter
{
    private void Awake()
    {
        SpawnUnits(startCount);
    }

    public override void FormatUnits(bool setPositionImmediately, int startIndex = 0)
    {
        base.FormatUnits(setPositionImmediately, startIndex);

        sortedUnits = unitsList.OrderBy(x => x.transform.position.z).ToList();
    }

    public override void SpawnUnits(int count)
    {
        for (int i = 0; i < count; i++)
        {
            EnemyUnit newUnit = Instantiate(unitPrefab, unitsStorage).GetComponent<EnemyUnit>();
            newUnit.transform.localRotation = Quaternion.Euler(0,180f,0);
            unitsList.Add(newUnit);
            newUnit.OnDieEvent += NewUnit_OnDieEvent;
        }

        FormatUnits(true, unitsList.Count - count);
    }

    private void NewUnit_OnDieEvent(object sender, System.EventArgs e)
    {
        unitsList.Remove((UnitLogic)sender);
        if (unitsList.Count == 0)
        {
            Destroy(this.gameObject);
        }
        UpdateCountText();
    }
}
