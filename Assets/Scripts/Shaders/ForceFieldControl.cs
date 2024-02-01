using UnityEngine;

public class ForceFieldControl : MonoBehaviour
{

    //GameObject forceField;
    public Material forceFieldMaterial;
    public GameObject forceField;

    public bool isActive = false;

    void Awake()
    {
        Debug.Log("ForceFieldControl Awake");
       
    }

    void Update()
    {
        if (isActive)
        {
            Debug.Log("ACTIVE");
            forceField.SetActive(true);
            forceFieldMaterial.SetShaderPassEnabled("_isActive", true);
        }
        else
        {
            Debug.Log("NOT ACTIVE");
            forceFieldMaterial.SetShaderPassEnabled("_isActive", false);
            forceField.SetActive(false);
        }


    }
}
