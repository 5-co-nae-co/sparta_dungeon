﻿using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.IO.Pipes;
using System.Net.Security;
using System.Numerics;
using System.Xml.Linq;
using static sparta_dungeon.Inventory;
using sparta_dungeon;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System;
using WMPLib;

namespace sparta_dungeon
{
    internal class Program
    {
        static Character player;
        static Inventory inventory = new Inventory();
        static Dungeon dungeon;
        public static Inventory GetInventory()
		{
			return inventory;
		}

        static WindowsMediaPlayer bgm = new WindowsMediaPlayer();
        static void settings()
        {
            player = new Character(1, "", "", 0, 0, 0, 0, 0);        //Charater 객체 생성
            player.SelectName(player);                            //플레이어 이름 설정
            player.SelectJob(player);                             //플레이어 직업 선택                        
            dungeon = new Dungeon(player);

            //소모품
            Item HealthPotion = new Item("치유 물약", -1, -1, "체력을 60 회복 시켜주는 물약 입니다.", false, false, 300, 0);
            inventory.Add(HealthPotion);

            //무기
            Item Keyboard = new Item("키보드", 5, 0, "분노로 휘두르면 생각보다 강한 타격을 줍니다만 무기는 아닙니다.", false, false, 100, 0);
            inventory.Add(Keyboard);
            Item OldSword = new Item("낡은 검", 10, 0, "쉽게 볼 수 있는 낡은 검 입니다", false, false, 400, 0);
            inventory.Add(OldSword);
            Item BronzeAxe = new Item("청동 도끼", 15, 0, "어디선가 사용됐던거 같은 도끼입니다.", false, false, 600, 0);
            inventory.Add(BronzeAxe);
            Item SpartaSpear = new Item("스파르타 창", 30, 0, "스파르타 전사들이 사용했다는 전설의 창입니다.", false, false, 3000, 1);
            inventory.Add(SpartaSpear);
            Item SilverFang = new Item("은빛 송곳니", 40, 0, "숙련된 도적들이 사용한다는 무기입니다.", false, false, 3000, 2);
            inventory.Add(SilverFang);
            Item ManaBow = new Item("마법의 활", 50, 0, "마법의 힘이 깃든 활입니다.", false, false, 3000, 3);
            inventory.Add(ManaBow);

            //방어구
            Item BeggarsCloth = new Item("거렁뱅이의 옷", 0, 5, "촌장의 마음보다 넓은 구멍이 뚫려있는 더러운 옷입니다.", false, false, 100, 0);
            inventory.Add(BeggarsCloth);
            Item TrainingArmor = new Item("수련자갑옷", 0, 10, "수련에 도움을 주는 헐렁한 갑옷입니다.", false, false, 400, 0);
            inventory.Add(TrainingArmor);
            Item IronArmor = new Item("무쇠 갑옷", 0, 15, "무쇠로 만들어져 튼튼한 갑옷입니다.", false, false, 600, 1);
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
            SoundPlay("shop");
            settings();
            Intro(); //Note 테스트 할 때에는 오래걸리니 주석화 한 뒤 진행하세요.
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
        //게임 불러오기 후 장비 장착 상태를 가져옴에도 불구하고 중첩 장착이 되는 버그 해결을 하고자 했는데 시간이 없어
        //장착 중인 장비 다시 체크해서 재장착 시키는 급조 방식으로 우선 해결했습니다.
        public static void FindEquippedItem() 
        {
            foreach (var item in inventory.inventoryList)
            {
                if (item.isEquiped && item.Offense > 0) //무기 재장착
                {
                    player.EquipedWeapon = item;
                    player.isEquipedWeapon = true;
                }
                else if (item.isEquiped && item.Defense > 0)
                {
                    player.EquipedArmor = item;
                    player.isEquipedArmor = true;
                }
            }
        }
        //min max까지 인풋 입력 가능 및 매개변수로 받은 인풋이 숫자인지 판단하는 메서드
        public static int CheckInput(int min, int max)
        {
            static void WrongInput()
            {
                ColorText("잘못된 입력입니다!", ConsoleColor.Red);
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
        public static void Start()
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
            Console.Write("\n\n");
            ColorText("스파르타 던전에 오신 여러분 환영합니다.\n\n", ConsoleColor.Cyan);
            Console.WriteLine("이곳에서 던전으로 들어가기전 활동을 할 수 있습니다.\n");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("1. 상태 보기");
            Console.WriteLine("2. 인벤토리");
            Console.WriteLine("3. 상점");
            Console.WriteLine("4. 던전 입장");
            Console.WriteLine("     ---     ");
            Console.WriteLine("5. 휴식하기 - 500G");
            Console.WriteLine("     ---     ");
            Console.WriteLine("6. 게임 저장");
            Console.WriteLine("7. 게임 불러오기");
            Console.ResetColor();
            Console.WriteLine("\n--------------------------------------------------------------------");
            Console.WriteLine("원하시는 행동을 입력해주세요.");
            Console.WriteLine(">>");
            int acton = CheckInput(1, 7);
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
                    string storeOwner = @"                              ,--- .
                             /    |
                            /     |
                           /       |
                          /        |
                     ___,'Sparta   |
                   <  -'           :
                    `-.__..--'``-,__\_
                       |o/ ` o,. )_`>
                       :/ `      ||/)
                       (_.).__ ,-` |\
                       /( `.``   ` | :
                       \'`-.)  `   ; ;
                       | `        /-<
                       |     `   /   `.
       ,-_-..____     /|  `     :__..-'\
      /,'-.__\\  ``-./ :`       ;       \
      `\ `\  `\\  \ :  (   `   /  ,   `. \
        \` \   \\   |  | `    :  :     .\ \
         \ `\_  ))  :  ;      |  |      ): :
        (`-.-'\ ||  |\ \   `  ;  ;       | |
         \-_   `;;._   ( `   /  /_       | |
          `-.-.// ,'`-._\__ /_,'         ; |
             \:: :     /     `      ,   /  |
              || |    (         ,' /   /   |
              ||                 ,'   /    |";
                    Console.Clear();
                    Console.WriteLine();
                    ColorText(storeOwner, ConsoleColor.Cyan);
                    Console.WriteLine("\n\n당신은 상점에 도착하셨습니다.\n");
                    Program.ColorText("문영오 매니저(상점주인): ", ConsoleColor.DarkCyan);
                    Program.SlowText("어서오시게나 젊은 르탄이. 무엇을 사고 싶으신가? 팔 것이라도?", 20);
                    Thread.Sleep(800);
                    Console.WriteLine();
                    Shop();
                    break;
                }
                else if (acton == 4)
                {
                    dungeon.StageSelect();
                    break;
                }
                else if (acton == 5)
                {
                    GetRest();
                    break;
                }
                else if (acton == 6)
                {
                    // Json
                    string _fileName = "playerInfo.json";
                    string _itemFileName = "itemList.json";
                    string _shopFileName = "shopInfo.json";
                    // 데이터 경로 저장. (C드라이브, Documents)
                    // Json 저장(파일로 저장)
                    string _userDocumentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    string _filePath = Path.Combine(_userDocumentsFolder, _fileName);
                    string _itemFilePath = Path.Combine(_userDocumentsFolder, _itemFileName);
                    string _shopFilePath = Path.Combine(_userDocumentsFolder, _shopFileName);
                    string _playerJson = JsonConvert.SerializeObject(player, Formatting.Indented);
                    string _itemJson = JsonConvert.SerializeObject(inventory.inventoryList, Formatting.Indented);
                    string _shopJson = JsonConvert.SerializeObject(inventory.shopItemList, Formatting.Indented);
                    File.WriteAllText(_filePath, _playerJson);
                    File.WriteAllText(_itemFilePath, _itemJson);
                    File.WriteAllText(_shopFilePath, _shopJson);
                    Console.ForegroundColor = ConsoleColor.Red;
                    SlowText("저장중 입니다...", 50);
                    Console.ResetColor();
                    Thread.Sleep(1000);
                    Start();
                    break;
                }
                else if (acton == 7)
                {
                    LoadGameData();
                    Start();
                    break;
                }
            }
        }
        static void LoadGameData()
        {
            //Json
            string _playerFileName = "playerInfo.json";
            string _itemFileName = "itemList.json";
            string _shopFileName = "shopInfo.json";
            // 데이터 경로 불러오기. (C드라이브, Documents)
            string _userDocumentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            // 플레이어 데이터 로드
            string _playerFilePath = Path.Combine(_userDocumentsFolder, _playerFileName);
            if (File.Exists(_playerFilePath))
            {
                string _playerJson = File.ReadAllText(_playerFilePath);
                player = JsonConvert.DeserializeObject<Character>(_playerJson);
                Console.ForegroundColor = ConsoleColor.Red;
                SlowText($"플레이어({player.Name}) 데이터를 불러왔습니다.", 50);
                Thread.Sleep(300);
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                SlowText("저장된 플레이어 데이터가 없습니다.", 50);
                Thread.Sleep(300);
                Console.ResetColor();
            }

            // 아이템 데이터 로드
            string _itemFilePath = Path.Combine(_userDocumentsFolder, _itemFileName);
            string _shopFilePath = Path.Combine(_userDocumentsFolder, _shopFileName);
            if (File.Exists(_itemFilePath) && File.Exists(_shopFilePath))
            {
                string _shopJson = File.ReadAllText(_shopFilePath);
                string _itemJson = File.ReadAllText(_itemFilePath);
                inventory.shopItemList = JsonConvert.DeserializeObject<List<Item>>(_shopJson);
                inventory.inventoryList = JsonConvert.DeserializeObject<List<Item>>(_itemJson);
                Console.ForegroundColor = ConsoleColor.Red;
                SlowText("아이템 데이터를 불러왔습니다.", 50);
                Thread.Sleep(300);
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                SlowText("저장된 아이템 데이터가 없습니다.", 50);
                Thread.Sleep(300);
                Console.ResetColor();
            }
            dungeon = new Dungeon(player);
            FindEquippedItem();
        }
        static void GetRest()
        {
            string innArt = @"


                  _      ()              ()      _
                 / \     ||______________||     / \
                /___\    |    스파르타    |    /___\
                  |      |      여관      |      |
                 (_)     |_______  _______|     (_)
              ___/_\___  {_______}{_______}  ___/_\___
               |__~__|   %%%%%%%%%%%%%%%%%%   |__~__|
            ___|_____|__%%%%%%%%%%%%%%%%%%%%__|_____|___
               |     | %%%%%%%%%%%%%%%%%%%%%% |     |
                `=====%%%%%%%%%%%%%%%%%%%%%%%%=====`
               `=====%%%%%%%%%%%%%%%%%%%%%%%%%%=====`
              `=====%%%%%%%%%%%%%%%%%%%%%%%%%%%%=====`
             `=====/||||||||||||||||||||||||||||\=====`
            `======||||||||||||||||||||||||||||||======`
           `=======|||||||||||||||||||||||||||lc|=======`
          `==============================================`
         `================================================`
        `==================================================`
       `====================================================`
";
            if (player.Gold >= 500)
            {
                Console.Clear();
                Console.WriteLine(innArt);
                Console.ForegroundColor = ConsoleColor.Yellow;
                SlowText("\n\n        당신은 여관에서 휴식을 취합니다 . . . . . . . .", 80);
                Console.ResetColor();
                
                if (player.Job == "전사")
                {
                    player.Hp = 200;
                    player.Mp = 100;
                }
                else if (player.Job == "도적")
                {
                    player.Hp = 100;
                    player.Mp = 100;
                }
                else if (player.Job == "궁수")
                {
                    player.Hp = 150;
                    player.Mp = 100;
                }
                player.Gold -= 500;
                Start();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                SlowText("\n휴식하기에 돈이 부족합니다!\n", 50);
                Thread.Sleep(600);
                Console.ResetColor();
                Start();
            }
        }

        static void State()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("\n+===========+");
            Console.WriteLine("||상태 보기||");
            Console.WriteLine("+===========+");
            Console.ResetColor();
            Console.WriteLine("\n캐릭터의 정보가 표시됩니다.\n");
            ColorText($"Lv. {player.Lv} ", ConsoleColor.Red);
            ColorText($"{player.Name} ( {player.Job} )\n\n", ConsoleColor.Yellow);        //"Chad > {player.Name}
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"공격력 : {player.Offense + player.AddedOffense}" + (player.AddedOffense == 0 ? "" : ($" (+{player.AddedOffense})")));
            Console.WriteLine($"방어력 : {player.Defence + player.AddedDefence}" + (player.AddedDefence == 0 ? "" : ($" (+{player.AddedDefence})")));
            Console.ResetColor();
            ColorText($"체력 : {player.Hp}\n\n", ConsoleColor.DarkGreen);
            ColorText($"Gold : {player.Gold}\n\n", ConsoleColor.DarkYellow);
            Console.WriteLine($"경험치 : {player.Exp} / {player.ExpToLevelUp}\n");
            ColorText("0. 나가기\n\n", ConsoleColor.Yellow);
            Console.WriteLine("-----------------------------------------------------------------------");
            Console.WriteLine("원하시는 행동을 입력해주세요.");
            Console.WriteLine(">>");

            int acton = CheckInput(0, 0);
            while (true)
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
            Console.WriteLine("\n보유 중인 아이템을 관리할 수 있습니다.\n");
            ColorText("[아이템 목록]\n\n", ConsoleColor.Cyan);

            inventory.isEquipedInventory();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n0. 나가기");
            Console.WriteLine("1. 장착 관리\n");
            Console.ResetColor();
            Console.WriteLine("-----------------------------------------------------------------------");
            Console.WriteLine("원하시는 행동을 입력해주세요.");
            Console.WriteLine(">>");

            List<Item> itemList = inventory.inventoryList;
            int acton = CheckInput(0, 1);
            while (true)
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
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("\n\n+======================+");
            Console.WriteLine("||인벤토리 - 장착 관리||");
            Console.WriteLine("+======================+\n");
            Console.ResetColor();

            Console.WriteLine("보유 중인 아이템을 관리할 수 있습니다.\n");
            ColorText("[보유 중인 아이템 목록]\n\n", ConsoleColor.Cyan);

            inventory.isEquipedInventory();

            ColorText("\n0. 나가기\n", ConsoleColor.Yellow);
            Console.WriteLine("\n-----------------------------------------------------------------------");
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
                    if (player.CanEquipItem(inputItem) == true)
                    {
                        if (inputItem.Offense < 0) // 소모품 사용 (소모품들은 각자 - 스탯 값을 가지고 있어 이를 체크)
                        {
                            player.UseConsumable(inputItem);
                            inventory.inventoryList.Remove(inputItem);
                        }
                        else if (inputItem.Offense > 0) //장착 대상이 무기일 때
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
            Console.WriteLine("+======+\n");
            Console.ResetColor();

            Console.WriteLine("필요한 아이템을 얻을 수 있는 상점입니다.\n");

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n[보유 골드]");
            Console.WriteLine($"{player.Gold} G\n");
            Console.ResetColor();

            Program.ColorText("[모든 아이템 목록]\n\n", ConsoleColor.Cyan);
        }
        static void Shop()
        {
            Console.Clear();
            ShopInterface();
            inventory.DisplayShop();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n0. 나가기");
            Console.WriteLine("1. 아이템 구매");
            Console.WriteLine("2. 아이템 판매\n");
            Console.ResetColor();
            Console.WriteLine("-----------------------------------------------------------------------");
            Console.WriteLine("원하시는 행동을 입력해주세요.");
            Console.WriteLine(">>");

            int acton = CheckInput(0, 2);
            while (true)
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

            ColorText("\n0. 나가기\n", ConsoleColor.Yellow);
            Console.WriteLine("\n-----------------------------------------------------------------------");
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
                        Thread.Sleep(700);
                    }
                    else if (inputItem.saleGold <= player.Gold)
                    {
                        inventory.inventoryList.Add(inputItem);
                        player.Gold -= inputItem.saleGold;
                        if (inputItem.Offense >= 0)
                            inputItem.isBuy = true;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Gold가 부족합니다.");
                        Console.ResetColor();
                        Thread.Sleep(700);
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

            Console.WriteLine("\n0. 나가기\n");
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
                    if (inputItem.Offense < 0)
                    {
                        inventory.inventoryList.Remove(inputItem);
                        player.Gold += (inputItem.saleGold * 85 / 100);
                    }
                    else
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
            string introArt = @"                                                           |>>>
                   _                      _                |
    ____________ .' '.    _____/----/-\ .' './========\   / \
   //// ////// /V_.-._\  |.-.-.|===| _ |-----| u    u |  /___\
  // /// // ///==\ u |.  || | ||===||||| |T| |   ||   | .| u |_ _ _ _ _ _
 ///////-\////====\==|:::::::::::::::::::::::::::::::::::|u u| U U U U U
 |----/\u |--|++++|..|'''''''''''::::::::::::::''''''''''|+++|+-+-+-+-+-+
 |u u|u | |u ||||||..|              '::::::::'           |===|>=== _ _ ==
 |===|  |u|==|++++|==|              .::::::::.           | T |....| V |..
 |u u|u | |u ||HH||                .::::::::::.
 |===|_.|u|_.|+HH+|_              .::::::::::::.              _
                __(_)___         .::::::::::::::.         ___(_)__
---------------/  / \  /|       .:::::;;;:::;;:::.       |\  / \  \-------
______________/_______/ |      .::::::;;:::::;;:::.      | \_______\________
|       |     [===  =] /|     .:::::;;;::::::;;;:::.     |\ [==  = ]   |
|_______|_____[ = == ]/ |    .:::::;;;:::::::;;;::::.    | \[ ===  ]___|____
     |       |[  === ] /|   .:::::;;;::::::::;;;:::::.   |\ [=  ===] |
_____|_______|[== = =]/ |  .:::::;;;::::::::::;;;:::::.  | \[ ==  =]_|______
 |       |    [ == = ] /| .::::::;;:::::::::::;;;::::::. |\ [== == ]      |
_|_______|____[=  == ]/ |.::::::;;:::::::::::::;;;::::::.| \[  === ]______|_
   |       |  [ === =] /.::::::;;::::::::::::::;;;:::::::.\ [===  =]   |
___|_______|__[ == ==]/.::::::;;;:::::::::::::::;;;:::::::.\[=  == ]___|_____";

            Console.Clear();
            Console.WriteLine();
            Console.WriteLine(introArt);
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.SetCursorPosition(28, Console.CursorTop);
            SlowText("이곳은 내일배움캠프 왕국.\n", 50);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.SetCursorPosition(25, Console.CursorTop);
            SlowText($"당신의 이름은 {player.Name}. 직업은 {player.Job}입니다.\n", 40);
            Console.SetCursorPosition(18, Console.CursorTop);
            SlowText("오랜 모험 끝에 스파르타 코딩 마을에 도착하셨습니다.", 50);
            Console.SetCursorPosition(17, Console.CursorTop);
            SlowText("이곳은 많은 초보 코딩 모험가들이\n", 30);

            Console.ForegroundColor = ConsoleColor.Red;
            Console.SetCursorPosition(35, Console.CursorTop);
            Console.WriteLine("==========");
            Console.SetCursorPosition(35, Console.CursorTop);
            SlowText("|| 회사 ||", 100);
            Console.SetCursorPosition(35, Console.CursorTop);
            Console.WriteLine("==========\n");

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.SetCursorPosition(10, Console.CursorTop);
            SlowText("라는 무시무시한 던전을 목표로 수련을 위해 거쳐가는 마을이기에", 30);
            Console.SetCursorPosition(9, Console.CursorTop);
            SlowText("밝은 미래를 꿈꾸는 다양한 모험가들이 이곳에서 준비를 마치고 갑니다.\n", 30);
            Thread.Sleep(400);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.SetCursorPosition(4, Console.CursorTop);
            SlowText("현 마을의 촌장은 절대적인 지지를 자랑하는 한효승(내배캠 매니저)씨 입니다.\n\n", 50);
            Console.ResetColor();
            Thread.Sleep(600);
            Console.SetCursorPosition(6, Console.CursorTop);
            Console.Write("게임에 입장하는 중 입니다"); //로딩 흉내내기
            SlowText(" . . . . . ", 150);
        }

        public static void SoundPlay(string name)
        {
            bgm.controls.stop();//사운드 재생 전 기존 사운드 stop
            bgm.URL = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + $"\\sound\\{name}.mp3";
            bgm.controls.play();
            bgm.settings.playCount = 20;//반복재생
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
        public int Mp;
        public int Gold;
        public int AddedOffense; //아이템 추가 공격력
        public int AddedDefence; //아이템 추가 방어력
        public int Exp {  get; set; }
        public int ExpToLevelUp {  get; set; }

        public Item EquipedWeapon;
        public Item EquipedArmor;
        public bool isEquipedWeapon = false;
        public bool isEquipedArmor = false;

        public Character(int level, string job, string name, int offense, int defense, int hp, int mp, int gold)
        {
            Lv = level;
            Name = name;
            Job = job;
            Offense = offense;
            Defence = defense;
            Hp = hp;
            Mp = mp;
            Gold = gold;
            Exp = 0;
            ExpToLevelUp = 10;
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
                Console.WriteLine("\n플레이어의 이름을 입력해주세요.\n");
                string playerName = Console.ReadLine();
                isCorrectName = true;
                Console.Write("\n선택하신 이름은 ");
                Program.ColorText(playerName, ConsoleColor.Cyan);
                Console.WriteLine(" 입니다.\n");
                Program.ColorText("1. 네  ", ConsoleColor.Yellow);
                Program.ColorText("2. 아니오\n\n", ConsoleColor.DarkYellow);

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
            Console.WriteLine("\n\n직업을 선택해주세요.\n");
            Program.ColorText("1. 전사  ", ConsoleColor.Cyan);
            Program.ColorText("2. 도적  ", ConsoleColor.DarkCyan);
            Program.ColorText("3. 궁수\n\n", ConsoleColor.Blue);

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
                        character.Mp = 100;
                        character.Gold = 1500;
                        Character.JobType = 1;
                        break;
                    case 2:
                        Console.WriteLine("선택한 직업은 도적입니다.");
                        character.Lv = 1;
                        character.Job = "도적";
                        character.Offense = 15;
                        character.Defence = 5;
                        character.Hp = 150;
                        character.Mp = 100;
                        character.Gold = 1500;
                        Character.JobType = 2;
                        break;
                    case 3:
                        Console.WriteLine("선택한 직업은 궁수입니다.");
                        character.Lv = 1;
                        character.Job = "궁수";
                        character.Offense = 20;
                        character.Defence = 5;
                        character.Hp = 100;
                        character.Mp = 100;
                        character.Gold = 1500;
                        Character.JobType = 3;
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
            if (!isEquipedArmor)
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
        public void UseConsumable(Item item)
        {
            if (item.Name == "치유 물약")
            {
                void UseWarning()
                {
                    Program.ColorText("치유 물약은 최대 체력 이상으로 회복 되지 않습니다.\n", ConsoleColor.Red);
                    Thread.Sleep(800);
                }
                Hp += 60;
                if (Job == "전사" && Hp > 200)
                {
                    Hp = 200;
                    UseWarning();
                }
                else if (Job == "도적" && Hp > 100)
                {
                    Hp = 100;
                    UseWarning();
                }
                else if (Job == "궁수" && Hp > 150)
                {
                    Hp = 150;
                    UseWarning();
                }
                else
                {
                    Program.ColorText("60의 체력을 회복 했습니다.\n", ConsoleColor.Red);
                    Thread.Sleep(800);
                }
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

        public Item(string name, int offense, int defense, string desc, bool isequiped, bool isbuy, int salegold, int itemType)
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
            foreach (Item item in inventoryList)
            {
                i++;
                Console.Write("- ");
                Console.Write(i);

                if (item.isEquiped)
                {
                    Program.ColorText(" [E]", ConsoleColor.Red);
                }

                Program.ColorText($" {item.Name}", ConsoleColor.Cyan);

                if (item.Offense > 0)
                {
                    Console.Write(" | ");
                    Program.ColorText($"공격력 +{item.Offense} ", ConsoleColor.Green);
                }
                if (item.Defense > 0)
                {
                    Console.Write(" | ");
                    Program.ColorText($"방어력 +{item.Defense} ", ConsoleColor.Green);
                }
                Console.Write(" | ");
                Program.ColorText($"{item.Desc}\n", ConsoleColor.White);
            }
        }

        public void SellingInventory()
        {
            int i = 0;
            foreach (Item item in inventoryList)
            {
                i++;
                Console.Write("- ");
                Console.Write(i);

                if (item.isEquiped)
                {
                    Program.ColorText(" [E]", ConsoleColor.Red);
                }

                Program.ColorText($" {item.Name}", ConsoleColor.Cyan);

                if (item.Offense > 0)
                {
                    Console.Write(" | ");
                    Program.ColorText($"공격력 +{item.Offense} ", ConsoleColor.DarkGreen);
                }
                if (item.Defense > 0)
                {
                    Console.Write(" | ");
                    Program.ColorText($"방어력 +{item.Defense} ", ConsoleColor.DarkGreen);
                }
                Console.Write(" | ");
                Program.ColorText($"{item.Desc}\n", ConsoleColor.White);
                Program.ColorText($" | {item.saleGold * 0.85}G\n", ConsoleColor.Green);
            }
        }

        public void DisplayShop()
        {
            int i = 0;

            foreach (Item item in shopItemList)
            {
                i++;
                Console.Write("- ");
                Console.Write(i);
                Program.ColorText($" {item.Name}", ConsoleColor.Cyan);

                if (item.Offense > 0)
                {
                    Console.Write(" | ");
                    Program.ColorText($"공격력 +{item.Offense} ", ConsoleColor.Green);
                }
                if (item.Defense > 0)
                {
                    Console.Write(" | ");
                    Program.ColorText($"방어력 +{item.Defense} ", ConsoleColor.Green);
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
                Console.WriteLine();
                //이미 구매 했을때
                if (item.isBuy)
                {
                    Program.ColorText($" | 구매완료\n", ConsoleColor.Red);
                }
                else
                {
                    Program.ColorText($" | {item.saleGold} G\n", ConsoleColor.Yellow);
                }
            }
        }
    }
}

public class Monster
{
    public string Name;
    public int Lv;
    public int Hp;
    public int Offense;
    public int Defense;
    public string Desc;//description(변수명)
	public int ExpReward { get; set; }
	public int GoldReward { get; set; }
	public List<Item> ItemRewards { get; set; }

	public List<Monster> monsters;
    public void Monsters()
    {
        monsters = new List<Monster>();
    }
    public void Add(Monster monster)
    {
        monster.Add(monster);
    }
    public Monster(string name, int level, int hp, int offense, int defense, string desc, int expReward, int goldReward, List<Item> itemRewards)
    {
        Name = name;
        Lv = level;
        Hp = hp;
        Offense = offense;
        Defense = defense;
        Desc = desc;
		ExpReward = expReward;
		GoldReward = goldReward;
		ItemRewards = itemRewards;

	}
}

internal class Dungeon
{
    Character player;
    List<Monster> monsters;
    Random random = new Random();
    int max_hp;
    int max_mp;


    //이제 필요없나?
    public Dungeon(Character player)
    {
        max_hp = player.Hp;
        max_mp = player.Mp;

        this.player = player;
        monsters = new List<Monster>();
    }

    public void Stage1()
    {
		monsters.Clear();
		Item healingPotion = new Item("치유 물약", -1, -1, "체력을 30 회복 시켜주는 물약 입니다.", false, false, 300, 0);
		List<Item> stage1Rewards = new List<Item>() { healingPotion };

		Monster minion = new Monster("미니언", 2, 15, 10, 0, "나약한 미니언이다.", 10, 100, stage1Rewards);
		Monster voidinsect = new Monster("공허충", 3, 20, 15, 1, "공허충입니다.", 15, 150, stage1Rewards);
		Monster cannonminion = new Monster("대포미니언", 4, 30, 20, 3, "대포미니언입니다.", 20, 200, stage1Rewards);

		monsters.Add(minion);
        monsters.Add(voidinsect);
        monsters.Add(cannonminion);

        monsters = monsters.OrderBy(monster => random.Next(-1, 1)).ToList();
    }
	public void Stage2()
	{
		monsters.Clear();
		Item ironArmor = new Item("무쇠 갑옷", 0, 15, "무쇠로 만들어져 튼튼한 갑옷입니다.", false, false, 600, 0);
		List<Item> stage2Rewards = new List<Item>() { ironArmor };

		Monster goblin = new Monster("고블린", 3, 20, 10, 0, "멍청하지만 지성이 있는 고블린입니다.", 20, 200, stage2Rewards);
		Monster warg = new Monster("와르그", 3, 20, 15, 0, "고블린들이 키우는 와르그입니다.", 25, 250, stage2Rewards);
		Monster hobgoblin = new Monster("홉고블린", 4, 30, 15, 1, "고블린들을 지배하는 홉고블린입니다.", 30, 300, stage2Rewards);
		Monster ogre = new Monster("오우거", 5, 50, 20, 5, "고블린들의 용병으로 일하는 오우거입니다.", 35, 350, stage2Rewards);

		monsters.Add(goblin);
		monsters.Add(warg);
		monsters.Add(hobgoblin);
		monsters.Add(ogre);

		monsters = monsters.OrderBy(monster => random.Next(-1, 2)).ToList();
	}

	public void Stage3()
	{
		monsters.Clear();
		// 스파르타 창과 갑옷 인스턴스 생성
		Item spartaSpear = new Item("스파르타 창", 30, 0, "스파르타 전사들이 사용했다는 전설의 창입니다.", false, false, 3000, 0);
		Item spartaArmor = new Item("스파르타 갑옷", 0, 30, "스파르타 전사들이 입던 갑옷입니다.", false, false, 3000, 0);
		List<Item> stage3Rewards = new List<Item>() { spartaArmor, spartaSpear };

		Monster cho = new Monster("초", 6, 350, 20, 0, "한 몸을 공유하는 두 형제입니다.", 40, 400, stage3Rewards);
		Monster gall = new Monster("갈", 6, 350, 35, 0, "두 형제 중 마법사입니다.", 45, 450, stage3Rewards);

		monsters.Add(cho);
		monsters.Add(gall);
	}


	public void StageSelect()
    {
        Program.SoundPlay("dungeon");
        string StageArt = @"______                                    
|  _  \                                   
| | | |_   _ _ __   __ _  ___  ___  _ __  
| | | | | | | '_ \ / _` |/ _ \/ _ \| '_ \ 
| |/ /| |_| | | | | (_| |  __/ (_) | | | |
|___/  \__,_|_| |_|\__, |\___|\___/|_| |_|
                    __/ |                 
                   |___/                  ";
        Console.Clear();
        Program.ColorText(StageArt, ConsoleColor.DarkGreen);
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("\n              =============");
        Console.WriteLine("              ||던전 선택||");
        Console.WriteLine("              =============\n");
        Console.ResetColor();

        Console.WriteLine("      스파르타 던전에 입장하셨습니다.\n");

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("0. 나가기");
        Console.WriteLine("1. 소환사의 협곡(쉬움)");
        Console.WriteLine("2. 고블린 캠프(보통)");
        Console.WriteLine("3. 시공의 폭풍(어려움)");
        Console.ResetColor();

        Console.WriteLine("\n------------------------------------------");
        Console.WriteLine("원하시는 던전 난이도를 입력해주세요.");
        Console.WriteLine(">>");

        while (true)
        {
            int acton = Program.CheckInput(0, 3);

            if (acton == 0)
            {
                Program.SoundPlay("shop");
                Program.Start();
            }
            else if (acton == 1)
            {
                Stage1();
            }
            else if (acton == 2)
            {
                Stage2();
            }
            else if (acton == 3)
            {
                Program.SoundPlay("cho'gall_comment");
                Thread.Sleep(9400);
                Program.SoundPlay("cho'gall_bgm");
                Stage3();
            }
            BattleStart();
        }
    }

