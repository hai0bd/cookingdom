// using System.Collections;
// using UnityEngine;

// namespace TidyCooking.Levels
// {
//     public abstract class CookCloseupViewAbstract : MonoBehaviour
//     {
//         [SerializeField] private AninControlCookCloseupView _animOpenClose;
//         protected LevelBase LevelBase { get; private set; }
//         private System.Action _onEnd;

//         public void SetUp(LevelBase levelBase)
//         {
//             LevelBase = levelBase;
//             OnSetUp();
//         }

//         protected abstract void OnSetUp();

//         public void Show(System.Action onEnd)
//         {
//             _onEnd = onEnd;
//             OnBeforeShow();
//             _animOpenClose.PlayAnimShow(OnShow);
//         }

//         protected virtual void OnBeforeShow() { }
//         protected abstract void OnShow();

//         protected void Finish()
//         {
//             _onEnd?.Invoke();
//             _onEnd = null;
//             _animOpenClose.PlayAnimHide(null);
//         }
//     }
// }