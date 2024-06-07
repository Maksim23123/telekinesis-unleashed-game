using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class HealthDisplayManager : MonoBehaviour
{
    TextMeshProUGUI _textMesh;

    [SerializeField]
    string _prefix;

    [SerializeField]
    EntityHealthManager _healthManager;

    private void Start()
    {
        _textMesh = GetComponent<TextMeshProUGUI>();
        if (_healthManager != null)
        {
            _healthManager.healthChanged += OnHealthChanged;
            UpdateHealthText();
        }
    }


    private void OnHealthChanged(int health)
    {
        UpdateHealthText();
    }

    private void UpdateHealthText()
    {
        _textMesh.text = _prefix + _healthManager.CurrentHealth + "/" + _healthManager.MaxHealth;
    }
}
