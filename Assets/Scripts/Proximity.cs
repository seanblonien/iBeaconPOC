using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

class Proximity : MonoBehaviour
{
    // Scriptable Object for persisting beacon status values
    [SerializeField]
    private List<VirtualObjectBase> virtualObjects;

    // Bluetooth status text value
    [SerializeField]
    private StringVariable _statusText;

    // Pirvate list of beacons
    private List<Beacon> _beacons = new List<Beacon>();

    // Co-routine for enabling bluetooth asynchronously
    IEnumerator coroutine;

    // Time to wait inbetween intervals of hecking if bluetooth has been turned on
    readonly private float bluetoothWaitTime = 0.5f;

    /// <summary>
    /// Initializes Bluetooth and registers the BeaconRangeChange event with the iBeacon receiver
    /// </summary>
    private void Start()
    {
        Debug.Log("Enabling bluetooth");
        BluetoothState.EnableBluetooth();
        BluetoothState.BluetoothStateChangedEvent += delegate (BluetoothLowEnergyState state) {
            switch (state)
            {
                case BluetoothLowEnergyState.TURNING_OFF:
                case BluetoothLowEnergyState.TURNING_ON:
                    break;
                case BluetoothLowEnergyState.UNKNOWN:
                case BluetoothLowEnergyState.RESETTING:
                    _statusText.Value = "Checking Device…";
                    break;
                case BluetoothLowEnergyState.UNAUTHORIZED:
                    _statusText.Value = "You don't have the permission to use beacons.";
                    break;
                case BluetoothLowEnergyState.UNSUPPORTED:
                    _statusText.Value = "Your device doesn't support beacons.";
                    break;
                case BluetoothLowEnergyState.POWERED_OFF:
                    _statusText.Value = "Enable Bluetooth";
                    break;
                case BluetoothLowEnergyState.POWERED_ON:
                    _statusText.Value = "Bluetooth already enabled";
                    break;
                case BluetoothLowEnergyState.IBEACON_ONLY:
                    _statusText.Value = "iBeacon only";
                    break;
                default:
                    _statusText.Value = "Unknown Error";
                    break;
            }
        };
        iBeaconReceiver.BeaconRangeChangedEvent += OnBeaconRangeChanged;
        coroutine = StartScan();
        StartCoroutine(coroutine);
    }

    /// <summary>
    /// <a href="https://docs.unity3d.com/Manual/Coroutines.html">Co-routine</a> for waiting for BLE to be turned on and start iBeacon scanning.
    /// </summary>
    /// <returns>Enumerator that waits for BLE to be enabled</returns>
    private IEnumerator StartScan()
    {
        // Bluetooth has to be turned on
        // Unitl bluetooth is on, wait for the state to change indefinitely
        while(BluetoothState.GetBluetoothLEStatus() != BluetoothLowEnergyState.POWERED_ON)
        {
            Debug.Log("Waiting for BLE with state: " + BluetoothState.GetBluetoothLEStatus().ToString());
            yield return new WaitForSeconds(bluetoothWaitTime);
        }
        iBeaconReceiver.Scan();
        Debug.Log("Now listening for iBeacons");
        yield break;
    }

    /// <summary>
    /// Beacon change event handler.
    /// </summary>
    /// <param name="beacons">The current list of beacons picked up by the iBeaconReceiver</param>
    private void OnBeaconRangeChanged(Beacon[] beacons)
    {
        AddBeacons(beacons);
        RemoveOldBeacons();
        PersistBeaconValues();
    }

    /// <summary>
    /// Adds the given beacons to the interal beacon list, updating beacons already added.
    /// </summary>
    /// <param name="beacons">Beacons to add to the beacon list</param>
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

    /// <summary>
    /// Removes old, expired beacons that haven't been detected for 10 seconds
    /// </summary>
    private void RemoveOldBeacons()
    {
        _beacons.FindAll(b => b.lastSeen.AddSeconds(10) < DateTime.Now).ForEach(b => _beacons.Remove(b));
    }

    /// <summary>
    /// Persists the data values for this beacon to the correspondng to its virtual object.
    /// </summary>
    /// <param name="b">The beacon to use for getting data values to persist</param>
    private void SetBeaconProperties(Beacon b)
    {
        var virtualObject = virtualObjects.Find((obj) => obj.ObjectId == b.ObjectId);
        if(virtualObject != null)
        {
            virtualObject.range = b.range;
            virtualObject.distance = b.accuracy;
        }
    }

    private void PersistBeaconValues()
    {
        _beacons.ForEach(SetBeaconProperties);
    }
}