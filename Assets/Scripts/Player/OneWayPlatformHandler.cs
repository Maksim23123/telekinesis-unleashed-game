using System.Collections;
using UnityEngine;

[RequireComponent(typeof(GravityScaleManager))]
public class OneWayPlatformHandler : MonoBehaviour
{
    [SerializeField] private LayerMask _oneWayPlatformLayer;
    [SerializeField] private float _fallThroughTime;
    [SerializeField] private float _fallThroughGravityScale;

    private GravityScaleRequestManager _gravityScaleRequestManager;
    private GameObject _currentOneWayPlatform;
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
            _currentOneWayPlatform = collision.gameObject;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {

        if (_oneWayPlatformLayer.Contains(collision.gameObject.layer))
        {
            _currentOneWayPlatform = null;
        }
    }

    private void ResetFallThrough()
    {
        _gravityScaleRequestManager.RequestIsActive = false;
        _fallThroughOnCooldown = false;
    }

    private IEnumerator DisableCollision()
    {
        BoxCollider2D platformCollider = _currentOneWayPlatform.GetComponent<BoxCollider2D>();

        Physics2D.IgnoreCollision(gameObject.GetComponent<Collider2D>(), platformCollider);
        yield return new WaitForSeconds(_fallThroughTime);
        Physics2D.IgnoreCollision(gameObject.GetComponent<Collider2D>(), platformCollider, false);
    }

    public void FallThroughCurrentPlatform()
    {
        if (!_fallThroughOnCooldown && _currentOneWayPlatform != null)
        {
            StartCoroutine(DisableCollision());
            _fallThroughOnCooldown = true;
            _gravityScaleRequestManager.RequestIsActive = true;
            Invoke(nameof(ResetFallThrough), _fallThroughTime);
        }
    }
}
