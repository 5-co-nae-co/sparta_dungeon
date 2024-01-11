using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using static System.Console;

// cs 파일을 구분해서 클래스를 불러올 수 있다는 사실을 배워서 코드를 구분해보기로 했다.
public class Character
{

    public List<Items> ownedItems;
    //캐릭터의 정보를 클래스로 저장
    public int Level { get; set; }
    public string CharName { get; set; }
    public string CharClass { get; set; }
    public int AtkStat { get; set; }
    public int AddAtk { get; set; }
    public int DefStat { get; set; }
    public int AddDef { get; set; }
    public float Health { get; set; }
    public float Wealth { get; set; }
    public Items Weapon;
    public Items Armour;

    public Character(int level, string charName, string charClass, int atkStat, int addAtk, int defStat, int addDef, float health, float wealth)
    {
        Level = level;
        CharName = charName;
        CharClass = charClass;
        AtkStat = atkStat;
        AddAtk = addAtk;
        DefStat = defStat;
        AddDef = addDef;
        Health = health;
        Wealth = wealth;
        Weapon = null;
        Armour = null;
        ownedItems = new List<Items>();
        CharInv();
    }

    public void CheckPcStat() //플레이어 스탯창 출력 메서드
    {

        Clear();
        WriteLine();
        ForegroundColor = ConsoleColor.DarkYellow;
        WriteLine("-------------");
        WriteLine("| 상태 보기 |");
        WriteLine("-------------");
        ResetColor();
        WriteLine("캐릭터의 정보가 표시됩니다.");
        WriteLine();
        ForegroundColor = ConsoleColor.Red;
        Write("Lv. " + Level + " ");
        ForegroundColor = ConsoleColor.Yellow;
        WriteLine(CharName + " " + CharClass);
        ResetColor();
        WriteLine();
        Write("공격력: ");
        ForegroundColor = ConsoleColor.Red;
        WriteLine((AtkStat+AddAtk) + (AddAtk == 0 ? "" : (" (+" + AddAtk + ")")));
        ResetColor();
        Write("방어력: ");
        ForegroundColor = ConsoleColor.Red;
        WriteLine(DefStat+AddDef + (AddDef == 0 ? "" : (" (+" + AddDef + ")")));
        ResetColor();
        Write("체 력: ");
        ForegroundColor = ConsoleColor.Red;
        WriteLine(Health);
        ResetColor();
        Write("Gold: ");
        ForegroundColor = ConsoleColor.Red;
        WriteLine(Wealth + " G");
        ResetColor();
        WriteLine();
        WriteLine("-----------------------------------------");
        WriteLine();
        ForegroundColor = ConsoleColor.Yellow;
        WriteLine("0. 나가기");
        ResetColor();
        WriteLine();
        WriteLine("원하시는 행동을 입력해주세요");
        WriteLine(">>");

    }
    public void CharInv()
    {
        //Armor
        Items aitem1 = new Items(true, false, "무쇠갑옷", "방어력", 5, "무쇠로 만들어져 튼튼한 갑옷입니다.", 2000);

        //Weapon
        Items witem1 = new Items(true, false, "스파르타의 창", "공격력", 7, "스파르타의 전사들이 사용했다는 전설의 창입니다.", 3000);
        Items witem2 = new Items(true, false, "낡은 검", "공격력", 2, "쉽게 볼 수 있는 낡은 검 입니다.", 600);

        ownedItems.Add(aitem1);
        ownedItems.Add(witem1);
        ownedItems.Add(witem2); //인벤토리 리스트를 정렬하는 방법도 생각해볼 것
    }
}

