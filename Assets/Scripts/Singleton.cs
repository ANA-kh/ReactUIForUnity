using System;
using System.Reflection;

namespace Singleton
{
    public interface ISingleton
    {
        void Init();
        void Cleanup();
    }

    public abstract class Singleton<T> : ISingleton where T : Singleton<T> //非常死的约束，使得无法像List<int> list 这样直接当作类型使用； 必须新建一个继承自Singleton<T>的类来使用
    {
        private static T _instance;
        private bool _hasInit = false;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    var type = typeof(T);
                    // 获取私有构造函数
                    var constructorInfos = type.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic);
                    
                    // 获取无参构造函数
                    var ctor = Array.Find(constructorInfos, c => c.GetParameters().Length == 0);
                    
                    if (ctor == null)
                    {
                        throw new Exception("Non-Public Constructor() not found! in " + type);
                    }

                    _instance = ctor.Invoke(null) as T;
                    if (_instance != null) _instance.Init();
                }
                return _instance;
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