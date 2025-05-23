using System.Collections.Generic;
using Mirror;
using UnityEngine;
using DG.Tweening;
using Player.PlayerMenu;
using UnityEngine.Animations.Rigging;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Runtime.InteropServices;

public class PlayerInteract : NetworkBehaviour
{
    public Transform HitPointInteract;
    public Animator anims;

    public GameObject cam;
    public PlayerMenuManager _PlayerMenuManager;

    // private RaycastHit hit;
    public Vector3 offset;
    public float yOffset = 10f;
    [SerializeField] [Min(1)] private float hitRange = 1;

    int pickupLayerMask;
    int pickupLayerMask2;
    int pickupLayerMask3;
    int pickupLayerMask4;
    int pickupLayerMask5;
    int pickupLayerMask6;
    int pickupLayerMask7;
    int pickupLayerMask8;
    int pickupLayerMask9;
    int pickupLayerMask10;
    int pickupLayerMask11;
    int pickupLayerMask12;
    int pickupLayerMask13;
    int pickupLayerMask14;
    int pickupLayerMask15;
    int pickupLayerMask16;
    int pickupLayerMask17;
    int pickupLayerMask18;
    int pickupLayerMask19;
    int pickupLayerMask20;
    int pickupLayerMask21;

    [SyncVar] public bool handFull = false;
    [SyncVar] public bool mouseActivity = false;
    public GameObject InteractObj;
    public GameObject HandObj;
    public Transform InteractPos;
    public Transform InteractPosBox; // Kutu
    public Transform InteractPosItem; // item
    public Transform InteractPosBrush; // brush

    [Header("El Sistemi")] [SyncVar] public bool isHandRightTrue;
    public RigBuilder rigBuilder;
    [SerializeField] Transform HandTargetRight, HandLookRight;
    [SerializeField] Transform HandTargetLeft, HandLookLeft;

    [SyncVar] private bool isChangeValue;
    public List<ItemBox> itemBoxes = new List<ItemBox>();

    public GameObject car;
    [SyncVar] public bool isDrive;
    [SyncVar] public bool isVehicle;

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
        anims = GetComponent<Animator>();
        cam = GameObject.Find("Main Camera");
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        anims = GetComponent<Animator>();
        cam = GameObject.Find("Main Camera");
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        anims = GetComponent<Animator>();
        cam = GameObject.Find("Main Camera");
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        anims = GetComponent<Animator>();
        cam = GameObject.Find("Main Camera");

        GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag("ItemBox");

