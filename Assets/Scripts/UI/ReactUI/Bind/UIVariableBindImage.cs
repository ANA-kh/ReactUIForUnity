using ResourceLoad_Temp;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace ReactUI
{
    [AddComponentMenu("ReactUI/UI/Bind/Variable Bind Image")]
    public class UIVariableBindImage : UIVariableBind
    {
        private static IResourceLoad _resources = ResourceManager.Instance.ResourceLoader;

        [SerializeField] [VariableName(UIVariableType.String)]
        private string _spriteBindName;

        [SerializeField] [VariableName(UIVariableType.Float)]
        private string _fillAmountBindName;

        [SerializeField] private bool autoFitNativeSize;

        [SerializeField] private bool autoDisable;

        private Image _image;
        private RawImage _rawImage;
        private bool _refreshImageOnInitial = true;
        
        private UIVariable _imageVariable;
        private UIVariable _fillAmountVariable;

        protected override void BindVariables()
        {
            Assert.IsNull(_imageVariable);
            Assert.IsNull(_fillAmountVariable);
            if (!string.IsNullOrEmpty(_spriteBindName))
            {
                _imageVariable = FindVariable(_spriteBindName);
                if (_imageVariable == null)
                {
                    Debug.LogError($"{name}can not find variable {_spriteBindName}");
                }
                else
                {
                    _imageVariable.OnValueInitialized += RefreshImage;
                    _imageVariable.OnValueChanged += RefreshImage;
                    _imageVariable.AddBind(this);
                    
                    if (Application.isPlaying)
                    {
                        if (_refreshImageOnInitial)
                        {
                            RefreshImage();
                        }
                    }
                    else
                    {
                        RefreshImage();
                    }
                }
            }
            if (!string.IsNullOrEmpty(_fillAmountBindName))
            {
                _fillAmountVariable = FindVariable(_fillAmountBindName);
                if (_fillAmountVariable == null)
                {
                    //.LogWarning("{0} can not find a variable {1}", base.name, spriteBind);
                    return;
                }
                _fillAmountVariable.OnValueInitialized += RefreshFillAmount;
                _fillAmountVariable.OnValueChanged += RefreshFillAmount;
                _fillAmountVariable.AddBind(this);

                if (Application.isPlaying)
                {
                    if (_refreshImageOnInitial)
                    {
                        RefreshFillAmount();
                    }
                }
                else
                {
                    RefreshFillAmount();
                }
            }
            
        }

        protected override void UnbindVariables()
        {
            if (_imageVariable != null)
            {
                
            }
            if (_imageVariable != null)
            {
                _imageVariable.OnValueInitialized -= RefreshImage;
                _imageVariable.OnValueChanged -= RefreshImage;
                _imageVariable.RemoveBind(this);
                _imageVariable = null;
            }
            if (_fillAmountVariable != null)
            {
                _fillAmountVariable.OnValueInitialized -= RefreshFillAmount;
                _fillAmountVariable.OnValueChanged -= RefreshFillAmount;
                _fillAmountVariable.RemoveBind(this);
                _fillAmountVariable = null;
            }
        }

        private void RefreshImage()
        {
            _image ??= GetComponent<Image>();
            if (_image == null && _rawImage == null)
            {
                _rawImage = GetComponent<RawImage>();
            }

            var spriteName = _imageVariable.GetString();

            if (_image != null)
            {
                if (string.IsNullOrEmpty(spriteName))
                {
                    _image.sprite = null;
                }
                else
                {
                    _image.sprite = _resources.Load<Sprite>(spriteName);
                }
                RefreshDisableStatus(_image);
            }

            if (_rawImage != null)
            {
                var tex = _resources.Load<Texture>(spriteName);
                if (tex != null)
                {
                    _rawImage.texture = tex;
                    _rawImage.enabled = true;
                    if (autoFitNativeSize)
                    {
                        _rawImage.SetNativeSize();
                    }
                }
                else
                {
                    _rawImage.enabled = false;
                }
            }
        }
        private void RefreshFillAmount()
        {
            if (_image == null)
            {
                _image = GetComponent<Image>();
            }

            var f = _fillAmountVariable.GetFloat();
            _image.fillAmount = f;
        }

        private void RefreshDisableStatus(Image img)
        {
            if (img == null)
            {
                return;
            }
            if (autoFitNativeSize)
            {
                img.SetNativeSize();
            }
            if (autoDisable)
            {
                img.enabled = img.sprite != null;
            }
        }
        
        private new void Awake()
        {
            base.Awake();
            _image = GetComponent<Image>();
            if (_image != null)
            {
                RefreshDisableStatus(_image);
            }
        }
    }
}