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
            player = new Character(1, "", "", 0, 0, 0, 0);        //Charater 객체 생성
            player.SelectName(player);                            //플레이어 이름 설정
            player.SelectJob(player);                             //플레이어 직업 선택                        

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
        
        //min max까지 인풋 입력 가능 및 매개변수로 받은 인풋이 숫자인지 판단하는 메서드
        public static int CheckInput(int min, int max) 
        {
            static void WrongInput()
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("잘못된 입력입니다!");
                Console.ResetColor();
            }
            var top = Console.CursorTop;

            while (true)
            {
                wrongInputRemover(top);
                var inputParse = -1;
                if (!int.TryParse(Console.ReadLine(), out inputParse)) //TryParse를 이용해 해결하는 방법으로 변환
                    inputParse = -1;
                if (inputParse >= min && inputParse <= max)
                    return inputParse;
                else
                    WrongInput();
            }
            static void wrongInputRemover(int top)
            {
                Console.SetCursorPosition(0, top);
                Console.WriteLine("                                     ");
                Console.SetCursorPosition(0, top);
            }
        }
        static void Start()
        {
            Console.WriteLine();
            Console.WriteLine("스파르타 마을에 오신 여러분 환영합니다.");
            Console.WriteLine("이곳에서 던전으로 들어가기전 활동을 할 수 있습니다.");
            Console.WriteLine("1. 상태 보기");
            Console.WriteLine("2. 인벤토리");
            Console.WriteLine("3. 상점");
            Console.WriteLine("원하시는 행동을 입력해주세요.");
            Console.WriteLine(">>");
            int acton = CheckInput(1, 3);
            while(true)
            {
                if (acton == 1)
                {
                    State();
                    break;
                }
                else if (acton == 2)
                {
                    Inventory();
                    break;
                }
                else if (acton == 3)
                {
                    Shop();
                    break;
                }
            } 
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
                else if (item.isEquiped)                        // if 문으로 되어있어서 스탯창만 열어도 아이템의 능력치가 추가되는 현상 수정
                {
                    if (item.Offense != 0)
                    {
                        player.Offense += item.Offense;
                    }
                    if (item.Defense != 0)
                    {
                        player.Defence += item.Defense;
                    }
                    break;
                }
            }
        }
        static void State()
        {
            //StateUpdate();
            Console.Clear();
            Console.WriteLine("상태 보기");
            Console.WriteLine("캐릭터의 정보가 표시됩니다.");
            Console.WriteLine($"Lv. {player.Lv}");
            Console.WriteLine($"{player.Name} ( {player.Job} )");        //"Chad > {player.Name}
            Console.WriteLine($"공격력 : {player.Offense}");
            Console.WriteLine($"방어력 : {player.Defence}");
            Console.WriteLine($"체력 : {player.Hp}");
            Console.WriteLine($"Gold : {player.Gold}");
            Console.WriteLine();
            Console.WriteLine("0. 나가기");
            Console.WriteLine();
            Console.WriteLine("원하시는 행동을 입력해주세요.");
            Console.WriteLine(">>");


            int acton = CheckInput(0, 0);
            while(true)
            {
                if (acton == 0)
                {
                    Start();
                    break;
                }
            }
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
            int acton = CheckInput(0, 1);
            while(true)
            {
                if (acton == 0)
                {
                    Start();
                    break;
                }
                else if (acton == 1)
                {
                    isEquipedInventory();
                    break;
                }
            }
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
            int acton = CheckInput(0, 4);
            while(true)
            {
                if (acton == 0)
                {
                    Inventory();
                    break;
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
                }
                isEquipedInventory();
            }
        }

        static void ShopInterface()
        {
            Console.WriteLine("상점");
            Console.WriteLine("필요한 아이템을 얻을 수 있는 상점입니다.");

            Console.WriteLine();
            Console.WriteLine("[보유 골드]");
            Console.WriteLine($"{player.Gold} G");

            Console.WriteLine();
            Console.WriteLine("[모든 아이템 목록]");

            inventory.DisplayShop();
        }
        static void Shop()
        {
            Console.Clear();
            ShopInterface();

            Console.WriteLine();
            Console.WriteLine("0. 나가기");
            Console.WriteLine("1. 아이템 구매");
            Console.WriteLine("2. 아이템 판매");
            Console.WriteLine();
            Console.WriteLine("원하시는 행동을 입력해주세요.");
            Console.WriteLine(">>");

            int acton = CheckInput(0, 2);
            while(true)
            {

                if (acton == 0)
                {
                    Start();
                    break;
                }
                else if (acton == 1)
                {
                    ShopBuy();
                    break;
                }
                else if (acton == 2)
                {
                    ShopSell();
                    break;
                }
            }
        }

        static void ShopBuy()
        {
            Console.Clear();
            ShopInterface();

            Console.WriteLine();
            Console.WriteLine("0. 나가기");
            Console.WriteLine();
            Console.WriteLine("구매를 원하시는 아이템의 번호를 입력해주세요.");
            Console.WriteLine(">>");

            List<Item> itemList = inventory.itemlist();
            int acton = CheckInput(0, 4);
            while(true)
            {
                if (acton == 0)
                {
                    Shop();
                    break;
                }
                
                else if (acton == 1 || acton == 2 || acton == 3 || acton == 4)
                {
                    Item inputItem = itemList[acton - 1];
                    if (inputItem.isBuy)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("이미 구매한 상품 입니다.");
                        Console.ResetColor();
                        Console.WriteLine("확인을 위해 아무 키나 눌러주세요");
                        Console.ReadLine();
                    }
                    else if (inputItem.saleGold <= player.Gold)
                    {
                        inputItem.isBuy = true;
                        player.Gold -= inputItem.saleGold;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Gold가 부족합니다.");
                        Console.ResetColor();
                        Console.WriteLine("확인을 위해 아무 키나 눌러주세요");
                        Console.ReadLine();
                    }
                    ShopBuy();
                }
            }
        }
        static void ShopSell()
        {
            Console.Clear();
            ShopInterface();

            Console.WriteLine();
            Console.WriteLine("0. 나가기");
            Console.WriteLine();
            Console.WriteLine("판매를 원하시는 아이템의 번호를 입력해주세요.");
            Console.WriteLine(">>");

            List<Item> itemList = inventory.itemlist();
            int acton = CheckInput(0, 4);
            while(true)
            {
                if (acton == 0)
                {
                    Shop();
                    break;
                }

                else if (acton == 1 || acton == 2 || acton == 3 || acton == 4)
                {
                    Item inputItem = itemList[acton - 1];
                    if (inputItem.isBuy)
                    {
                        inputItem.isBuy = false;
                        player.Gold += inputItem.saleGold;
                    }
                }
                ShopSell();
            }
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

        public static int CheckInput(int min, int max)
        {
            static void WrongInput()
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("잘못된 입력입니다!");
                Console.ResetColor();
            }
            var top = Console.CursorTop;

            while (true)
            {
                wrongInputRemover(top);
                var inputParse = -1;
                if (!int.TryParse(Console.ReadLine(), out inputParse)) //TryParse를 이용해 해결하는 방법으로 변환
                    inputParse = -1;
                if (inputParse >= min && inputParse <= max)
                    return inputParse;
                else
                    WrongInput();
            }
            static void wrongInputRemover(int top)
            {
                Console.SetCursorPosition(0, top);
                Console.WriteLine("                                     ");
                Console.SetCursorPosition(0, top);
            }
        }

        public void SelectName(Character character)     // 이름 선택 화면
        {
            bool isCorrectName = false;
            int selectNum = 0;

            while (isCorrectName == false)
            {
                Console.WriteLine("플레이어의 이름을 입력해주세요.");
                string playerName = Console.ReadLine();
                isCorrectName = true;
                Console.WriteLine($"선택하신 이름은 {playerName}입니다.");
                Console.WriteLine("1. 네  2. 아니오");
                int acton = CheckInput(1, 2);

                if (acton == 1)
                {
                    character.Name = playerName;
                    isCorrectName = true;
                }
                else if (acton == 2)
                {
                    isCorrectName = false;
                }

                //Console.WriteLine("플레이어의 이름을 입력해주세요.");
                //string playerName = Console.ReadLine();
                //isCorrectName = true;
                //Console.WriteLine($"선택하신 이름은 {playerName}입니다.");
                //Console.WriteLine("1. 네  2. 아니오");
                //selectNum = int.Parse(Console.ReadLine());

                //if(selectNum == 1)
                //{
                //    character.Name = playerName;
                //    isCorrectName = true;
                //}
                //else if(selectNum == 2)
                //{
                //    isCorrectName = false;
                //}
            }
        }

        public void SelectJob(Character character)         // 직업 선택화면
        {
            bool isCorrectNum = false;
            
            Console.WriteLine("직업을 선택해주세요.");
            Console.WriteLine("1. 전사  2. 도적 3. 궁수");

            while(isCorrectNum == false)
            {
                isCorrectNum = true;
                int acton = CheckInput(1, 3);

                switch (acton)
                {
                    case 1 :
                        Console.WriteLine("선택한 직업은 전사입니다.");
                        character.Lv = 1;
                        character.Job = "전사";
                        character.Offense = 10;
                        character.Defence = 10;
                        character.Hp = 200;
                        character.Gold = 1500;
                        break;
                    case 2 :
                        Console.WriteLine("선택한 직업은 도적입니다.");
                        character.Lv = 1;
                        character.Job = "도적";
                        character.Offense = 20;
                        character.Defence = 5;
                        character.Hp = 100;
                        character.Gold = 1500;
                        break;
                    case 3 :
                        Console.WriteLine("선택한 직업은 궁수입니다.");
                        character.Lv = 1;
                        character.Job = "궁수";
                        character.Offense = 15;
                        character.Defence = 5;
                        character.Hp = 150;
                        character.Gold = 1500;
                        break;
                }
            }


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
