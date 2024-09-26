using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.InputSystem;

public class Inventory_Manager : MonoBehaviour
{
    
    [SerializeField] GameObject droppoint;
    [SerializeField] GameObject BuyMenu;
    [SerializeField] Human user;

    [HideInInspector]
    public Weapon[] HAND = new Weapon[5]; //���� ���� ���� ���
    public Dictionary<int, Weapon> WeaponInfo = new Dictionary<int, Weapon>(); // ��� ����� �����ȣ�� ������.
    [HideInInspector]
    public Weapon curweapon; //���� ����
    [HideInInspector]
    public Weapon prevweapon;
    [HideInInspector]
    public bool CanIPlantBomb;
    [HideInInspector]
    public bool IsBuyCant;

    bool weaponcooltime;

    Coroutine firecoroutine;
    Coroutine reloadcoroutine;
    void FirstSetting() //���� �� ��� ���� ��Ȱ��ȭ+ Į ����� ����
    {
        Weapon[] Weapons = gameObject.GetComponentsInChildren<Weapon>(); //�ڽ� �Ʒ��� ��� ���� �������� ��ųʸ��� �߰�.

        for (int x = 0; x < Weapons.Length; x++)
        {
            WeaponInfo.Add(Weapons[x].GetComponent<Weapon>().weaponnumber, Weapons[x]);
        }

        foreach (Weapon a in gameObject.GetComponentsInChildren<Weapon>()) //��� ���� ��Ȱ��ȭ
        {
            a.gameObject.SetActive(false);
        }

        HAND[2] = WeaponInfo[50]; // weapon number 50 -> knife
        curweapon = HAND[2];
        //user.audiosource.clip = curweapon.FireSound;

    }
    void Start()
    {
        FirstSetting();
    }
    private void Update()
    {
        curweapon.gameObject.SetActive(true);
    }
    void OnReload()
    {
        if (user != null)
        {
          user.Reload(curweapon);
        }
    }

    public void PickUpWeapon(GameObject pickupweapon)
    {
        int number = pickupweapon.GetComponent<item>().weaponnumber;

        switch (pickupweapon.GetComponent<item>().weaponstyle)
        {
            case 0:
                if (HAND[0] == null)
                {
                    HAND[0] = WeaponInfo[number];
                    Destroy(pickupweapon);
                }
                break;
            case 1:
                if (HAND[1] == null)
                {
                    HAND[1] = WeaponInfo[number];
                    Destroy(pickupweapon);
                }
                break;
            case 3:
                if (HAND[3] == null)
                {
                    HAND[3] = WeaponInfo[number];
                    Destroy(pickupweapon);
                }
                break;
            case 4:
                if (HAND[4] == null)
                {
                    HAND[4] = WeaponInfo[number];
                    Destroy(pickupweapon);
                }
                break;
        }
    }
    void OnDropWeapon(InputValue value)
    {
        DropWeapon();
    }
    void DropWeapon()
    {
        if(curweapon == HAND[2])
        {
            return;
        }
        if (curweapon != prevweapon)
        {
            GameObject dropitem = curweapon.GetComponent<Weapon>().dropPrefab;
            Instantiate(dropitem, droppoint.transform.position, transform.rotation);
            for (int x = 0; x < HAND.Length; x++) //���⸦ ���� �� ���� ���� ��Ͽ��� ����.
            {
                if (curweapon == HAND[x])
                {
                    HAND[x] = null;
                }
            }
            ChangeWeapon(HAND[2]);
        }

    }
    void OnRifle(InputValue button){if (HAND[0] != null){ ChangeWeapon(HAND[0]); }}
    void OnPistol(InputValue button) { if (HAND[1] != null) { ChangeWeapon(HAND[1]); } }
    void OnMelee(InputValue button) { if (HAND[2] != null) { ChangeWeapon(HAND[2]); } }
    void OnGrenade(InputValue button) { if (HAND[3] != null) { ChangeWeapon(HAND[3]); } }
    void OnBomb(InputValue button) { if (HAND[4] != null) { ChangeWeapon(HAND[4]); } }
    void OnBuyMenu(InputValue button)
    {
        if (GetComponentInParent<AI>()) { return; }

        if (IsBuyCant)
        {
            if (BuyMenu.activeSelf == true)
            {
                BuyMenu.SetActive(false);
            }
            else if (BuyMenu.activeSelf == false)
            {
                BuyMenu.SetActive(true);
            }
        }
        else
        {
            BuyMenu.SetActive(false);
        }
    }



    public void ChangeWeapon(Weapon swapweapon) //�ٸ� ����� ���.
    {

        if(curweapon == swapweapon) // ���� ����� ������ ���⸦ ����� ���
        {
            prevweapon = null;
            return;
        }
        // ��ü�Ϸ��� �� ���⸦ ������ �־��� ���� 
        if (HAND[swapweapon.weaponstyle] == swapweapon) 
        {   prevweapon = curweapon;
            //���� ���� ��Ȱ��ȭ �� �� ���� Ȱ��ȭ.
            prevweapon.gameObject.SetActive(false);
            curweapon = swapweapon;

        }
        else if(HAND[swapweapon.weaponstyle] != swapweapon)
        {
            prevweapon = HAND[2]; //�� ���⸦ Į�� ����.
            Debug.Log("2");
            this.curweapon = swapweapon;
            prevweapon.gameObject.SetActive(false);
            curweapon.gameObject.SetActive(true);
        }

        //user.audiosource.clip = swapweapon.FireSound;

    }

    public void BuyWeapon(Weapon purchaseweapon)
    {
        if(IsBuyCant)
        {
            
            if (HAND[purchaseweapon.weaponstyle] != null) //�� ��Ÿ���� ���⸦ �̹� ���� �־��ٸ�
            {
                if (HAND[purchaseweapon.weaponstyle] != purchaseweapon) //���� ����� �ٸ� ���⸦ ����Ѵٸ�
                {
                    DropWeapon(); //�� ���⸦ ������, ���� ��Ͽ��� ����. Į����� �ٲ�.
                    HAND[purchaseweapon.weaponstyle] = purchaseweapon;
                    ChangeWeapon(HAND[purchaseweapon.weaponstyle]);
                }
                else if (HAND[purchaseweapon.weaponstyle] == purchaseweapon)
                {
                    return;
                }

            }
            else //�� ��ȣ�� ���Ⱑ ���� ���.
            {
                HAND[purchaseweapon.weaponstyle] = purchaseweapon;
                ChangeWeapon(purchaseweapon);
            }
        }

    }
    public void DefuseBomb()
    {
        if(transform.parent.tag == "T")
        {
            Debug.Log("�׷�����Ʈ�� ��ź�� ��ü�� �� �����ϴ�.");
            return;
        }
    }
    public void BombPlanted()
    {
        if (CanIPlantBomb)
        {
            ChangeWeapon(HAND[2]);
            prevweapon = null;
            HAND[4] = null;
        }
    }
}