using System.IO;
using UnityEngine;

namespace ReactUI
{
    public interface IResourceLoad
    {
        T Load<T>(string spriteName) where T : Object;
        void UnloadAsset(Object imageSprite);
    }

    public class ResourceLoad : IResourceLoad
    {
        private static readonly string ResourcePath = "Free";
        public T Load<T>(string name) where T : Object
        {
            return Resources.Load<T>(Path.Combine(ResourcePath, name));
        }

        public void UnloadAsset(Object imageSprite)
        {
            throw new System.NotImplementedException();
        }
    }
    
    public class AssetBundleLoad : IResourceLoad
    {
        private AssetBundle _assetBundle;
        private static readonly string AssetBundleDirectory = "Assets/AssetBundles/Free";

        private void Init()
        {
            _assetBundle = AssetBundle.LoadFromFile(AssetBundleDirectory);
        }
        public T Load<T>(string name) where T : Object
        {
            if (_assetBundle == null)
            {
                Init();
            }
            if (_assetBundle != null)
            {
                return _assetBundle.LoadAsset<T>(name);
            }

            return null;
        }

        public void UnloadAsset(Object imageSprite)
        {
            throw new System.NotImplementedException();
        }
    }
}