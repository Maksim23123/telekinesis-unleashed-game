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
            if (_instance == null)
            {
                _instance = FindObjectOfType<PlayerController>();
            }

            return _instance;
        }
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
