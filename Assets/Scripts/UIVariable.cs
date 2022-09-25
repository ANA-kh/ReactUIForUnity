using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace ReactUI
{
    public enum UIVariableType
    {
        Boolean = 0,
        Integer = 1,
        Float = 2 ,
        String = 3,
        // Asset = 4,
        Array = 5,
        Object = 6,
    }
    [Serializable]
    public sealed class UIVariable
    {
        [SerializeField]
        private string name;

        [SerializeField]
        private UIVariableType type;

        [SerializeField]
        private bool booleanValue;

        [SerializeField]
        private long integerValue;

        [SerializeField]
        private float floatValue;

        [SerializeField]
        private string stringValue;

        [SerializeField]
        private IList arrayValue;

        [SerializeField]
        private object objectValue;

        private List<UIVariableBind> binderList = new List<UIVariableBind>();

        private Action actionOnValueChanged;

        private Action actionOnValueInit;

        public string Name => name;

        public UIVariableType Type => type;

        public object ValueObject
        {
            get
            {
                switch (type)
                {
                    case UIVariableType.Boolean:
                        return booleanValue;
                    case UIVariableType.Integer:
                        return integerValue;
                    case UIVariableType.Float:
                        return floatValue;
                    case UIVariableType.String:
                        return stringValue;
                    case UIVariableType.Array:
                        return arrayValue;
                    case UIVariableType.Object:
                        return objectValue;
                    default:
                        return null;
                }
            }
        }

        public ICollection<UIVariableBind> Binds => binderList;

        #region Bind and Invoke

        public event Action OnValueChanged
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            add
            {
                actionOnValueChanged = (Action)Delegate.Combine(actionOnValueChanged, value);
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            remove
            {
                actionOnValueChanged = (Action)Delegate.Remove(actionOnValueChanged, value);
            }
        }

        public event Action OnValueInitialized
        {
            [MethodImpl(MethodImplOptions.Synchronized)]//可以确保在不同线程中运行的该方式以同步的方式运行
            add => actionOnValueInit = (Action)Delegate.Combine(actionOnValueInit, value);
            [MethodImpl(MethodImplOptions.Synchronized)]
            remove => actionOnValueInit = (Action)Delegate.Remove(actionOnValueInit, value);
        }
        internal void InvokeValueChange()
        {
            if (actionOnValueChanged != null)
            {
                actionOnValueChanged();
            }
        }

        internal void InvokeValueInit()
        {
            if (actionOnValueInit != null)
            {
                actionOnValueInit();
            }
        }
        
        //binderList只用于显示和点击后快速聚焦绑定的对象
        public void AddBind(UIVariableBind bind)
        {
            if (binderList.IndexOf(bind) == -1)
            {
                binderList.Add(bind);
            }
        }

        public void RemoveBind(UIVariableBind bind)
        {
            binderList.Remove(bind);
        }
        internal void ClearBinderList()
        {
            binderList.RemoveAll((UIVariableBind P_0) => P_0 == null);
        }

        #endregion

        #region Get\Set\Reset Values

         public bool GetBoolean()
        {
            return booleanValue;
        }

        public int GetInteger()
        {
            return (int)integerValue;
        }

        public long GetLongVal()
        {
            return integerValue;
        }

        public float GetFloat()
        {
            return floatValue;
        }

        public string GetString()
        {
            return stringValue;
        }

        public IList GetArray()
        {
            return arrayValue;
        }

        public object GetObject()
        {
            return objectValue;
        }

        public void InitBoolean(bool value)
        {
            if (booleanValue != value)
            {
                booleanValue = value;
                InvokeValueInit();
            }
        }

        public void InitInteger(long value)
        {
            if (integerValue != value)
            {
                integerValue = value;
                InvokeValueInit();
            }
        }

        public void InitFloat(float value)
        {
            if (floatValue != value)
            {
                floatValue = value;
                InvokeValueInit();
            }
        }

        public void InitString(string value)
        {
            if (stringValue != value)
            {
                stringValue = value;
                InvokeValueInit();
            }
        }
        public void InitArray(IList arr)
        {
            arrayValue = arr;
            InvokeValueInit();
        }

        public void InitObject(object obj)
        {
            objectValue = obj;
            InvokeValueInit();
        }

        public void SetBoolean(bool value, bool forceSet = false)
        {
            if (booleanValue != value || forceSet)
            {
                booleanValue = value;
                InvokeValueChange();
            }
        }

        public void SetInteger(long value, bool forceSet = false)
        {
            if (integerValue != value || forceSet)
            {
                integerValue = value;
                InvokeValueChange();
            }
        }

        public void SetFloat(float value, bool forceSet = false)
        {
            if (Math.Abs(floatValue - value) > 0.001f || forceSet)
            {
                floatValue = value;
                InvokeValueChange();
            }
        }

        public void SetString(string value, bool forceSet = false)
        {
            if (stringValue != value || forceSet)
            {
                stringValue = value;
                InvokeValueChange();
            }
        }

        public void SetArray(IList value)
        {
            arrayValue = value;
            InvokeValueChange();
        }

        public void SetObject(object obj)
        {
            objectValue = obj;
            InvokeValueChange();
        }

        public void InitValue(bool value)
        {
            switch (type)
            {
                case UIVariableType.Boolean:
                    InitBoolean(value);
                    break;
                case UIVariableType.Integer:
                    InitInteger(value ? 1 : 0);
                    break;
                case UIVariableType.Float:
                    InitFloat((!value) ? 0f : 1f);
                    break;
                case UIVariableType.String:
                    InitString(value.ToString());
                    break;
            }
        }

        public void InitValue(long value)
        {
            switch (type)
            {
                case UIVariableType.Boolean:
                    InitBoolean(value != 0);
                    break;
                case UIVariableType.Integer:
                    InitInteger(value);
                    break;
                case UIVariableType.Float:
                    InitFloat(value);
                    break;
                case UIVariableType.String:
                    InitString(value.ToString());
                    break;
            }
        }

        public void InitValue(float value)
        {
            switch (type)
            {
                case UIVariableType.Boolean:
                    InitBoolean(!Mathf.Approximately(value, 0f));
                    break;
                case UIVariableType.Integer:
                    InitInteger((long)value);
                    break;
                case UIVariableType.Float:
                    InitFloat(value);
                    break;
                case UIVariableType.String:
                    InitString(value.ToString());
                    break;
            }
        }

        public void InitValue(string value)
        {
            switch (type)
            {
                case UIVariableType.Boolean:
                    InitBoolean(bool.Parse(value));
                    break;
                case UIVariableType.Integer:
                    InitInteger(long.Parse(value));
                    break;
                case UIVariableType.Float:
                    InitFloat(float.Parse(value));
                    break;
                case UIVariableType.String:
                    InitString(value);
                    break;
            }
        }

        public void SetValue(bool value)
        {
            switch (type)
            {
                case UIVariableType.Boolean:
                    SetBoolean(value);
                    break;
                case UIVariableType.Integer:
                    SetInteger(value ? 1 : 0);
                    break;
                case UIVariableType.Float:
                    SetFloat((!value) ? 0f : 1f);
                    break;
                case UIVariableType.String:
                    SetString(value.ToString());
                    break;
            }
        }

        public void SetValue(long value)
        {
            switch (type)
            {
                case UIVariableType.Boolean:
                    SetBoolean(value != 0);
                    break;
                case UIVariableType.Integer:
                    SetInteger(value);
                    break;
                case UIVariableType.Float:
                    SetFloat(value);
                    break;
                case UIVariableType.String:
                    SetString(value.ToString());
                    break;
            }
        }

        public void SetValue(float value)
        {
            switch (type)
            {
                case UIVariableType.Boolean:
                    SetBoolean(!Mathf.Approximately(value, 0f));
                    break;
                case UIVariableType.Integer:
                    SetInteger((long)value);
                    break;
                case UIVariableType.Float:
                    SetFloat(value);
                    break;
                case UIVariableType.String:
                    SetString(value.ToString());
                    break;
            }
        }

        public void SetValue(string value)
        {
            switch (type)
            {
                case UIVariableType.Boolean:
                    SetBoolean(bool.Parse(value));
                    break;
                case UIVariableType.Integer:
                    SetInteger(long.Parse(value));
                    break;
                case UIVariableType.Float:
                    SetFloat(float.Parse(value));
                    break;
                case UIVariableType.String:
                    SetString(value);
                    break;
            }
        }

        public void SetValue(object value)
        {
            try
            {
                switch (type)
                {
                    case UIVariableType.Boolean:
                        SetBoolean((bool)value);
                        break;
                    case UIVariableType.Integer:
                        var longValue = System.Convert.ToInt64(value.ToString());
                        SetInteger(longValue);
                        break;
                    case UIVariableType.Float:
                        SetFloat((float)value);
                        break;
                    case UIVariableType.String:
                        SetString(value.ToString());
                        break;
                    case UIVariableType.Array:
                        SetArray((IList)value);
                        break;
                    case UIVariableType.Object:
                        SetObject(value);
                        break;
                }
            }
            catch (System.Exception)
            {
            }

        }
        
        internal void ResetValue()
        {
            switch (type)
            {
                case UIVariableType.Boolean:
                    integerValue = 0L;
                    floatValue = 0f;
                    stringValue = null;
                    break;
                case UIVariableType.Integer:
                    booleanValue = false;
                    floatValue = 0f;
                    stringValue = null;
                    break;
                case UIVariableType.Float:
                    booleanValue = false;
                    integerValue = 0L;
                    stringValue = null;
                    break;
                case UIVariableType.String:
                    booleanValue = false;
                    integerValue = 0L;
                    floatValue = 0f;
                    break;
                case UIVariableType.Array:
                    booleanValue = false;
                    integerValue = 0L;
                    floatValue = 0f;
                    stringValue = null;
                    if (arrayValue != null)
                    {
                        arrayValue.Clear();
                    }
                    break;
                case UIVariableType.Object:
                    booleanValue = false;
                    integerValue = 0L;
                    floatValue = 0f;
                    arrayValue = null;
                    stringValue = null;
                    objectValue = null;
                    break;
            }
        }

        #endregion
        
    }

    public class UIVariableBind : MonoBehaviour
    {
    }
}
