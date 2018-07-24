using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour
{

    public float HP;
    public float DMG;
    public float inv;
    bool invC = false;
    float dodgeC;

    GameObject coll;


    //this skills define the next floor, active and passive buffs
    public int MentalHealth; //obligatory to be at a good level
    public int Social; //


    //this skills are a active buff
    public int STR; //damage
    public int RES; //hp
    public int DEX; //luck on scavange
    public int AGL; //dodge
    public int INT; //mana
    public int Luck; //luck

    public float attSpeed;
    public float pen;

    void Start()
    {
        if (HP <= 0)
        {
            HP = 1;
        }

        if (DMG <= 0)
        {
            DMG = 1;
        }
    }

    void Update()
    {

        if (HP <= 0)
        {
            Debug.Log("Died");
            Destroy(gameObject);
        }

        dodgeC = (AGL/2) + (Luck * .1f); //calc of the dodgeChance, half agility+1/10 of luck

        if (dodgeC > 50)
        {
            dodgeC = 50; //cap for the dodgeChance, max 50% of dodge chance
        }

        DMG = (STR*.1f)+(RES*.01f)+(DEX*.01f);

        if (attSpeed <= 0f)
        {
            attSpeed = 0.3f;
        }

        damage();

    }

    void OnCollisionEnter2D(Collision2D col)
    {
        coll = col.gameObject;
    }

    void OnCollisionExit2D()
    {

        coll = null;

    }

    void damage()
    {
        if (coll != null)
        {
            if (coll.tag == "Enemy" && invC == false)
            {
                float dodge = Random.Range(0, 100);

                EnemySeekingAI enemy = coll.GetComponent<EnemySeekingAI>();

                if (dodge > dodgeC) //if the random number is bigger than the dodgeChance damage will be made, else player wont take damage
                {
                    HP -= enemy.DMG;
                    Debug.Log("you had " + dodgeC + "% chance of dodging");
                }
                else
                {
                    Debug.Log("dodged");
                }
                StartCoroutine("Invencibility");
            }
        }
    }

    IEnumerator Invencibility()
    {
        invC = true;
        yield return new WaitForSeconds(inv);
        invC = false;
    }
}
