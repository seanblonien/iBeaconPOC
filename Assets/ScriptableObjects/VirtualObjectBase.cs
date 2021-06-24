using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class VirtualObjectBase : ScriptableObject
{
    public bool IsFound = false;
    public string UUID;
    public string Major;
    public string Minor;
    public BeaconRange range;
    public double distance;

    public string ObjectId
    {
        get
        {
            return UUID + "_" + Major + "_" + Minor;
        }
    }
}
