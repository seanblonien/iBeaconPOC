using UnityEngine;

[CreateAssetMenu]
public class VirtualObjectBase : ScriptableObject
{
    // Virtual Object values
    public ZoneNames zoneName;
    public GameObject immediatePrefab;
    public GameObject nearPrefab;
    public GameObject farPrefab;
    public GameObject foundPrefab;

    public Vector3 offset;

    // Game state values
    public bool isFound = false;

    // Beacon values
    public string UUID;
    public string major;
    public string minor;
    public BeaconRange range;
    public double distance;
    public string ObjectId
    {
        get
        {
            return UUID + "_" + major + "_" + minor;
        }
    }
}
