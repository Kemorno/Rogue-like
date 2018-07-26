using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySeekingAI : MonoBehaviour {

    public float HP;
    public int DMG;
    public float moveSpeed;

    public int detectDist;


    public float dPlayer;
    public float dEnemy;


    public float inv;
    bool invC = false;


    GameObject coll;
    public GameObject player;

    void Start () {
        
        if (HP <= 0)
        {
            HP = 1;
        }

        if (DMG <= 0)
        {
            DMG = 1;
        }

    }
	
	void Update () {

        Vector2 pPos = player.transform.position;

        if (HP <= 0)
        {
            Debug.Log(gameObject.name + "died");
            Destroy(gameObject);
        }

        if (Vector2.Distance(pPos, transform.position) <= detectDist && Vector2.Distance(pPos, transform.position) >= dEnemy/2+dPlayer/2)
        {
            transform.LookAt(pPos);
            transform.Translate(Vector3.forward*Time.deltaTime*moveSpeed);
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
            if (coll.tag == "Player" && invC == false)
            {
                Stats player = coll.GetComponent<Stats>();

                HP -= player.DMG;

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
