using System.Collections;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace TidyCooking.Levels
{
    public abstract class LevelCookActAbstract : MonoBehaviour
    {
        private System.Action _onEnd;
        public Task TaskSetUpOnInitLevel { get; private set; } = null;
        public Task TaskPreload { get; private set; } = null;
        public bool IsDoneSetUpOnStartLevel => TaskSetUpOnInitLevel != null && TaskSetUpOnInitLevel.IsCompleted;
        public bool IsDonePreload => TaskPreload != null && TaskPreload.IsCompleted;
        [SerializeField] protected int _phaseId = 0;

        /// <summary>
        /// Set up the act when the level starts
        /// </summary>
        public async Task SetUpOnStartLevel()
        {
            if (TaskSetUpOnInitLevel == null)
            {
                TaskSetUpOnInitLevel = SetUpOnInitLevelAsync();
            }
            await TaskSetUpOnInitLevel;
        }
        protected abstract Task SetUpOnInitLevelAsync();

        #region Load/Unload
        /// <summary>
        /// Preload all assets needed for this act
        /// </summary>
        public async Task Preload()
        {
            if (TaskPreload == null)
            {
                TaskPreload = PreloadAsync();
            }
            await TaskPreload;
        }
        protected abstract Task PreloadAsync();

        public abstract void Unload();
        #endregion

        public void StartAct(System.Action onEnd)
        {
            _onEnd = onEnd;
            OnStartAct();
        }

        protected abstract void OnStartAct();

        [Sirenix.OdinInspector.Button]
        protected void EndAct()
        {
            OnEndAct();
            _onEnd?.Invoke();
            _onEnd = null;
        }

        protected virtual void OnEndAct() { }
    }
}