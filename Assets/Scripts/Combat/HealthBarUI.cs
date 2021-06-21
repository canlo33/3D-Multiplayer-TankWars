using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    private HealthSystem healthSystem = null;
    [SerializeField] private GameObject healthBarParent = null;
    [SerializeField] private Image healthBarImage = null;

    private void Awake()
    {
        healthSystem = transform.GetComponent<HealthSystem>();
        healthSystem.ClientOnHealthChanged += HandleHealthChanged;
    }
    private void Start()
    {
    }

    private void OnDestroy()
    {
        healthSystem.ClientOnHealthChanged -= HandleHealthChanged;
    }

    private void OnMouseEnter()
    {
        healthBarParent.SetActive(true);
    }

    private void OnMouseExit()
    {
        healthBarParent.SetActive(false);
    }

    private void HandleHealthChanged(int currentHealth, int maxHealth)
    {
        float value = (float)currentHealth / maxHealth;
        healthBarImage.fillAmount = value;
    }
}
