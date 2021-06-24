using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class VirtualObjectBase : ScriptableObject
{
    public string ObjectId;
    public bool IsFound = false;
    public string BeaconId;
    public string ProximityStateAtLastInteraction;




}
