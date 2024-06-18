using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


[Serializable]
public class ObjectData
{
    public Dictionary<string, string> variableValues = new Dictionary<string, string>();

    public Dictionary<string, ObjectData> objectDataUnits = new Dictionary<string, ObjectData>();
}
