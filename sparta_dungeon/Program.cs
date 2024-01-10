using System.ComponentModel.DataAnnotations;
using System.IO.Pipes;
using System.Net.Security;
using System.Numerics;
using System.Xml.Linq;

namespace sparta_dungeon
{
    internal class Program
    {
        static Character player;
        static Inventory inventory = new Inventory();
        static void settings()
        {
            player = new Character(1, "마법사", "이호열", 10, 5, 100, 1500);

            Item ChainArmor = new Item("무쇠 갑옷", 0, 10, "무쇠로 만들어져 튼튼한 갑옷입니다.", false, false, 300);
            inventory.Add(ChainArmor);
            Item OldSword = new Item("낡은 검", 10, 0, "낡은 검", false, false, 400);
            inventory.Add(OldSword);
            Item SpartaSpear = new Item("스파르타 창", 15, 0, "스파르타 전사들이 사용했다는 전설의 창입니다.", false, false, 500);
            inventory.Add(SpartaSpear);
            Item SpartaArmor = new Item("스파르타 갑옷", 0, 20, "스파르타 전사들이 입던 갑옷입니다.", false, false, 3000);
            inventory.Add(SpartaArmor);
        }

        public static void Main()
        {
            settings();
            Start();
        }
        static void Start()
        {
            Console.Clear();
            Console.WriteLine("스파르타 마을에 오신 여러분 환영합니다.");
            Console.WriteLine("이곳에서 던전으로 들어가기전 활동을 할 수 있습니다.");
            Console.WriteLine("1. 상태 보기");
            Console.WriteLine("2. 인벤토리");
            Console.WriteLine("3. 상점");
            Console.WriteLine("원하시는 행동을 입력해주세요.");
            Console.WriteLine(">>");
            int acton = int.Parse(Console.ReadLine());
            bool isMain = true;
            do
            {
                isMain = false;
                if (acton == 1)
                {
                    State();
                }
                else if (acton == 2)
                {
                    Inventory();
                }
                else if (acton == 3)
                {
                    Shop();
                }
                else
                {
                    Console.WriteLine("잘못된 입력입니다.");
                    acton = int.Parse(Console.ReadLine());
                    isMain = true;
                }
            } while (isMain);
        }
        //반복적으로 State 진입시에 플레이어 스텟이 추가되지 않도록 추가해야함.
        static void StateUpdate()
        {
            foreach (Item item in inventory.itemList)
            {
                if (!item.isEquiped)
                {
                    Console.WriteLine("변경사항 없음");    
                }
                if (item.Offense != 0)
                {
                    player.Offense += item.Offense;
                }
                if (item.Defense != 0)
                {
                    player.Defence += item.Defense;
                }
            }

        }
        static void State()
        {
            StateUpdate();
            Console.Clear();
            Console.WriteLine("상태 보기");
            Console.WriteLine("캐릭터의 정보가 표시됩니다.");
            Console.WriteLine($"Lv. {player.Lv}");
            Console.WriteLine($"Chad ( {player.Job} )");
            Console.WriteLine($"공격력 : {player.Offense}");
            Console.WriteLine($"방어력 : {player.Defence}");
            Console.WriteLine($"체력 : {player.Hp}");
            Console.WriteLine($"Gold : {player.Gold}");
            Console.WriteLine();
            Console.WriteLine("0. 나가기");
            Console.WriteLine();
            Console.WriteLine("원하시는 행동을 입력해주세요.");
            Console.WriteLine(">>");


            int acton = int.Parse(Console.ReadLine());
            bool isMain = true;
            do
            {
                isMain = false;
                if (acton == 0)
                {
                    Start();
                }
                else
                {
                    Console.WriteLine("잘못된 입력입니다.");
                    acton = int.Parse(Console.ReadLine());
                    isMain = true;
                }
            } while (isMain);
        }

