using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class CharacterControllerScriptTest
{
    GameObject _gameObject;

    CharacterControllerScript _characterControllerScript;

    [SetUp]
    public void SetUp()
    {
        _gameObject = new GameObject();
        _characterControllerScript = _gameObject.AddComponent<CharacterControllerScript>();
    }

    //MethodName_StateUnderTest_ExpectedBehavior
    [Test]
    [TestCase(1)]
    [TestCase(-1)]
    [TestCase(0)]
    [TestCase(5)]
    [TestCase(-10)]
    [TestCase(0.5f)]
    public void DirectionalFactor_IsWithinRange_True(float initialDirectionalFactorValue)
    {
        _characterControllerScript.DirectionalFactor = initialDirectionalFactorValue;

        bool isWithinRange = _characterControllerScript.DirectionalFactor >= -1
            && _characterControllerScript.DirectionalFactor <= 1;

        Assert.IsTrue(isWithinRange);
    }
}
