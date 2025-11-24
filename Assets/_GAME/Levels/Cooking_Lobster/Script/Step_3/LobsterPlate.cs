using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Link.Cooking.Lobster
{
    public class LobsterPlate : ItemIdleBase
    {
        [SerializeField] Transform piece_1, piece_2;

        LobsterPiece lobster_1, lobster_2;
        public override bool IsDone => lobster_1 != null && lobster_2 != null;

        public override bool OnTake(IItemMoving item)
        {
            if (item is Lobster)
            {
                item.TF.SetParent(null);
                item.OnMove(TF.position, Quaternion.identity, 0.2f);
                item.OnSave(0.2f);
                item.IfChangeState(Lobster.State.Stream, Lobster.State.InPlate);
                LevelControl.Ins.SetStep(LevelName.Lobster, Step.LobsterInDish);

                return true;
            }
            if (item is LobsterPiece)
            {
                if ((item as LobsterPiece).Name == DecoreItem.NameType.Piece_1)
                {
                    item.OnMove(piece_1.position, piece_1.rotation, 0.2f);
                    lobster_1 = item as LobsterPiece;
                }
                if ((item as LobsterPiece).Name == DecoreItem.NameType.Piece_2)
                {
                    item.OnMove(piece_2.position, piece_2.rotation, 0.2f);
                    lobster_2 = item as LobsterPiece;
                }
                item.OnSave(0.2f);
                item.IfChangeState(LobsterPiece.State.Normal, LobsterPiece.State.InPlate);
                LevelControl.Ins.CheckStep();
                LevelControl.Ins.SetStep(LevelName.Lobster, Step.LobsterPieceInDish);

                return true;
            }
            return base.OnTake(item);
        }
    }
}