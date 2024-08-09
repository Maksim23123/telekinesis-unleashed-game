using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class RoomContentManager : MonoBehaviour
{
    [SerializeField] private GameObject _defaultPointerPrefab;

    [SerializeField] private Transform _pointerContainer;

    [SerializeField] private float _playerDistanceActivationPosition;

    [SerializeReference] private List<ContentPointer> _contentPointers = new();

    [SerializeField] private List<GameObject> _contentPointerGameObjects = new();

    private void Update()
    {
        if (PlayerStatusInformer.PlayerGameObject != null)
        {
            Vector2 playerPosition = PlayerStatusInformer.PlayerGameObject.transform.position;
            Vector2 gameObjectPosition = gameObject.transform.position;
            if (Vector2.Distance(playerPosition, gameObjectPosition) < _playerDistanceActivationPosition)
            {
                foreach (ContentPointer contentPointer in _contentPointers)
                {
                    contentPointer.ActivatePointerAction(gameObjectPosition);
                }
            }
        }
    }

    public void GameObjectsToPointers()
    {
        foreach (GameObject pointerGameObject in _contentPointerGameObjects.ToList())
        {
            if (pointerGameObject.TryGetComponent(out GameObjectPointer gameObjectPointer))
            {
                ContentPointer currentContentPointer = gameObjectPointer.ToRegularPointer(gameObject.transform.position);
                _contentPointers.Add(currentContentPointer);

                _contentPointerGameObjects.Remove(pointerGameObject);
                DestroyImmediate(pointerGameObject);
            }
            else
            {
                Debug.Log("GameObjecPointer script not found.");
            }
        }
#if UNITY_EDITOR
        UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(this);
#endif
    }

    public void PointersToGameObjects()
    {
        foreach (ContentPointer contentPointer in _contentPointers.ToList())
        {
            GameObject pointerGameObject = contentPointer.ToGameObject(gameObject.transform.position, _defaultPointerPrefab
                , _pointerContainer);
            _contentPointerGameObjects.Add(pointerGameObject);
            _contentPointers.Remove(contentPointer);
        }

#if UNITY_EDITOR
        UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(this);
#endif
    }
}
