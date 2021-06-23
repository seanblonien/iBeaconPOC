using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Collections;

class Proximity : MonoBehaviour
{
    // GameObjects to show/hide according to proximity
    [SerializeField]
    private GameObject objectImmediate;
    [SerializeField]
    private GameObject objectNear;
    [SerializeField]
    private GameObject objectFar;

    // UI text fields for displaying values
    [SerializeField]
    private Text range;
    [SerializeField]
    private Text distance;
    [SerializeField]
    private Text _statusText;

    private List<Beacon> _beacons = new List<Beacon>();

    // Co-routine for enabling bluetooth asynchronously
    IEnumerator coroutine;

    readonly private float bluetoothWaitTime = 1f;

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
        Debug.Log("Listening for iBeacons");
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
        UpdateText();
    }

    /// <summary>
    /// Adds the given beacons to the interal beacon list, ignoring beacons already added.
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
    /// Sets the UI text elements with the given range/distance of the beacon.
    /// Also shows/hides the given prefabs depending on how far the beacon is.
    /// </summary>
    /// <param name="b">The beacon to use for setting properties</param>
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