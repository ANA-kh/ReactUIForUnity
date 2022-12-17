using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace ReactUI
{
    [AddComponentMenu("ReactUI/UI/Bind/Variable Bind Color By Integer")]
    [RequireComponent(typeof(Graphic))]
    public sealed class UIVariableBindColorByInteger : UIVariableBind
    {
        [Serializable]
        struct ColorPair
        {
            public int index;
            public Color color;
        }

        [SerializeField]
        [VariableName(UIVariableType.Integer)]
        private string integerBind;

        [SerializeField]
        private ColorPair[] _colors;
        private Graphic _graphic;
        private UIVariable _bindVariable;

        protected override void BindVariables()
        {
            Assert.IsNull(_bindVariable);
            if (!string.IsNullOrEmpty(integerBind))
            {
                _bindVariable = FindVariable(integerBind);
                if (_bindVariable == null)
                {
                    Debug.LogWarning(string.Format("{0} can not find a variable {1}", base.name, integerBind));
                    return;
                }
                _bindVariable.OnValueInitialized += OnValueChanged;
                _bindVariable.OnValueChanged += OnValueChanged;
                _bindVariable.AddBind(this);
                OnValueChanged();
            }
        }

        protected override void UnbindVariables()
        {
            if (_bindVariable != null)
            {
                _bindVariable.OnValueInitialized -= OnValueChanged;
                _bindVariable.OnValueChanged -= OnValueChanged;
                _bindVariable.RemoveBind(this);
                _bindVariable = null;
            }
        }

        private void OnValueChanged()
        {
            if (_graphic == null)
            {
                _graphic = GetComponent<Graphic>();
            }
            if (_graphic && _colors != null)
            {
                int integer = _bindVariable.GetInteger();
                foreach (var item in _colors)
                {
                    if (item.index == integer)
                    {
                        _graphic.color = item.color;
                        break;
                    }
                }
            }
        }
    }
}
