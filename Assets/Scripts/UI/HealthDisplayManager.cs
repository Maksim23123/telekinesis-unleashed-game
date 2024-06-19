using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class HealthDisplayManager : MonoBehaviour
{
    protected TextMeshProUGUI _textMesh;

    [SerializeField]
    protected string _prefix;
    [SerializeField]
    protected EntityHealthManager _healthManager;

    private void Start()
    {
        _textMesh = GetComponent<TextMeshProUGUI>();
        if (_healthManager != null)
        {
            _healthManager.healthChanged += OnHealthChanged;
            UpdateHealthText();
        }
    }

    protected void OnHealthChanged(int health)
    {
        UpdateHealthText();
    }

    protected void UpdateHealthText()
    {
        _textMesh.text = _prefix + _healthManager.CurrentHealth + "/" + _healthManager.MaxHealth;
    }
}
