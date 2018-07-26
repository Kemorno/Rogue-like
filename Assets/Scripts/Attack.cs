using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour {

    enum AttackTypes { Meele, Ranged, Magic, Summon };

    //default ones
    public GameObject Rangedp;
    public GameObject Meele;
    public GameObject Magicp;
    public GameObject Summon;

    //Ranged
    bool RangedCD = false;
    public float range;


    // Use this for initialization
    void Start () {


    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey(KeyCode.UpArrow) && RangedCD == false)
        {
            StartCoroutine("Ranged");
        }
	}

    IEnumerator Ranged()
    {
        Stats player = GetComponent<Stats>();

        GameObject Projectile = Instantiate(Rangedp, transform.position, transform.rotation);
        Rigidbody2D Projectilerb = Projectile.GetComponent<Rigidbody2D>();
        Projectilerb.AddForce(transform.up * range);

        RangedCD = true;
        yield return new WaitForSeconds(player.attSpeed);
        RangedCD = false;
    }
}
