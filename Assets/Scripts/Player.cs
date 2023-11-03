using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

enum PlayerForm
{
    Normal = 0,
    Ghost = 1
}

public class Player : ObjectHealth
{
    [Header("Form Values:")]
    [SerializeField] 
    private PlayerForm form;

    [SerializeField]
    private SpriteRenderer body;

    [SerializeField]
    [ShowIf("form", PlayerForm.Ghost)]
    private float ghostDamage = 2f;
    [SerializeField]
    [ShowIf("form", PlayerForm.Ghost)]
    private Color ghostColor = Color.black;

    [SerializeField]
    [ShowIf("form", PlayerForm.Normal)]
    private float normalDamage = 1f;
    [SerializeField]
    [ShowIf("form", PlayerForm.Normal)]
    private Color normalColor = Color.white;

    // Start is called before the first frame update
    void Start()
    {
        StartHealth();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool IsGhost() { return form == PlayerForm.Ghost; }

    public void ChangeForm() 
    { 
        bool isGhost = IsGhost();
        form = isGhost ? PlayerForm.Normal : PlayerForm.Ghost;
        if (body != null )
        {
            body.color = isGhost ? normalColor : ghostColor;
        }
    }

    [Button]
    private void ChangeTestForm()
    {
        ChangeForm();
    }
}
