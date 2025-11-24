using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Link;

namespace HoangLinh.Cooking.Test
{
    public class RottenGarlic : ItemMovingBase
    {
        public override bool IsCanMove => true;
        [SerializeField] TrashBin trashBin;

    }
}