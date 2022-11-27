using ReactUI;
using Singleton;
using UnityEngine;

namespace ResourceLoad_Temp
{
    public class ResourceManager : Singleton<ResourceManager>
    {
        [SerializeField]
        private bool UseResource = true;
        public IResourceLoad ResourceLoader { get; private set; }

        private ResourceManager() { }

        protected override void OnInit()
        {
            if (UseResource)
            {
                ResourceLoader = new ResourceLoad();
            }
            else
            {
                ResourceLoader = new AssetBundleLoad();
            }
        }

        protected override void OnCleanup()
        {
            throw new System.NotImplementedException();
        }
        
    }
}