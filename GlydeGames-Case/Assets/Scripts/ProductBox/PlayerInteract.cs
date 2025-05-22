using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using DG.Tweening;
using UnityEditor;
using UnityEngine.Animations.Rigging;

public class PlayerInteract : NetworkBehaviour
{
    public Transform HitPointInteract;
    public Animator anims;

    public GameObject cam;

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

    [SyncVar] public bool handFull = false;
    [SyncVar] public bool mouseActivity = false;
    public GameObject InteractObj;
    public GameObject HandObj;
    public Transform InteractPos;
    public Transform InteractPosBox; // Kutu
    public Transform InteractPosItem; // item
    public Transform InteractPosKnife; // Knife

    [Header("El Sistemi")] [SyncVar] public bool isHandRightTrue;
    public RigBuilder rigBuilder;
    [SerializeField] Transform HandTargetRight, HandLookRight;
    [SerializeField] Transform HandTargetLeft, HandLookLeft;
    
    // test spawn
    public GameObject cubeSpaAWNobj;

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
    }

    void Start()
    {
        if (!isLocalPlayer) return;
        LayerAdd();
        mouseActivity = false;
    }

    private void LayerAdd()
    {
        pickupLayerMask = LayerMask.GetMask("Laptop");
        pickupLayerMask2 = LayerMask.GetMask("BoxObj");
        pickupLayerMask3 = LayerMask.GetMask("Shelf");
        pickupLayerMask4 = LayerMask.GetMask("CashRegister");
        pickupLayerMask5 = LayerMask.GetMask("Counter");
        pickupLayerMask6 = LayerMask.GetMask("Grill");
        pickupLayerMask7 = LayerMask.GetMask("Item");
        pickupLayerMask8 = LayerMask.GetMask("Knife");
        pickupLayerMask9 = LayerMask.GetMask("Customer");
        pickupLayerMask10 = LayerMask.GetMask("Door");
        pickupLayerMask11 = LayerMask.GetMask("TrashBin");
    }

    void Update()
    {
        if (isLocalPlayer && isServer)
        {
            //CmdAnimation();
            //CmdViewCanvas();
        }
    }

    // Animasyon
    [Command]
    private void CmdAnimation()
    {
        if (handFull)
        {
            anims.SetBool("isHand", true);
        }

        if (!handFull)
        {
            anims.SetBool("isHand", false);
        }
    }

    [Command]
    private void CmdViewCanvas()
    {
        RaycastHit hit;
        // Laptop
        if (Physics.Raycast(cam.transform.position, cam.transform.TransformDirection(Vector3.forward), out hit,
                hitRange, pickupLayerMask7))
        {
            RpcViewCanvasItem(hit.collider.gameObject);
        }
    }

    [ClientRpc]
    private void RpcViewCanvasItem(GameObject obj)
    {
        if (obj == null) return;
        if (obj.CompareTag("Packet"))
        {
            obj.GetComponent<ItemPacket>().Canvas.SetActive(true);
        }
    }

    public void ServerInteract() // E ile etkileşim
    {
        //TestSpawnObj();
        if (mouseActivity)
        {
            if (InteractObj != null)
            {
                mouseActivity = false;
                CmdInteract(InteractObj);
                InteractObj = null;
            }
        }
        else
        {
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

            // raf
            else if (Physics.Raycast(cam.transform.position, cam.transform.TransformDirection(Vector3.forward), out hit,
                         hitRange, pickupLayerMask3))
            {
                CmdBoxInteract(hit.collider.gameObject);
            }

            // Item
            else if (Physics.Raycast(cam.transform.position, cam.transform.TransformDirection(Vector3.forward), out hit,
                         hitRange, pickupLayerMask7))
            {
                CmdTakeItem(hit.collider.gameObject); // item ele alma
                CmdCombineItem(hit.collider.gameObject); // item combine
                CmdItemPacketing(hit.collider.gameObject);
            }

            // Knife
            else if (Physics.Raycast(cam.transform.position, cam.transform.TransformDirection(Vector3.forward), out hit,
                         hitRange, pickupLayerMask8))
            {
                CmdTakeKnife(hit.collider.gameObject); // bıçak ele alma
            }

            // Customer
            else if (Physics.Raycast(cam.transform.position, cam.transform.TransformDirection(Vector3.forward), out hit,
                         hitRange*20, pickupLayerMask9))
            {
                CmdTakeCustomer(hit.collider.gameObject); // Musteri Teslim
            }

            // Door
            else if (Physics.Raycast(cam.transform.position, cam.transform.TransformDirection(Vector3.forward), out hit,
                    hitRange, pickupLayerMask10))
            {
                CmdDoorSystem(hit.collider.gameObject); // Musteri Teslim
            }
            // Çöpe boşalt
            else if (Physics.Raycast(cam.transform.position, cam.transform.TransformDirection(Vector3.forward), out hit,
                    hitRange, pickupLayerMask11))
            {
                CmdTrashSystem(hit.collider.gameObject); // Musteri Teslim
            }
        }
    }

    [ClientRpc]
    public void pos(GameObject obj)
    {
        obj.transform.position = Vector3.one;
    }
    // test spawn denemesi bitti
    
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
                mouseActivity = true;
                CmdInteract(InteractObj);
            }
        }

        if (InteractObj.GetComponent<CashRegister>())
        {
            if (!InteractObj.GetComponent<CashRegister>().isInteractCR)
            {
                InteractObj.GetComponent<CashRegister>().SetPlayerPosition();
                mouseActivity = true;
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
                InteractObj.GetComponent<ObjInteract>().BoxParentObj(InteractPos, InteractPosBox);
                InteractObj.GetComponent<ObjInteract>().HandSystem(HandTargetRight, HandLookRight, HandTargetLeft, HandLookLeft);
                InteractObj.GetComponent<ObjInteract>().CmdSetInteractBox();
                HandFullTest(true);
            }
        }

        // raftan item alma
        if (InteractObj.GetComponent<Shelf>())
        {
            Shelf shelf = InteractObj.GetComponent<Shelf>();
            if (shelf.item.Count > 0)
            {
                shelf.ShelfTransferSendItemPlayer(InteractPos, InteractPosItem, true,HandTargetRight, HandLookRight, HandTargetLeft, HandLookLeft);
                ServerShelfTest(shelf.CurrentObj);
                isHandRightTrue = true;
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
            HandObj.GetComponent<ItemInteract>().HandSystem(HandTargetRight, HandLookRight, HandTargetLeft, HandLookLeft);
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
        if (!handFull && HandObj == null ) return;
        if(HandObj.CompareTag("Knife"))return;
        if (HandObj.GetComponent<ItemInteract>().isReadyProduct)
        {
            if (obj.GetComponent<CombineProduct>())
            {
                HandObj.GetComponent<ItemInteract>().ServerStateChange(false);
                obj.GetComponent<CombineProduct>().Combine(HandObj.GetComponent<ItemInteract>().itemName, HandObj, obj);
                HandObj.GetComponent<ItemInteract>().isInteract = false;
                HandObj = null;
                InteractObj = null;
                isHandRightTrue = false;
                HandFullTest(false);
            }
        }
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
            if(HandObj.CompareTag("Knife"))return;
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

    // item Kesme *knife / E
    [Command]
    private void CmdTakeKnife(GameObject obj)
    {
        if (obj != null)
        {
            RpcTakeKnife(obj);
        }
    }

    [ClientRpc]
    private void RpcTakeKnife(GameObject obj)
    {
        if (handFull) return;
        HandObj = obj;
        if (HandObj.GetComponent<Knife>())
        {
            HandObj.GetComponent<Knife>().ServerStateChange(false);
            HandObj.GetComponent<Knife>().ItemParentObj(InteractPosKnife);
            HandObj.GetComponent<Knife>().HandSystem(HandTargetRight, HandLookRight, HandTargetLeft, HandLookLeft);
            isHandRightTrue = true;
            HandFullTest(true);
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
        if (!handFull || obj == null || HandObj == null) return;
        Customer Obj = obj.GetComponent<Customer>();
        if (Obj.isStop && Obj.GetComponent<Customer>().customerQueue == 1 && HandObj.CompareTag("Packet"))
        {
            Obj.PlayerGiveProduct(HandObj, gameObject);
        }
    }

    // Kapı Sistemi / E
    [Command]
    private void CmdDoorSystem(GameObject obj)
    {
        if (obj != null)
        {
            RpcDoorSystem(obj);
        }
    }

    [ClientRpc]
    private void RpcDoorSystem(GameObject obj)
    {
        Door Obj = obj.GetComponent<Door>();
        Obj.DoorSystem();
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
            }
        }
        if (HandObj != null)
        {
            if (HandObj.CompareTag("Item"))
            {
                HandObj.GetComponent<ItemInteract>().Trash(obj.transform);
            }  
        }
        else
        {
            print("boş");
        }
       
        
        // if (InteractObj.GetComponent<ObjInteract>())
        // {
        //     if (!InteractObj.GetComponent<ObjInteract>().isInteract)
        //     {
        //         InteractObj.GetComponent<ObjInteract>().BoxParentObj(InteractPos, obj.transform);
        //         //InteractObj.GetComponent<ObjInteract>().HandSystem(HandTargetRight, HandLookRight, HandTargetLeft, HandLookLeft);
        //         //InteractObj.GetComponent<ObjInteract>().CmdSetInteractBox();
        //     }
        // }
        // else if (HandObj.GetComponent<ItemInteract>())
        // {
        //     //HandObj.GetComponent<ItemInteract>().ServerStateChange(false);
        //     HandObj.GetComponent<ItemInteract>().ItemParentObj(obj.transform);
        //    // HandObj.GetComponent<ItemInteract>().HandSystem(HandTargetRight, HandLookRight, HandTargetLeft, HandLookLeft);
        // }
        // else
        // {
        //     print("boş dön");
        // }
        HandFullTest(false);
        InteractObj = null;
        HandObj = null;  
    }

    // ele objeyi datasını atma
    [Server]
    private void ServerShelfTest(GameObject item)
    {
        RpcShelfTest(item);
    }

    [ClientRpc]
    private void RpcShelfTest(GameObject item)
    {
        HandObj = item;
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

        // if (obj.GetComponent<ObjInteract>())
        // {
        //     obj.GetComponent<ObjInteract>().CmdSetInteractBox();
        // }
    }

    // item rotasyonlama / R
    public void ItemRotationProduct() // input sisteminde
    {
        RaycastHit hit;
        // Item
        if (Physics.Raycast(cam.transform.position, cam.transform.TransformDirection(Vector3.forward), out hit,
                hitRange, pickupLayerMask7))
        {
            CmdRotationItem(hit.collider.gameObject);
        }
    }

    // Itemi masadan geri alma / R
    [Command]
    private void CmdRotationItem(GameObject obj)
    {
        if (obj != null)
        {
            RpcRotationItem(obj);
        }
    }

    [ClientRpc]
    private void RpcRotationItem(GameObject obj)
    {
        HandObj = obj;
        if (HandObj.GetComponent<GrillProduct>())
        {
            HandObj.GetComponent<GrillProduct>().ItemRotation();
            HandObj = null;
        }
    }

    // **  kutudan Rafa (mouse Sol Click)
    public void BoxInteractShelftoBoxClick() // input sisteminde
    {
        RaycastHit hit;
        //*
        // etkileşim dışında kutuya ekileşim vermesini engeller
        if (!Physics.Raycast(cam.transform.position, cam.transform.TransformDirection(Vector3.forward), out hit,
                hitRange, pickupLayerMask) || !Physics.Raycast(cam.transform.position,
                cam.transform.TransformDirection(Vector3.forward), out hit,
                hitRange, pickupLayerMask4))
        {
            CmdBoxSettings(InteractObj);
        }

        // rafa dizme 
        if (Physics.Raycast(cam.transform.position, cam.transform.TransformDirection(Vector3.forward), out hit,
                hitRange, pickupLayerMask3))
        {
            CmdShelfAdd(hit.collider.gameObject);
        }
    }

    // **  kutudan Rafa (mouse Sol Hold)
    public void BoxInteractShelftoBoxHold() // input sisteminde
    {
        RaycastHit hit;
        // Grill
        if (Physics.Raycast(cam.transform.position, cam.transform.TransformDirection(Vector3.forward), out hit,
                hitRange, pickupLayerMask6))
        {
            CmdDropGrill(hit.point); // ızgaraya yere bırakma eylemi 
        }

        // Item Kesmek için
        if (Physics.Raycast(cam.transform.position, cam.transform.TransformDirection(Vector3.forward), out hit,
                hitRange, pickupLayerMask7))
        {
            CmdSliceItem(hit.transform.gameObject);
        }
        // Tezgah * itemi ve bıçağı bırakma
        else if (Physics.Raycast(cam.transform.position, cam.transform.TransformDirection(Vector3.forward), out hit,
                     hitRange, pickupLayerMask5))
        {
            CmdDropItem(hit.point); // item tezgaha bırakma eylemi 
        }
    }

    [Command]
    private void CmdBoxSettings(GameObject obj)
    {
        // kutunun item yerleştirirek kapağının açılması
        if (obj == null) return;
        if (obj.GetComponent<ObjInteract>())
        {
            obj.GetComponent<ObjInteract>().BoxCoverOpen();
            obj.GetComponent<ObjInteract>().CmdBoxStatus();
        }
    }

    // kutuda rafa Dizme
    [Command]
    public void CmdShelfAdd(GameObject Obj)
    {
        RpcShelfAdd(Obj);
    }

    [ClientRpc]
    private void RpcShelfAdd(GameObject Obj)
    {
        if (Obj == null || InteractObj == null || !handFull) return;
        if (InteractObj.GetComponent<ObjInteract>())
        {
            ObjInteract objInteract = InteractObj.GetComponent<ObjInteract>();
            if (!objInteract.isClosed)
            {
                Shelf shelf = Obj.GetComponent<Shelf>();
                if (shelf.ItemName == null || objInteract.itemName == shelf.ItemName)
                {
                    shelf.ItemName = objInteract.itemName;
                    objInteract.BoxTransferSendItem(shelf.Slots, shelf.itemsSlot, shelf.item, shelf.slotCurrent,
                        shelf.ItemName, true);
                }
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
        if (HandObj == null) return;
        if (HandObj.GetComponent<ItemInteract>())
        {
            offset = hitPoint;
            HitPointInteract.position = offset + new Vector3(0, 0.1f, 0);
            ItemInteract itemInteract = HandObj.GetComponent<ItemInteract>();
            itemInteract.ItemNotParentObj(HitPointInteract.position);
            itemInteract.ServerStateChange(true);
            isHandRightTrue = false;
            InteractObj = null;
            HandObj = null;
            HandFullTest(false);
        }
        else if (HandObj.GetComponent<Knife>())
        {
            offset = hitPoint;
            HitPointInteract.position = offset + new Vector3(0, 0.1f, 0);
            Knife itemInteract = HandObj.GetComponent<Knife>();
            itemInteract.ItemNotParentObj(HitPointInteract.position);
            itemInteract.ServerStateChange(true);
            isHandRightTrue = false;
            InteractObj = null;
            HandObj = null;
            HandFullTest(false);
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
        if (HandObj.GetComponent<ItemInteract>())
        {
            offset = hitPoint;
            HitPointInteract.position = offset + new Vector3(0, 0.1f, 0);
            ItemInteract itemInteract = HandObj.GetComponent<ItemInteract>();
            itemInteract.ItemNotParentObj(HitPointInteract.position);
            itemInteract.ServerStateChange(true);
            InteractObj = null;
            HandObj = null;
            HandFullTest(false);
        }
    }

    // item Kesme
    [Command]
    private void CmdSliceItem(GameObject Obj)
    {
        RpcSliceItem(Obj);
    }

    [ClientRpc]
    private void RpcSliceItem(GameObject Obj)
    {
        if (HandObj == null) return;
        if (!Obj.CompareTag("CutItem")) return;
        if (HandObj.GetComponent<Knife>())
        {
            SliceProduct itemInteract = Obj.GetComponent<SliceProduct>();
            itemInteract.ServerDelaySlide();
        }
    }

    // **  etkileşim iptali (mouse sol)
    public void MouseInputCanceled() // input sisteminde
    {
        RaycastHit hit;
        // kesme iptali
        if (Physics.Raycast(cam.transform.position, cam.transform.TransformDirection(Vector3.forward), out hit,
                hitRange, pickupLayerMask7))
        {
            CmdSliceItemCanceled(hit.transform.gameObject);
        }
    }

    // item Kesme
    [Command]
    private void CmdSliceItemCanceled(GameObject Obj)
    {
        RpcSliceItemCanceled(Obj);
    }

    [ClientRpc]
    private void RpcSliceItemCanceled(GameObject Obj)
    {
        if (HandObj == null) return;
        if (!Obj.CompareTag("CutItem")) return;
        if (HandObj.GetComponent<Knife>())
        {
            SliceProduct itemInteract = Obj.GetComponent<SliceProduct>();
            if (!itemInteract.isSlice)
            {
                itemInteract.ServerDelaySlideCanceled();
            }
        }
    }

    // **  Raftan kutuya (mouse Sağ)
    public void BoxInteractBoxtoShelf() // input sisteminde
    {
        RaycastHit hit;
        // raftan kutuya geri alma
        if (Physics.Raycast(cam.transform.position, cam.transform.TransformDirection(Vector3.forward), out hit,
                hitRange, pickupLayerMask3))
        {
            CmdBoxAdd(hit.collider.gameObject);
        }
    }

    // raftan kutuya
    [Command]
    public void CmdBoxAdd(GameObject Obj)
    {
        RpcBoxAdd(Obj);
    }

    [ClientRpc]
    private void RpcBoxAdd(GameObject Obj)
    {
        if (Obj == null || InteractObj == null || !handFull) return;
        if (InteractObj.GetComponent<ObjInteract>())
        {
            ObjInteract objInteract = InteractObj.GetComponent<ObjInteract>();
            if (!objInteract.isClosed)
            {
                Shelf shelf = Obj.GetComponent<Shelf>();
                if (objInteract.itemName == shelf.ItemName)
                {
                    shelf.ShelfTransferSendItem(objInteract.BoxPosition, objInteract.itemsSlot, objInteract.item, objInteract.slotCurrent, objInteract.itemName, true);
                }
            }
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

    // kutu Kapağını Kapatma Eylemi
    public void BoxCoverClose() // input sisteminde 
    {
        CmdBoxClose(InteractObj);
    }

    [Command]
    private void CmdBoxClose(GameObject obj)
    {
        if (obj == null) return;
        if (obj.GetComponent<ObjInteract>())
        {
            obj.GetComponent<ObjInteract>().BoxCoverClose();
            obj.GetComponent<ObjInteract>().CmdBoxStatus();
        }
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

        CmdAnimation();
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