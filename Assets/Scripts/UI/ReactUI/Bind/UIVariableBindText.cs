#define UNITY_ASSERTIONS
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace ReactUI
{
    [AddComponentMenu("ReactUI/UI/Bind/Variable Bind TMP_Text")]
    public sealed class UIVariableBindText : UIVariableBind
    {
        [TextArea(2, 10)]
        [Delayed]
        [SerializeField]
        private string format;

        [SerializeField]
        [VariableName(UIVariableType.Boolean, UIVariableType.Integer, UIVariableType.Float, UIVariableType.String, UIVariableType.Array)]
        private string[] paramBinds;

        [SerializeField] 
        private bool setTextNotKey;
        
        private TextMeshProUGUI _text;
        private Text _unityText;

        private UIVariable[] _bindVariable;

        public string Format
        {
            get
            {
                return format;
            }
            set
            {
                if (format != value)
                {
                    format = value;
                    RefreshText();
                }
            }
        }

        protected override void BindVariables()
        {
            Assert.IsNull(_bindVariable);
            if (paramBinds == null || paramBinds.Length <= 0)
            {
                return;
            }
            _bindVariable = new UIVariable[paramBinds.Length];
            for (int i = 0; i < paramBinds.Length; i++)
            {
                string text = paramBinds[i];
                if (!string.IsNullOrEmpty(text))
                {
                    UIVariable uIVariable = FindVariable(text);
                    if (uIVariable == null)
                    {
                        //Debug.LogWarning("{0} can not find a variable {1}", base.name, text);
                        continue;
                    }
                    uIVariable.OnValueInitialized += RefreshText;
                    uIVariable.OnValueChanged += RefreshText;
                    uIVariable.AddBind(this);
                    _bindVariable[i] = uIVariable;
                }
            }
            RefreshText();
        }

        protected override void UnbindVariables()
        {
            if (_bindVariable == null)
            {
                return;
            }
            UIVariable[] array = _bindVariable;
            foreach (UIVariable uIVariable in array)
            {
                if (uIVariable != null)
                {
                    uIVariable.OnValueInitialized -= RefreshText;
                    uIVariable.OnValueChanged -= RefreshText;
                    uIVariable.RemoveBind(this);
                }
            }
            _bindVariable = null;
        }

        [SerializeField]
        string sepForArrayValue = " ";
        private void RefreshText()
        {
            if (_text == null)
            {
                _text = GetComponent<TextMeshProUGUI>();
                if (_unityText == null)
                {
                    _unityText = GetComponent<Text>();
                }
            }

            if ((_text == null && _unityText == null) || paramBinds == null || _bindVariable == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(format))
            {
                if (paramBinds.Length <= 0)
                {
                    return;
                }
                UIVariable variable = _bindVariable[0];
                if (variable != null)
                {
                    if (variable.Type == UIVariableType.Array)
                    {
                        System.Text.StringBuilder sb = new System.Text.StringBuilder();
                        var objList = variable.GetArray();
                        if (objList != null && objList.Count > 0)
                        {
                            for (int i = 0; i < objList.Count; i++)
                            {
                                object v = objList[i];
                                sb.Append(v.ToString()).Append(sepForArrayValue);
                            }
                        }
                        if (_text)
                        {
                            _text.text = (sb.ToString());
                        }
                        if (_unityText)
                        {
                            _unityText.text = (sb.ToString());
                        }
                    }
                    else
                    {
                        object valueObject = variable.ValueObject;
                        if (valueObject != null)
                        {
                            if (_text)
                            {
                                if (setTextNotKey)
                                {
                                    _text.text = valueObject.ToString();
                                }
                                else
                                {
                                    _text.text = valueObject.ToString();
                                }
                            }
                            if (_unityText)
                            {
                                string text = valueObject.ToString();
                                _unityText.text = text;
                            }
                            //m_text.text = (valueObject.ToString());
                        }
                        else
                        {
                            if (_text)
                            {
                                _text.text = String.Empty;
                            }
                            if (_unityText)
                            {
                                _unityText.text = String.Empty;
                            }
                        }
                    }
                }
                return;
            }
            object[] array = new object[paramBinds.Length];
            for (int i = 0; i < paramBinds.Length; i++)
            {
                UIVariable uIVariable2 = _bindVariable[i];
                if (uIVariable2 != null)
                {
                    if (uIVariable2.Type == UIVariableType.String)
                    {
                        array[i] = uIVariable2.ValueObject as string;
                    }
                    else
                    {
                        array[i] = uIVariable2.ValueObject;
                    }
                }
            }
            try
            {
                //m_text.SetLoc(valueObject.ToString());
                if (_text)
                {
                    _text.text = string.Format(format, array);
                }
                if (_unityText)
                {
                    string text = string.Format(format, array);
                    _unityText.text = text;
                }
            }
            catch (FormatException ex)
            {
                if (Application.isPlaying)
                {
                    Debug.LogError(ex.Message, this);
                }
            }
        }

        private new void Awake()
        {
            base.Awake();
            _text = GetComponent<TextMeshProUGUI>();
            _unityText = GetComponent<Text>();
        }
    }
}
