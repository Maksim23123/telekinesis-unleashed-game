using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(GravityScaleManager))]
public class OneWayPlatformHandler : MonoBehaviour
{
    [SerializeField] private LayerMask _oneWayPlatformLayer;
    [SerializeField] private float _fallThroughTime;
    [SerializeField] private float _fallThroughGravityScale;

    private GravityScaleRequestManager _gravityScaleRequestManager;
    private List<GameObject> _oneWayPlatformsInContact = new();
    private bool _fallThroughOnCooldown;
    private static OneWayPlatformHandler _instance;

    public static OneWayPlatformHandler Instance
    {
        get
        {
            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;

        if (TryGetComponent(out GravityScaleManager gravityScaleManager))
        {
            _gravityScaleRequestManager = new GravityScaleRequestManager(gravityScaleManager, _fallThroughGravityScale, 1);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_oneWayPlatformLayer.Contains(collision.gameObject.layer))
        {
            _oneWayPlatformsInContact.Add(collision.gameObject);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (_oneWayPlatformLayer.Contains(collision.gameObject.layer) &&
                _oneWayPlatformsInContact.Contains(collision.gameObject))
        {
            _oneWayPlatformsInContact.Remove(collision.gameObject);
        }
    }

    private void ResetFallThrough()
    {
        _gravityScaleRequestManager.RequestIsActive = false;
        _fallThroughOnCooldown = false;
    }

    private IEnumerator DisableCollision()
    {
        List<BoxCollider2D> platformColliders = new();
        foreach (GameObject oneWayPlatformObject in _oneWayPlatformsInContact)
        {
            BoxCollider2D platformCollider = oneWayPlatformObject.GetComponent<BoxCollider2D>();

            platformColliders.Add(platformCollider);
            Physics2D.IgnoreCollision(gameObject.GetComponent<Collider2D>(), platformCollider);
        }
        
        yield return new WaitForSeconds(_fallThroughTime);

        foreach (BoxCollider2D platformCollider in platformColliders)
        {
            Physics2D.IgnoreCollision(gameObject.GetComponent<Collider2D>(), platformCollider, false);
        }
    }

    public void FallThroughCurrentPlatform()
    {
        if (!_fallThroughOnCooldown && _oneWayPlatformsInContact.Count > 0)
        {
            ClearNullContactObjects();
            StartCoroutine(DisableCollision());
            _fallThroughOnCooldown = true;
            _gravityScaleRequestManager.RequestIsActive = true;
            Invoke(nameof(ResetFallThrough), _fallThroughTime);
        }
    }

    private void ClearNullContactObjects()
    {
        foreach (GameObject oneWayPlatform in _oneWayPlatformsInContact.ToList())
        {
            if (oneWayPlatform == null)
            {
                _oneWayPlatformsInContact.Remove(oneWayPlatform);
            }
        }
    }
}
