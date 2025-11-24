using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using Utilities;

// namespace TidyCooking.Levels
// {
//     public class LevelBaseCookDaivq : LevelBase
//     {
//         //public enum CookingAct
//         //{
//         //    None,
//         //    Hunt,
//         //    Cook,
//         //    Decor,
//         //    Serve
//         //}
//         //[ShowInInspector, ReadOnly]
//         //public CookingAct CurrentAct { get; private set; } = CookingAct.None;

//         [SerializeField] private LevelCookActAbstract[] _acts;
//         private int _currentActIndex = -1;
//         public LevelCookActAbstract CurrentAct => _acts[_currentActIndex];

//         protected override void Start()
//         {
//             base.Start();
//             SetUpThenPlay().RunWithLogException();
//         }

//         private async Task SetUpThenPlay()
//         {
//             Task[] tasks = new Task[_acts.Length];
//             for (int i = 0; i < _acts.Length; i++)
//             {
//                 tasks[i] = _acts[i].SetUpOnStartLevel();
//             }
//             await Task.WhenAll(tasks);

//             _currentActIndex = 0;

//             LoadThenBeginAct().RunWithLogException();
//         }

//         private async Task LoadThenBeginAct()
//         {
//             await _acts[_currentActIndex].Preload();
//             _acts[_currentActIndex].StartAct(OnEndAct);
//         }

//         private void OnEndAct()
//         {
//             _acts[_currentActIndex].Unload();

//             _currentActIndex++;
//             if (_currentActIndex < _acts.Length)
//             {
//                 LoadThenBeginAct().RunWithLogException();
//             }
//             else
//             {
//                 WinGame();
//             }
//         }
//     }
// }