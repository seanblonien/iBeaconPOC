using System.Collections.Generic;
using System.Linq;
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
        _prefabZoneContainers = GetImmediateChildrenGameObjects(_prefabContainer);
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
    /// <summary>
    /// Get all immediate children of the parent game object
    /// </summary>
    /// <param name="parentContainer"></param>
    private Dictionary<string, GameObject> GetImmediateChildrenGameObjects(GameObject parentContainer)
    {
        var childGameObjects = new Dictionary<string, GameObject>();
        var parentTransform = parentContainer.transform;
        foreach (Transform child in parentTransform)
        {
            childGameObjects.Add(child.gameObject.name, child.gameObject);
        }

        return childGameObjects;
    }

    /// <summary>
    /// Dynamically handle the activating and deactivating of multiple objects based on which image is tracked.
    /// </summary>
    /// <param name="trackedImage"></param>
    private void ActivateNewZoneForTrackedImage(ARTrackedImage trackedImage)
    {
        GameObject activeContainer;
        switch (trackedImage.referenceImage.name)
        {
            case _zone1TrackedImageName:
                _prefabZoneContainers.TryGetValue(_zone1PrefabContainerName, out activeContainer);
                Debug.Log($"KEON LOG - activeContainer for jira-avatar: {activeContainer.name}");
                GameObject zone1Container = ActivateZoneContainer(activeContainer, trackedImage);
                DeactivateInactiveObjectStates(zone1Container);
                DeactivateAllOtherZones(zone1Container);
                break;
            case _zone2TrackedImageName:
                _prefabZoneContainers.TryGetValue(_zone2PrefabContainerName, out activeContainer);
                Debug.Log($"KEON LOG - activeContainer for shapes: {activeContainer.name}");
                GameObject zone2Container = ActivateZoneContainer(activeContainer, trackedImage);
                DeactivateInactiveObjectStates(zone2Container);
                DeactivateAllOtherZones(zone2Container);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Get zone container that correlates to the image that has just been tracked.
    /// If the container has already been instantiated, get it from the zone containers dictionary
    /// Otherwise, instantiate the zone container and add it to the dictionary.
    /// </summary>
    /// <param name="prefabZoneContainer"></param>
    /// <param name="trackedImage"></param>
    /// <returns>Activated Zone Object</returns>
    private GameObject ActivateZoneContainer(GameObject prefabZoneContainer, ARTrackedImage trackedImage)
    {
        GameObject container;
        if (_zoneContainers.ContainsKey(prefabZoneContainer.name))
        {
            _zoneContainers.TryGetValue(prefabZoneContainer.name, out container);
            Debug.Log($"KEON LOG - found container: {container.name}");
            container.SetActive(true);
            return container;
        } else
        {
            container = Instantiate(prefabZoneContainer, trackedImage.transform, false);
            container.name = prefabZoneContainer.name;
            Debug.Log($"KEON LOG - Instantiated container: {container.name}");
            _zoneContainers.Add(container.name, container);
            container.SetActive(true);
            return container;
        }
    }

    /// <summary>
    /// Deactivate all zone containers that aren't associated with the currently tracked image
    /// </summary>
    /// <param name="activeZone"></param>
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

    /// <summary>
    /// Deactivates all object states except the initial states. TODO: make more dynamic after state management has been introduced.
    /// </summary>
    /// <param name="activeZoneContainer"></param>
    private void DeactivateInactiveObjectStates(GameObject activeZoneContainer)
    {
        // in this case, zoneSubContainers should contain the Green, Blue, and Red containers for the active zone
        var zoneSubContainers = GetImmediateChildrenGameObjects(activeZoneContainer);

        foreach(GameObject zoneSubContainer in zoneSubContainers.Values)
        {
            // ex: Green Cube, Green Capsule, etc.
            var subContainerGameObjects = GetImmediateChildrenGameObjects(zoneSubContainer);

            // ensure only first game object in each sub-container is active
            for(int i = 0; i < subContainerGameObjects.Count; i++)
            {
                if(subContainerGameObjects.Count > 1 && i > 0)
                {
                    subContainerGameObjects.ElementAt(i).Value.SetActive(false);
                }
            }

        }

    }

    #endregion
}
