using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualObjectContainer : MonoBehaviour
{
    public List<VirtualObjectBase> virtualObjects;
    private Dictionary<ZoneNames, GameObject> zones = new Dictionary<ZoneNames, GameObject>();
    public StringVariable activeZone;
    public GameObject virtualObjectPrefab;

    // Start is called before the first frame update
    void Start()
    {
        // Create zones
        foreach (ZoneNames zone in Enum.GetValues(typeof(ZoneNames)))
        {
            var zoneObject = new GameObject(zone.ToString());
            zoneObject.transform.SetParent(transform);
            zones.Add(zone, zoneObject);
            Debug.Log($"KEON LOG - created zone with name: {zone}");
        }
        Debug.Log($"KEON LOG - zone keys: {zones.Keys} length: {zones.Keys.Count}");
        Debug.Log($"KEON LOG - zone values: {zones.Values}");
        // Create Virtual Objects in each zone
        foreach (var virtualObjectData in virtualObjects)
        {
            var virtualObject = Instantiate(virtualObjectPrefab, zones[virtualObjectData.zoneName].transform, false);
            var actor = virtualObject.GetComponent<VirtualObjectActor>();
            actor.Init(virtualObjectData.name ,virtualObjectData);
            Debug.Log($"KEON LOG - virtual object zone name: {virtualObjectData.zoneName}");
            Debug.Log($"KEON LOG - created virtual object with name: {virtualObjectData.name}");
        }
    }

    private void Update()
    {
        Debug.Log($"KEON LOG - activeZone.Value: {activeZone.Value} !notemptyornull: {!string.IsNullOrEmpty(activeZone.Value)}");
        if (!string.IsNullOrEmpty(activeZone.Value))
        {
            Debug.Log($"KEON LOG - before parse: {activeZone.Value}");
            var currentActiveZone = (ZoneNames)Enum.Parse(typeof(ZoneNames), activeZone.Value);
            Debug.Log($"KEON LOG - after parse: {currentActiveZone}");
            foreach (var zone in zones)
            {
                var active = zone.Key == currentActiveZone ? "true" : "false";
                Debug.Log($"KEON LOG - after parse, activating: {zone.Key} with active: {active}");
                zone.Value.SetActive(zone.Key == currentActiveZone);
            }
        }
    }
}
