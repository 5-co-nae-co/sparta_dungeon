using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;
class Title
{
    //static 사용하며 생긴 이슈 해결한 흔적
    public Character character;
    public Inventory inventory;
    public Store store;
    public Title()
    {
        character = new Character(01, "김준하(Unity_3기)", "(전사)", 10, 0, 5, 0, 100, 1500);
        inventory = new Inventory(character);
        store = new Store(character);
        TitleStart();
    }
    public void TitleStart()
    {
        //게임 시작시 간단한 소개 말과 마을에서 할 수 있는 행동을 알려줍니다.
        Clear(); // 타이틀 불러올 때 화면 초기화

        WriteLine();
        ForegroundColor = ConsoleColor.Yellow;
        WriteLine("-------------------------------------------");
        WriteLine("| 스파르타 마을에 오신 여러분 환영합니다. |");
        WriteLine("-------------------------------------------");
        ResetColor();
        WriteLine();
        WriteLine("이곳은 내일배움캠프 왕국의 속한 마을이며");
        Write("많은 초보 모험가들이 ");
        ForegroundColor = ConsoleColor.Red;
        Write("'회사'");
        ResetColor();
        WriteLine("라는 던전에 들어가기 위해 거쳐가는 마을이기에");
        WriteLine("밝은 미래를 꿈꾸는 다양한 모험가들이 이곳에서 준비를 마치고 갑니다.");
        WriteLine();
        ForegroundColor = ConsoleColor.Cyan;
        Write("현 마을의 촌장은 ");
        ForegroundColor = ConsoleColor.Blue;
        Write("한효승(내배캠 매니저)");
        ForegroundColor = ConsoleColor.Cyan;
        WriteLine("씨 입니다.");
        ResetColor();
        WriteLine("-> 촌장님은 마을 교육의 책임자로, 높은 지지도로 개인 팬들을 거느리고 있는 것으로 보입니다.");
        WriteLine();//물론 저도 촌장님을 지지합니다.
        WriteLine("---------------------------------------------------------------------------------------");
        WriteLine("이곳에서 던전으로 들어가기전 활동을 할 수 있습니다.");
        WriteLine();
        ForegroundColor = ConsoleColor.Yellow;
        WriteLine("1. 상태 보기");
        WriteLine("2. 인벤토리");
        WriteLine("3. 상점");
        ResetColor();
        WriteLine();
        WriteLine("원하시는 행동을 입력해주세요.");
        WriteLine(">>");

        int titleInput = CheckInput(1, 3); //탭 이동 매니저, 초반 코드를 static으로 cs를 구분하며 잘못 써서 너무 복잡해졌다.
        switch (titleInput)
        {
            case 1: //상태보기
                GoCharacter();
                break;
            case 2: //인벤토리
                GoInventory();
                break;
            case 3://상점
                GoStore();
                break;
        }
    }
    public void GoCharacter() //상태창 보러가는 메서드
    {
        while(true)
        {
            character.CheckPcStat();
            int statInput = CheckInput(0, 0);
            switch (statInput)
            {
                case 0:
                    TitleStart();
                    break;
            }
        }
    }

    public void GoInventory() //인벤토리 보러가기 메서드 + 장착 관리
    {
        while(true)
        {
            inventory.CheckInventory(false);
            int inventoryInput = CheckInput(0, 1);
            switch (inventoryInput)
            {
                case 0:
                    TitleStart();
                    break;
                case 1:
                    while (true)
                    {
                        inventory.CheckInventory(true);
                        int manageInput = CheckInput(0, character.ownedItems.Count);
                        if (manageInput <= character.ownedItems.Count && manageInput > 0)
                            inventory.ItemEquipping(manageInput);
                        else if (manageInput == 0)
                            break;
                        else
                            WrongInput();
                    }
                    break;
            }
        }
    }
    public void GoStore() //상점창 보러가는 메서드 BuyingInput
    {
        while (true)
        {
            store.CheckStore(false, false);
            int statInput = CheckInput(0, 2);
            switch (statInput)
            {
                case 0: //나가기
                    TitleStart();
                    break;
                case 1: //아이템 구매
                    while (true)
                    {
                        store.CheckStore(true, false);
                        int BuyingInput = CheckInput(0, store.storeItems.Count);
                        if (BuyingInput <= store.storeItems.Count && BuyingInput > 0)
                            store.ItemBuying(BuyingInput);
                        else if (BuyingInput == 0)
                            break;
                        else
                            WrongInput();
                    }
                    break;
                case 2: //아이템 판매
                    while (true)
                    {
                        store.CheckStore(false, true);
                        int SellingInput = CheckInput(0, character.ownedItems.Count);
                        if (SellingInput <= character.ownedItems.Count && SellingInput > 0)
                            store.ItemSelling(SellingInput);
                        else if (SellingInput == 0)
                            break;
                        else
                            WrongInput();
                    }
                    break;
            }

        }
    }

    public void WrongInput()
    {
        ForegroundColor = ConsoleColor.Red;
        WriteLine("잘못된 입력입니다!");
        ResetColor();
    }

    public int CheckInput(int min, int max) //if문으로 매개변수로 받은 min, max까지 숫자인지 판단하는 메서드
    {
        var top = CursorTop;
        //원하는 행동의 숫자를 타이핑하면 실행합니다. 
        //제시되는 숫자 이외 입력시 -잘못된 입력입니다 출력
        while (true)
        {
            wrongInputRemover(top);
            var inputParse = -1;
            if (!int.TryParse(ReadLine(), out inputParse)) //TryParse를 이용해 해결하는 방법으로 변환
                inputParse = -1;
            if (inputParse >= min && inputParse <= max)
                return inputParse;
            else
                WrongInput();
        }

    }

    public void wrongInputRemover(int top)
    {
        SetCursorPosition(0, top);
        WriteLine("                                     ");
        SetCursorPosition(0, top);
    }
}
