using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

internal class ProximityShapes : MonoBehaviour
{
    [SerializeField]
    private GameObject objectImmediate;
    [SerializeField]
    private GameObject objectNear;
    [SerializeField]
    private GameObject objectFar;

    [SerializeField]
    private Text range;
    [SerializeField]
    private Text distance;

    private List<Beacon> _beacons = new List<Beacon>();

    private void Start()
    {
        BluetoothState.Init();
        iBeaconReceiver.BeaconRangeChangedEvent += OnBeaconRangeChanged;
        // !!! Bluetooth has to be turned on !!! TODO
        iBeaconReceiver.Scan();
        Debug.Log("Listening for beacons");
    }


    private void OnBeaconRangeChanged(Beacon[] beacons)
    {
        AddBeacons(beacons);
        RemoveOldBeacons();
        UpdateText();
    }

    private void AddBeacons(Beacon[] beacons)
    {
        foreach (Beacon b in beacons)
        {
            if (_beacons.IndexOf(b) == -1)
            {
                _beacons.Add(b);
            }
        }
    }

    private void RemoveOldBeacons()
    {
        for (int i = _beacons.Count - 1; i >= 0; --i)
        {
            if (_beacons[i].lastSeen.AddSeconds(10) < DateTime.Now)
            {
                _beacons.RemoveAt(i);
            }
        }
    }

    private void SetBeaconProperties(Beacon b)
    {
        range.text = b.range.ToString();
        distance.text = b.accuracy.ToString();
    }

    private void UpdateText()
    {
        _beacons.ForEach(SetBeaconProperties);
    }
}