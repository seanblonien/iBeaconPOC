using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class RecenterOrigin : MonoBehaviour
{
    private ARSessionOrigin origin;
    public Text txt;
    public GameObject sphere;

    public void Start()
    {
        origin = FindObjectOfType<ARSessionOrigin>();
    }

    public void moveOrigin()
    {
        //ARTrackedImageManager manager = FindObjectOfType<ARTrackedImageManager>();
        //Debug.Log($"Current position: {origin.transform.position}");
        //Debug.Log($"Next position: {transform.position}");
        //manager.transform.position = transform.position;
    }

    public void Update()
    {
        txt.text = origin.transform.position.ToString();
    }

    public void MoveSpheres()
    {

        var p = sphere.transform.position;
        sphere.transform.position = new Vector3(p.x + 1, p.y, p.z);
        Debug.Log($"New position: {sphere.transform.position}");
    }
}
