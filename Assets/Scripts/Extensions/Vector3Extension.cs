using UnityEngine;

public static class Vector3Extension
{
    public static ObjectData ToObjectData(this Vector3 vector)
    {
        ObjectData objectData = new ObjectData();
        objectData.VariableValues.Add(nameof(vector.x), vector.x.ToString());
        objectData.VariableValues.Add(nameof(vector.y), vector.y.ToString());
        objectData.VariableValues.Add(nameof(vector.z), vector.z.ToString());
        return objectData;
    }

    public static Vector3 ObjectDataToVector3(ObjectData objectData)
    {
        Vector3 vector3 = new Vector3();
        float.TryParse(objectData.VariableValues[nameof(vector3.x)], out vector3.x);
        float.TryParse(objectData.VariableValues[nameof(vector3.y)], out vector3.y);
        float.TryParse(objectData.VariableValues[nameof(vector3.z)], out vector3.z);
        return vector3;
    }
}
