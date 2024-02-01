using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum Class
{
    Warrior,
    Unity,
    Monk
}


public class CardHandler : MonoBehaviour
{
    [SerializeField] GameObject card1;
    [SerializeField] GameObject card2;
    [SerializeField] GameObject card3;
    [SerializeField] GameObject cardsView;
    [SerializeField] AudioSource audioSource;
    private GameObject class1;
    private GameObject class2;
    private GameObject class3;

    [SerializeField] Card classCardMonk;
    [SerializeField] Card classCardUnity;
    [SerializeField] Card classCardWarrior;

    private Card selectedClassCard = null;

    private Class classType;

    private List<int> skillLvlUpList = new List<int> { 1, 1, 1 };
    private Player ourPlayer; //change to waves after :P
    private float scoreForLvlUp = 50;
    private float scoreMultiplier = 1.3f;
    private float lvlUpQueuer = 0;

    private void Awake()
    {
        ourPlayer = FindObjectOfType<Player>();

        if (cardsView == null)
        {
            card1 = GameObject.Find("CardsView");
        }

        if (card1 == null)
        {
            card1 = GameObject.Find("Card1");
        }
        if (card2 == null)
        {
            card2 = GameObject.Find("Card2");
        }
        if (card3 == null)
        {
            card3 = GameObject.Find("Card3");
        }
    }

    void Update()
    {
        if (scoreForLvlUp <= ourPlayer.score)
        {
            scoreForLvlUp *= scoreMultiplier;
            lvlUpQueuer++;

        }
        if(lvlUpQueuer > 0)
        {
            LvlUp();
            audioSource.Play();
        }

    }

    //Warrior = 0, Unity = 1, Monk = 2
    public void SelectClass(int classSelected)
    {

        class1 = GameObject.Find("Class1");
        class2 = GameObject.Find("Class2");
        class3 = GameObject.Find("Class3");
        if(classSelected == 0)
        {
            classType = Class.Monk;
        }  
        if(classSelected == 1)
        {
            classType = Class.Unity;
        }   
        if(classSelected == 2)
        {
            classType = Class.Warrior;
        }
 
        switch (classType)
        {
            case Class.Monk:
                selectedClassCard = classCardMonk;

                ourPlayer.gameObject.GetComponent<Movement>().speed
                 += ourPlayer.gameObject.GetComponent<Movement>().speed * selectedClassCard.skill1;

                ourPlayer.attackDelay -= ourPlayer.attackDelay * selectedClassCard.skill2;
                ourPlayer.formChangeCooldown -= ourPlayer.formChangeCooldown * selectedClassCard.skill2;

                ourPlayer.gameObject.GetComponent<Movement>().jumpForce
                        += ourPlayer.gameObject.GetComponent<Movement>().jumpForce * selectedClassCard.skill3;

                break;
            case Class.Unity:
                selectedClassCard = classCardUnity;

                ourPlayer.superAttackCooldown -= ourPlayer.superAttackCooldown * selectedClassCard.skill1;

                ourPlayer.circleRadius += ourPlayer.circleRadius * selectedClassCard.skill2;
                ourPlayer.bloodAttackRange += ourPlayer.bloodAttackRange * selectedClassCard.skill2;

                ourPlayer.skillBonusFactor += selectedClassCard.skill3;
                break;
            case Class.Warrior:
                selectedClassCard = classCardWarrior;

                ourPlayer.AddHealth(selectedClassCard.skill1);
                ourPlayer.SetMaxHealth(ourPlayer.GetMaxHealth() + selectedClassCard.skill1);

                ourPlayer.spiritDamage += selectedClassCard.skill2;
                ourPlayer.bloodDamage += selectedClassCard.skill2;

                ourPlayer.lifeSteal += selectedClassCard.skill3;
                break;
            default:
                Debug.LogError("That class doesn't exist! Fix it!");
                break;
        }

        Destroy(class1);
        Destroy(class2);
        Destroy(class3);
        card1.SetActive(true);
        card2.SetActive(true);
        card3.SetActive(true);
        lvlUpQueuer--;
        cardsView.SetActive(false);
        Time.timeScale = 1;
        GameTimer.StartTime();
    }

