using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;
public class Inventory
{
    public Character character;
    public Inventory(Character cha)
    {
        character = cha;
    }

    public void InventoryViewer() //인벤토리 아이템 리스트
    {
        for (var i = 0; i < character.ownedItems.Count; i++)
        {
            ForegroundColor = ConsoleColor.Red;
            Write(character.ownedItems[i].isEquipped ? "- [E]" : "-    ");
            ResetColor();
            WriteLine($"{character.ownedItems[i].iName} | {character.ownedItems[i].iType} +{character.ownedItems[i].iStat} | {character.ownedItems[i].iInfo}");
        }
    }
    public void InventoryManage() //장착 관리 아이템 리스트
    {
        for (var i = 0; i < character.ownedItems.Count; i++)
        {
            ForegroundColor = ConsoleColor.Red;
            Write(character.ownedItems[i].isEquipped ? "- [E]" : "-    ");
            ResetColor();
            WriteLine($" {i + 1} {character.ownedItems[i].iName} | {character.ownedItems[i].iType} +{character.ownedItems[i].iStat} | {character.ownedItems[i].iInfo}");
        }
    }

    public void ItemEquipping(int EquipInput) //아이템 장착 해제 교체를 담당하는 메서드
    {
        var itemInfo = character.ownedItems[EquipInput - 1];
        if (itemInfo.isEquipped == true)
        {
            // 장착 해제 
            if (itemInfo.iType == "공격력")
            {
                character.Weapon.isEquipped = false;
                character.Weapon = null;
                character.AddAtk -= itemInfo.iStat;
            }
            else if (itemInfo.iType == "방어력")
            {
                character.Armour.isEquipped = false;
                character.Armour = null;
                character.AddDef -= itemInfo.iStat;
            }
        }
        else if (itemInfo.isEquipped == false && itemInfo.iType == "공격력")
        {
        // 무기 장착
            if (character.Weapon == null) //null일때 무기
            {
                character.Weapon = itemInfo;
                character.AddAtk += itemInfo.iStat;
                character.Weapon.isEquipped = true;
            }
            //장착 중 무기 교체
            else if (character.Weapon.isEquipped == true)
            {
                character.Weapon.isEquipped = false;
                character.AddAtk -= character.Weapon.iStat;
                character.Weapon = itemInfo;
                character.Weapon.isEquipped = true;
                character.AddAtk += itemInfo.iStat;
            }
        }
        // 방어구 장착
        else if (itemInfo.isEquipped == false && itemInfo.iType == "방어력") 
        {
            if (character.Armour == null) //null일때 방어구
            {
                character.Armour = itemInfo;
                character.AddDef += itemInfo.iStat;
                character.Armour.isEquipped = true;
            }
            //장착 중 방어구 교체
            else if (character.Armour.isEquipped == true)
            {
                character.Armour.isEquipped = false;
                character.AddDef -= character.Armour.iStat;
                character.Armour = itemInfo;
                character.Armour.isEquipped = true;
                character.AddDef += itemInfo.iStat;
            }
        }
    }

    public void CheckInventory(bool isManage) //인벤토리 출력 메서드
    {
        Clear();

        WriteLine();
        ForegroundColor = ConsoleColor.DarkYellow;
        WriteLine("------------");
        WriteLine("| 인벤토리 |");
        WriteLine("------------");
        ResetColor();
        WriteLine("보유 중인 아이템을 관리할 수 있습니다.");
        WriteLine();
        ForegroundColor = ConsoleColor.DarkCyan;
        WriteLine("[아이템 목록]");
        ResetColor();
        WriteLine(); // 이 부분에 후에 보유중인 아이템 출력해보자 
        if (isManage == true)
        {
            InventoryManage();
            WriteLine();
            WriteLine("--------------------------------------------------");
            WriteLine();
            ForegroundColor = ConsoleColor.Yellow;
            WriteLine("장착하고자 하는 장비의 번호를 입력");
            WriteLine("0. 나가기");
            ResetColor();
            WriteLine();
            WriteLine("원하시는 행동을 입력해주세요");
            WriteLine(">>");
        }
        else
        { 
            InventoryViewer();
            WriteLine();
            WriteLine("--------------------------------------------------");
            WriteLine();
            ForegroundColor = ConsoleColor.Yellow;
            WriteLine("1. 장착 관리");
            WriteLine("0. 나가기");
            ResetColor();
            WriteLine();
            WriteLine("원하시는 행동을 입력해주세요");
            WriteLine(">>");
        }

        
    }
}

