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
        
        private TextMeshProUGUI m_text;
        private Text unityText;

        private UIVariable[] m_bindVariable;

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
            Assert.IsNull(m_bindVariable);
            if (paramBinds == null || paramBinds.Length <= 0)
            {
                return;
            }
            m_bindVariable = new UIVariable[paramBinds.Length];
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
                    m_bindVariable[i] = uIVariable;
                }
            }
            RefreshText();
        }

        protected override void UnbindVariables()
        {
            if (m_bindVariable == null)
            {
                return;
            }
            UIVariable[] array = m_bindVariable;
            foreach (UIVariable uIVariable in array)
            {
                if (uIVariable != null)
                {
                    uIVariable.OnValueInitialized -= RefreshText;
                    uIVariable.OnValueChanged -= RefreshText;
                    uIVariable.RemoveBind(this);
                }
            }
            m_bindVariable = null;
        }

        [SerializeField]
        string sepForArrayValue = " ";
        private void RefreshText()
        {
            if (m_text == null)
            {
                m_text = GetComponent<TextMeshProUGUI>();
                if (unityText == null)
                {
                    unityText = GetComponent<Text>();
                }
            }

            if ((m_text == null && unityText == null) || paramBinds == null || m_bindVariable == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(format))
            {
                if (paramBinds.Length <= 0)
                {
                    return;
                }
                UIVariable variable = m_bindVariable[0];
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
                        if (m_text)
                        {
                            m_text.text = (sb.ToString());
                        }
                        if (unityText)
                        {
                            unityText.text = (sb.ToString());
                        }
                    }
                    else
                    {
                        object valueObject = variable.ValueObject;
                        if (valueObject != null)
                        {
                            if (m_text)
                            {
                                if (setTextNotKey)
                                {
                                    m_text.text = valueObject.ToString();
                                }
                                else
                                {
                                    m_text.text = valueObject.ToString();
                                }
                            }
                            if (unityText)
                            {
                                string text = valueObject.ToString();
                                unityText.text = text;
                            }
                            //m_text.text = (valueObject.ToString());
                        }
                        else
                        {
                            if (m_text)
                            {
                                m_text.text = String.Empty;
                            }
                            if (unityText)
                            {
                                unityText.text = String.Empty;
                            }
                        }
                    }
                }
                return;
            }
            object[] array = new object[paramBinds.Length];
            for (int i = 0; i < paramBinds.Length; i++)
            {
                UIVariable uIVariable2 = m_bindVariable[i];
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
                if (m_text)
                {
                    m_text.text = string.Format(format, array);
                }
                if (unityText)
                {
                    string text = string.Format(format, array);
                    unityText.text = text;
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
            m_text = GetComponent<TextMeshProUGUI>();
            unityText = GetComponent<Text>();
        }
    }
}
