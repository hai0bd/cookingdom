using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Link.Cooking.Lobster
{
    public class Constant : MonoBehaviour
    {

    }

    public enum Step
    {
        FruitInBoard = 0,
        FruitCut = 1,
        FruitInPlate = 2,
        LobsterInBoard_1 = 3,
        LobsterCleaned = 4,
        LobsterInTray = 5,
        OpenLid_1 = 6,
        StreamDishInPot = 7,
        LobsterInPot = 8,
        FruitInPot = 9,
        BearInPot = 10,
        TurnOnStove_1 = 11,
        CoverLid_1 = 12,
        SpiceInPan = 13,
        WineInPan = 14,
        TurnOnStove_2 = 15,
        MixInStart = 16,
        MixInBowl = 17,
        OpenLid_2 = 18,
        LobsterInDish = 19,
        TurnOffStove_1 = 20,
        TurnOffStove_2 = 21,
        CoverLid_2 = 22,
        LobsterInBoard_2 = 23,
        LobsterCut = 24,
        LobsterPieceInDish = 25,
        DecoreInFinish = 26,
        MixInFinish = 27,
        LobsterPieceInFinish = 28,
        Done = 29,     
    }
}
