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

    [SerializeField] private GameManager gameController;
    
    // counting score for score display
    [HideInInspector] public float score = 0;

    void Start()
    {
        StartHealth();
        gameController = GameObject.FindWithTag("GameController").GetComponent<GameManager>();
        if (gameController == null)
        {
            Debug.LogError("Could not find Game Manager! Paste prefab on scene please <3");
        }
    }

    void Update()
    {
        if (gameController.score < score)
        {
            gameController.UpdateScore(score);
        }
    }

    public bool IsSpirit() { return form == PlayerForm.Spirit; }

    public void ChangeForm() 
    { 
        bool isSpirit = IsSpirit();
        form = isSpirit ? PlayerForm.Blood : PlayerForm.Spirit;
        if (body != null )
        {
            body.color = isSpirit ? bloodColor : spiritColor;
        }
    }

    [Button]
    private void ChangeFormTest()
    {
        ChangeForm();
    }

    public override void OnDead()
    {

        Time.timeScale = 0;
        gameController.DeadScreen();
    }
}
