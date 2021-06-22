using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Collections;

class ProximityShapes : MonoBehaviour
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
    [SerializeField]
    private Text _statusText;

    private List<Beacon> _beacons = new List<Beacon>();
    IEnumerator coroutine;

    private void Start()
    {
        Debug.Log("SeaniBeacon - enble bluetooth");
        BluetoothState.EnableBluetooth();
        BluetoothState.BluetoothStateChangedEvent += delegate (BluetoothLowEnergyState state) {
            switch (state)
            {
                case BluetoothLowEnergyState.TURNING_OFF:
                case BluetoothLowEnergyState.TURNING_ON:
                    break;
                case BluetoothLowEnergyState.UNKNOWN:
                case BluetoothLowEnergyState.RESETTING:
                    _statusText.text = "Checking Device…";
                    break;
                case BluetoothLowEnergyState.UNAUTHORIZED:
                    _statusText.text = "You don't have the permission to use beacons.";
                    break;
                case BluetoothLowEnergyState.UNSUPPORTED:
                    _statusText.text = "Your device doesn't support beacons.";
                    break;
                case BluetoothLowEnergyState.POWERED_OFF:
                    _statusText.text = "Enable Bluetooth";
                    break;
                case BluetoothLowEnergyState.POWERED_ON:
                    _statusText.text = "Bluetooth already enabled";
                    break;
                case BluetoothLowEnergyState.IBEACON_ONLY:
                    _statusText.text = "iBeacon only";
                    break;
                default:
                    _statusText.text = "Unknown Error";
                    break;
            }
        };
        iBeaconReceiver.BeaconRangeChangedEvent += OnBeaconRangeChanged;
        coroutine = StartScan();
        StartCoroutine(coroutine);
    }

    private IEnumerator StartScan()
    {
        // !!! Bluetooth has to be turned on !!! TODO
        Debug.Log("SeaniBeacon - while loop");
        while(BluetoothState.GetBluetoothLEStatus() != BluetoothLowEnergyState.POWERED_ON)
        {
            Debug.Log("SeaniBeacon - LE state: " + BluetoothState.GetBluetoothLEStatus().ToString());
            yield return new WaitForSeconds(1f);
        }
        iBeaconReceiver.Scan();
        Debug.Log("SeaniBeacon - Listening for beacons");
        yield break;
    }


    private void OnBeaconRangeChanged(Beacon[] beacons)
    {
        Debug.Log("SeaniBeacon - Beacons changed " + DateTime.Now.ToString());
        Debug.Log("SeaniBeacon - Beacons changed count " + beacons.Length);
        Debug.Log("SeaniBeacon - Beacons changed new " + _beacons.IndexOf(beacons[0]).ToString());
        AddBeacons(beacons);
        RemoveOldBeacons();
        UpdateText();
    }

    private void AddBeacons(Beacon[] beacons)
    {
        foreach (Beacon b in beacons)
        {
            var index = _beacons.IndexOf(b);
            if (index == -1)
            {
                _beacons.Add(b);
            }
            else
            {
                _beacons[index] = b;
            }
        }
    }

    private void RemoveOldBeacons()
    {
        _beacons.FindAll(b => b.lastSeen.AddSeconds(10) < DateTime.Now).ForEach(b => _beacons.Remove(b));
    }

    private void SetBeaconProperties(Beacon b)
    {
        range.text = b.range.ToString();
        distance.text = b.accuracy.ToString();
        objectImmediate.SetActive(b.range == BeaconRange.IMMEDIATE);
        objectNear.SetActive(b.range == BeaconRange.NEAR);
        objectFar.SetActive(b.range == BeaconRange.FAR);
    }

    private void UpdateText()
    {
        _beacons.ForEach(SetBeaconProperties);
    }
}