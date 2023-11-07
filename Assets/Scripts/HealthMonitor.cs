using UnityEngine;
using UnityEngine.UI;

public class HealthMonitor : MonoBehaviour
{
    [SerializeField] private ObjectHealth healthToMonitor;
    [SerializeField] private Image image;

    void Update()
    {
        image.fillAmount = healthToMonitor.GetHealth() / healthToMonitor.GetMaxHealth();
    }
}
