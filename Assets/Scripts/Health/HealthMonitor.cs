using UnityEngine;
using UnityEngine.UI;

public class HealthMonitor : MonoBehaviour
{
    [SerializeField] private ObjectHealth healthToMonitor;
    [SerializeField] private Image image;

    void Update()
    {
        if (image == null)
        {
            Debug.Log("Pls assign image for me to monitor health :(");
        }
        else
        {
            image.fillAmount = healthToMonitor.GetHealth() / healthToMonitor.GetMaxHealth();
        }
    }
}
