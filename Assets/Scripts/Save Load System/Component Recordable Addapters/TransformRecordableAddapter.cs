using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformRecordableAddapter : MonoBehaviour, IRecordable
{
    public int Priority => 0;

    [SerializeField]
    bool _savePossition = true;

    [SerializeField]
    bool _saveRotation = true;

    public ObjectData GetObjectData()
    {
        ObjectData objectData = new ObjectData();
        if (_savePossition)
            objectData.objectDataUnits.Add(nameof(transform.position), transform.position.ToObjectData());
        if (_saveRotation)
            objectData.objectDataUnits.Add(nameof(transform.rotation), transform.rotation.ToObjectData());
        return objectData;
    }

    public void SetObjectData(ObjectData objectData)
    {
        if (_savePossition)
            transform.position = Vector3Extension.ObjectDataToVector3(objectData.objectDataUnits[nameof(transform.position)]);
        if (_saveRotation)
            transform.rotation = QuaternionExtension.ObjectDataToVector3(objectData.objectDataUnits[nameof(transform.rotation)]);
    }
}
