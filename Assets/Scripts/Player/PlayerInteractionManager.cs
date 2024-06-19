using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerInteractionManager : MonoBehaviour
{
    [SerializeField]
    private LayerMask _raycastTestLayers;
    
    private static PlayerInteractionManager _instance;

    public static PlayerInteractionManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<PlayerInteractionManager>();
            }

            return _instance;
        }
    }

    private bool PerformFinalObjectTest(GameObject testedObject, float maxObjectDistance)
    {
        Vector2 directionTowardsItem = (testedObject.transform.position - gameObject.transform.position).normalized;
        RaycastHit2D raycastHit = Physics2D.Raycast(gameObject.transform.position, directionTowardsItem, maxObjectDistance, _raycastTestLayers);
        if (raycastHit.transform != null && raycastHit.transform.gameObject == testedObject.gameObject)
            return true;
        else
            return false;
    }

    public bool TryGetAvailableInteractableObject(out GameObject firstAvailable, LayerMask interactableObjectsLayerMask, float interactionZoneRadius)
    {
        firstAvailable = null;

        Vector3 mousePos = StaticTools.GetMousePositionInWorld();

        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, interactionZoneRadius, Vector2.down, 0, interactableObjectsLayerMask);

        GameObject[] reachableObjects = hits.Select(x => x.transform.gameObject).ToArray();

        List<GameObject> objectsByDistance = reachableObjects
            .OrderBy(x => Vector3.Distance(x.transform.position, mousePos)).ToList();

        if (objectsByDistance.Count > 0 && PerformFinalObjectTest(objectsByDistance[0], interactionZoneRadius))
        {
            firstAvailable = objectsByDistance[0];
            return true;
        }

        return false;
    }
}
