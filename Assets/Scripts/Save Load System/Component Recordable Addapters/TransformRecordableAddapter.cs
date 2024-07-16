using UnityEngine;

public class TransformRecordableAddapter : MonoBehaviour, IRecordable
{
    [SerializeField] private bool _savePossition = true;
    [SerializeField] private bool _saveRotation = true;

    public ObjectData GetObjectData()
    {
        ObjectData objectData = new ObjectData();
        if (_savePossition)
            objectData.ObjectDataUnits.Add(nameof(transform.position), transform.position.ToObjectData());
        if (_saveRotation)
            objectData.ObjectDataUnits.Add(nameof(transform.rotation), transform.rotation.ToObjectData());
        return objectData;
    }

    public void SetObjectData(ObjectData objectData)
    {
        if (_savePossition)
            transform.position = Vector3Extension.ObjectDataToVector3(objectData.ObjectDataUnits[nameof(transform.position)]);
        if (_saveRotation)
            transform.rotation = QuaternionExtension.ObjectDataToVector3(objectData.ObjectDataUnits[nameof(transform.rotation)]);
    }
}
