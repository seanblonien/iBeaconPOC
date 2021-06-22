using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualObjectBase : ScriptableObject
{

    public ObjectStateEnum objectState;
    public string objectName;

    public VirtualObjectBase()
    {
        objectState = ObjectStateEnum.notFound_Far;
    }



}
