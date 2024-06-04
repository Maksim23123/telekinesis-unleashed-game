using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneWayPlatformHandler : MonoBehaviour
{
    GameObject _currentOneWayPlatform;

    [SerializeField]
    LayerMask _oneWayPlatformLayer;
    [SerializeField]
    float _fallThroughTime;

    private static OneWayPlatformHandler _instance;

    
    bool _fallThroughOnCooldown;

    public static OneWayPlatformHandler Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<OneWayPlatformHandler>();
            }

            return _instance;
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

    public void FallThroughCurrentPlatform()
    {
        if (!_fallThroughOnCooldown && _currentOneWayPlatform != null)
        {
            StartCoroutine(DisableCollision());
            _fallThroughOnCooldown = true;
            Invoke(nameof(ResetFallThrough), _fallThroughTime);
        }
    }

    private void ResetFallThrough ()
    {
        _fallThroughOnCooldown = false;
    }

    private IEnumerator DisableCollision()
    {
        BoxCollider2D platformCollider = _currentOneWayPlatform.GetComponent<BoxCollider2D>();

        Physics2D.IgnoreCollision(gameObject.GetComponent<Collider2D>(), platformCollider);
        yield return new WaitForSeconds(_fallThroughTime);
        Physics2D.IgnoreCollision(gameObject.GetComponent<Collider2D>(), platformCollider, false);
    }
}
