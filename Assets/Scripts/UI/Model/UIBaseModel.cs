using System.Collections.Generic;

namespace UI
{
    public interface IUIModelDataChangeObserver
    {
        void OnDataChanged(UIBaseModel model, uint propID, params object[] param);
        uint GetInterestedPropID(UIBaseModel model);
    }

    public abstract class UIBaseModel
    {
        private List<IUIModelDataChangeObserver> _observers;
        private List<IUIModelDataChangeObserver> _workingObservers;

        public virtual void Init()
        {

        }
        public void RegisterDataChangedNotification(IUIModelDataChangeObserver observer)
        {
            if (_observers == null)
            {
                _observers = new List<IUIModelDataChangeObserver>();
            }
            if (!_observers.Contains(observer))
            {
                _observers.Add(observer);
            }
        }
        public void UnRegisterDataChangedNotification(IUIModelDataChangeObserver observer)
        {
            if (_observers == null)
            {
                return;
            }
            _observers.Remove(observer);
        }

        protected virtual void NotifyDataChanged(uint propID, params object[] param)
        {
            if (_observers == null)
            {
                return;
            }

            if (_workingObservers == null)
            {
                _workingObservers = new List<IUIModelDataChangeObserver>();
            }
            _workingObservers.Clear();
            _workingObservers.AddRange(_observers);
            foreach (var observer in _workingObservers)
            {
                if (observer != null && BitArray.HasFlag(propID, observer.GetInterestedPropID(this)))
                {
                    observer.OnDataChanged(this, propID, param);
                }
            }
            _workingObservers.Clear();
        }
        public abstract uint GetModelType();
        
        public void Cleanup()
        {
            _observers = null; //clear all observers
            OnCleanup();
        }
        protected virtual void OnCleanup()
        {

        }
        public virtual void OnUpdate(float gameTime, float deltaTime)
        {

        }
    }
    public enum EUIModelType
    {
        LootWindow
    }
}
