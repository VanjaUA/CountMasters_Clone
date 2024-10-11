using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Gate : MonoBehaviour
{
    public enum GateType
    {
        Addition,
        Subrtactoin,
        Multiplication,
    }

    private readonly Dictionary<GateType, string> gateTypeToSign = new Dictionary<GateType, string>
    {
        {GateType.Addition, "+"},
        {GateType.Subrtactoin, "-"},
        {GateType.Multiplication, "X"}
    };

    [SerializeField] private GateType gateType;
    [SerializeField] private uint gateValue;
    private TextMeshPro gateText;

    private void Awake()
    {
        gateText = GetComponentInChildren<TextMeshPro>();
        UpdateText();
    }

    private void UpdateText() 
    {
        gateText.text = gateTypeToSign[gateType] + gateValue.ToString();
    }

    private void OnTriggerEnter(Collider other)
    {
        UnitLogic unit;
        if (other.TryGetComponent<UnitLogic>(out unit))
        {
            unit.EnterGateTrigger(new UnitLogic.OnGateTrigger_EventArgs() { gateType = gateType, value = gateValue });
            GetComponent<BoxCollider>().enabled = false;
            return;
        }
    }
}
