using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Link.Turkey
{
    public class PlateControl : MonoBehaviour
    {
        [SerializeField] List<Plate> plates;

        public void DonePlate(Plate plate)
        {
            plates.Remove(plate);
            if (plates.Count == 0)
            {
                LevelControl.Ins.SetHintTextDone(1, 1);
            }
        } 
    }
}