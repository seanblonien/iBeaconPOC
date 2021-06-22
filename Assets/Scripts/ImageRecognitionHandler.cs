using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ImageRecognitionHandler : MonoBehaviour
{
    // Constants
    private const string _zone1TrackedImageName = "jira-avatar";
    private const string _zone2TrackedImageName = "shapes";
    private const string _zone1PrefabContainerName = "Zone1Container";
    private const string _zone2PrefabContainerName = "Zone2Container";

    // Variables
    private ARTrackedImageManager _arTrackedImageManager;
    private Dictionary<string, GameObject> _prefabZoneContainers;
    private Dictionary<string, GameObject> _zoneContainers = new Dictionary<string, GameObject>();
    [SerializeField]
    private GameObject _prefabContainer;

    // Init Method
    private void Awake()
    {
        Debug.Log("KEON LOG - Enter method: Awake");
        _arTrackedImageManager = FindObjectOfType<ARTrackedImageManager>();
        _prefabZoneContainers = GetAllZoneContainers(_prefabContainer);
    }

    // When MonoBehaviour is enabled
    public void OnEnable()
    {
        Debug.Log("KEON LOG - Enter method: OnEnable");
        _arTrackedImageManager.trackedImagesChanged += OnImageChanged;
    }

    //When MonoBehaviour is disabled
    public void OnDisable()
    {
        Debug.Log("KEON LOG - Enter method: OnDisable");
        _arTrackedImageManager.trackedImagesChanged -= OnImageChanged;
    }

    // Event listener for all image state changes
    public void OnImageChanged(ARTrackedImagesChangedEventArgs args)
    {
        // When images are tracked for the very first time
        foreach (var trackedImage in args.added)
        {
            Debug.Log($"KEON LOG - Started tracking image: {trackedImage.referenceImage.name}");
            ActivateNewZoneForTrackedImage(trackedImage);
        }

        // When images are tracked subsequent times
        foreach(var trackedImage in args.updated)
        {
            Debug.Log($"KEON LOG - Updated image: {trackedImage.referenceImage.name}");
            ActivateNewZoneForTrackedImage(trackedImage);
        }
        
    }

    #region Implementation Details
    // Get all immediate children of the parent prefab container - in this case, Zone1Container and Zone2Container
    private Dictionary<string, GameObject> GetAllZoneContainers(GameObject parentPrefab)
    {
        var zoneContainers = new Dictionary<string, GameObject>();
        var parentTransform = parentPrefab.transform;
        Debug.Log($"KEON LOG - parent of which to get children: {parentPrefab.name}");
        foreach (Transform child in parentTransform)
        {
            Debug.Log($"KEON LOG - Adding {child.gameObject.name} to zoneContainers dictionary.");
            zoneContainers.Add(child.gameObject.name, child.gameObject);
        }

        return zoneContainers;
    }

    // Dynamically handle the activating and deactivating of multiple objects based on which image is tracked.
    private void ActivateNewZoneForTrackedImage(ARTrackedImage trackedImage)
    {
        GameObject activeContainer;
        switch (trackedImage.referenceImage.name)
        {
            case _zone1TrackedImageName:
                _prefabZoneContainers.TryGetValue(_zone1PrefabContainerName, out activeContainer);
                Debug.Log($"KEON LOG - activeContainer for jira-avatar: {activeContainer.name}");
                GameObject zone1Container = GetZoneContainer(activeContainer, trackedImage);
                zone1Container.SetActive(true);
                DeactivateAllOtherZones(zone1Container);
                break;
            case _zone2TrackedImageName:
                _prefabZoneContainers.TryGetValue(_zone2PrefabContainerName, out activeContainer);
                Debug.Log($"KEON LOG - activeContainer for shapes: {activeContainer.name}");
                GameObject zone2Container = GetZoneContainer(activeContainer, trackedImage);
                zone2Container.SetActive(true);
                DeactivateAllOtherZones(zone2Container);
                break;
            default:
                break;
        }
    }

    /* Get zone container that correlates to the image that has just been tracked.
       If the container has already been instantiated, get it from the zone containers dictionary
       Otherwise, instantiate the zone container and add it to the dictionary. */
    private GameObject GetZoneContainer(GameObject prefabZoneContainer, ARTrackedImage trackedImage)
    {
        GameObject container;
        if (_zoneContainers.ContainsKey(prefabZoneContainer.name))
        {
            _zoneContainers.TryGetValue(prefabZoneContainer.name, out container);
            Debug.Log($"KEON LOG - found container: {container.name}");
            return container;
        } else
        {
            container = Instantiate(prefabZoneContainer, trackedImage.transform, false);
            container.name = prefabZoneContainer.name;
            Debug.Log($"KEON LOG - Instantiated container: {container.name}");
            _zoneContainers.Add(container.name, container);
            return container;
        }
    }

    // Deactivate all zone containers that aren't associated with the currently tracked image
    private void DeactivateAllOtherZones(GameObject activeZone)
    {
        foreach (GameObject zone in _zoneContainers.Values)
        {
            if (zone.name != activeZone.name)
            {
                Debug.Log($"KEON LOG - Setting this container to inactive: {zone.name}");
                zone.SetActive(false);
            }
        }
    }
    #endregion
}
