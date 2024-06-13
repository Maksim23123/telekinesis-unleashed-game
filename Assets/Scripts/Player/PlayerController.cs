using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private static PlayerController _instance;

    public static PlayerController Instance
    {
        get
        {
            return _instance;
        }
    }

    void Awake()
    {
        _instance = this;
    }

    [SerializeField]
    private CharacterControllerScript _characterControllerScript;

    public CharacterControllerScript CharacterControllerScript 
    { 
        get
        {
            return _characterControllerScript;
        }
    }
}
