using UnityEngine;

namespace Singleton
{
    public abstract class MonoSingleton<T> : MonoBehaviour,ISingleton where T : MonoSingleton<T>
    {
        private static T _instance;
        private bool _hasInit = false;
        
        public static T Instance 
        {
            get
            {
                if (_instance == null)
                {
                    _instance = GameObject.FindObjectOfType(typeof(T)) as T;
                    if (_instance == null)
                    {
                        var go = new GameObject("Singleton of " + typeof(T).ToString(), typeof(T))
                        {
                            //对象不会保存到场景中。加载新场景时不会被销毁。相当于HideFlags.DontSaveInBuild | HideFlags.DontSaveInEditor | HideFlags.DontUnloadUnusedAsset
                            hideFlags = HideFlags.DontSave
                        };
                        
                        _instance = go.GetComponent<T>();
                        _instance.Init();
                    }
                }

                return _instance;
            }
        }
        
        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
                if (_instance != null) _instance.Init();
            }
        }
        
        public void Init()
        {
            if (_hasInit == false)
            {
                OnInit();
                _hasInit = true;
            }
        }

        public void Cleanup()
        {
            OnCleanup();
            _hasInit = false;
        }

        protected abstract void OnInit();
        protected abstract void OnCleanup();
    }
}