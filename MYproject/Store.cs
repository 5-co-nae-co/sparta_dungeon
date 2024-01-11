using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;

class Store
{
    public Character character;
    public List<Items> storeItems;
    public Store(Character cha)
    {
        character = cha;
        storeItems = new List<Items>();
        StoreOwn();
    }
    public void StoreViewer() //일반 상점 창
    {
        for (var i = 0; i < storeItems.Count; i++)
        {
            ForegroundColor = ConsoleColor.Yellow;
            Write($"{storeItems[i].iName}");
            ResetColor();
            Write($" | {storeItems[i].iType} +{storeItems[i].iStat} | {storeItems[i].iInfo} | ");
            ForegroundColor = ConsoleColor.Red;
            WriteLine(storeItems[i].isBought ? "구매 완료" : $"{storeItems[i].iPrice}G");
            ResetColor();
            WriteLine();
        }
    }
    public void BuyingViewer() //상점 구매 창
    {
        for (var i = 0; i < storeItems.Count; i++)
        {
            ForegroundColor = ConsoleColor.Red;
            Write($"{i + 1} ");
            ForegroundColor = ConsoleColor.Yellow;
            Write($"{storeItems[i].iName}");
            ResetColor();
            Write($" | {storeItems[i].iType} +{storeItems[i].iStat} | {storeItems[i].iInfo} | ");
            ForegroundColor = ConsoleColor.Red;
            WriteLine(storeItems[i].isBought ? "구매 완료" : $"{storeItems[i].iPrice}G");
            ResetColor();
            WriteLine();
        }
    }
    public void SellingViewer() //상점 판매 창
    {
        for (var i = 0; i < character.ownedItems.Count; i++)
        {
            ForegroundColor = ConsoleColor.Red;
            Write(character.ownedItems[i].isEquipped ? "- [E] " : "-     ");
            Write($"{i + 1} ");
            ForegroundColor = ConsoleColor.Yellow;
            Write($"{character.ownedItems[i].iName}");
            ResetColor();
            Write($" | {character.ownedItems[i].iType} +{character.ownedItems[i].iStat} | {character.ownedItems[i].iInfo} | ");
            ForegroundColor = ConsoleColor.Red;
            WriteLine($"{character.ownedItems[i].iPrice}G");
            ResetColor();
            WriteLine();
        }
    }
    //아이템 구매 메소드
    public void ItemBuying(int BuyingInput)
    {
        var itemBuying = storeItems[BuyingInput - 1];
        if (itemBuying.isBought == false)
        {
            if (itemBuying.iPrice <= character.Wealth)
            {
                itemBuying.isBought = true;
                character.Wealth -= itemBuying.iPrice;
                character.ownedItems.Add(storeItems[BuyingInput - 1]);
            }
            else
            {
                ForegroundColor = ConsoleColor.Red;
                WriteLine("\"오이 오이 돈이 부족하다고.\"");
                ResetColor();
                WriteLine("확인을 위해 아무 키나 눌러주세요");
                ReadLine();
            }
        }
        else if (itemBuying.isBought == true)
        {
            ForegroundColor = ConsoleColor.Red;
            WriteLine("이미 구매한 상품 입니다.");
            ResetColor();
            WriteLine("확인을 위해 아무 키나 눌러주세요");
            ReadLine();
        }
    }
    //아이템 판매 메소드
    public void ItemSelling(int SellingInput)
    {
        var itemSelling = character.ownedItems[SellingInput - 1];
        if (itemSelling.isBought == true)
        {
            if(itemSelling.isEquipped == true)
            {
                itemSelling.isEquipped = false; //장비 해제
                if (itemSelling.iType == "공격력") //추가 스탯 초기화
                {
                    character.Weapon = null;
                    character.AddAtk -= itemSelling.iStat;
                }
                else
                {
                    character.Armour = null;
                    character.AddDef -= itemSelling.iStat;
                }
                itemSelling.isBought = false;
            }
            character.Wealth += (itemSelling.iPrice * 0.85f);
            character.ownedItems.Remove(character.ownedItems[SellingInput - 1]);
        }
    }
    public void CheckStore(bool isBuying, bool isSelling)
    {
        Clear();

        WriteLine();
        ForegroundColor = ConsoleColor.DarkYellow;
        WriteLine("--------");
        WriteLine("| 상점 |");
        WriteLine("--------");
        ResetColor();
        WriteLine("필요한 아이템을 얻을 수 있는 상점입니다.");
        WriteLine();
        ForegroundColor = ConsoleColor.Cyan;
        Write("상점 주인");
        ResetColor();
        ForegroundColor = ConsoleColor.Blue;
        Write(": ");
        WriteLine("어서오시게나 젊은 르탄이. 무엇을 사고 싶으신가? 팔 것이라도?");
        ResetColor();
        WriteLine();
        ForegroundColor = ConsoleColor.Yellow;
        WriteLine("[보유 골드]");
        SetCursorPosition(2, CursorTop);
        WriteLine(character.Wealth + " G");
        ResetColor();
        WriteLine();
        WriteLine();
        ForegroundColor = ConsoleColor.Cyan;
        WriteLine("[아이템 목록]");
        ResetColor();
        WriteLine();
        //여기에다가 목록 띄우기
        if (!isBuying && !isSelling)
        {
            StoreViewer();//일반

            WriteLine("-------------------------------------------------------------------------------");
            WriteLine();
            ForegroundColor = ConsoleColor.Yellow;
            WriteLine("1. 아이템 구매");
            WriteLine("2. 아이템 판매");
            WriteLine("0. 나가기");
            ResetColor();
            WriteLine();
            WriteLine("원하시는 행동을 입력해주세요.");
            WriteLine(">>");
        }
        else if (isBuying && !isSelling) //구매
        {
            BuyingViewer();

            WriteLine("-------------------------------------------------------------------------------");
            WriteLine();
            ForegroundColor = ConsoleColor.Yellow;
            WriteLine("구매하고자 하는 아이템의 번호를 입력");
            WriteLine("0. 나가기");
            ResetColor();
            WriteLine();
            WriteLine("원하시는 행동을 입력해주세요.");
            WriteLine(">>");
        }
        else if (!isBuying && isSelling) //판매
        {
            SellingViewer();

            WriteLine("-------------------------------------------------------------------------------");
            WriteLine();
            ForegroundColor = ConsoleColor.Yellow;
            WriteLine("판매하고자 하는 아이템의 번호를 입력");
            WriteLine("0. 나가기");
            ResetColor();
            WriteLine();
            WriteLine("원하시는 행동을 입력해주세요.");
            WriteLine(">>");
        }
    }
    public void StoreOwn()
    {
        //Armor
        Items aitem1 = new Items(false, false, "거렁뱅이의 옷", "방어력", 1, "마을 촌장의 마음보다 넓은 구멍이 뚫려있는 의복입니다.", 100);
        Items aitem2 = new Items(false, false, "수련자갑옷", "방어력", 5, "수련에 도움을 주는 갑옷입니다.", 1000);
        Items aitem3 = new Items(true, false, "무쇠갑옷", "방어력", 9, "무쇠로 만들어져 튼튼한 갑옷입니다.", 2000);
        Items aitem4 = new Items(false, false, "스파르타의 갑옷", "방어력", 15, "스파르타의 전사들이 사용했다는 전설의 갑옷입니다.", 3500);
        Items aitem5 = new Items(false, false, "플롯아머", "방어력", 30, "당신이 이야기의 주인공입니다. 죽지 않습니다.", 7000);


        //Weapon
        Items witem1 = new Items(false, false, "풍선 검", "공격력", 1, "풍선은 무기가 아닙니다. 무기가.", 100);
        Items witem2 = new Items(true, false, "낡은 검", "공격력", 2, "쉽게 볼 수 있는 낡은 검 입니다.", 600);
        Items witem3 = new Items(false, false, "청동 도끼", "공격력", 5, "어디선가 사용됐던거 같은 도끼입니다.", 600);
        Items witem4 = new Items(true, false, "스파르타의 창", "공격력", 7, "스파르타의 전사들이 사용했다는 전설의 창입니다.",3000);
        Items witem5 = new Items(false, false, "촌장의 몽둥이", "공격력", 15, "촌장이 과거 말 안듣는 이들을 향해 휘둘렀다는 몽둥이입니다.", 5000);

        storeItems.Add(aitem1);
        storeItems.Add(aitem2);
        storeItems.Add(aitem3);
        storeItems.Add(aitem4);
        storeItems.Add(aitem5);

        storeItems.Add(witem1);
        storeItems.Add(witem2);
        storeItems.Add(witem3);
        storeItems.Add(witem4);
        storeItems.Add(witem5);
    }
}
