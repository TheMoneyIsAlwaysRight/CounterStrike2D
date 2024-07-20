using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

public class WeaponManager : MonoBehaviour
{
    [SerializeField] Human user;
    [SerializeField] public Weapon[] HAND = new Weapon[5]; //현재 가진 무기 목록
    [SerializeField] public Dictionary<int, Weapon> WeaponInfo = new Dictionary<int, Weapon>(); // 모든 무기는 무기번호로 지정됨.
    [SerializeField] GameObject droppoint;
    [SerializeField] public Weapon curweapon; //현재 무기
    [SerializeField] public Weapon prevweapon;
    [SerializeField] GameObject BuyMenu;
    [SerializeField] public bool CanIPlantBomb;
    [SerializeField] public bool IsBuyCant;

    bool weaponcooltime;


    Coroutine firecoroutine;
    Coroutine reloadcoroutine;
    void FirstSetting() //시작 시 모든 무기 비활성화+ 칼 무기로 시작
    {
        Weapon[] Weapons = gameObject.GetComponentsInChildren<Weapon>(); //자식 아래의 모든 무기 정보들을 딕셔너리에 추가.

        for (int x = 0; x < Weapons.Length; x++)
        {
            WeaponInfo.Add(Weapons[x].GetComponent<Weapon>().weaponnumber, Weapons[x]);
        }
        foreach (Weapon a in gameObject.GetComponentsInChildren<Weapon>()) //모든 무기 비활성화
        {
            a.gameObject.SetActive(false);
        }
        HAND[2] = WeaponInfo[50]; // weapon number 50 -> knife
        curweapon = HAND[2];
        user.audiosource.clip = curweapon.FireSound;

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
            Debug.Log("칼 무기는 버릴 수 없습니다.");
            return;
        }
        if (curweapon != prevweapon)
        {
            GameObject dropitem = curweapon.GetComponent<Weapon>().dropPrefab;
            Instantiate(dropitem, droppoint.transform.position, transform.rotation);
            for (int x = 0; x < HAND.Length; x++) //무기를 버린 뒤 현재 무기 목록에서 지움.
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
            Debug.Log("상점 지역이 아닙니다.");
        }
    }



    public void ChangeWeapon(Weapon swapweapon) //다른 무기로 들기.
    {

        if(curweapon == swapweapon) // 기존 무기와 동일한 무기를 들려할 경우
        {
            prevweapon = null;
            return;
        }

        if (HAND[swapweapon.weaponstyle] == swapweapon) // 교체하려는 그 무기를 가지고 있었을 때만   
        {   prevweapon = curweapon;
            prevweapon.gameObject.SetActive(false);
            curweapon = swapweapon;

        }
        else if(HAND[swapweapon.weaponstyle] != swapweapon)
        {
            prevweapon = HAND[2]; //전 무기를 칼로 고정.
            Debug.Log("2");
            this.curweapon = swapweapon;
            prevweapon.gameObject.SetActive(false);
            curweapon.gameObject.SetActive(true);
        }

        user.audiosource.clip = swapweapon.FireSound;

    }

    public void BuyWeapon(Weapon purchaseweapon)
    {
        if(IsBuyCant)
        {
            
            if (HAND[purchaseweapon.weaponstyle] != null) //그 스타일의 무기를 이미 갖고 있었다면
            {
                if (HAND[purchaseweapon.weaponstyle] != purchaseweapon) //기존 무기와 다른 무기를 사려한다면
                {
                    DropWeapon(); //그 무기를 버리고, 현재 목록에서 삭제. 칼무기로 바뀜.
                    HAND[purchaseweapon.weaponstyle] = purchaseweapon;
                    ChangeWeapon(HAND[purchaseweapon.weaponstyle]);
                }
                else if (HAND[purchaseweapon.weaponstyle] == purchaseweapon)
                {
                    Debug.Log("같은 무기를 사려했습니다.");
                    return;
                }

            }
            else //그 번호에 무기가 없을 경우.
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
            Debug.Log("테러리스트는 폭탄을 해체할 수 없습니다.");
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
