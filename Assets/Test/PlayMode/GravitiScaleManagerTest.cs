using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class GravityScaleManagerTest
{
    GameObject _gameObject;
    Rigidbody2D _rigidbody;
    GravityScaleManager _gravityScaleManager;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        _gameObject = new GameObject();
        _rigidbody = _gameObject.AddComponent<Rigidbody2D>();
        _gravityScaleManager = _gameObject.AddComponent<GravityScaleManager>();
        yield return null;
    }


    [UnityTest]
    public IEnumerator AddGravityValue_AddSingleGravityValue_NewGravityValueTransferedToRigidbody()
    {
        float defaultGravityValue = _rigidbody.gravityScale;
        float newGravityValue = defaultGravityValue + 10;

        GravityScaleRequestManager gravityScaleRequestManager = new GravityScaleRequestManager(_gravityScaleManager
            , newGravityValue, 0);

        gravityScaleRequestManager.RequestIsActive = true;

        yield return new WaitForFixedUpdate();

        Assert.AreEqual(newGravityValue, _rigidbody.gravityScale);
    }

    [UnityTest]
    public IEnumerator AddGravityValue_AddMultipleGravityValues_HighestPriorityValueTransferedToRigidbody()
    {
        float defaultGravityValue = _rigidbody.gravityScale;
        float highPriorityValue = defaultGravityValue + 10;
        float lowPriorityValue = defaultGravityValue + 5;

        GravityScaleRequestManager highPriorityRequestManager = new GravityScaleRequestManager(_gravityScaleManager
            , highPriorityValue, 1);
        GravityScaleRequestManager lowPrioirtyRequestManager = new GravityScaleRequestManager(_gravityScaleManager
            , lowPriorityValue, 0);

        lowPrioirtyRequestManager.RequestIsActive = true;
        highPriorityRequestManager.RequestIsActive = true;

        yield return new WaitForFixedUpdate();

        Assert.AreEqual(highPriorityValue, _rigidbody.gravityScale);
    }

    [UnityTest]
    public IEnumerator RemoveGravityValue_RemoveSingleGravityValue_GravityValueSetToDefault()
    {
        float defaultGravityValue = _rigidbody.gravityScale;
        float newGravityValue = defaultGravityValue + 10;

        GravityScaleRequestManager gravityScaleRequestManager = new GravityScaleRequestManager(_gravityScaleManager
            , newGravityValue, 0);

        gravityScaleRequestManager.RequestIsActive = true;

        yield return new WaitForFixedUpdate();

        Assert.AreNotEqual(defaultGravityValue, _rigidbody.gravityScale);

        gravityScaleRequestManager.RequestIsActive = false;

        yield return new WaitForFixedUpdate();

        Assert.AreEqual(defaultGravityValue, _rigidbody.gravityScale);
    }

    [UnityTest]
    public IEnumerator RemoveGravityValue_RemoveGravityValueAmongOtherValues_GravityValueSetFromPresentValues()
    {
        float defaultGravityValue = _rigidbody.gravityScale;
        float highPriorityValue = defaultGravityValue + 10;
        float lowPriorityValue = defaultGravityValue + 5;

        GravityScaleRequestManager highPriorityRequestManager = new GravityScaleRequestManager(_gravityScaleManager
            , highPriorityValue, 1);
        GravityScaleRequestManager lowPrioirtyRequestManager = new GravityScaleRequestManager(_gravityScaleManager
            , lowPriorityValue, 0);

        lowPrioirtyRequestManager.RequestIsActive = true;
        highPriorityRequestManager.RequestIsActive = true;

        yield return new WaitForFixedUpdate();

        Assert.AreEqual(highPriorityValue, _rigidbody.gravityScale);

        highPriorityRequestManager.RequestIsActive = false;

        yield return new WaitForFixedUpdate();

        Assert.AreEqual(lowPriorityValue, _rigidbody.gravityScale);
    }
}
