using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class BuyMenu : BaseBuy
{
    [SerializeField] GameObject cursor;
    [SerializeField] Stack<BaseBuy> MenuStack;
    [SerializeField] BaseBuy[] buymenu = new BaseBuy[7];
    [SerializeField]  BaseBuy curmenu;

    private void OnEnable()
    {
        cursor.SetActive(false);
        MenuStack = new Stack<BaseBuy>();
        CallNextMenu(buymenu[0]);
    }
    private void OnDisable()
    {
        cursor.SetActive(true);
    }
    private void Update()
    {
        curmenu.gameObject.SetActive(true);
    }
    public void CallNextMenu(BaseBuy buymenuInfo) //버튼을 눌러 가기.
    {
        MenuStack.Push(buymenuInfo);

        if (curmenu != null) //처음 시작.
        {
            curmenu.gameObject.SetActive(false);
        }
        curmenu = buymenuInfo;
    }

    public void CallPrevMenu()
    {
        if (MenuStack != null)
        {
           curmenu = MenuStack.Pop();
        }
    }

    public void BuyMenuFirst() { CallNextMenu(buymenu[0]);}
    public void RifleMenu() { CallNextMenu(buymenu[1]);}
    public void PistolMenu() { CallNextMenu(buymenu[2]); }
    public void SmgMenu() { CallNextMenu(buymenu[3]); }
    public void HEAVYMenu() { CallNextMenu(buymenu[4]); }
    public void GrenadeMenu() { CallNextMenu(buymenu[5]); }
    public void EquipmentMenu() { CallNextMenu(buymenu[6]); }


}
