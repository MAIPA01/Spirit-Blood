using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

enum PlayerForm
{
    Blood = 0,
    Spirit = 1
}

public class Player : ObjectHealth
{
    [Header("Form Values:")]
    [SerializeField] 
    private PlayerForm form;

    [SerializeField]
    private SpriteRenderer body;

    [SerializeField]
    [ShowIf("form", PlayerForm.Spirit)]
    private float spiritDamage = 2f;
    [SerializeField]
    [ShowIf("form", PlayerForm.Spirit)]
    private Color spiritColor = Color.black;

    [SerializeField]
    [ShowIf("form", PlayerForm.Blood)]
    private float bloodDamage = 1f;
    [SerializeField]
    [ShowIf("form", PlayerForm.Blood)]
    private Color bloodColor = Color.white;

    void Start()
    {
        StartHealth();
    }

    void Update()
    {
        
    }

    public bool IsSpirit() { return form == PlayerForm.Spirit; }

    public void ChangeForm() 
    { 
        bool isGhost = IsSpirit();
        form = isGhost ? PlayerForm.Blood : PlayerForm.Spirit;
        if (body != null )
        {
            body.color = isGhost ? bloodColor : spiritColor;
        }
    }

    [Button]
    private void ChangeFormTest()
    {
        ChangeForm();
    }
}