        static void Inventory()
        {
            Console.Clear();
            Console.WriteLine("인벤토리");
            Console.WriteLine("보유 중인 아이템을 관리할 수 있습니다.");
            Console.WriteLine();
            Console.WriteLine("[아이템 목록]");

            inventory.isEquipedInventory();

            Console.WriteLine();
            Console.WriteLine("0. 나가기");
            Console.WriteLine("1. 장착 관리");
            Console.WriteLine();
            Console.WriteLine("원하시는 행동을 입력해주세요.");
            Console.WriteLine(">>");

            List<Item> itemList = inventory.itemList;
            int acton = int.Parse(Console.ReadLine());
            bool isMain = true;
            do
            {
                isMain = false;
                if (acton == 0)
                {
                    Start();
                }
                else if (acton == 1)
                {
                    isEquipedInventory();
                }
                else
                {
                    Console.WriteLine("잘못된 입력입니다.");
                    acton = int.Parse(Console.ReadLine());
                    isMain = true;
                }
            } while (isMain);
        }

        static void isEquipedInventory()
        {
            Console.Clear();
            Console.WriteLine("인벤토리 - 장착 관리");
            Console.WriteLine("보유 중인 아이템을 관리할 수 있습니다.");
            Console.WriteLine();
            Console.WriteLine("[보유 중인 아이템 목록]");

            inventory.isEquipedInventory();

            Console.WriteLine();
            Console.WriteLine("0. 나가기");
            Console.WriteLine();
            Console.WriteLine("원하시는 행동을 입력해주세요.");
            Console.WriteLine(">>");

            List<Item> itemList = inventory.itemList;
            int acton = int.Parse(Console.ReadLine());
            bool isMain = true;
            do
            {
                isMain = false;

                if (acton == 0)
                {
                    Start();
                }
                else if (acton == 1 || acton == 2 || acton == 3|| acton == 4)
                {
                    Item inputItem = itemList[acton - 1];
                    player.EquipWeapon(inputItem);
                    if (!inputItem.isEquiped)
                    {
                        inputItem.isEquiped = true;
                    }
                    else
                    {
                        inputItem.isEquiped = false;
                    }
                    isEquipedInventory();
                }
                else
                {
                    Console.WriteLine("잘못된 입력입니다.");
                    acton = int.Parse(Console.ReadLine());
                    isMain = true;
                }
            } while (isMain);
        }
        static void Shop()
        {
            Console.Clear();
            Console.WriteLine("상점");
            Console.WriteLine("필요한 아이템을 얻을 수 있는 상점입니다.");

            Console.WriteLine();
            Console.WriteLine("[보유 골드]");
            Console.WriteLine($"{player.Gold} G");

            Console.WriteLine();
            Console.WriteLine("[모든 아이템 목록]");

            inventory.DisplayShop();

            Console.WriteLine();
            Console.WriteLine("0. 나가기");
            Console.WriteLine("1. 아이템 구매");
            Console.WriteLine();
            Console.WriteLine("원하시는 행동을 입력해주세요.");
            Console.WriteLine(">>");

            int acton = int.Parse(Console.ReadLine());
            bool isMain = true;
            do
            {
                isMain = false;

                if (acton == 0)
                {
                    Start();
                }
                else if (acton == 1)
                {
                    ShopBuy();
                }
                else
                {
                    Console.WriteLine("잘못된 입력입니다.");
                    acton = int.Parse(Console.ReadLine());
                    isMain = true;
                }
            } while (isMain);
        }

