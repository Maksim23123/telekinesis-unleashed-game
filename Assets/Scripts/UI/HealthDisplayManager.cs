using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))] 
public class HealthDisplayManager : MonoBehaviour
{
    [SerializeField] protected string _prefix;
    [SerializeField] protected EntityHealthManager _healthManager;

    protected TextMeshProUGUI _textMesh;

    private void Start()
    {
        _textMesh = GetComponent<TextMeshProUGUI>();
        if (_healthManager != null)
        {
            _healthManager.HealthChanged += OnHealthChanged;
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
