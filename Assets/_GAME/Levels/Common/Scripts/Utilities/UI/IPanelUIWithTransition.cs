using System.Collections;
using UnityEngine;

namespace Utilities
{
    public interface IPanelUIWithTransition
    {
        public enum ShowState
        {
            Hide,
            Show,
            Showing,
            Hiding
        }
        ShowState State { get; }
        void ShowImmediately();
        void HideImmediately();
        void ShowWithTransition(System.Action onComplete);
        void HideWithTransition(System.Action onComplete);
    }
}