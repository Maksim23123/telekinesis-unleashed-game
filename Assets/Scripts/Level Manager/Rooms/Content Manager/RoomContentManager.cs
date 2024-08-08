using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class RoomContentManager : MonoBehaviour
{
    [SerializeField] private GameObject _defaultPointerPrefab;

    [SerializeField] private float _playerDistanceActivationPosition;

    [SerializeReference] private List<ContentPointer> contentPointers = new();

    [SerializeField] private List<GameObject> contentPointerGameObjects = new();

    private void Start()
    {
        foreach (ContentPointer contentPointer in contentPointers)
        {
            contentPointer.ActivatePointerAction(gameObject.transform.position);
        }
    }

    public void GameObjectsToPointers()
    {
        foreach (GameObject pointerGameObject in contentPointerGameObjects.ToList())
        {
            if (pointerGameObject.TryGetComponent(out GameObjectPointer gameObjectPointer))
            {
                ContentPointer currentContentPointer = gameObjectPointer.ToRegularPointer(gameObject.transform.position);
                contentPointers.Add(currentContentPointer); // point 1

                

                contentPointerGameObjects.Remove(pointerGameObject);
                DestroyImmediate(pointerGameObject);
            }
            else
            {
                Debug.Log("GameObjecPointer script not found.");
            }
        }
#if UNITY_EDITOR
        // Optionally, if you want to apply changes to the prefab asset itself
        UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(this);
#endif
    }

    public void PointersToGameObjects()
    {
        foreach (GameObject pointerGameObject in contentPointerGameObjects.ToList())
        {
            if (pointerGameObject.TryGetComponent(out GameObjectPointer gameObjectPointer))
            {
                ContentPointer currentContentPointer = gameObjectPointer.ToRegularPointer(gameObject.transform.position);
                contentPointers.Add(currentContentPointer);

                contentPointerGameObjects.Remove(pointerGameObject);
                DestroyImmediate(pointerGameObject);
            }
            else
            {
                Debug.Log("GameObjecPointer script not found.");
            }
        }
#if UNITY_EDITOR
        // Optionally, if you want to apply changes to the prefab asset itself
        UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(this);
#endif
    }
}
