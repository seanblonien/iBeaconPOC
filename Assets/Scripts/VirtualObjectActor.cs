using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualObjectActor : MonoBehaviour
{

    public GameObject immediateObject;
    public GameObject nearObject;
    public GameObject farObject;
    public GameObject foundObject;
    private Camera aRCamera;

    public VirtualObjectBase objectStateContainer;

    public void Start()
    {
        SetObjectsActive(false, false, false, false);
        aRCamera = FindObjectOfType<Camera>();
        Debug.Log($"Touch - Camera: {aRCamera}");
    }

    public void Update()
    {
        if(Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            // Detect if object is clicked
            if (touch.phase == TouchPhase.Began)
            {
                Debug.Log("Touch - Begin touch");
                Ray raycast = aRCamera.ScreenPointToRay(touch.position);
                RaycastHit raycastHit;
                if (Physics.Raycast(raycast, out raycastHit))
                {
                    Debug.Log("Touch - Hit");
                    if (raycastHit.collider.CompareTag("VirtualObject"))
                    {
                        objectStateContainer.IsFound = true;
                        Debug.Log("Touch - Found");
                    }
                }
            }
        }

        if(objectStateContainer.IsFound)
        {
            SetFoundObjectActive();
        }
        else
        {
            if (objectStateContainer.range == BeaconRange.IMMEDIATE)
            {
                SetImmediateObjectActive();
            }
            else if (objectStateContainer.range == BeaconRange.NEAR)
            {
                SetNearObjectActive();
            }
            else if (objectStateContainer.range == BeaconRange.FAR)
            {
                SetFarObjectActive();
            }
        }
    }

    private void SetObjectsActive(bool immediate, bool near, bool far, bool found)
    {
        immediateObject.SetActive(immediate);
        nearObject.SetActive(near);
        farObject.SetActive(far);
        foundObject.SetActive(found);
    }

    private void SetObjectActive(GameObject obj, bool immediate, bool near, bool far, bool found)
    {
        if (obj != null)
        {
            SetObjectsActive(immediate, near, far, found);
            Debug.Log($"SetObjectActive - {obj.name} object found and set to active");
        }
        else
        {
            Debug.Log($"Object object was null");
        }
    }

    private void SetImmediateObjectActive()
    {
        SetObjectActive(farObject, true, false, false, false);
    }

    private void SetNearObjectActive()
    {
        SetObjectActive(nearObject, false, true, false, false);
    }

    private void SetFarObjectActive()
    {
        SetObjectActive(farObject, false, false, true, false);
    }

    private void SetFoundObjectActive()
    {
        SetObjectActive(foundObject, false, false, false, true);
    }
}
