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
    private Text status;

    [SerializeField]
    private StringVariable rangeValue;
    [SerializeField]
    private StringVariable distanceValue;
    [SerializeField]
    private StringVariable statusValue;

    public void Update()
    {
        range.text = rangeValue.Value;
        distance.text = distanceValue.Value;
        status.text = statusValue.Value;
    }
}
