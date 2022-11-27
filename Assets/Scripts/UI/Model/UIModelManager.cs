using System.Collections.Generic;
using Singleton;

namespace UI
{
    public class UIModelManager : Singleton<UIModelManager>
    {
        private Dictionary<int, UIBaseModel> _UIModels = new Dictionary<int, UIBaseModel>();
        private List<UIBaseModel> _modelLists;

        private UIModelManager() { }

        public T GetModel<T>() where T : UIBaseModel, new()
        {
            var hash = typeof(T).GetHashCode();
            if (_UIModels.TryGetValue(hash, out UIBaseModel model) == false)
            {
                model = new T();
                model.Init();
                _UIModels.Add(hash, model);
            }

            return (T)model;
        }

        protected override void OnInit()
        {
        }

        protected override void OnCleanup()
        {
            if (_UIModels != null)
            {
                foreach (var item in GetModels())
                {
                    item.Cleanup();
                }

                _UIModels = null;
                _modelLists = null;
            }
        }

        public void MUpdate(float gameTime, float deltaTime)
        {
            foreach (var item in GetModels())
            {
                item.OnUpdate(gameTime, deltaTime);
            }
        }

        private IEnumerable<UIBaseModel> GetModels() //using model list to fix "InvalidOperationException: out of sync"
        {
            if (_modelLists == null)
            {
                _modelLists = new List<UIBaseModel>();
            }

            _modelLists.Clear();
            _modelLists.InsertRange(0, _UIModels.Values);
            return _modelLists;
        }
    }
    
}