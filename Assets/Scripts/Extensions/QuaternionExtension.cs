using UnityEngine;

public static class QuaternionExtension
{
    public static ObjectData ToObjectData(this Quaternion quaternion)
    {
        ObjectData objectData = new ObjectData();
        objectData.VariableValues.Add(nameof(quaternion.x), quaternion.x.ToString());
        objectData.VariableValues.Add(nameof(quaternion.y), quaternion.y.ToString());
        objectData.VariableValues.Add(nameof(quaternion.z), quaternion.z.ToString());
        objectData.VariableValues.Add(nameof(quaternion.w), quaternion.w.ToString());
        return objectData;
    }

    public static Quaternion ObjectDataToVector3(ObjectData objectData)
    {
        float x, y, z, w;
        if (float.TryParse(objectData.VariableValues[nameof(x)], out x)
                && float.TryParse(objectData.VariableValues[nameof(y)], out y)
                && float.TryParse(objectData.VariableValues[nameof(z)], out z)
                && float.TryParse(objectData.VariableValues[nameof(w)], out w))
        {
            return new Quaternion(x, y, z, w);
        }
        else
            return new Quaternion();
    }
}
