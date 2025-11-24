using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace TidyCooking.Levels
{
    public class CookModuleHarvestRoots : MonoBehaviour, ICookModule
    {
        [SerializeField] private float _delayStartShowRoots = 0.5f;
        [SerializeField] private float _delayBetweenShowRoots = 0.1f;
        [SerializeField] private HarvestableRootItem[] _rootItems;
        [SerializeField] private HarvestableRootItem.GeneralConfig _rootConfig;

        public HarvestableRootItem[] RootItems => _rootItems;
        public event System.Action<HarvestableRootItem> onRootDetached;
        private System.Action<ICookModule> _onEnd;

        public Task SetUp()
        {
            for (int i = 0; i < _rootItems.Length; i++)
            {
                _rootItems[i].SetUp(_rootConfig);
                _rootItems[i].onDetached += OnRootDetached;
            }
            return Task.CompletedTask;
        }

        public Task StartModule(Action<ICookModule> onEnd)
        {
            _onEnd = onEnd;
            StartCoroutine(IEShowRoots());
            return Task.CompletedTask;

            IEnumerator IEShowRoots()
            {
                yield return new WaitForSeconds(_delayStartShowRoots);
                var wait = new WaitForSeconds(_delayBetweenShowRoots);
                for (int i = 0; i < _rootItems.Length; i++)
                {
                    _rootItems[i].Appear();
                    yield return wait;
                }
            }
        }

        private void OnRootDetached(HarvestableRootItem item)
        {
            onRootDetached?.Invoke(item);
            if (IsAllRootsDetached())
            {
                _onEnd?.Invoke(this);
            }

            bool IsAllRootsDetached()
            {
                foreach (var root in _rootItems)
                {
                    if (root.CurrentState != HarvestableRootItem.State.Detached)
                        return false;
                }
                return true;
            }
        }
    }
}