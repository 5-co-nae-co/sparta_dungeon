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

            //무기
            Item Keyboard = new Item("키보드", 5, 0, "분노로 휘두르면 생각보다 강한 타격을 줍니다만 무기는 아닙니다.", false, false, 100,0);
            inventory.Add(Keyboard);
            Item OldSword = new Item("낡은 검", 10, 0, "쉽게 볼 수 있는 낡은 검 입니다", false, false, 400,0);
            inventory.Add(OldSword);
            Item BronzeAxe = new Item("청동 도끼", 15, 0, "어디선가 사용됐던거 같은 도끼입니다.", false, false, 600, 0);
            inventory.Add(BronzeAxe);
            Item SpartaSpear = new Item("스파르타 창", 30, 0, "스파르타 전사들이 사용했다는 전설의 창입니다.", false, false, 3000, 0);
            inventory.Add(SpartaSpear);

            //방어구
            Item BeggarsCloth = new Item("거렁뱅이의 옷", 0, 5, "촌장의 마음보다 넓은 구멍이 뚫려있는 더러운 옷입니다.", false, false, 100, 0);
            inventory.Add(BeggarsCloth);
            Item TrainingArmor = new Item("수련자갑옷", 0, 10, "수련에 도움을 주는 헐렁한 갑옷입니다.", false, false, 400, 0);
            inventory.Add(TrainingArmor);
            Item IronArmor = new Item("무쇠 갑옷", 0, 15, "무쇠로 만들어져 튼튼한 갑옷입니다.", false, false, 600, 0);
            inventory.Add(IronArmor);
            Item SpartaArmor = new Item("스파르타 갑옷", 0, 30, "스파르타 전사들이 입던 갑옷입니다.", false, false, 3000, 0);
            inventory.Add(SpartaArmor);
        }

        //텍스트 컬러 변경 메소드
        public static void ColorText(string text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.Write(text);
            Console.ResetColor();
        }
        //천천히 프린트 되는 텍스트 메소드
        public static void SlowText(string msg, int delay)
        {
            foreach (char c in msg.ToCharArray())
            {
                Console.Write(c);
                Thread.Sleep(delay);
            }
            Console.WriteLine();
        }


        public static void Main()
        {
            Console.SetWindowSize(105, 43);//시작 화면 크기 고정 코드
            settings();
            //Intro(); //Note 테스트 할 때에는 오래걸리니 주석화 한 뒤 진행하세요.
            Start();
        }

        public static void CalcAddedStat() //추가 아이템 스텟을 체크하는 메서드
        {
            player.AddedOffense = 0;
            player.AddedDefence = 0;

            foreach (var item in inventory.inventoryList)
            {
                if (item.isEquiped)
                {
                    player.AddedOffense += item.Offense;
                    player.AddedDefence += item.Defense;
                }
            }
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
            string title = @"+================================================================+ 
||  _____             _          ___                            ||
|| |   __|___ ___ ___| |_ ___   |    ＼ _ _ ___ ___ ___ ___ ___ ||
|| |__   | . | .'|  _|  _| .'|  |  |  | | |   | . | -_| . |   | ||
|| |_____|  _|__,|_| |_| |__,|  |____/|___|_|_|_  |___|___|_|_| ||
||       |_|                                  |___|             ||
||                                                              ||
+================================================================+ ";
            Console.Clear(); // 캐릭터 크리에이션 글 남는 것 지우기
            Console.WriteLine();
            ColorText(title, ConsoleColor.Yellow);
            Console.WriteLine();
            Console.WriteLine();
            ColorText("스파르타 던전에 오신 여러분 환영합니다.", ConsoleColor.Cyan);
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("이곳에서 던전으로 들어가기전 활동을 할 수 있습니다.");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine();
            Console.WriteLine("1. 상태 보기");
            Console.WriteLine("2. 인벤토리");
            Console.WriteLine("3. 상점");
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine("-----------------------------------------------------------------------");
            Console.WriteLine("원하시는 행동을 입력해주세요.");
            Console.WriteLine(">>");
            int acton = CheckInput(1, 3);
            while (true)
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
                    Console.Clear();
                    Console.WriteLine();
                    Console.WriteLine("당신은 상점에 도착하셨습니다.");
                    Console.WriteLine();
                    Program.ColorText("문영오 매니저(상점주인): ", ConsoleColor.DarkCyan);
                    Program.SlowText("어서오시게나 젊은 르탄이. 무엇을 사고 싶으신가? 팔 것이라도?", 30);
                    Thread.Sleep(800);
                    Console.WriteLine();
                    Shop();
                    break;
                }
            }
        }
        
        static void State()
        {
            Console.Clear();
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("+===========+");
            Console.WriteLine("||상태 보기||");
            Console.WriteLine("+===========+");
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine("캐릭터의 정보가 표시됩니다.");
            Console.WriteLine();
            ColorText($"Lv. {player.Lv} ", ConsoleColor.Red);
            ColorText($"{player.Name} ( {player.Job} )", ConsoleColor.Yellow);
            Console.WriteLine();
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"공격력 : {player.Offense + player.AddedOffense}" + (player.AddedOffense == 0 ? "" : ($" (+{player.AddedOffense})")));
            Console.WriteLine($"방어력 : {player.Defence + player.AddedDefence}" + (player.AddedDefence == 0 ? "" : ($" (+{player.AddedDefence})")));
            Console.ResetColor();
            ColorText($"체력 : {player.Hp}", ConsoleColor.DarkGreen);
            Console.WriteLine();
            Console.WriteLine();
            ColorText($"Gold : {player.Gold}", ConsoleColor.DarkYellow);
            Console.WriteLine();
            Console.WriteLine();
            ColorText("0. 나가기", ConsoleColor.Yellow);
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("-----------------------------------------------------------------------");
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
            string inventorytitle = @"  ___                      _                   
 |_ _|_ ____   _____ _ __ | |_ ___  _ __ _   _ 
  | || '_ \ \ / / _ | '_ \| __/ _ \| '__| | | |
  | || | | \ V |  __| | | | || (_) | |  | |_| |
 |___|_| |_|\_/ \___|_| |_|\__\___/|_|   \__, |
                                         |___/ ";
            Console.Clear();
            ColorText(inventorytitle, ConsoleColor.Yellow);
            Console.WriteLine();
            Console.WriteLine("보유 중인 아이템을 관리할 수 있습니다.");
            Console.WriteLine();
            ColorText("[아이템 목록]", ConsoleColor.Cyan);
            Console.WriteLine();
            Console.WriteLine();

            inventory.isEquipedInventory();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine();
            Console.WriteLine("0. 나가기");
            Console.WriteLine("1. 장착 관리");
            Console.WriteLine();
            Console.ResetColor();
            Console.WriteLine("-----------------------------------------------------------------------");
            Console.WriteLine("원하시는 행동을 입력해주세요.");
            Console.WriteLine(">>");

            List<Item> itemList = inventory.inventoryList;
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

        public static void isEquipedInventory()
        {
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("+======================+");
            Console.WriteLine("||인벤토리 - 장착 관리||");
            Console.WriteLine("+======================+");
            Console.WriteLine();
            Console.ResetColor();

            Console.WriteLine("보유 중인 아이템을 관리할 수 있습니다.");
            Console.WriteLine();
            ColorText("[보유 중인 아이템 목록]", ConsoleColor.Cyan);
            Console.WriteLine();
            Console.WriteLine();

            inventory.isEquipedInventory();

            Console.WriteLine();
            ColorText("0. 나가기", ConsoleColor.Yellow);
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("-----------------------------------------------------------------------");
            Console.WriteLine("장착을 원하시는 아이템의 번호를 입력해주세요.");
            Console.WriteLine(">>");

            List<Item> itemList = inventory.inventoryList;
            int acton = CheckInput(0, inventory.inventoryList.Count);

            while (true)
            {
                if (acton == 0)
                {
                    Inventory();
                    break;
                }
                else if (acton <= itemList.Count)
                {
                    //NOTE 현재 공격력 방어력으로 구분되는 아이템 클래스가 이후 아이템 추가로
                    //공격력 방어력 두가지 스탯을 동시에 가진 아이템이 나오게 되면 문제가 생김
                    Item inputItem = itemList[acton - 1];
                    if(player.CanEquipItem(inputItem) == true)
                    {
                        if (inputItem.Offense > 0) //장착 대상이 무기일 때
                        {
                            if (player.EquipedWeapon == inputItem)
                            {
                                inputItem.isEquiped = false;
                            }
                            else if (player.isEquipedWeapon) //이미 장착 중인 무기가 있을 때 해제 후 장착
                            {
                                player.EquipedWeapon.isEquiped = false;
                                player.EquipedWeapon = null;
                                player.isEquipedWeapon = false;
                                inputItem.isEquiped = true;
                            }
                            else if (!player.isEquipedWeapon) //장착 중인 무기가 없을 때 장착
                            {
                                inputItem.isEquiped = true;
                            }
                            player.EquipWeapon(inputItem);
                        }
                        else if (inputItem.Defense > 0) //장착 대상이 방어구일 때
                        {
                            if (player.EquipedArmor == inputItem) //장착 중인 방어구 해제
                            {
                                inputItem.isEquiped = false;
                            }
                            else if (player.isEquipedArmor) //이미 장착 중인 방어구가 있을 때 해제 후 장착
                            {
                                player.EquipedArmor.isEquiped = false;
                                player.EquipedArmor = null;
                                player.isEquipedArmor = false;
                                inputItem.isEquiped = true;
                            }
                            else if (!player.isEquipedArmor) //장착 중인 방어구가 없을 때 장착
                            {
                                inputItem.isEquiped = true;
                            }
                            player.EquipArmor(inputItem);
                        }
                    }
                    else
                    {
                        Program.SlowText("다른 직업의 장비입니다", 30);
                        Thread.Sleep(800);
                    }
                }
                CalcAddedStat();
                isEquipedInventory();
            }
        }

        static void ShopInterface()
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("+======+");
            Console.WriteLine("||상점||");
            Console.WriteLine("+======+");
            Console.ResetColor();
            Console.WriteLine();

            Console.WriteLine("필요한 아이템을 얻을 수 있는 상점입니다.");
            Console.WriteLine();

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[보유 골드]");
            Console.WriteLine($"{player.Gold} G");
            Console.ResetColor();

            Console.WriteLine();
            Program.ColorText("[모든 아이템 목록]", ConsoleColor.Cyan);
            Console.WriteLine();
            Console.WriteLine();
        }
        static void Shop()
        {
            Console.Clear();
            ShopInterface();
            inventory.DisplayShop();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine();
            Console.WriteLine("0. 나가기");
            Console.WriteLine("1. 아이템 구매");
            Console.WriteLine("2. 아이템 판매");
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine("-----------------------------------------------------------------------");
            Console.WriteLine("원하시는 행동을 입력해주세요.");
            Console.WriteLine(">>"); ;

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
            inventory.DisplayShop();

            Console.WriteLine();
            ColorText("0. 나가기", ConsoleColor.Yellow);
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("-----------------------------------------------------------------------");
            Console.WriteLine("구매를 원하시는 아이템의 번호를 입력해주세요.");
            Console.WriteLine(">>");

            List<Item> itemList = inventory.shopItemList;
            int acton = CheckInput(0, itemList.Count);
            while (true)
            {
                if (acton == 0)
                {
                    Shop();
                    break;
                }

                else if (acton <= itemList.Count)
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
                        inventory.inventoryList.Add(inputItem);
                        player.Gold -= inputItem.saleGold;
                        inputItem.isBuy = true;
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
            inventory.SellingInventory(); ;

            Console.WriteLine();
            Console.WriteLine("0. 나가기");
            Console.WriteLine();
            Console.WriteLine("-----------------------------------------------------------------------");
            Console.WriteLine("판매를 원하시는 아이템의 번호를 입력해주세요.");
            Console.WriteLine(">>");

            List<Item> itemList = inventory.inventoryList;
            int acton = CheckInput(0, itemList.Count);
            while (true)
            {
                if (acton == 0)
                {
                    Shop();
                    break;
                }

                else if (acton <= itemList.Count)
                {
                    Item inputItem = itemList[acton - 1];
                    if (inputItem.isBuy)
                    {
                        if (inputItem == player.EquipedWeapon) //판매 전에 장비 상태 해체
                        {
                            player.isEquipedWeapon = false;
                            player.EquipedWeapon = null;
                        }
                        else if (inputItem == player.EquipedArmor)
                        {
                            player.isEquipedArmor = false;
                            player.EquipedArmor = null;
                        }
                        inputItem.isEquiped = false;
                        inputItem.isBuy = false;
                        inventory.inventoryList.Remove(inputItem);
                        player.Gold += (inputItem.saleGold * 85 / 100);
                    }
                }
                CalcAddedStat();
                ShopSell();
            }
        }

        public static void Intro()
        {
            Console.Clear();
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            SlowText("이곳은 내일배움캠프 왕국.", 50);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine();
            SlowText($"당신의 이름은 {player.Name}. 직업은 {player.Job}입니다.", 40);
            Console.WriteLine();
            SlowText("오랜 모험 끝에 스파르타 코딩 마을에 도착하셨습니다.", 50);
            SlowText("이곳은 많은 초보 코딩 모험가들이 ", 30);
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Red;
            Console.SetCursorPosition(2, Console.CursorTop);
            Console.WriteLine("==========");
            Console.SetCursorPosition(2, Console.CursorTop);
            SlowText("|| 회사 ||", 100);
            Console.SetCursorPosition(2, Console.CursorTop);
            Console.WriteLine("==========");
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Yellow;
            SlowText("라는 무시무시한 던전을 목표로 수련을 위해 거쳐가는 마을이기에", 30);
            SlowText("밝은 미래를 꿈꾸는 다양한 모험가들이 이곳에서 준비를 마치고 갑니다.", 30);
            Console.WriteLine();
            Thread.Sleep(400);
            Console.ForegroundColor = ConsoleColor.Cyan;
            SlowText("현 마을의 촌장은 절대적인 지지를 자랑하는 한효승(내배캠 매니저)씨 입니다.", 50);
            Console.WriteLine();
            Console.ResetColor();
            Console.WriteLine();
            Thread.Sleep(600);
            Console.Write("게임에 입장하는 중 입니다"); //로딩 흉내내기
            SlowText(" . . . . . ", 150);
        }
    }
    
    class Character
    {
        public int Lv;
        public string Job;
        public static int JobType;  // 1.전사 2. 도적 3.궁수
        public string Name;
        public int Offense;
        public int Defence;
        public int Hp;
        public int Gold;
        public int AddedOffense; //아이템 추가 공격력
        public int AddedDefence; //아이템 추가 방어력

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

        public bool CanEquipItem(Item item)   // 아이템이 착용 가능한지 확인
        {
            if (item.ItemType == 0 || item.ItemType == Character.JobType)
            {
                return true;
            }
            else
                return false;
        }

        public void SelectName(Character character)     // 이름 선택 화면
        {
            bool isCorrectName = false;
            int selectNum = 0;
            string CharacterTitle = @"   ____ _                          _                                  _   _             
  / ___| |__   __ _ _ __ __ _  ___| |_ ___ _ __    ___ _ __ ___  __ _| |_(_) ___  _ __  
 | |   | '_ \ / _` | '__/ _` |/ __| __/ _ | '__|  / __| '__/ _ \/ _` | __| |/ _ \| '_ \ 
 | |___| | | | (_| | | | (_| | (__| ||  __| |    | (__| | |  __| (_| | |_| | (_) | | | |
  \____|_| |_|\__,_|_|  \__,_|\___|\__\___|_|     \___|_|  \___|\__,_|\__|_|\___/|_| |_|
                                                                                        ";
            while (isCorrectName == false)
            {
                Console.Clear();
                Program.ColorText(CharacterTitle, ConsoleColor.Yellow);
                Console.WriteLine();
                Console.WriteLine("플레이어의 이름을 입력해주세요.");
                Console.WriteLine();
                string playerName = Console.ReadLine();
                isCorrectName = true;
                Console.WriteLine();
                Console.Write("선택하신 이름은 ");
                Program.ColorText(playerName, ConsoleColor.Cyan);
                Console.WriteLine(" 입니다.");
                Console.WriteLine();
                Program.ColorText("1. 네  ", ConsoleColor.Yellow);
                Program.ColorText("2. 아니오", ConsoleColor.DarkYellow);
                Console.WriteLine();
                Console.WriteLine();
                int acton = Program.CheckInput(1, 2);

                if (acton == 1)
                {
                    character.Name = playerName;
                    isCorrectName = true;
                }
                else if (acton == 2)
                {
                    isCorrectName = false;
                }
            }
        }

        public void SelectJob(Character character)         // 직업 선택화면
        {
            bool isCorrectNum = false;
            string ClassTitle = @"   ____ _                          _           _   _             
  / ___| | __ _ ___ ___   ___  ___| | ___  ___| |_(_) ___  _ __  
 | |   | |/ _` / __/ __| / __|/ _ | |/ _ \/ __| __| |/ _ \| '_ \ 
 | |___| | (_| \__ \__ \ \__ |  __| |  __| (__| |_| | (_) | | | |
  \____|_|\__,_|___|___/ |___/\___|_|\___|\___|\__|_|\___/|_| |_|";

            Console.Clear();
            Console.WriteLine();
            Program.ColorText(ClassTitle, ConsoleColor.Yellow);
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("직업을 선택해주세요.");
            Console.WriteLine();
            Program.ColorText("1. 전사  ", ConsoleColor.Cyan);
            Program.ColorText("2. 도적  ", ConsoleColor.DarkCyan);
            Program.ColorText("3. 궁수", ConsoleColor.Blue);
            Console.WriteLine();
            Console.WriteLine();

            while (isCorrectNum == false)
            {
                isCorrectNum = true;
                int acton = Program.CheckInput(1, 3);

                switch (acton)
                {
                    case 1:
                        Console.WriteLine("선택한 직업은 전사입니다.");
                        character.Lv = 1;
                        character.Job = "전사";
                        character.Offense = 10;
                        character.Defence = 10;
                        character.Hp = 200;
                        character.Gold = 1500;
                        Character.JobType = 1;
                        break;
                    case 2:
                        Console.WriteLine("선택한 직업은 도적입니다.");
                        character.Lv = 1;
                        character.Job = "도적";
                        character.Offense = 20;
                        character.Defence = 5;
                        character.Hp = 100;
                        character.Gold = 1500;
                        Character.JobType = 2;
                        break;
                    case 3:
                        Console.WriteLine("선택한 직업은 궁수입니다.");
                        character.Lv = 1;
                        character.Job = "궁수";
                        character.Offense = 15;
                        character.Defence = 5;
                        character.Hp = 150;
                        character.Gold = 1500;
                        Character.JobType = 3;
                        break;
                }
            }
        }

        public  void EquipWeapon(Item item)
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
                EquipedWeapon = item;
                isEquipedWeapon = true;
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
        public int ItemType; // 공용 아이템은 0, 전사는 1, 도적은 2, 궁수는 3

        public Item (string name, int offense, int defense, string desc, bool isequiped, bool isbuy, int salegold, int itemType)
        {
            Name = name;
            Offense = offense;
            Defense = defense;
            Desc = desc;
            isEquiped = isequiped;
            isBuy = isbuy;
            saleGold = salegold;
            ItemType = itemType;
        }
    }
    public class Inventory
    {
        public List<Item> inventoryList;
        public List<Item> shopItemList;

        public Inventory()
        {
            inventoryList = new List<Item>();
            shopItemList = new List<Item>();
        }

        public void Add(Item item)
        {
            shopItemList.Add(item);
        }

        public void isEquipedInventory()
        {
            int i = 0;

            foreach (Item item in inventoryList.Where(item => item.isBuy))
            {
                i++;
                Console.Write("- ");
                Console.Write(i);

                if (item.isEquiped)
                {
                    Program.ColorText(" [E]", ConsoleColor.Red);
                }

                Program.ColorText($" {item.Name}", ConsoleColor.Cyan);
                Console.Write(" | ");

                if (item.Offense > 0)
                {
                    Program.ColorText($"공격력 +{item.Offense} ", ConsoleColor.Green);
                }
                if (item.Defense > 0)
                {
                    Program.ColorText($"방어력 +{item.Defense} ", ConsoleColor.Green);
                }
                Console.Write(" | ");
                Program.ColorText($"{item.Desc}", ConsoleColor.White);
                Console.WriteLine();
            }
        }

        public void SellingInventory()
        {
            int i = 0;
            foreach (Item item in inventoryList.Where(item => item.isBuy))
            {
                i++;
                Console.Write("- ");
                Console.Write(i);

                if (item.isEquiped)
                {
                    Program.ColorText(" [E]", ConsoleColor.Red);
                }

                Program.ColorText($" {item.Name}", ConsoleColor.Cyan);
                Console.Write(" | ");

                if (item.Offense > 0)
                {
                    Program.ColorText($"공격력 +{item.Offense} ", ConsoleColor.DarkGreen);
                }
                if (item.Defense > 0)
                {
                    Program.ColorText($"방어력 +{item.Defense} ", ConsoleColor.DarkGreen);
                }
                Console.Write(" | ");
                Program.ColorText($"{item.Desc}", ConsoleColor.White);
                Console.WriteLine();
                Program.ColorText($" | {item.saleGold * 0.85} G", ConsoleColor.Green);
                Console.WriteLine();
            }
        }

        public void DisplayShop()
        {
            int i = 0;

            foreach (Item item in shopItemList)
            {
                i++;
                Console.WriteLine();
                Console.Write("- ");
                Console.Write(i + " ");
                Console.Write($"{item.Name} | ");

                if (item.Offense > 0)
                {
                    Console.Write($"공격력 +{item.Offense} ");
                }
                if (item.Defense > 0)
                {
                    Console.Write($"방어력 +{item.Defense} ");
                }
                Console.Write($" | {item.Desc} |");
                if (item.ItemType == 0)
                {
                    Console.Write(" 공용");
                }
                else if (item.ItemType == 1)
                {
                    Console.Write(" 전사");
                }
                else if (item.ItemType == 2)
                {
                    Console.Write(" 도적");
                }
                else if (item.ItemType == 3)
                {
                    Console.Write(" 궁수");
                }

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
