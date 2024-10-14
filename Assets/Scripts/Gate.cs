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
    [SerializeField] Transform transparentBox;

    private bool doubleGate;
    private Gate secondGate;

    private void Awake()
    {
        gateText = GetComponentInChildren<TextMeshPro>();
        UpdateText();
    }

    private void Start()
    {
        doubleGate = DoubleGateCheck();
    }

    private bool DoubleGateCheck() 
    {
        Vector3 startPos = new Vector3(InputManager.Instance.GetRoadWidth() / -2f, 1f, transform.position.z);
        Vector3 endPos = new Vector3(InputManager.Instance.GetRoadWidth() / 2f, 1f, transform.position.z);
        RaycastHit hit;
        if (Physics.Linecast(startPos, endPos, out hit))
        {
            if (hit.transform.TryGetComponent<Gate>(out secondGate))
            {
                if (secondGate != this)
                {
                    return true;
                }
            }
        }
        if (Physics.Linecast(endPos, startPos, out hit))
        {
            if (hit.transform.TryGetComponent<Gate>(out secondGate))
            {
                if (secondGate != this)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void UpdateText() 
    {
        gateText.text = gateTypeToSign[gateType] + gateValue.ToString();
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerUnit unit;
        if (other.TryGetComponent<PlayerUnit>(out unit))
        {
            unit.EnterGateTrigger(new PlayerUnit.OnGateTrigger_EventArgs() { gateType = gateType, value = gateValue });
            DisableGate();
            if (doubleGate)
            {
                secondGate.DisableGate();
            }
            return;
        }
    }

    public void DisableGate() 
    {
        GetComponent<BoxCollider>().enabled = false;
        gateText.gameObject.SetActive(false);
        transparentBox.gameObject.SetActive(false);
    }
}
