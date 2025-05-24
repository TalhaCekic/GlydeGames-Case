using Mirror;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ObjData
{
    public string name;
    public GameObject obj;
    public int currentAmount;

    public ObjData(string name,GameObject obj, int currentAmount)
    {
        this.name = name;
        this.obj = obj;
        this.currentAmount = currentAmount;
    }
}
public class LobbyDatas : NetworkBehaviour
{
    public static LobbyDatas instance;

    public List<ObjData> objList = new List<ObjData>();

    
    void Awake()
    {
        
        if (instance == null)
        {
            // Eğer başka bir obje yoksa bu objeyi tut
            instance = this;
            DontDestroyOnLoad(gameObject);  // Bu objeyi yok etme
        }
        // else if (instance != this)
        // {
        //     // Eğer sahnede başka bir obje varsa, yeniyi tut ve eskiyi yok et
        //     Destroy(instance.gameObject);
        //     instance = this;
        //     DontDestroyOnLoad(gameObject);  // Bu objeyi tut
        // }
    }
    void Start()
    {
        instance = this;
        // DontDestroyOnLoad(gameObject);
        // ChangeUpdateReset();
    }
    
}
