using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Toilet : NetworkBehaviour
{
    public GameObject pool;
    public Door _Door;

    public void PoolEnable()
    {
        pool.SetActive(true);
    }
    public void PoolDisable()
    {
        pool.SetActive(false);
    }
}
