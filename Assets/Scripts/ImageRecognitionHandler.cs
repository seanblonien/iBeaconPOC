using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ImageRecognitionHandler : MonoBehaviour
{
    // Variables
    private ARTrackedImageManager _arTrackedImageManager;

    public StringVariable activeZone;

    // Init Method
    private void Awake()
    {
        Debug.Log("KEON LOG - Enter method: Start");
        _arTrackedImageManager = FindObjectOfType<ARTrackedImageManager>();
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
        Debug.Log($"KEON LOG - OnImageChanged");
        // When images are tracked for the very first time
        foreach (var trackedImage in args.added)
        {
            Debug.Log($"KEON LOG - Started tracking image: {trackedImage.referenceImage.name}");
            activeZone.Value = trackedImage.referenceImage.name;
        }

        // When images are tracked subsequent times
        foreach(var trackedImage in args.updated)
        {
            Debug.Log($"KEON LOG - Updated image: {trackedImage.referenceImage.name}");
            activeZone.Value = trackedImage.referenceImage.name;
        }
        
    }

    #region Implementation Details
    ///// <summary>
    ///// Get all immediate children of the parent game object
    ///// </summary>
    ///// <param name="parentContainer"></param>
    //private static Dictionary<string, GameObject> GetImmediateChildrenGameObjects(GameObject parentContainer)
    //{
    //    var childGameObjects = new Dictionary<string, GameObject>();
    //    var parentTransform = parentContainer.transform;
    //    foreach (Transform child in parentTransform)
    //    {
    //        childGameObjects.Add(child.gameObject.name, child.gameObject);
    //    }

    //    return childGameObjects;
    //}

    ///// <summary>
    ///// Dynamically handle the activating and deactivating of multiple objects based on which image is tracked.
    ///// </summary>
    ///// <param name="trackedImage"></param>
    //private void ActivateZone(ARTrackedImage trackedImage)
    //{
    //    var activeZone = _zoneContainers[trackedImage.referenceImage.name];
    //    Debug.Log($"KEON LOG - active zone{activeZone.name}");
    //    activeZone.SetActive(true);
    //    DeactivateAllOtherZones(activeZone);
    //}

    ///// <summary>
    ///// Deactivate all zone containers that aren't associated with the currently tracked image
    ///// </summary>
    ///// <param name="activeZone"></param>
    //private void DeactivateAllOtherZones(GameObject activeZone)
    //{
    //    foreach (GameObject zone in _zoneContainers.Values)
    //    {
    //        if (zone.name != activeZone.name)
    //        {
    //            Debug.Log($"KEON LOG - Setting this container to inactive: {zone.name}");
    //            zone.SetActive(false);
    //        }
    //    }
    //}

    #endregion
}