    public void BattleStart()
    {
        while (player.Hp > 0 && player.Hp > 0 && monsters.Any(m => m.Hp > 0))
        {
            battleInfo("battle_start");
            int acton = Program.CheckInput(1, 2);
            while (true)
            {
                if (acton == 1)
                {
                    playerBasicAttack();
                    break;
                }
                else if (acton == 2)
                {
                    playerSkill();
                    break;
                }
            }
            //몬스터 턴
            foreach (var monster in monsters)
            {
                if (player.Hp <= 0)
                    break;
                else if (monster.Hp > 0)
                {
                    MonsterAttack(monster);
                }
            }
        }
        Result();
    }
    public void playerBasicAttack()
    {
        battleInfo("basic_attack");

        string attackArt = @"


                        /\                                                 /\
              _         )( ______________________   ______________________ )(         _
             (_)///////(**)______________________> <______________________(**)\\\\\\\(_)
                        )(                                                 )(
                        \/                                                 \/
";


        int acton = Program.CheckInput(1, monsters.Count);

        // [player 공격 로직]
        Random random = new Random();
        double damage = random.Next(9, 12);
        if (random.Next(1, 101) < 15)
            damage = damage * 1.6;
            
        Console.Clear();
        Console.WriteLine(attackArt);
        Program.ColorText("\n\n                            " + player.Name + "의 공격!\n\n", ConsoleColor.Yellow);

        //기본공격시 회피 기능
        if (random.Next(1, 101) > 10)
        {
            Console.Write("\n                                Lv. " + monsters[acton - 1].Lv + monsters[acton - 1].Name + "을(를) 맞췄습니다. ");
            Program.ColorText("[데미지 : " + (int)((player.Offense + player.AddedOffense) * (0.1 * damage)) + "]\n\n", ConsoleColor.Red);
            Console.WriteLine("                                  Lv. " + monsters[acton - 1].Lv + " " + monsters[acton - 1].Name);
            Console.Write("                                  HP " + monsters[acton - 1].Hp + " -> ");

            if (monsters[acton - 1].Name == "초" || monsters[acton - 1].Name == "갈") //초갈용 대미지 계산
            {
                if(monsters[acton - 1].Name == "초")
                {
                    monsters[acton - 1].Hp -= (int)(player.Offense + player.AddedOffense * (0.1 * damage));
                    monsters[acton].Hp -= (int)(player.Offense + player.AddedOffense * (0.1 * damage));
                }
                else
                {
                    monsters[acton - 1].Hp -= (int)(player.Offense + player.AddedOffense * (0.1 * damage));
                    monsters[acton - 2].Hp -= (int)(player.Offense + player.AddedOffense * (0.1 * damage));
                }
            }
            else
                monsters[acton - 1].Hp -= (int)(player.Offense + player.AddedOffense * (0.1 * damage));
            if (monsters[acton - 1].Hp > 0)
                Program.ColorText("HP " + monsters[acton - 1].Hp, ConsoleColor.Green);
            else
            {
                monsters[acton - 1].Hp = 0;
                Program.ColorText("Dead\n", ConsoleColor.Red);
            }

            Console.WriteLine();
        }
        else
        {
            Program.ColorText("\n                                 Lv. " + monsters[acton - 1].Lv + monsters[acton - 1].Name + "을(를) 공격했지만 대상이 회피 했습니다.\n", ConsoleColor.Red);
        }
        Console.WriteLine("\n                                 엔터를 입력하세요.\n");

        Console.WriteLine("                                 >>");
        Console.ReadLine();
    }
    public void playerSkill()
    {
        battleInfo("skill");

        int acton = Program.CheckInput(0, 2);
        while (true)
        {
            if (acton == 0)
            {
                BattleStart();
                break;
            }
            else if (acton == 1)
            {
                skill_AlphaStrike();
                break;
            }
            else if (acton == 2)
            {
                skill_DoubleStrike();
                break;
            }
        }
    }
    public void battleInfo(string acton)
    {
        Console.Clear();
        int i = 0;
        string battleArt = @"
                                     ___       _   _   _      
                                    / __\ __ _| |_| |_| | ___ 
                                   /__\/// _` | __| __| |/ _ \
                                  / \/  \ (_| | |_| |_| |  __/
                                  \_____/\__,_|\__|\__|_|\___|";
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(battleArt);
        Console.ResetColor();
        foreach (var monster in monsters)
        {
            Program.ColorText($"\n\n                               [{i + 1}]", ConsoleColor.Cyan);
            Console.Write($"  Lv.{monster.Lv} {monster.Name} ");
            if (monster.Hp > 0)
                Program.ColorText($"\n                                                          HP {monster.Hp}\n", ConsoleColor.DarkGreen);
            else
                Program.ColorText("\n                                                          Dead\n", ConsoleColor.Red);
            i += 1;
        }
        Console.WriteLine();
        Program.ColorText($"\n                                            [내정보]\n\n", ConsoleColor.Yellow);
        Console.Write($"                                        Lv.{player.Lv} {player.Name} ({player.Job})\n");
        Program.ColorText($"                                          HP {player.Hp}/{max_hp}\n", ConsoleColor.Green);
        Program.ColorText($"                                          MP {player.Mp}/{max_mp}\n", ConsoleColor.Blue);

        if (acton == "basic_attack")
        {
            Console.WriteLine("\n                                 공격할 대상을 선택해주세요.");
        }
        else if (acton == "battle_start")
        {
            Console.WriteLine("\n                              1. 공격\n");
            Console.WriteLine("                              2. 스킬\n");
            Program.ColorText("\n                                 원하시는 행동을 입력해주세요.\n", ConsoleColor.Yellow);
        }
        else if (acton == "skill")
        {
            Program.ColorText("\n               1. 알파 스트라이크 - MP 10", ConsoleColor.Blue);
            Console.WriteLine(" 공격력 * 2 로 하나의 적을 공격합니다.");
            Program.ColorText("\n               2. 더블 스트라이크 - MP 15", ConsoleColor.Blue);
            Console.WriteLine(" 공격력 * 1.5 로 2명의 적을 랜덤으로 공격합니다.");
            Console.WriteLine("\n               0. 취소");
            Program.ColorText("\n                                 원하시는 행동을 입력해주세요.\n", ConsoleColor.Yellow);
        }
        else if (acton == "alpha_strike")
        {
            Console.WriteLine("\n                             '알파 스트라이크'로 공격할 대상을 선택해주세요.");
        }
        else if (acton == "double_strike")
        {
            //더블 스트라이크는 즉시발동스킬이라 대상을 선택할 필요 없음.
            //Console.WriteLine("'더블 스트라이크'로 공격할 대상을 선택해주세요.");
        }
        Console.WriteLine("                                 >>");
    }

    public void skill_AlphaStrike()
    {
        string alphastrikeArt = @"
                                            )         
                                              (            
                                            '    }      
                                          (    '      
                                         '      (   
                                          )  |    ) 
                                        '   /|\    `
                                       )   / | \  ` )   
                                      {    | | |  {   
                                     }     | | |  .
                                      '    | | |    )
                                     (    /| | |\    .
                                      .  / | | | \  (
                                    }    \ \ | / /  .        
                                     (    \ `-' /    }
                                     '    / ,-. \    ' 
                                      }  / / | \ \  }
                                     '   \ | | | /   } 
                                      (   \| | |/  (
                                        )  | | |  )
                                        .  | | |  '
                                           J | L
                                     /|    J_|_L    |\
                                     \ \___/ o \___/ /
                                      \_____ _ _____/
                                            |-|
                                            |-|
                                            |-|
                                           ,'-'.
                                           '---'
