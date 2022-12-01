#define UNITY_ASSERTIONS
using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Assertions;

namespace ReactUI
{
    public abstract class UIVariableBindBool : UIVariableBind
    {
        private enum BooleanLogic
        {
            And,
            Or
        }

        private enum CompareModeEnum
        {
            Less,
            LessEqual,
            Equal,
            Great,
            GreatEqual
        }

        [Serializable]
        private class OneVar
        {
            [SerializeField]
            [VariableName(UIVariableType.Boolean, UIVariableType.Integer, UIVariableType.Float, UIVariableType.String)]
            private string variableName;

            [SerializeField]
            private CompareModeEnum compareMode = CompareModeEnum.Equal;

            [SerializeField]
            private int referenceInt;

            [SerializeField]
            private float referenceFloat;

            [SerializeField]
            [VariableName(UIVariableType.Boolean, UIVariableType.Integer, UIVariableType.Float)]
            private string referenceVariableName;
            [SerializeField]
            private bool reverse;

            private UIVariable _ownerVar;
            private UIVariable _referenceVar;
            public UIVariable ReferenceVar
            {
                get => _referenceVar;
                set => _referenceVar = value;
            }
            public string GetRefVariableName()
            {
                return referenceVariableName;
            }

            public string GetVariableName()
            {
                return variableName;
            }

            public UIVariable GetVariable()
            {
                return _ownerVar;
            }

            public void SetVariable(UIVariable v)
            {
                _ownerVar = v;
            }

            public bool GetValue()
            {
                if (GetVariable() == null)
                {
                    return false;
                }
                if (GetVariable().Type == UIVariableType.Boolean)
                {
                    bool flag = GetVariable().GetBoolean();
                    if (reverse)
                    {
                        flag = !flag;
                    }
                    return flag;
                }
                if (GetVariable().Type == UIVariableType.Integer)
                {
                    long integer = GetVariable().GetLongVal();
                    long cmpValue = referenceInt;
                    if (ReferenceVar != null)
                    {
                        cmpValue = ReferenceVar.GetLongVal();
                    }
                    bool flag2 = false;
                    switch (compareMode)
                    {
                        case CompareModeEnum.Less:
                            flag2 = (integer < cmpValue);
                            break;
                        case CompareModeEnum.LessEqual:
                            flag2 = (integer <= cmpValue);
                            break;
                        case CompareModeEnum.Equal:
                            flag2 = (integer == cmpValue);
                            break;
                        case CompareModeEnum.Great:
                            flag2 = (integer > cmpValue);
                            break;
                        case CompareModeEnum.GreatEqual:
                            flag2 = (integer >= cmpValue);
                            break;
                    }
                    if (reverse)
                    {
                        flag2 = !flag2;
                    }
                    return flag2;
                }
                if (GetVariable().Type == UIVariableType.Float)
                {
                    float @float = GetVariable().GetFloat();
                    float cmpValue = referenceFloat;
                    if (ReferenceVar != null)
                    {
                        cmpValue = ReferenceVar.GetFloat();
                    }
                    bool flag3 = false;
                    switch (compareMode)
                    {
                        case CompareModeEnum.Less:
                            flag3 = (@float < cmpValue);
                            break;
                        case CompareModeEnum.LessEqual:
                            flag3 = (@float <= cmpValue);
                            break;
                        case CompareModeEnum.Equal:
                            flag3 = Mathf.Approximately(@float, cmpValue);
                            break;
                        case CompareModeEnum.Great:
                            flag3 = (@float > cmpValue);
                            break;
                        case CompareModeEnum.GreatEqual:
                            flag3 = (@float >= cmpValue);
                            break;
                    }
                    if (reverse)
                    {
                        flag3 = !flag3;
                    }
                    return flag3;
                }

                if (GetVariable().Type == UIVariableType.String)
                {
                    var str = GetVariable().GetString();
                    var flag = !string.IsNullOrEmpty(str);
                    if (reverse)
                    {
                        flag = !flag;
                    }
                    return flag;
                }
                //_2011_206E.LogError("Variable {0} type is {1}, does not support this variable type.", GetVariable().Name, GetVariable().Type);
                return false;
            }
        }
        

        [SerializeField]
        [Tooltip("The boolean logic.")]
        private BooleanLogic booleanLogic;

        [SerializeField]
        [Tooltip("The variables for calculate the boolean value.")]
        private OneVar[] variables;

        [SerializeField]
        [Tooltip("Is Reverse")]
        private bool reverse;

        public new UIVariable FindVariable(string name)
        {
            return base.FindVariable(name);
        }

        protected bool GetResult()
        {
            var result = GetResultInternal();
            if (reverse)
            {
                result = !result;
            }
            return result;
        }

        private bool GetResultInternal()
        {
            if (variables == null)
            {
                return false;
            }
            if (booleanLogic == BooleanLogic.And)
            {
                bool flag = true;
                OneVar[] array = variables;
                foreach (OneVar OneVar in array)
                {
                    if (OneVar != null)
                    {
                        flag = (flag && OneVar.GetValue());
                    }
                }
                return flag;
            }
            bool flag2 = false;
            OneVar[] arr = variables;
            foreach (OneVar v in arr)
            {
                if (v != null)
                {
                    flag2 = (flag2 || v.GetValue());
                }
            }
            return flag2;
        }

        protected abstract void OnValueChanged();

        protected override void BindVariables()
        {
            if (variables != null)
            {
                OneVar[] array = variables;
                foreach (OneVar OneVar in array)
                {
                    Assert.IsNull(OneVar.GetVariable());
                    if (!string.IsNullOrEmpty(OneVar.GetVariableName()))
                    {
                        OneVar.SetVariable(FindVariable(OneVar.GetVariableName()));
                        if (OneVar.GetVariable() == null)
                        {
                            //Debugger.LogWarning("{0} can not find a variable {1}", base.name, OneVar.Get());
                            continue;
                        }
                        OneVar.GetVariable().OnValueInitialized += OnValueChanged;
                        OneVar.GetVariable().OnValueChanged += OnValueChanged;
                        OneVar.GetVariable().AddBind(this);
                    }
                    if (!string.IsNullOrEmpty(OneVar.GetRefVariableName()))
                    {
                        OneVar.ReferenceVar = FindVariable(OneVar.GetRefVariableName());
                        if (OneVar.ReferenceVar == null)
                        {
                            //Debugger.LogWarning("{0} can not find a variable {1}", base.name, OneVar.Get());
                            continue;
                        }
                        OneVar.ReferenceVar.OnValueInitialized += OnValueChanged;
                        OneVar.ReferenceVar.OnValueChanged += OnValueChanged;
                        OneVar.ReferenceVar.AddBind(this);
                    }
                }
            }
            OnValueChanged();
        }

        protected override void UnbindVariables()
        {
            if (variables == null)
            {
                return;
            }
            OneVar[] array = variables;
            foreach (OneVar OneVar in array)
            {
                if (OneVar.GetVariable() != null)
                {
                    OneVar.GetVariable().OnValueInitialized -= OnValueChanged;
                    OneVar.GetVariable().OnValueChanged -= OnValueChanged;
                    OneVar.GetVariable().RemoveBind(this);
                    OneVar.SetVariable(null);
                }
                if (OneVar.ReferenceVar != null)
                {
                    OneVar.ReferenceVar.OnValueInitialized -= OnValueChanged;
                    OneVar.ReferenceVar.OnValueChanged -= OnValueChanged;
                    OneVar.ReferenceVar.RemoveBind(this);
                    OneVar.ReferenceVar = null;
                }
            }
        }
    }
}