    public void SelectSkill(int skillID)
    {
        skillLvlUpList[skillID]++;
        switch (classType)
        {
            case Class.Monk:
                if (skillID == 0)
                {
                    ourPlayer.gameObject.GetComponent<Movement>().speed 
                        += ourPlayer.gameObject.GetComponent<Movement>().speed * selectedClassCard.skill1Up;
                }
                else if (skillID == 1)
                {
                    ourPlayer.attackDelay -= ourPlayer.attackDelay * selectedClassCard.skill2Up;
                    ourPlayer.formChangeCooldown -= ourPlayer.formChangeCooldown * selectedClassCard.skill2Up;
                }
                else if (skillID == 2)
                {
                    ourPlayer.gameObject.GetComponent<Movement>().jumpForce
                        += ourPlayer.gameObject.GetComponent<Movement>().jumpForce * selectedClassCard.skill3Up;
                }
                break;
            case Class.Unity:
                if (skillID == 0)
                {
                    ourPlayer.superAttackCooldown -= ourPlayer.superAttackCooldown * selectedClassCard.skill1Up;
                }
                else if (skillID == 1)
                {
                    ourPlayer.circleRadius += ourPlayer.circleRadius * selectedClassCard.skill2Up;
                    ourPlayer.bloodAttackRange += ourPlayer.bloodAttackRange * selectedClassCard.skill2Up;
                }
                else if (skillID == 2)
                {
                    ourPlayer.skillBonusFactor += selectedClassCard.skill3Up;
                }
                break;

            case Class.Warrior:
                if(skillID == 0)
                {
                    ourPlayer.AddHealth(selectedClassCard.skill1Up);
                    ourPlayer.SetMaxHealth(ourPlayer.GetMaxHealth() + selectedClassCard.skill1Up);
                }
                else if (skillID == 1)
                {
                    ourPlayer.spiritDamage += selectedClassCard.skill2Up;
                    ourPlayer.bloodDamage += selectedClassCard.skill2Up;
                }
                else if (skillID == 2)
                {
                    ourPlayer.lifeSteal += selectedClassCard.skill3Up;
                }
                break;

            default:
                Debug.LogError("That class doesn't exist! Fix it!");
                break;
        }
        lvlUpQueuer--;
        cardsView.SetActive(false);
        Time.timeScale = 1;
        GameTimer.StartTime();
    }

    private void LvlUp()
    {
        cardsView.SetActive(true);
        Time.timeScale = 0;
        GameTimer.StopTime();
        if (selectedClassCard != null)
        {
            switch (classType)
            {
                case Class.Monk:
                    card1.GetComponentInChildren<TMP_Text>().text = "Move Speed " + skillLvlUpList[0];
                    card2.GetComponentInChildren<TMP_Text>().text = "Attack Speed " + skillLvlUpList[1];
                    card3.GetComponentInChildren<TMP_Text>().text = "Jump Strength " + skillLvlUpList[2];
                    break;
                case Class.Unity:
                    card1.GetComponentInChildren<TMP_Text>().text = "Skill Cooldown Reduction " + skillLvlUpList[0];
                    card2.GetComponentInChildren<TMP_Text>().text = "Attack Range " + skillLvlUpList[1];
                    card3.GetComponentInChildren<TMP_Text>().text = "Longer Effects " + skillLvlUpList[2];
                    break;
                case Class.Warrior:
                    card1.GetComponentInChildren<TMP_Text>().text = "Bonus HP " + skillLvlUpList[0];
                    card2.GetComponentInChildren<TMP_Text>().text = "Attack Damage " + skillLvlUpList[1];
                    card3.GetComponentInChildren<TMP_Text>().text = "Life Steal " + skillLvlUpList[2];
                    break;
            }
        }
        lvlUpQueuer--;
    }

}