        static void ShopBuy()
        {
            Console.Clear();
            Console.WriteLine("상점 - 아이템 구매");
            Console.WriteLine("필요한 아이템을 얻을 수 있는 상점입니다.");

            Console.WriteLine();
            Console.WriteLine("[보유 골드]");
            Console.WriteLine($"{player.Gold} G");

            Console.WriteLine();
            Console.WriteLine("[구매가능한 아이템 목록]");

            inventory.DisplayShop();

            Console.WriteLine();
            Console.WriteLine("0. 나가기");
            Console.WriteLine("1. 아이템 구매");
            Console.WriteLine();
            Console.WriteLine("원하시는 행동을 입력해주세요.");
            Console.WriteLine(">>");

            List<Item> itemList = inventory.itemlist();
            int acton = int.Parse(Console.ReadLine());
            bool isMain = true;
            do
            {
                isMain = false;
                if (acton == 0)
                {
                    Shop();
                }
                
                else if (acton == 1 || acton == 2 || acton == 3 || acton == 4)
                {
                    Item inputItem = itemList[acton - 1];
                    if (inputItem.isBuy)
                    {
                        Console.WriteLine("이미 구매한 상품입니다.");
                        Console.ReadLine();

                    }
                    else if (inputItem.saleGold <= player.Gold)
                    {
                        inputItem.isBuy = true;
                        player.Gold -= inputItem.saleGold;
                    }
                    else
                    {
                        Console.WriteLine("Gold가 부족합니다.");
                        Console.ReadLine();
                    }
                    ShopBuy();
                }

                else
                {
                    Console.WriteLine("잘못된 입력입니다.");
                    acton = int.Parse(Console.ReadLine());
                    isMain = true;
                }
            } while (isMain);
        }

       

    }

    class Character
    {
        public int Lv;
        public string Job;
        public string Name;
        public int Offense;
        public int Defence;
        public int Hp;
        public int Gold;

        public Item EquipedWeapon;
        public Item EquipedArmor;
        public bool isEquipedWeapon = false;
        public bool isEquipedArmor = false;

        public Character(int level, string job, string name, int offense, int defense, int hp, int gold)
        {
            Lv = level;
            Name = name;
            Job = job;
            Offense = offense;
            Defence = defense;
            Hp = hp;
            Gold = gold;
        }

        public void EquipWeapon(Item item)
        {
            if (!isEquipedWeapon)
            {
                EquipedWeapon = item;
                isEquipedWeapon = true;
            }
            else if (isEquipedWeapon)
            {
                EquipedWeapon = null;
                isEquipedWeapon = false;
            }
        }
        public void EquipArmor(Item item)
        {
            if(!isEquipedArmor)
            {
                EquipedArmor = item;
                isEquipedArmor = true;
            }
            else if (isEquipedArmor)
            {
                EquipedArmor = null;
                isEquipedArmor = false;
            }
        }

    }
    public class Item
    {
        public string Name;
        public int Offense;
        public int Defense;
        public string Desc;//description(변수명)
        public bool isEquiped;
        public bool isBuy;
        public int saleGold;

        public Item (string name, int offense, int defense, string desc, bool isequiped, bool isbuy, int salegold)
        {
            Name = name;
            Offense = offense;
            Defense = defense;
            Desc = desc;
            isEquiped = isequiped;
            isBuy = isbuy;
            saleGold = salegold;
        }
    }
    public class Inventory
    {
        public List<Item> itemList;
        public Inventory()
        {
            itemList = new List<Item>();
        }

        public void Add(Item item)
        {
            itemList.Add(item);
        }

        
        


        public void isEquipedInventory()
        {
            foreach (Item item in itemList.Where(item => item.isBuy))
            {
                Console.Write("- ");

                if (item.isEquiped) 
                { 
                    Console.Write("[E]");
                }

                Console.Write($"{item.Name} | ");

                if (item.Offense > 0) 
                { 
                    Console.Write($"공격력 +{item.Offense} "); 
                }
                if (item.Defense > 0) 
                { 
                    Console.Write($"방어력 +{item.Defense} "); 
                }
                Console.WriteLine($" | {item.Desc}");
            }
        }

        public List<Item> itemlist()
        {
            return itemList;
        }

        public void DisplayShop()
        {
            foreach (Item item in itemList)
            {
                Console.Write("- ");

                Console.Write($"{item.Name} | ");

                if (item.Offense > 0)
                {
                    Console.Write($"공격력 +{item.Offense} ");
                }
                if (item.Defense > 0)
                {
                    Console.Write($"방어력 +{item.Defense} ");
                }
                Console.WriteLine($" | {item.Desc}");
                
                //이미 구매 했을때
                if (item.isBuy) 
                { 
                    Console.WriteLine($" | 구매완료"); 
                }
                else 
                { 
                    Console.WriteLine($" | {item.saleGold} G"); 
                }
            }
        }
    }
}
