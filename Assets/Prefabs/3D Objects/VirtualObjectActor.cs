using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualObjectActor : MonoBehaviour
{

    public GameObject nearObject;
    public GameObject farObject;
    public GameObject immediateObject;

    public VirtualObjectBase objectStateContainer;

    public void SetNearObjectActive()
    {
        if(nearObject != null)
        {
            nearObject.SetActive(true);
            farObject.SetActive(false);
            immediateObject.SetActive(false);
            Debug.Log("Near object found and set to active");
        }
        else
        {
            Debug.Log("Near object was null dummy");
        }

    }


    public void SetFarObjectActive()
    {
        if (farObject != null)
        {
            farObject.SetActive(true);
            nearObject.SetActive(false);
            immediateObject.SetActive(false);
            Debug.Log("far object found and set to active");
        }
        else
        {
            Debug.Log("far object was null dummy");
        }
    }

    public void SetImmediateObjectActive()
    {
        if (immediateObject != null)
        {
            immediateObject.SetActive(true);
            farObject.SetActive(false);
            nearObject.SetActive(false);
            Debug.Log("immediate object found and set to active");
        }
        else
        {
            Debug.Log("immediate object was null dummy");
        }
    }
}
