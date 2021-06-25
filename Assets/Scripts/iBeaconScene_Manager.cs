using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class iBeaconScene_Manager : MonoBehaviour
{
    [SerializeField]
    private Text range;
    [SerializeField]
    private Text distance;
    [SerializeField]
    private Text isFoundText;
    [SerializeField]
    private Text statusText;

    [SerializeField]
    private VirtualObjectBase beacon;
    [SerializeField]
    private StringVariable status;

    public void Update()
    {
        range.text = beacon.range.ToString();
        distance.text = beacon.distance.ToString();
        isFoundText.text = beacon.isFound.ToString();
        statusText.text = status.Value;
    }
}