        // Her bir objede ItemBox component'i olup olmadığını kontrol et ve listeye ekle
        foreach (GameObject obj in taggedObjects)
        {
            ItemBox itemBox = obj.GetComponent<ItemBox>();
            if (itemBox != null)
            {
                itemBoxes.Add(itemBox);
            }
        }
    }

    void Start()
    {
        if (!isLocalPlayer) return;
        LayerAdd();
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0) return;
        if (InGameHud.instance.isStatistic)
        {
            mouseActivity = true;
            //InGameHud.instance.ServerCross(true);
        }

        if (itemBoxes.Count != 0)
            foreach (ItemBox box in itemBoxes)
            {
                box.ServerAddAmount();
                //isChangeValue = true;
            }

        if (!isVehicle) return;
        //RpcEnterVehicle();
    }

    [ClientRpc]
    void RpcEnterVehicle()
    {
        if (car == null) return;
        // Oyuncuyu araç koltuğuna taşı
        if (!isVehicle) return;
        transform.position = car.GetComponent<carMovement>().Sit1Pos.position; // Koltuk pozisyonuna taşı
        transform.rotation = car.GetComponent<carMovement>().Sit1Pos.rotation; // Koltuk yönüne döndür

        //StartCoroutine(UpdatePlayerPosition());
    }

    IEnumerator UpdatePlayerPosition()
    {
        while (isDrive)
        {
            //transform.SetParent(car.transform);
            transform.position = car.GetComponent<carMovement>().Sit1Pos.position;
            transform.rotation = car.GetComponent<carMovement>().Sit1Pos.rotation;
            yield return null; // Bir sonraki kareyi bekle
        }

        while (!isDrive && isVehicle)
        {
            transform.position = car.GetComponent<carMovement>().Sit2Pos.position;
            transform.rotation = car.GetComponent<carMovement>().Sit2Pos.rotation;
            yield return null; // Bir sonraki kareyi bekle
        }
    }

    private void LayerAdd()
    {
        pickupLayerMask = LayerMask.GetMask("Laptop");
        pickupLayerMask2 = LayerMask.GetMask("BoxObj");
        pickupLayerMask3 = LayerMask.GetMask("ItemSlot");
        pickupLayerMask4 = LayerMask.GetMask("CashRegister");
        pickupLayerMask5 = LayerMask.GetMask("Counter");
        pickupLayerMask6 = LayerMask.GetMask("Grill");
        pickupLayerMask7 = LayerMask.GetMask("Item");
        pickupLayerMask8 = LayerMask.GetMask("OrderSlot");
        pickupLayerMask9 = LayerMask.GetMask("Customer");
        pickupLayerMask10 = LayerMask.GetMask("Tray");
        pickupLayerMask11 = LayerMask.GetMask("TrashBin");
        pickupLayerMask12 = LayerMask.GetMask("GrillButton");
        pickupLayerMask13 = LayerMask.GetMask("DrinkMachine");
        pickupLayerMask14 = LayerMask.GetMask("DrinkButton");
        pickupLayerMask15 = LayerMask.GetMask("FryerSlot");
        pickupLayerMask16 = LayerMask.GetMask("FryerButton");
        pickupLayerMask17 = LayerMask.GetMask("Door");
        pickupLayerMask18 = LayerMask.GetMask("Pool");
        pickupLayerMask19 = LayerMask.GetMask("Brush");
        pickupLayerMask20 = LayerMask.GetMask("Car");
        pickupLayerMask21 = LayerMask.GetMask("OnlineDoor");
    }

    public void Throw()
    {
        if (!isLocalPlayer) return;
        CmdThrow();
    }

    [Command]
    private void CmdThrow()
    {
        RpcThrow();
    }

    [ClientRpc]
    private void RpcThrow()
    {
        if (!handFull) return;
        if (HandObj == null) return;
        HandObj.GetComponent<ItemInteract>().enabled = true;
        HandObj.GetComponent<ItemInteract>().isInteract = false;
        HandObj.GetComponent<ItemInteract>().transform.parent = null;
        HandObj.GetComponent<ItemInteract>().isStateFree = true;
        HandObj.GetComponent<ItemInteract>().rb.isKinematic = false;
        HandObj.GetComponent<ItemInteract>().collision.enabled = true;
        HandObj.GetComponent<ItemInteract>().isThrow = true;
        HandObj.GetComponent<Rigidbody>().AddForce(InteractPosBox.transform.forward * 700);
        isHandRightTrue = false;
        InteractObj = null;
        HandObj = null;
        HandFullTest(false);
    }

    public void ServerInteract() // E ile etkileşim
    {
        if (!isLocalPlayer) return;
        if (mouseActivity)
        {
            if (InteractObj != null)
            {
                mouseActivity = false;
                //InGameHud.instance.ServerCross(false);
                CmdInteract(InteractObj);
                InteractObj = null;
            }
        }
        else
        {
            if (car != null && isVehicle)
            {
                CarRemove();

                // if (isDrive)
                // {
                //     //transform.SetParent(null);
                //     //transform.position = car.GetComponent<carMovement>().Out1Pos.position;
                //     ServerDriveChange();
                // }
                // else
                // {
                //     //transform.SetParent(null);
                //     //transform.position = car.GetComponent<carMovement>().Out2Pos.position;
                //     ServerSittingCar();
                // }
            }

            RaycastHit hit;
            // Laptop
            if (Physics.Raycast(cam.transform.position, cam.transform.TransformDirection(Vector3.forward), out hit,
                    hitRange, pickupLayerMask))
            {
                CmdServerInteract(hit.collider.gameObject);
            }
            // Kasa Etkileşimi
            else if (Physics.Raycast(cam.transform.position, cam.transform.TransformDirection(Vector3.forward), out hit,
                         hitRange, pickupLayerMask4))
            {
                CmdServerInteract(hit.collider.gameObject);
            }

            // box Objeler
            else if (Physics.Raycast(cam.transform.position, cam.transform.TransformDirection(Vector3.forward), out hit,
                         hitRange, pickupLayerMask2))
            {
                CmdBoxInteract(hit.collider.gameObject);
            }
            // direkt slottan item alma
            else if (Physics.Raycast(cam.transform.position, cam.transform.TransformDirection(Vector3.forward), out hit,
                         hitRange, pickupLayerMask3))
            {
                CmdSlotInteractSpawn(hit.collider.gameObject);
            }
            // Fritöz item patatese çevirir
            else if (Physics.Raycast(cam.transform.position, cam.transform.TransformDirection(Vector3.forward), out hit,
                         hitRange, pickupLayerMask15))
            {
                CmdFryerPacketPatato(hit.collider.gameObject);
            }
            // Fritöz Button indir kaldır
            else if (Physics.Raycast(cam.transform.position, cam.transform.TransformDirection(Vector3.forward), out hit,
                         hitRange, pickupLayerMask16))
            {
                CmdFryerButton(hit.collider.gameObject);
            }

            // Item
            else if (Physics.Raycast(cam.transform.position, cam.transform.TransformDirection(Vector3.forward), out hit,
                         hitRange, pickupLayerMask7))
            {
                CmdTakeItem(hit.collider.gameObject); // item ele alma
                CmdCombineItem(hit.collider.gameObject); // item combine
                //CmdItemPacketing(hit.collider.gameObject);
            }
            // Customer
            else if (Physics.Raycast(cam.transform.position, cam.transform.TransformDirection(Vector3.forward), out hit,
                         hitRange * 20, pickupLayerMask9))
            {
                CmdTakeCustomer(hit.collider.gameObject); // Musteri Teslim
            }
            // Tepsi
            else if (Physics.Raycast(cam.transform.position, cam.transform.TransformDirection(Vector3.forward), out hit,
                         hitRange * 20, pickupLayerMask10))
            {
                CmdTrayPutItem(hit.collider.gameObject); // tepsiye ürün yerleştir
            }
            // grill Button
            else if (Physics.Raycast(cam.transform.position, cam.transform.TransformDirection(Vector3.forward), out hit,
                         hitRange * 20, pickupLayerMask12))
            {
                CmdGrillButton(hit.collider.gameObject); // 
            }
            // Drink put
            else if (Physics.Raycast(cam.transform.position, cam.transform.TransformDirection(Vector3.forward), out hit,
                         hitRange * 20, pickupLayerMask13))
            {
                CmdDrinkPut(hit.collider.gameObject); // 
            }
            // Drink başlangıç
            else if (Physics.Raycast(cam.transform.position, cam.transform.TransformDirection(Vector3.forward), out hit,
                         hitRange * 20, pickupLayerMask14))
            {
                CmdDrinkButton(hit.collider.gameObject); // 
            }

            // Çöpe boşalt
            else if (Physics.Raycast(cam.transform.position, cam.transform.TransformDirection(Vector3.forward), out hit,
                         hitRange, pickupLayerMask11))
            {
                CmdTrashSystem(hit.collider.gameObject); // Musteri Teslim
            }
            // Kapı
            else if (Physics.Raycast(cam.transform.position, cam.transform.TransformDirection(Vector3.forward), out hit,
                         hitRange, pickupLayerMask17))
            {
                CmdDoor(hit.collider.gameObject); // Musteri Teslim
            }
            // Boku temizle
            else if (Physics.Raycast(cam.transform.position, cam.transform.TransformDirection(Vector3.forward), out hit,
                         hitRange, pickupLayerMask18))
            {
                CmdPool(hit.collider.gameObject); // Musteri Teslim
            }
            // fırçayı al
            else if (Physics.Raycast(cam.transform.position, cam.transform.TransformDirection(Vector3.forward), out hit,
                         hitRange, pickupLayerMask19))
            {
                CmdBrush(hit.collider.gameObject); // Musteri Teslim
            }
            // Ürün Yerleştir (Araca)
            else if (Physics.Raycast(cam.transform.position, cam.transform.TransformDirection(Vector3.forward), out hit,
                         hitRange, pickupLayerMask8))
            {
                CmdPutSlotOrder(hit.collider.gameObject); // Musteri Teslim
            }
            // Araca gir veya in
            else if (Physics.Raycast(cam.transform.position, cam.transform.TransformDirection(Vector3.forward), out hit,
                         hitRange, pickupLayerMask20))
            {
                CmdCarInteract(hit.collider.gameObject); // Araç etkileşimi
            }
            // Siparişi teslim et (Online)
            else if (Physics.Raycast(cam.transform.position, cam.transform.TransformDirection(Vector3.forward), out hit,
                         hitRange, pickupLayerMask21))
            {
                ServerGiveOrderOnline(hit.collider.gameObject); // Araç etkileşimi
            }
        }
    }

    // online ürün teslim
    [Command]
    private void ServerGiveOrderOnline(GameObject obj)
    {
        RpcGiveOrderOnline(obj);
    }

    [ClientRpc]
    private void RpcGiveOrderOnline(GameObject obj)
    {
        if (!handFull) return;
        if (InteractObj == null) return;
        obj.GetComponent<OnlineDoors>().RpcOrderGive(InteractObj, gameObject);
    }
    // car
    [Command]
    private void CarRemove()
    {
        if (isDrive)
        {
            car.GetComponent<NetworkIdentity>().RemoveClientAuthority();
            RpcDriveChange();
        }
        else
        {
            RpcSittingCar(); 
        }
    }

    // araçtan inme
    [Command]
    private void ServerDriveChange()
    {
        RpcDriveChange();
        car.GetComponent<NetworkIdentity>().RemoveClientAuthority();
    }

    [ClientRpc]
    private void RpcDriveChange()
    {
       // if(!isLocalPlayer)return;
        print("araçtan sürücü iner");
        //car.GetComponent<carCameraController>().isChange = false;
        car.GetComponent<carMovement>().enabled = false; // Araç kontrolünü kapat
        car.GetComponent<carCameraController>().enabled = false; // Araç camera
        GetComponent<Player.PlayerControl.PlayerController>().enabled = true;
        GetComponent<Player.PlayerControl.PlayerController>().StartFonction(car);
        GetComponent<NetworkTransformReliable>().enabled = true;
        car.GetComponent<carMovement>().isDriving = false; // Araç kontrolünü etkinleştir
        //GetComponent<CharacterControl>().enabled = true;
        isVehicle = false;
        isDrive = false;
        car = null;
    }

    [Command]
    private void ServerSittingCar()
    {
        RpcSittingCar();
    }

    [ClientRpc]
    private void RpcSittingCar()
    {
       // if(!isLocalPlayer)return;
        print("araçtan oturan iner");

        car.GetComponent<carCameraController>().isChange = false;
        GetComponent<NetworkTransformReliable>().enabled = false;
        car.GetComponent<carMovement>().enabled = false; // Araç kontrolünü kapat
        car.GetComponent<carCameraController>().enabled = false; // Araç camera
        GetComponent<Player.PlayerControl.PlayerController>().enabled = true;
        GetComponent<Player.PlayerControl.PlayerController>().StartFonction(car);
        GetComponent<NetworkTransformReliable>().enabled = true;
        //GetComponent<CharacterControl>().enabled = true;
        isVehicle = false;
        car = null;
    }

    // Araç etkileşimi
    [Command]
    private void CmdCarInteract(GameObject obj)
    {
        if (!obj.GetComponent<carMovement>().isDriving)
        {
            if (!obj.GetComponent<NetworkIdentity>().isOwned)
            {
                if (obj.GetComponent<NetworkIdentity>() != null)
                {
                    obj.GetComponent<NetworkIdentity>()
                        .AssignClientAuthority(connectionToClient); // Araç kontrolünü oyuncuya ver
                    RpcCarisDrive(obj);
                }
            }
        }
        else
        {
           RpcCarInteract(obj); 
        }
        
    }

    //[ClientRpc]
    private void RpcCarisDrive(GameObject obj)
    {
        //if(!isLocalPlayer)return;
        print("sürücü koltuğa geçer");
        car = obj;
        //transform.SetParent(obj.transform);
        //GetComponent<CharacterControl>().enabled = false;
        GetComponent<Player.PlayerControl.PlayerController>().StartEnter(obj.transform);
        GetComponent<Player.PlayerControl.PlayerController>().enabled = false; // Oyuncunun kontrolünü devre dışı bırak
        //if (!obj.GetComponent<NetworkIdentity>().isOwned)return;
        obj.GetComponent<carMovement>().enabled = true; // Araç kontrolünü etkinleştir
        obj.GetComponent<carMovement>().isDriving = true; // Araç kontrolünü etkinleştir
        obj.GetComponent<carCameraController>().enabled = true; // Araç camera
        obj.GetComponent<carCameraController>().StartFonction(); // cam start
        GetComponent<NetworkTransformReliable>().enabled = false;
        isVehicle = true;
        isDrive = true;
        print("hopbala");
    }

    //[ClientRpc]
    private void RpcCarInteract(GameObject obj)
    {
       // if(!isLocalPlayer)return;
        print("yan koltuğa geçer");
        car = obj;
        //if (obj.GetComponent<carMovement>().isDriving) return;
        //transform.SetParent(obj.transform);
        GetComponent<Player.PlayerControl.PlayerController>().StartEnter(obj.transform);
        GetComponent<Player.PlayerControl.PlayerController>().enabled = false; // Oyuncunun kontrolünü devre dışı bırak
        obj.GetComponent<carMovement>().enabled = true; // Araç kontrolünü etkinleştir
        obj.GetComponent<carCameraController>().enabled = true; // Araç camera
        obj.GetComponent<carCameraController>().StartFonction(); // cam start 
        GetComponent<NetworkTransformReliable>().enabled = false;
        //GetComponent<CharacterControl>().enabled = false;

        isVehicle = true;
        //print("Araç dolu");
    }

    // boku temizle
    [Command]
    private void CmdPutSlotOrder(GameObject obj)
    {
        if (obj != null)
        {
            RpcPutSlotOrder(obj);
        }
    }

    [ClientRpc]
    private void RpcPutSlotOrder(GameObject obj)
    {
        // if (!handFull) return;
        if (obj.GetComponent<StorageSystem>().obj == null)
        {
            if (!handFull) return;
            if (InteractObj == null) return;
            obj.GetComponent<StorageSystem>().obj = InteractObj;
            //InteractObj.GetComponent<TrayInteract>().ItemNotParentObj(obj.transform.position);
            InteractObj.GetComponent<TrayInteract>().ItemParentObj(obj.transform);
            //InteractObj.GetComponent<TrayInteract>().ServerColliderFalse();
            obj.GetComponent<StorageSystem>().TransportPutItem(gameObject);
        }
        else
        {
            if (!obj.GetComponent<StorageSystem>().isInteracting)
            {
                obj.GetComponent<StorageSystem>().TransportTakeItem(gameObject);
                if (InteractObj == null) return;
                InteractObj.GetComponent<TrayInteract>().BoxParentObj(InteractPosBox);
                InteractObj.GetComponent<TrayInteract>().ServerStateChange(true);
            }
        }
    }

    // boku temizle
    [Command]
    private void CmdBrush(GameObject obj)
    {
        if (obj != null)
        {
            RpcBrush(obj);
        }
    }

    [ClientRpc]
    private void RpcBrush(GameObject obj)
    {
        if (handFull && HandObj != null) return;
        HandObj = obj;
        if (HandObj.GetComponent<BrushInteract>())
        {
            HandObj.GetComponent<BrushInteract>().ServerStateChange(false);
            HandObj.GetComponent<BrushInteract>().BoxParentObj(InteractPosBrush);
            HandObj.GetComponent<BrushInteract>().HandSystem(HandTargetRight, HandTargetLeft);
            // isHandRightTrue = true;
            HandFullTest(true);
        }
    }

    // boku temizle
    [Command]
    private void CmdPool(GameObject obj)
    {
        if (obj != null)
        {
            RpcPool(obj);
        }
    }

    [ClientRpc]
    private void RpcPool(GameObject obj)
    {
        //if (!handFull && HandObj == null) return;
        if (HandObj.CompareTag("Brush"))
        {
            obj.GetComponent<Toilet>().PoolDisable();
        }
    }

    // kapı sistemi
    [Command]
    private void CmdDoor(GameObject obj)
    {
        if (obj != null)
        {
            RpcDoor(obj);
        }
    }

    [ClientRpc]
    private void RpcDoor(GameObject obj)
    {
        obj.GetComponent<Door>().ServerDoorOpen();
    }

    // Fryer objeyi pakete yerleştir
    [Command]
    private void CmdFryerButton(GameObject obj)
    {
        if (obj != null)
        {
            RpcFryerButton(obj);
        }
    }

    [ClientRpc]
    private void RpcFryerButton(GameObject obj)
    {
        obj.GetComponent<FryerButton>().ServerStartChange();
    }

    // Fryer objeyi pakete yerleştir
    [Command]
    private void CmdFryerPacketPatato(GameObject obj)
    {
        if (obj != null)
        {
            if (!handFull) return;
            RpcFryerPacketPatato(obj);
        }
    }

    [ClientRpc]
    private void RpcFryerPacketPatato(GameObject obj)
    {
        if (HandObj == null) return;
        if (!HandObj.CompareTag("Patato")) return;
        if (!obj.GetComponent<DeepFryer>().Availability) return;
        if (obj.GetComponent<ItemBox>()._itemAmount < 1) return;
        if (HandObj.GetComponent<Patato>().Patatos.activeSelf) return;
        obj.GetComponent<ItemBox>().ServerAmountChange(1);
        HandObj.GetComponent<Patato>().ServerPatatoView();
        obj.GetComponent<DeepFryer>().avalibleChack();
    }

    // Bardak Doldur
    [Command]
    private void CmdDrinkButton(GameObject obj)
    {
        if (obj != null)
        {
            RpcDrinkButton(obj);
        }
    }

    [ClientRpc]
    private void RpcDrinkButton(GameObject obj)
    {
        if (obj == null) return;

        obj.GetComponent<DrinkButton>().DrinkStart();
    }

    // Bardak yerleştir
    [Command]
    private void CmdDrinkPut(GameObject obj)
    {
        if (obj != null)
        {
            RpcDrinkPut(obj);
        }
    }

    [ClientRpc]
    private void RpcDrinkPut(GameObject obj)
    {
        if (obj != null)
        {
            if (obj.GetComponent<DrinkSlot>().Cup == null)
            {
                if (!handFull) return;
                if (!HandObj.CompareTag("Cup")) return;
                if (!HandObj.GetComponent<DrinkProduct>().enabled) return;
                HandObj.GetComponent<ItemInteract>().ItemParentObj(obj.GetComponent<DrinkSlot>().ParentTransform);
                // HandObj.GetComponent<Rigidbody>().isKinematic = true;
                // HandObj.GetComponent<ItemInteract>().collision.enabled = false;
                obj.GetComponent<DrinkSlot>().AddCup(HandObj);
                isHandRightTrue = false;
                InteractObj = null;
                HandObj = null;
                HandFullTest(false);
            }
            else
            {
                if (handFull) return;
                if (!obj.GetComponent<DrinkSlot>().Cup.GetComponent<DrinkProduct>().isStartFull)
                {
                    CmdTakeItem(obj.GetComponent<DrinkSlot>().Cup);
                    obj.GetComponent<DrinkSlot>().Cup.GetComponent<DrinkProduct>().drinkButton.TickCanvasClose();
                    obj.GetComponent<DrinkSlot>().Cup = null;
                }
            }
        }
    }

    // ocak butonu aktifleştir
    [Command]
    private void CmdGrillButton(GameObject obj)
    {
        if (obj != null)
        {
            RpcGrillButton(obj);
        }
    }

    [ClientRpc]
    private void RpcGrillButton(GameObject obj)
    {
        if (obj == null) return;
        obj.GetComponent<GrillMachineButton>().GrillStartAndStop();
    }

    // tepsiye ürün yerleştir
    [Command]
    private void CmdTrayPutItem(GameObject obj)
    {
        if (obj != null)
        {
            if (handFull)
            {
                RpcTrayPutItem(obj);
            }
            else
            {
                RpcTrayInteract(obj);
            }
        }
    }

    [ClientRpc]
    private void RpcTrayPutItem(GameObject obj)
    {
        if (obj == null || HandObj == null) return;
        if (HandObj.CompareTag("Burger") || HandObj.CompareTag("Cup") || HandObj.CompareTag("Patato"))
        {
            if (HandObj.GetComponent<ItemInteract>().isReadyProduct)
            {
                ItemInteract itemInteract = HandObj.GetComponent<ItemInteract>();
                itemInteract.ServerStateChange(true);
                if (HandObj.GetComponent<ItemName>().isDrink && obj.GetComponent<Tray>().Drink == null)
                {
                    obj.GetComponent<Tray>()
                        .ServerTrayAddItemName(HandObj.GetComponent<ItemName>()._ItemName, 2, HandObj);
                    HandObj.GetComponent<ItemInteract>().enabled = false;
                }

                if (HandObj.GetComponent<ItemName>().isFood && obj.GetComponent<Tray>().Burger == null)
                {
                    obj.GetComponent<Tray>()
                        .ServerTrayAddItemName(HandObj.GetComponent<ItemName>()._ItemName, 1, HandObj);
                    HandObj.GetComponent<ItemInteract>().enabled = false;
                }

                if (HandObj.GetComponent<ItemName>().isSnack && obj.GetComponent<Tray>().Snack == null)
                {
                    obj.GetComponent<Tray>()
                        .ServerTrayAddItemName(HandObj.GetComponent<ItemName>()._ItemName, 3, HandObj);
                    HandObj.GetComponent<ItemInteract>().enabled = false;
                }

                isHandRightTrue = false;
                InteractObj = null;
                HandObj = null;
                HandFullTest(false);
            }
        }
    }

    [ClientRpc]
    private void RpcTrayInteract(GameObject obj)
    {
        if (obj == null || InteractObj != null) return;
        InteractObj = obj;
        InteractObj.GetComponent<TrayInteract>().BoxParentObj(InteractPosBox);
        InteractObj.GetComponent<TrayInteract>()
            .HandSystem(HandTargetRight, HandTargetLeft);
        InteractObj.GetComponent<TrayInteract>().CmdSetInteractBox();

        HandFullTest(true);
    }

    // slot etkileşimi
    [Command]
    private void CmdSlotInteractSpawn(GameObject obj)
    {
        if (obj != null)
        {
            if (handFull)
            {
                RpcSlotAdd(obj);
            }
            else
            {
                if (obj.GetComponent<ItemBox>()._itemAmount < 1) return;
                GameObject spawnObj = Instantiate(obj.GetComponent<ItemBox>().Prefab);
                NetworkServer.Spawn(spawnObj);
                RpcSlotInteractSpawn(spawnObj, obj);
            }
        }
    }

    [ClientRpc]
    private void RpcSlotInteractSpawn(GameObject spawnObj, GameObject slot)
    {
        if (spawnObj == null || InteractPosItem == null) return;
        if (spawnObj.CompareTag("Tray"))
        {
            if (spawnObj.GetComponent<TrayInteract>())
            {
                if (slot.GetComponent<ItemBox>()._itemAmount < 1) return;
                InteractObj = spawnObj;
                spawnObj.GetComponent<TrayInteract>().ServerStateChange(true);
                spawnObj.GetComponent<TrayInteract>().BoxParentObj(InteractPosBox);
                spawnObj.GetComponent<TrayInteract>().HandSystem(HandTargetRight, HandTargetLeft);
                slot.GetComponent<ItemBox>().ServerAmountChange(1);
                HandFullTest(true);
            }
        }
        else
        {
            if (spawnObj.GetComponent<ItemInteract>())
            {
                if (slot.GetComponent<ItemBox>()._itemAmount < 1) return;
                HandObj = spawnObj;
                spawnObj.GetComponent<ItemInteract>().ServerStateChange(false);
                spawnObj.GetComponent<ItemInteract>().ItemParentObj(InteractPosItem);
                spawnObj.GetComponent<ItemInteract>().HandSystem(HandTargetRight);
                slot.GetComponent<ItemBox>().ServerAmountChange(1);
                isHandRightTrue = true;
                HandFullTest(true);
            }
        }
    }

    [ClientRpc]
    private void RpcSlotAdd(GameObject slot)
    {
        //if (InteractObj == null || InteractPosItem == null) return;
        if (InteractObj != null)
        {
            if (InteractObj.CompareTag("Tray"))
            {
                if (InteractObj.GetComponent<TrayInteract>())
                {
                    if (InteractObj != null)
                    {
                        if (slot.GetComponent<ItemBox>()._maxItemAmount > slot.GetComponent<ItemBox>()._itemAmount)
                        {
                            slot.GetComponent<ItemBox>().ServerAmountChangeAdd(1);
                            InteractObj.GetComponent<TrayInteract>().ServerDeleteSystem(true);
                            InteractObj.GetComponent<TrayInteract>().ItemNotParentObj(slot.transform.position);
                            InteractObj = null;
                            HandObj = null;
                            HandFullTest(false);
                        }
                    }
                }
            }
        }

        if (HandObj != null)
        {
            if (HandObj.layer == LayerMask.NameToLayer("Item"))
            {
                // switch (slot.GetComponent<ItemBox>()._itemName)
                // {
                //     case "Hamburger Bread":
                //         print("ekmek silinebilir");
                //         break;
                //     
                // }
                if (slot.GetComponent<ItemBox>()._itemName == HandObj.GetComponent<ItemName>()._ItemName)
                {
                    if (slot.GetComponent<ItemBox>()._maxItemAmount > slot.GetComponent<ItemBox>()._itemAmount)
                    {
                        slot.GetComponent<ItemBox>().ServerAmountChangeAdd(1);
                        HandObj.GetComponent<ItemInteract>().ServerDeleteSystem(true);
                        HandObj.GetComponent<ItemInteract>().ItemNotParentObj(slot.transform.position);
                        InteractObj = null;
                        HandObj = null;
                        HandFullTest(false);
                        print(slot.GetComponent<ItemBox>()._itemName);
                        print("item delete");
                    }
                }
            }
        }
    }

    // pc/cash Register etkileşimi 
    [Command]
    private void CmdServerInteract(GameObject obj)
    {
        if (obj != null)
        {
            RpcServerInteract(obj);
        }
    }

    [ClientRpc]
    private void RpcServerInteract(GameObject obj)
    {
        if (handFull) return;
        InteractObj = obj;
        if (InteractObj.GetComponent<LaptopInteract>())
        {
            if (!InteractObj.GetComponent<LaptopInteract>().isInteractPC)
            {
                InteractObj.GetComponent<LaptopInteract>().SetCameraPosition();
                // PlayerMenuManager
                mouseActivity = true;
                //InGameHud.instance.ServerCross(true);
                CmdInteract(InteractObj);
            }
        }

        if (InteractObj.GetComponent<CashRegister>())
        {
            if (!InteractObj.GetComponent<CashRegister>().isInteractCR)
            {
                InteractObj.GetComponent<CashRegister>().SetPlayerPosition();
                mouseActivity = true;
                //InGameHud.instance.ServerCross(true);
                CmdInteract(InteractObj);
            }
        }
    }

    // kutu etkileşimi başlangıcı - ele alma eylemi.
    [Command]
    private void CmdBoxInteract(GameObject obj)
    {
        if (obj != null)
        {
            RpcBoxInteract(obj);
        }
    }

    [ClientRpc]
    private void RpcBoxInteract(GameObject obj)
    {
        if (handFull) return;
        InteractObj = obj;
        if (InteractObj.GetComponent<ObjInteract>())
        {
            if (!InteractObj.GetComponent<ObjInteract>().isInteract)
            {
                InteractObj.GetComponent<ObjInteract>().BoxParentObj(InteractPosBox);
                InteractObj.GetComponent<ObjInteract>()
                    .HandSystem(HandTargetRight, HandLookRight, HandTargetLeft, HandLookLeft);
                InteractObj.GetComponent<ObjInteract>().CmdSetInteractBox();
                HandFullTest(true);
            }
        }
    }

    // Itemi masadan geri alma / E
    [Command]
    private void CmdTakeItem(GameObject obj)
    {
        if (obj != null)
        {
            RpcTakeItem(obj);
        }
    }

    [ClientRpc]
    private void RpcTakeItem(GameObject obj)
    {
        if (handFull) return;
        HandObj = obj;
        if (HandObj.GetComponent<ItemInteract>())
        {
            HandObj.GetComponent<ItemInteract>().ServerStateChange(false);
            HandObj.GetComponent<ItemInteract>().ItemParentObj(InteractPosItem);
            HandObj.GetComponent<ItemInteract>().HandSystem(HandTargetRight);
            HandObj.GetComponent<Rigidbody>().isKinematic = true;
            //HandObj.GetComponent<ItemInteract>().HandSystem(HandTargetRight);
            isHandRightTrue = true;
            HandFullTest(true);
        }
    }

    // item Combine yapma / E
    [Command]
    private void CmdCombineItem(GameObject obj)
    {
        if (obj != null)
        {
            RpcCombineItem(obj);
        }
    }

    [ClientRpc]
    private void RpcCombineItem(GameObject obj)
    {
        if (!obj.CompareTag("Burger")) return;
        if (HandObj.CompareTag("Meat") || HandObj.CompareTag("Tomato") || HandObj.CompareTag("Lettuce") ||
            HandObj.CompareTag("Cheese"))
        {
            if (!handFull && HandObj == null) return;
            CombineProduct _combineProduct = obj.GetComponent<CombineProduct>();
            if (!_combineProduct.Meat || !_combineProduct.Tomato || !_combineProduct.Lettuce || !_combineProduct.Cheese)
            {
                obj.GetComponent<CombineProduct>()
                    .Combine(HandObj.GetComponent<ItemName>()._ItemName, HandObj, this.gameObject);
                obj.GetComponent<ItemInteract>().enabled = false;
                HandObj = null;
                InteractObj = null;
                isHandRightTrue = false;
                HandFullTest(false);
            }
        }
    }

    [ClientRpc]
    public void CombineInteract()
    {
        HandObj = null;
        InteractObj = null;
        isHandRightTrue = false;
        HandFullTest(false);
    }

    // item paketleme yapma / E
    [Command]
    private void CmdItemPacketing(GameObject obj)
    {
        if (obj != null)
        {
            RpcItemPacketing(obj);
        }
    }

    [ClientRpc]
    private void RpcItemPacketing(GameObject obj)
    {
        if (obj.CompareTag("Packet"))
        {
            if (!handFull && HandObj == null) return;
            if (HandObj.CompareTag("Item"))
            {
                ItemPacket Obj = obj.GetComponent<ItemPacket>();
                HandObj.GetComponent<ItemInteract>().isInteract = false;
                Obj.PossessionItem(HandObj);
                isHandRightTrue = false;
                HandFullTest(false);
                HandObj = null;
            }
        }
    }

    // Musteri Teslim / E
    [Command]
    private void CmdTakeCustomer(GameObject obj)
    {
        if (obj != null)
        {
            RpcTakeCustomer(obj);
        }
    }

    [ClientRpc]
    private void RpcTakeCustomer(GameObject obj)
    {
        if (!handFull || obj == null || InteractObj == null) return;
        Customer Obj = obj.GetComponent<Customer>();
        if (Obj.isStop && InteractObj.CompareTag("Tray"))
        {
            Obj.PlayerGiveProduct(InteractObj, gameObject);
        }
    }

    // Çöpe item boşatma / E
    [Command]
    private void CmdTrashSystem(GameObject obj)
    {
        if (obj != null)
        {
            RpcTrashSystem(obj);
        }
    }

    [ClientRpc]
    private void RpcTrashSystem(GameObject obj)
    {
        if (InteractObj != null)
        {
            if (InteractObj.CompareTag("Box"))
            {
                InteractObj.GetComponent<ObjInteract>().Trash(obj.transform);
                HandFullTest(false);
                InteractObj = null;
                HandObj = null;
            }
            else
            {
                InteractObj.GetComponent<Tray>().Trash();
            }
        }

        if (HandObj != null)
        {
            if (HandObj.CompareTag("Item"))
            {
                HandObj.GetComponent<ItemInteract>().Trash(obj.transform);
                HandFullTest(false);
                InteractObj = null;
                HandObj = null;
            }
        }
        else
        {
            print("boş");
        }
    }

    // duraklatılma yapılınca burası ekranı bırakmayı yarar
    public void Interact() // input sistemi içerisinde
    {
        if (InteractObj != null)
        {
            if (InteractObj.GetComponent<LaptopInteract>())
            {
                if (InteractObj.GetComponent<LaptopInteract>().isInteractPC)
                {
                    CmdInteract(InteractObj);
                }
            }

            if (InteractObj.GetComponent<CashRegister>())
            {
                if (InteractObj.GetComponent<CashRegister>().isInteractCR)
                {
                    CmdInteract(InteractObj);
                }
            }
        }
    }

    [Command]
    private void CmdInteract(GameObject obj)
    {
        if (obj.GetComponent<CashRegister>())
        {
            obj.GetComponent<CashRegister>().CmdSetInteractCR();
        }

        if (obj.GetComponent<LaptopInteract>())
        {
            obj.GetComponent<LaptopInteract>().CmdSetInteractPC();
        }
    }

    // sol Click
    public void BoxInteractShelftoBoxHold() // input sisteminde
    {
        RaycastHit hit;
        // Grill
        if (Physics.Raycast(cam.transform.position, cam.transform.TransformDirection(Vector3.forward), out hit,
                hitRange, pickupLayerMask6))
        {
            CmdDropGrill(hit.point); // ızgaraya yere bırakma eylemi 
        }
        // Slota yerleştir
        else if (Physics.Raycast(cam.transform.position, cam.transform.TransformDirection(Vector3.forward), out hit,
                     hitRange, pickupLayerMask3))
        {
            CmdSlotInteract(hit.collider.gameObject);
        }
        // Tezgah * itemi ve bıçağı bırakma
        else if (Physics.Raycast(cam.transform.position, cam.transform.TransformDirection(Vector3.forward), out hit,
                     hitRange, pickupLayerMask5))
        {
            CmdDropItem(hit.point); // item tezgaha bırakma eylemi 
        }
        // fryer slota yerleştir
        else if (Physics.Raycast(cam.transform.position, cam.transform.TransformDirection(Vector3.forward), out hit,
                     hitRange, pickupLayerMask15))
        {
            CmdSlotInteract(hit.collider.gameObject);
        }
    }

    // kutudaki itemleri boşaltma
    [Command]
    private void CmdSlotInteract(GameObject obj)
    {
        if (obj != null)
        {
            RpcSlotInteract(obj);
        }
    }

    [ClientRpc]
    private void RpcSlotInteract(GameObject obj)
    {
        if (InteractObj == null) return;
        if (InteractObj.CompareTag("Box"))
        {
            obj.GetComponent<ItemBox>().GiveItem(InteractObj.GetComponent<ObjInteract>()._objAmount, InteractObj);
            if (obj.CompareTag("Fryer"))
            {
                obj.GetComponent<DeepFryer>().Availability = false;
            }
        }
    }

    // item bırakma eylemi (tezgaha)
    [Command]
    private void CmdDropItem(Vector3 hitPoint)
    {
        RpcDropItem(hitPoint);
    }

    [ClientRpc]
    private void RpcDropItem(Vector3 hitPoint)
    {
        if (HandObj != null)
        {
            if (HandObj.CompareTag("Brush"))
            {
                if (HandObj.GetComponent<BrushInteract>())
                {
                    offset = hitPoint;
                    HitPointInteract.position = offset + new Vector3(0, 0.2f, 0);
                    BrushInteract itemInteract = HandObj.GetComponent<BrushInteract>();
                    itemInteract.ItemNotParentObj(HitPointInteract.position);
                    itemInteract.ServerStateChange(true);
                    isHandRightTrue = false;
                    InteractObj = null;
                    HandObj = null;
                    HandFullTest(false);
                }
            }
            else
            {
                if (HandObj.GetComponent<ItemInteract>())
                {
                    offset = hitPoint;
                    HitPointInteract.position = offset + new Vector3(0, 0.2f, 0);
                    ItemInteract itemInteract = HandObj.GetComponent<ItemInteract>();
                    itemInteract.ItemNotParentObj(HitPointInteract.position);
                    itemInteract.ServerStateChange(true);
                    isHandRightTrue = false;
                    InteractObj = null;
                    HandObj = null;
                    HandFullTest(false);
                }
            }
        }

        if (InteractObj != null)
        {
            if (InteractObj.CompareTag("Box"))
            {
                if (InteractObj.GetComponent<ObjInteract>())
                {
                    offset = hitPoint;
                    HitPointInteract.position = offset + new Vector3(0, 0.2f, 0);
                    ObjInteract boxObj = InteractObj.GetComponent<ObjInteract>();
                    boxObj.ItemNotParentObj(HitPointInteract.position);
                    boxObj.ServerStateChange(false);
                    boxObj.CmdSetDropBox();
                    boxObj.BoxNotParentObj();
                    isHandRightTrue = false;
                    InteractObj = null;
                    HandObj = null;
                    HandFullTest(false);
                }
            }
            else
            {
                if (InteractObj.GetComponent<TrayInteract>())
                {
                    offset = hitPoint;
                    HitPointInteract.position = offset + new Vector3(0, 0.2f, 0);
                    TrayInteract boxObj = InteractObj.GetComponent<TrayInteract>();
                    boxObj.ItemNotParentObj(HitPointInteract.position);
                    boxObj.ServerStateChange(false);
                    //boxObj.GetComponent<Rigidbody>().isKinematic = false;
                    isHandRightTrue = false;
                    InteractObj = null;
                    HandObj = null;
                    HandFullTest(false);
                }
            }
        }
    }

    // item bırakma eylemi (ızgaraya)
    [Command]
    private void CmdDropGrill(Vector3 hitPoint)
    {
        RpcDropGrill(hitPoint);
    }

    [ClientRpc]
    private void RpcDropGrill(Vector3 hitPoint)
    {
        if (HandObj == null) return;
        if (HandObj.CompareTag("Meat"))
        {
            offset = hitPoint;
            HitPointInteract.position = offset + new Vector3(0, 0.1f, 0);
            ItemInteract itemInteract = HandObj.GetComponent<ItemInteract>();
            itemInteract.ItemNotParentObj(HitPointInteract.position);
            itemInteract.ServerStateChange(true);
            HandObj.GetComponent<GrillProduct>().ServerPosChangeBool(true);
            InteractObj = null;
            HandObj = null;
            HandFullTest(false);
        }
    }


    // Kutu Droplama
    public void DropedInteract() // input sisteminde
    {
        CmdDrop(InteractObj);
    }

    // sade kutu ayarları sunucuya gönderimi (kutuyu yere bırakma)
    [Command]
    private void CmdDrop(GameObject obj)
    {
        RpcDrop(obj);
    }

    [ClientRpc]
    private void RpcDrop(GameObject obj)
    {
        if (obj == null) return;
        if (obj.GetComponent<ObjInteract>())
        {
            HandFullTest(false);
            obj.GetComponent<ObjInteract>().CmdSetDropBox();
            obj.GetComponent<ObjInteract>().BoxNotParentObj();
            InteractObj = null;
        }
    }

    public void HandTakeRight()
    {
        isHandRightTrue = false;
        HandFullTest(false);
    }

    public void HandTakeAll()
    {
        isHandRightTrue = true;
        HandFullTest(true);
    }

    public void HandRes()
    {
        isHandRightTrue = false;
        HandFullTest(false);
        HandObj = null;
        InteractObj = null;
    }

    [Command]
    public void HandFullTest(bool value)
    {
        handFull = value;
        RpcRigHandSystem(value);
    }

    [ClientRpc]
    private void RpcRigHandSystem(bool value)
    {
        if (value)
        {
            if (isHandRightTrue)
            {
                rigBuilder.layers[1].active = value;
                rigBuilder.layers[2].active = !value;
            }
            else
            {
                rigBuilder.layers[2].active = value;
                rigBuilder.layers[1].active = value;
            }
        }
        else
        {
            rigBuilder.layers[2].active = value;
            rigBuilder.layers[1].active = value;
        }

        //CmdAnimation();
    }

    [Command]
    private void MouseActivi()
    {
        if (DailyStatistics.instance.isDayPanel)
        {
            mouseActivity = true;
        }
    }
}