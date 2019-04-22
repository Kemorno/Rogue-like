using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Resources;
using Enums;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class PlayerController : MonoBehaviour
{
    private Mob Player;
    private Rigidbody2D rb;
    private Item[] Items = new Item[3];

    private void Awake()
    {
        Player = new Mob(0, 4, MobType.Player);
        rb = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        if (Player.Health <= 0)
            Debug.Log("You died");

        if (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0)
            StartCoroutine(Move(new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"))));

        if (Input.GetMouseButton(0))
            StartCoroutine(Attack(Camera.main.ScreenToWorldPoint(Input.mousePosition)));

        if (Input.GetKeyDown(KeyCode.Q))
            StartCoroutine(Magic());

        if (Input.GetKeyDown(KeyCode.E))
            StartCoroutine(Auxiliar());

        if (Input.GetKeyDown(KeyCode.F))
            StartCoroutine(Taunt());

        if (Input.GetKeyDown(KeyCode.R))
            StartCoroutine(Especial());

        if (Input.GetKeyDown(KeyCode.Alpha1))
            StartCoroutine(Item(Items[0], 0));

        if (Input.GetKeyDown(KeyCode.Alpha2))
            StartCoroutine(Item(Items[1], 1));

        if (Input.GetKeyDown(KeyCode.Alpha3))
            StartCoroutine(Item(Items[2], 2));
    }

    #region Attacks
    private bool canAttack = true;

    IEnumerator Attack(Vector2 vector)
    {
        if (canAttack)
        {
            canAttack = false;
            Vector2 Direction = new Vector2(transform.position.x - vector.x, transform.position.y - vector.y);

            InstantiateAttack(Direction, Direction.normalized, new Object());

            yield return new WaitForSeconds(1 / Player.AttackSpeed);
            canAttack = true;
        }
    }

    private bool canMagic = true;

    IEnumerator Magic()
    {
        if (canMagic)
        {
            canMagic = false;

            float Cooldown = 1;
            yield return new WaitForSeconds(Cooldown);
            canMagic = true;
        }
    }

    private bool canAuxiliar = true;

    IEnumerator Auxiliar()
    {
        if (canAuxiliar)
        {
            canAuxiliar = false;

            float Cooldown = 1;
            yield return new WaitForSeconds(Cooldown);
            canAuxiliar = true;
        }
    }
    
    private bool canTaunt = true;

    IEnumerator Taunt()
    {
        if (canTaunt)
        {
            canTaunt = false;

            float Cooldown = 1;
            yield return new WaitForSeconds(Cooldown);
            canTaunt = true;
        }
    }

    private bool canEspecial = true;

    IEnumerator Especial()
    {
        if (canEspecial)
        {
            canEspecial = false;

            float Cooldown = 1;
            yield return new WaitForSeconds(Cooldown);
            canEspecial = true;
        }
    }
    #endregion

    IEnumerator Item(Item item, int Pos)
    {
        /*
        if (!item.inCooldown)
        {
            item.Use();
            
            //Do Item

            if (item.Uses == 0 || item.Consumable)
            {
                Items[Pos] = null;
                yield break;
            }

            yield return new WaitForSeconds(1 / item.CooldownTime);
            item.Cooldown(false);
        }
        */
        yield return null;
    }
    
    private void InstantiateAttack(Vector2 Direction, Vector2 Speed, Object Projectile)
    {

    }

    private bool canMove = true;

    IEnumerator Move(Vector2 vector)
    {
        if (canMove)
        {
            rb.AddForce(vector * Player.MovementSpeed);
        }
        yield return null;
    }

    private bool canDamage = true;

    IEnumerator Damage(int Damage)
    {
        if (canDamage)
        {
            canDamage = false;

            Player.RecieveDamage(Damage);

            yield return new WaitForSeconds(Player.Invencibility);
            canDamage = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject other = collision.gameObject;

        Debug.Log("collided with " + other.name);


        if (other.tag == "Mob")
        {
            MobController mc = other.GetComponent<MobController>();
            if (mc.mob.Type == MobType.Enemy)
                StartCoroutine(Damage((int)mc.mob.AttackDamage));
        }
        if (other.tag == "Player")
        {
            PlayerController pc = other.GetComponent<PlayerController>();
        }
        if(other.tag == "Projectile")
        {
            ProjectileController pc = other.GetComponent<ProjectileController>();
        }
        if (other.tag == "Item")
        {
            ItemController item = other.GetComponent<ItemController>();
            /*
            switch (item.item.Type)
            {
                case ItemType.None:
                    {

                    }
                    break;
                case ItemType.Attack:
                    {

                    }
                    break;
                case ItemType.Auxiliar:
                    {

                    }
                    break;
                case ItemType.Magic:
                    {

                    }
                    break;
                case ItemType.Especial:
                    {

                    }
                    break;
                case ItemType.Taunt:
                    {

                    }
                    break;
                case ItemType.Item:
                    {
                        for(int i = 0; i < Items.Length; i++)
                        {
                            if (Items[i] == null)
                            {
                                Items[i] = item.item;
                                Destroy(other);
                                break;
                            }
                        }
                    }
                    break;
            }
            */
        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        
    }
}
