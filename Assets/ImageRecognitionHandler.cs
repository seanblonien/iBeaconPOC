using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ImageRecognitionHandler : MonoBehaviour
{
    private ARTrackedImageManager _arTrackedImageManager;

    // private readonly Vector3 scaleFactor = new Vector3(0.2F, 0.2F, 0.2F);



    // private Dictionary<string, GameObject> prefabDictionary = new Dictionary<string, GameObject>();

    private void Awake()
    {
        Debug.Log("KEON LOG - Enter method: Awake");
        _arTrackedImageManager = FindObjectOfType<ARTrackedImageManager>();
    }

    public void OnEnable()
    {
        Debug.Log("KEON LOG - Enter method: OnEnable");
        _arTrackedImageManager.trackedImagesChanged += OnImageChanged;
    }

    public void OnDisable()
    {
        Debug.Log("KEON LOG - Enter method: OnDisable");
        _arTrackedImageManager.trackedImagesChanged -= OnImageChanged;
    }

    public void OnImageChanged(ARTrackedImagesChangedEventArgs args)
    {
        foreach (var trackedImage in args.added)
        {
            Debug.Log("KEON LOG - Started tracking image: " + trackedImage.name);
            // GameObject prefab = Instantiate(_prefab, trackedImage.transform, false);
            // prefab.transform.localPosition = scaleFactor;
            // prefabDictionary.Add(trackedImage.name, prefab);
            // Debug.Log($"KEON LOG - localPosition: {prefab.transform.localPosition}");
        }
    }
}
