using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private CharacterControllerScript _characterControllerScript;

    private static PlayerController _instance;

    public static PlayerController Instance
    {
        get
        {
            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
    }

    public CharacterControllerScript CharacterControllerScript
    {
        get
        {
            return _characterControllerScript;
        }
    }
}
