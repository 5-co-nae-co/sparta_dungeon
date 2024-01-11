using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;
public class Items
{
    public bool isBought { get; set; }
    public bool isEquipped { get; set; }
    public string iName { get; set; }
    public string iType { get; set; }
    public int iStat { get; set; }
    public string iInfo { get; set; }
    public int iPrice { get; set; }


    public Items(bool iB, bool iE, string iN, string iT, int iS, string iI, int iP)
    {
        isBought = iB;
        isEquipped = iE;
        iName = iN;
        iType = iT;
        iStat = iS;
        iInfo = iI;
        iPrice = iP;
    }
}

