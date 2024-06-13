using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector3Extension
{
    public static ObjectData ToObjectData(this Vector3 vector)
    {
        ObjectData objectData = new ObjectData();
        objectData.variableValues.Add(nameof(vector.x), vector.x.ToString());
        objectData.variableValues.Add(nameof(vector.y), vector.y.ToString());
        objectData.variableValues.Add(nameof(vector.z), vector.z.ToString());
        return objectData;
    }

    public static Vector3 ObjectDataToVector3(ObjectData objectData)
    {
        Vector3 vector3 = new Vector3();
        float.TryParse(objectData.variableValues[nameof(vector3.x)], out vector3.x);
        float.TryParse(objectData.variableValues[nameof(vector3.y)], out vector3.y);
        float.TryParse(objectData.variableValues[nameof(vector3.z)], out vector3.z);
        return vector3;
    }
}
