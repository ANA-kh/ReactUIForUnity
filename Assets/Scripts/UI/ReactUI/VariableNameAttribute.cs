using UnityEngine;

namespace ReactUI
{
	public sealed class VariableNameAttribute : PropertyAttribute
	{
		private int _typeMask;

		public VariableNameAttribute()
		{
			_typeMask = -1;
		}

		public VariableNameAttribute(UIVariableType t1)
		{
			_typeMask = 1 << (int)t1;
		}

		public VariableNameAttribute(UIVariableType t1, UIVariableType t2)
		{
			_typeMask = ((1 << (int)t1) | (1 << (int)t2));
		}

		public VariableNameAttribute(UIVariableType t1, UIVariableType t2, UIVariableType t3)
		{
			_typeMask = ((1 << (int)t1) | (1 << (int)t2) | (1 << (int)t3));
		}

		public VariableNameAttribute(UIVariableType t1, UIVariableType t2, UIVariableType t3, UIVariableType t4)
		{
			_typeMask = ((1 << (int)t1) | (1 << (int)t2) | (1 << (int)t3) | (1 << (int)t4));
		}
		public VariableNameAttribute(UIVariableType t1, UIVariableType t2, UIVariableType t3, UIVariableType t4, UIVariableType t5)
		{
			_typeMask = ((1 << (int)t1) | (1 << (int)t2) | (1 << (int)t3) | (1 << (int)t4) | (1 << (int)t5));
		}

		public bool IsValid(UIVariableType type)
		{
			return (_typeMask & (1 << (int)type)) != 0;
		}
	}
	public sealed class AutoBindVariableAttribute : PropertyAttribute
    {

    }
	
	public sealed class AutoBindGameObjectAttribute : PropertyAttribute
	{
		
	}
}
