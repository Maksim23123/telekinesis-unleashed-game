using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class QuaternionExtension
{
    public static ObjectData ToObjectData(this Quaternion quaternion)
    {
        ObjectData objectData = new ObjectData();
        objectData.variableValues.Add(nameof(quaternion.x), quaternion.x.ToString());
        objectData.variableValues.Add(nameof(quaternion.y), quaternion.y.ToString());
        objectData.variableValues.Add(nameof(quaternion.z), quaternion.z.ToString());
        objectData.variableValues.Add(nameof(quaternion.w), quaternion.w.ToString());
        return objectData;
    }

    public static Quaternion ObjectDataToVector3(ObjectData objectData)
    {
        float x, y, z, w;
        if (    float.TryParse(objectData.variableValues[nameof(x)], out x)
                && float.TryParse(objectData.variableValues[nameof(y)], out y)
                && float.TryParse(objectData.variableValues[nameof(z)], out z)
                && float.TryParse(objectData.variableValues[nameof(w)], out w))
        {
            return new Quaternion(x, y, z, w);
        }
        else
            return new Quaternion();
    }
}
