using System;
// 可使用BindableProperty 通过双向绑定，改造ReactUi整体结构
public interface IUnRegister
{
    void UnRegister();
}

public class BindableProperty<T>
{
    public BindableProperty(T defaultValue = default)
    {
        _value = defaultValue;
    }

    private T _value = default(T);
    //public Action<T> OnValueChanged;

    public T Value
    {
        get => _value;
        set
        {
            if (value == null && _value == null) return;
            if (value != null && value.Equals(_value)) return;

            _value = value;
            _onValueChanged?.Invoke(value);
        }
    }

    private Action<T> _onValueChanged = v => { };

    public IUnRegister Register(Action<T> onValueChanged)
    {
        _onValueChanged += onValueChanged;
        return new BindablePropertyUnRegister<T>()
        {
            BindableProperty = this,
            OnValueChanged = onValueChanged,
        };
    }

    public IUnRegister RegisterWithInitValue(Action<T> onValueChanged)
    {
        onValueChanged(_value);
        return Register(onValueChanged);
    }

    public void UnRegister(Action<T> onValueChanged)
    {
        _onValueChanged -= onValueChanged;
    }

    public static implicit operator T(BindableProperty<T> property)
    {
        return property.Value;
    }
}

public class BindablePropertyUnRegister<T> : IUnRegister
{
    public BindableProperty<T> BindableProperty { get; set; }

    public Action<T> OnValueChanged { get; set; }

    public void UnRegister()
    {
        BindableProperty.UnRegister(OnValueChanged);

        BindableProperty = null;
        OnValueChanged = null;
    }
}