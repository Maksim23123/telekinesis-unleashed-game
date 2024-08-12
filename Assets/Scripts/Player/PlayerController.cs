using UnityEngine;

/// <summary>
/// This class is a singleton that provides access to the player's CharacterControllerScript.
/// It ensures that there is only one instance of PlayerController in the scene.
/// </summary>
public class PlayerController : MonoBehaviour
{
    [SerializeField] private CharacterControllerScript _characterControllerScript;

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
