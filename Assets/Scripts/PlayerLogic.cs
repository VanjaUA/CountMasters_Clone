using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLogic : MonoBehaviour
{
    private const int MAX_COUNT = 100;

    [SerializeField] private GameObject unitPrefab;
    private int unitsCount;

    public void SpawnUnits(int count) 
    {
        if (unitsCount + count > MAX_COUNT)
        {
            count = MAX_COUNT - unitsCount;
        }

        for (int i = 0; i < count; i++)
        {
            Vector2 randomDirection = Random.insideUnitCircle;

            GameObject newUnit = Instantiate(unitPrefab, this.transform);
            newUnit.transform.localPosition = Vector3.zero + new Vector3(randomDirection.x,0f, randomDirection.y);
        }
    }
}
