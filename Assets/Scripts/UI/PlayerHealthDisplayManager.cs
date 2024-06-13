using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using TMPro;
using UnityEngine;

public class PlayerHealthDisplayManager : HealthDisplayManager
{
    private void Start()
    {
        GetPlayerHealthManager(PlayerStatusInformer.PlayerGameObject);
        PlayerStatusInformer.newPlayerAssigned += OnNewPlayerAssigned;
        _textMesh = GetComponent<TextMeshProUGUI>();
        ConnectToHealthManager();
    }

    private void OnNewPlayerAssigned(GameObject player)
    {
        GetPlayerHealthManager(player);
        ConnectToHealthManager();
    }

    private void GetPlayerHealthManager(GameObject player)
    {
        if (player.TryGetComponent(out EntityHealthManager playerHealthManager))
        {
            _healthManager = playerHealthManager;
        } 
    }

    private void ConnectToHealthManager()
    {
        if (_healthManager != null)
        {
            _healthManager.healthChanged += OnHealthChanged;
            UpdateHealthText();
        }
    }
}
