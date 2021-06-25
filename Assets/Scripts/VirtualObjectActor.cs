using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualObjectActor : MonoBehaviour
{
    // Camera in Scene
    private Camera aRCamera;
    // Game object instances for this object, dynamically created from prefabs
    private GameObject immediateObject;
    private GameObject nearObject;
    private GameObject farObject;
    private GameObject foundObject;

    // Data for this virtual object
    public VirtualObjectBase virtualObject;

    private readonly string ComapreTag = "VirtualObject";
    private bool hasInit = false;

    public void Init(string newName, VirtualObjectBase virtualObjectBase)
    {
        name = newName;
        virtualObject = virtualObjectBase;
        transform.position += virtualObject.offset;
        immediateObject = Instantiate(virtualObject.immediatePrefab, transform);
        nearObject = Instantiate(virtualObject.nearPrefab, transform);
        farObject = Instantiate(virtualObject.farPrefab, transform);
        foundObject = Instantiate(virtualObject.foundPrefab, transform);
        aRCamera = FindObjectOfType<Camera>();
        Debug.Log($"KEON LOG - created virtual object actor at location {transform.position}");
        SetObjectsActive(false, false, false, false);
        hasInit = true;
    }

    public void Update()
    {
        if(hasInit)
        {
            HandleTouch();
            if (virtualObject.isFound)
            {
                SetFoundObjectActive();
            }
            else
            {
                if (virtualObject.range == BeaconRange.IMMEDIATE)
                {
                    SetImmediateObjectActive();
                }
                else if (virtualObject.range == BeaconRange.NEAR)
                {
                    SetNearObjectActive();
                }
                else // Used for both FAR and UNKNOWN
                {
                    SetFarObjectActive();
                }
            }
        }
    }

    private void HandleTouch()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            // Detect if object is clicked
            if (touch.phase == TouchPhase.Began)
            {
                Ray raycast = aRCamera.ScreenPointToRay(touch.position);
                RaycastHit raycastHit;
                if (Physics.Raycast(raycast, out raycastHit))
                {
                    if (raycastHit.collider.CompareTag(ComapreTag))
                    {
                        virtualObject.isFound = true;
                    }
                }
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
