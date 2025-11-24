using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Utilities
{
    public class AddressableHandlerControl<Key, HandleType> where HandleType : Object
    {
        private Dictionary<Key, AsyncOperationHandle<HandleType>> _dictCustomHandler;

        public async Task<HandleType> Load(Key key, string assetKey)
        {
            if (_dictCustomHandler == null) _dictCustomHandler = new();

            AsyncOperationHandle<HandleType> op;
            if (_dictCustomHandler.TryGetValue(key, out op))
            {
                // ok
            }
            else
            {
                op = Addressables.LoadAssetAsync<HandleType>(assetKey);
                _dictCustomHandler.Add(key, op);
            }
            await op.Task;
            return op.Result;
        }

        public async Task<HandleType> Load(Key key, AssetReference asset)
        {
            if (_dictCustomHandler == null) _dictCustomHandler = new();

            AsyncOperationHandle<HandleType> op;
            if (_dictCustomHandler.TryGetValue(key, out op))
            {
                // ok
            }
            else
            {
                op = Addressables.LoadAssetAsync<HandleType>(asset);
                _dictCustomHandler.Add(key, op);
            }
            await op.Task;
            return op.Result;
        }

        public void ReleaseAll()
        {
            foreach (var item in _dictCustomHandler)
            {
                Addressables.Release(item.Value);
            }
        }
    }

    public class AddressableAssetHandlerControl<HandleType> where HandleType : Object
    {
        private Dictionary<AssetReference, AsyncOperationHandle<HandleType>> _dictAssetHandler;
        private Dictionary<string, AsyncOperationHandle<HandleType>> _dictKeyStrHandler;

        public async Task<HandleType> Load(AssetReference asset)
        {
            if (_dictAssetHandler == null) _dictAssetHandler = new();

            AsyncOperationHandle<HandleType> op;
            if (_dictAssetHandler.TryGetValue(asset, out op))
            {
                // ok
            }
            else
            {
                op = Addressables.LoadAssetAsync<HandleType>(asset);
                _dictAssetHandler.Add(asset, op);
            }
            await op.Task;
            return op.Result;
        }

        public async Task<HandleType> Load(string key)
        {
            if (_dictKeyStrHandler == null) _dictKeyStrHandler = new();
            AsyncOperationHandle<HandleType> op;
            if (_dictKeyStrHandler.TryGetValue(key, out op))
            {
                // ok
            }
            else
            {
                op = Addressables.LoadAssetAsync<HandleType>(key);
                _dictKeyStrHandler.Add(key, op);
            }
            await op.Task;
            return op.Result;
        }

        public void ReleaseAll()
        {
            if (_dictAssetHandler != null)
            {
                foreach (var item in _dictAssetHandler)
                {
                    Addressables.Release(item.Value);
                }
            }
            if (_dictKeyStrHandler != null)
            {
                foreach (var item in _dictKeyStrHandler)
                {
                    Addressables.Release(item.Value);
                }
            }
        }
    }
}