using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Resources;
using Enums;

public class MobController : MonoBehaviour
{
    public Mob mob;

    private void Awake()
    {
        mob = new Mob(0, 4, MobType.Neutral);
    }
}
