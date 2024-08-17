using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the player's interactions with objects of type <see cref="InteractableObject"/>.
/// </summary>
public class PlayerInteractableObjectManager : MonoBehaviour
{
    [SerializeField] private LayerMask _interactableObjectsLayers;

    private readonly float _pickUpZoneRadius = 2.7f;
    private bool _interactionAllowed = true;

    public int Priority => 0;

    private void Interact()
    {
        if (PlayerInteractionManager.Instance.TryGetAvailableInteractableObject(out GameObject firstAvailable
                , _interactableObjectsLayers, _pickUpZoneRadius))
        {
            if (firstAvailable.TryGetComponent(out InteractableObject interactableObject))
            {
                interactableObject.Activate();
            }
        }
    }

    private void RestorInteractionAbility()
    {
        _interactionAllowed = true;
    }

    /// <summary>
    /// Requests an interaction with the object the player is pointing at, if interaction is allowed.
    /// </summary>
    public void RequestInteraction()
    {
        if (_interactionAllowed)
        {
            Interact();
            _interactionAllowed = false;
            Invoke(nameof(RestorInteractionAbility), 0.1f);
        }
    }
}