";
        if (player.Mp < 10)
        {
            Program.ColorText("                                 마나가 부족합니다.\n", ConsoleColor.Red);
            Thread.Sleep(600);
            BattleStart();
        }
        int now_mp = player.Mp;
        player.Mp -= 10;
        battleInfo("alpha_strike");
        int acton = Program.CheckInput(1, monsters.Count);

        Random random = new Random();
        double damage = random.Next(9, 12);

        //critical
        if (15 > random.Next(1, 101))
            damage = damage * 1.6;

        Console.Clear();
        Console.WriteLine(alphastrikeArt);
        Program.ColorText("                                  " + player.Name + "의 알파 스트라이크!\n", ConsoleColor.Red);
        Program.ColorText($"                                      MP {now_mp} -> {player.Mp}", ConsoleColor.Blue);
        Console.Write("\n\n                            Lv. " + monsters[acton - 1].Lv + monsters[acton - 1].Name + "을(를) 맞췄습니다. ");
        Program.ColorText("[데미지 : " + (int)((player.Offense + player.AddedOffense) * (0.1 * damage) * 2) + "]\n\n", ConsoleColor.Red);
        Console.WriteLine("                            Lv. " + monsters[acton - 1].Lv + " " + monsters[acton - 1].Name);
        Console.Write("                                  HP " + monsters[acton - 1].Hp + " -> ");
        if (monsters[acton - 1].Name == "초" || monsters[acton - 1].Name == "갈") //초갈용 대미지 계산
        {
            if (monsters[acton - 1].Name == "초")
            {
                monsters[acton - 1].Hp -= (int)((player.Offense + player.AddedOffense) * (0.1 * damage) * 2);
                monsters[acton].Hp -= (int)((player.Offense + player.AddedOffense) * (0.1 * damage) * 2);
            }
            else
            {
                monsters[acton - 1].Hp -= (int)((player.Offense + player.AddedOffense) * (0.1 * damage) * 2);
                monsters[acton - 2].Hp -= (int)((player.Offense + player.AddedOffense) * (0.1 * damage) * 2);
            }
        }
        else
            monsters[acton - 1].Hp -= (int)((player.Offense + player.AddedOffense) * (0.1 * damage) * 2);
        if (monsters[acton - 1].Hp > 0)
            Program.ColorText($"HP {monsters[acton - 1].Hp}\n", ConsoleColor.Green);
        else
        {
            monsters[acton - 1].Hp = 0;
            Program.ColorText("Dead\n", ConsoleColor.Red);
        }
        Console.WriteLine("\n                                 엔터를 입력하세요.");
        Console.WriteLine("                                 >>");
        Console.ReadLine();
    }
    public void skill_DoubleStrike()
    {
        string doubleStrikeArt = @"


                                     /\                    /\
                                     \ \                  / /
                                      \ \                / /
                                       \.\              /./
                                        \.\            /./
                                         \.\          /./
                                          \\\        ///
                                           \.\      /./
                                            \.\    /./
                                             \.\__/./
                                            _/)))(((\_
                                            \|)\##/(|/
                                            _|)/##\(|_
                                            \|)))(((|/
                                             /o/  \o\
                                            /o/    \o\
                                           /_/      \_\

";
        if (player.Mp < 15)
        {
            Program.ColorText("                                 마나가 부족합니다.\n", ConsoleColor.Red);
            Thread.Sleep(600);
            BattleStart();
        }
        int now_mp = player.Mp;
        player.Mp -= 15;
        battleInfo("double_strike");
        //int acton = Program.CheckInput(1, monsters.Count);
        int monstercount = 0;
        foreach (var monster in monsters)
        {
            if (monster.Hp > 0)
                monstercount++;
            if (monster.Name == "초" || monster.Name == "갈")
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Program.SlowText("\n\n                초&갈 : \"우리는 둘이지만 몸은 하나다! 그런 수법에 당하지 않는다!\"", 20);
                Console.ResetColor();
                Thread.Sleep(600);
                return;
            }
        }
        if (monstercount < 2)
        {
            Program.ColorText("                 더블 스트라이크는 몬스터 수가 2마리 이상일때 사용가능합니다.\n", ConsoleColor.Red);
            Console.WriteLine("\n                                 엔터를 입력하세요.");
            Console.WriteLine("                                 >>");
            Console.ReadLine();
            playerSkill();
            return;
        }

            Random random = new Random();
        double damage = random.Next(9, 12);
        if (15 > random.Next(1, 101))
            damage = damage * 1.6;

        //몬스터가 3마리 이상일때 중복되지않은 2개의 난수 생성
        int target1 = random.Next(0, monstercount);
        int target2 = 0;
        while (true)
        {
            target2 = random.Next(0, monstercount);
            if (target1 != target2)
                break;
        }

        Console.Clear();
        Console.WriteLine(doubleStrikeArt);
        Program.ColorText("                            " + player.Name + "의 더블 스트라이크!\n", ConsoleColor.Cyan);
        Program.ColorText($"                                MP {now_mp} -> {player.Mp}\n", ConsoleColor.Blue);
        Console.Write("\n                            Lv. " + monsters[target1].Lv + monsters[target1].Name + "을(를) 맞췄습니다. ");
        Program.ColorText("[데미지 : " + (int)((player.Offense + player.AddedOffense) * (0.1 * damage) * 1.5) + "]\n\n", ConsoleColor.Red);
        Console.WriteLine("                            Lv. " + monsters[target1].Lv + " " + monsters[target1].Name);
        Console.Write("\n                                  HP " + monsters[target1].Hp + " -> ");

        monsters[target1].Hp -= (int)((player.Offense + player.AddedOffense) * (0.1 * damage) * 1.5);
        if (monsters[target1].Hp > 0)
            Program.ColorText($"HP {monsters[target2].Hp}\n", ConsoleColor.Green);
        else
        {
            monsters[target1].Hp = 0;
            Program.ColorText("Dead\n", ConsoleColor.Red);
        }
        Console.WriteLine();

        Program.ColorText("\n                            " + player.Name + "의 2차 공격!\n", ConsoleColor.Cyan);
        Console.Write("                            Lv. " + monsters[target2].Lv + monsters[target2].Name + "을(를) 맞췄습니다. ");
        Program.ColorText("[데미지 : " + (int)((player.Offense + player.AddedOffense) * (0.1 * damage) * 1.5) + "]\n\n", ConsoleColor.Red);
        Console.WriteLine("                            Lv. " + monsters[target2].Lv + " " + monsters[target2].Name);
        Console.Write("\n                                  HP " + monsters[target2].Hp + " -> ");

        monsters[target2].Hp -= (int)((player.Offense + player.AddedOffense) * (0.1 * damage) * 1.5);
        if (monsters[target2].Hp > 0)
            Program.ColorText($"HP {monsters[target2].Hp}\n", ConsoleColor.Green);
        else
        {
            monsters[target2].Hp = 0;
            Program.ColorText("Dead\n", ConsoleColor.Red);
        }
        Console.WriteLine("\n                                 엔터를 입력하세요.");
        Console.WriteLine("                                 >>");
        Console.ReadLine();
    }

    public void MonsterAttack(Monster monster)
    {
        string monsterattackArt = @"
                                             ______
                                         .-""      ""-.
                                        /              \
                                       |               |
                                       |,  .-.   .-.  ,|
                                       | )(__/   |__)( |
                                       |/     /\      \|
                             (@_       (_     ^^      _)
                        _     ) \_______\__|IIIIII||__/_________________
                       (_)@8@8{}<________|-\IIIIII|/-|___________________>
                              )_/        \          /
                             (@           `--------` 


";
        Console.Clear();
        Console.WriteLine(monsterattackArt);
        if (player.Hp > 0)
        {
            Program.ColorText("                             Lv. " + monster.Lv + " " + monster.Name + "의 공격!\n", ConsoleColor.Yellow);
            Console.Write($"\n                                {player.Name} 을(를) 맞췄습니다. ");
            Program.ColorText($"[데미지 :  { (int)Math.Ceiling(monster.Offense - ((player.Defence + player.AddedDefence) * 0.1))}]", ConsoleColor.Red);
            Console.WriteLine("\n\n                                  Lv. " + player.Lv + " " + player.Name);
            Console.Write("                                  HP " + player.Hp + " -> ");
            player.Hp -= (int)Math.Ceiling(monster.Offense - ((player.Defence + player.AddedDefence) * 0.1));
            if (player.Hp > 0)
            {
                Program.ColorText($"{player.Hp}\n", ConsoleColor.Green);
            }
            
            else
            {
                player.Hp = 0;
                Program.ColorText("Dead\n", ConsoleColor.Red);
            }
        }
        Console.WriteLine("\n                                 엔터를 입력하세요");
        Console.WriteLine("                                 >>");
        Console.ReadLine();
    }
	public void Result()
	{
		Console.Clear();
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine("                                      ====================");
        Console.WriteLine("                                      ||던전 전투 - 결과||");
        Console.WriteLine("                                      ====================");
        Console.ResetColor();

        if (player.Hp > 0)
		{
			int totalExpGained = 0;
			int totalGoldGained = 0;
			List<Item> itemsGained = new List<Item>();

			foreach (Monster monster in monsters)
			{
				totalExpGained += monster.ExpReward;
				totalGoldGained += monster.GoldReward;

				// 스파르타 창과 갑옷이 5% 확률로 드랍되게 설정
				if (random.Next(100) < 5) // 5% 확률
				{
					Item dropItem = random.Next(2) == 0 ? new Item("스파르타 창", 30, 0, "전설의 창", false, false, 3000, 0) :
														  new Item("스파르타 갑옷", 0, 30, "전설의 갑옷", false, false, 3000, 0);
					itemsGained.Add(dropItem);
				}


				itemsGained.AddRange(monster.ItemRewards);
			}

			player.Exp += totalExpGained;
			player.Gold += totalGoldGained;
			foreach (Item item in itemsGained)
			{
				Program.GetInventory().inventoryList.Add(item);
			}

			// 레벨업 확인
			while (player.Exp >= player.ExpToLevelUp)
			{
				player.Exp -= player.ExpToLevelUp;
				player.Lv++;
				player.Offense += 1; // 혹은 레벨업에 따른 적절한 스탯 증가
				player.Defence += 1; // 혹은 레벨업에 따른 적절한 스탯 증가
				player.ExpToLevelUp = CalculateExpForNextLevel(player.Lv);
			}

            Console.WriteLine("\n                                            Victory\n\n");
			Console.WriteLine($"                         던전에서 몬스터 {monsters.Count}마리를 잡았습니다.\n");
			Program.ColorText($"                                       [캐릭터 정보]\n\n", ConsoleColor.Yellow);
			Console.WriteLine($"                         현재레벨: Lv.{player.Lv} {player.Name} -> 다음레벨: Lv.{player.Lv + 1} {player.Name}\n");
			Console.WriteLine($"                         HP {max_hp} -> {player.Hp}\n");
			Console.WriteLine($"                         MP {max_mp} -> {player.Mp}\n");
			Console.WriteLine($"                         exp {player.Exp - totalExpGained} -> {player.Exp}\n");

			Program.ColorText("                                       [획득 아이템]\n", ConsoleColor.Green);
			foreach (Item item in itemsGained)
			{
				Console.WriteLine($"                         {item.Name} - 1");
			}
		}
		else
		{
            Console.WriteLine("\n                                             Lost\n\n");
			Console.WriteLine($"                         Lv.{player.Lv} {player.Name}");
			Console.WriteLine($"                         HP {max_hp} -> {player.Hp}\n");
			Console.WriteLine($"                         MP {max_mp} -> {player.Mp}\n");
		}

		Console.WriteLine("                         0. 다음\n");
		Console.WriteLine("                         >>");
		Console.WriteLine("                         엔터를 입력하면 마을로 이동합니다.");

        //일단 종료하지는 않고 마을로 이동함(회복아이템?)
        Console.ReadLine();
        Console.SetCursorPosition(37, Console.CursorTop);
        Console.Clear();
        Program.SoundPlay("shop");
        Program.Start();
    }

    int CalculateExpForNextLevel(int currentLevel)
    {
		return currentLevel switch
		{
			1 => 10,
			2 => 35,
			3 => 65,
			4 => 100,
			
		};
	}
}

