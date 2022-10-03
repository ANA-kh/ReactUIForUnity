using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace ReactUI
{
    public enum EUIItemExportType
    {
        GameObject,
        Button,
        Label,
        InputField,
        Texture,
        Transform,
        CustomType,
        Animator,
        CanvasGroup,
        RectTransform,
        Grid,
        Toggle,
        ToggleGroup,
        VerticalLayoutGroup,
        HorizontalLayoutGroup,
        Scrollbar,
        ScrollRect,
        Slider,
        RawImage,
        Dropdown,
        TMP_Text,
        TMP_InputField,
        TMP_Dropdown,
    }

    public class UIItemVariable : MonoBehaviour
    {
        public EUIItemExportType ExportType;
        [HideInInspector] [SerializeField] public string CustomExportTypeName;
        public string ExportName;

        [HideInInspector]
        public Object ExportObject;
        public void TryToAttachObject()
        {
            switch (ExportType)
            {
                case EUIItemExportType.GameObject:
                    ExportObject = this.gameObject;
                    break;
                case EUIItemExportType.Transform:
                    ExportObject = this.gameObject.transform;
                    break;
                case EUIItemExportType.Animator:
                    ExportObject = this.gameObject.GetComponent<Animator>();
                    break;
                case EUIItemExportType.CustomType:
                    ExportObject = this.gameObject.GetComponent(CustomExportTypeName);
                    break;
                case EUIItemExportType.Label:
                    ExportObject = this.gameObject.GetComponent<Text>();
                    break;
                case EUIItemExportType.Texture:
                    ExportObject = this.gameObject.GetComponent<Image>();
                    break;
                case EUIItemExportType.CanvasGroup:
                    ExportObject = this.gameObject.GetComponent<CanvasGroup>();
                    break;
                case EUIItemExportType.Button:
                    ExportObject = this.gameObject.GetComponent<Button>();
                    break;
                case EUIItemExportType.RectTransform:
                    ExportObject = this.gameObject.GetComponent<RectTransform>();
                    break;
                case EUIItemExportType.Grid:
                    ExportObject = this.gameObject.GetComponent<GridLayoutGroup>();
                    break;
                case EUIItemExportType.Toggle:
                    ExportObject = this.gameObject.GetComponent<Toggle>();
                    break;
                case EUIItemExportType.ToggleGroup:
                    ExportObject = this.gameObject.GetComponent<ToggleGroup>();
                    break;
                case EUIItemExportType.HorizontalLayoutGroup:
                    ExportObject = this.gameObject.GetComponent<HorizontalLayoutGroup>();
                    break;
                case EUIItemExportType.VerticalLayoutGroup:
                    ExportObject = this.gameObject.GetComponent<VerticalLayoutGroup>();
                    break;
                case EUIItemExportType.InputField:
                    ExportObject = this.gameObject.GetComponent<InputField>();
                    break;
                case EUIItemExportType.Scrollbar:
                    ExportObject = this.gameObject.GetComponent<Scrollbar>();
                    break;
                case EUIItemExportType.ScrollRect:
                    ExportObject = this.gameObject.GetComponent<ScrollRect>();
                    break;
                case EUIItemExportType.Slider:
                    ExportObject = this.gameObject.GetComponent<Slider>();
                    break;
                case EUIItemExportType.RawImage:
                    ExportObject = this.gameObject.GetComponent<RawImage>();
                    break;
                case EUIItemExportType.Dropdown:
                    ExportObject = this.gameObject.GetComponent<Dropdown>();
                    break;
                case EUIItemExportType.TMP_Text:
                    ExportObject = this.gameObject.GetComponent<TMPro.TextMeshProUGUI>();
                    break;
                case EUIItemExportType.TMP_InputField:
                    ExportObject = this.gameObject.GetComponent<TMPro.TMP_InputField>();
                    break;
                case EUIItemExportType.TMP_Dropdown:
                    ExportObject = this.gameObject.GetComponent<TMPro.TMP_Dropdown>();
                    break;
                default:
                    Debug.LogError("UIItemVariable dont implement uitype > " + ExportType);
                    break;
            }

            if (ExportObject == null)
                Debug.LogWarning("UIItemVariable dont get exportobject > " + ExportType);
        }

        public string GetExportedName()
        {
            if (string.IsNullOrEmpty(ExportName))
            {
                return this.gameObject.name;
            }
            return ExportName;
        }

        //将EUIItemExportType类型映射为对应类型
        public string GetEUIItemExportTypeCorrespondOriginTypeName(EUIItemExportType euiItemExportType, UIItemVariable InItemVar)
        {
            var ret = "";
            switch (euiItemExportType)
            {
                case EUIItemExportType.GameObject:
                    ret = "GameObject";
                    break;
                case EUIItemExportType.Label:
                    ret = "Text";
                    break;
                case EUIItemExportType.Transform:
                    ret = "Transform";
                    break;
                case EUIItemExportType.Animator:
                    ret = "Animator";
                    break;
                case EUIItemExportType.CustomType: // Customized export type for variable which will ease the accessing for coding.
                    ret = InItemVar.CustomExportTypeName;
                    break;
                case EUIItemExportType.Texture:
                    ret = "Image";
                    break;
                case EUIItemExportType.CanvasGroup:
                    ret = "CanvasGroup";
                    break;
                case EUIItemExportType.Button:
                    ret = "Button";
                    break;
                case EUIItemExportType.RectTransform:
                    ret = "RectTransform";
                    break;
                case EUIItemExportType.Grid:
                    ret = "GridLayoutGroup";
                    break;
                case EUIItemExportType.Toggle:
                    ret = "Toggle";
                    break;
                case EUIItemExportType.ToggleGroup:
                    ret = "ToggleGroup";
                    break;
                case EUIItemExportType.VerticalLayoutGroup:
                    ret = "VerticalLayoutGroup";
                    break;
                case EUIItemExportType.HorizontalLayoutGroup:
                    ret = "HorizontalLayoutGroup";
                    break;
                case EUIItemExportType.InputField:
                    ret = "InputField";
                    break;
                case EUIItemExportType.Scrollbar:
                    ret = "Scrollbar";
                    break;
                case EUIItemExportType.ScrollRect:
                    ret = "ScrollRect";
                    break;
                case EUIItemExportType.Slider:
                    ret = "Slider";
                    break;
                case EUIItemExportType.RawImage:
                    ret = "RawImage";
                    break;
                case EUIItemExportType.Dropdown:
                    ret = "Dropdown";
                    break;
                case EUIItemExportType.TMP_Text:
                    ret = "TMPro.TextMeshProUGUI";
                    break;
                case EUIItemExportType.TMP_InputField:
                    ret = "TMPro.TMP_InputField";
                    break;
                case EUIItemExportType.TMP_Dropdown:
                    ret = "TMPro.TMP_Dropdown";
                    break;
                default:
                    Debug.LogError("UIItemVariable dont implement uitype > " + ExportType.ToString());
                    break;
            }
            return ret;
        }
    }
}

