using UnityEditor;
using UnityEngine;

namespace Bomberman.GameManager
{
	[CustomPropertyDrawer(typeof(OddRangeAttribute))]
	public class OddRangeDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (property.propertyType != SerializedPropertyType.Integer)
			{
				EditorGUI.LabelField(position, label.text, "OddRange is only compatible with int");
				return;
			}
			
			OddRangeAttribute range = (OddRangeAttribute)attribute;

			int currentValue = property.intValue;

			currentValue = Mathf.Clamp(currentValue, range.Min, range.Max);

			if ((currentValue & 1) == 0)
			{
				currentValue++;
			}

			int newValue = EditorGUI.IntField(position, label.text, currentValue);
			
			if ((newValue & 1) == 0)
			{
				if (newValue < currentValue)
				{
					newValue--;
				}
				else
				{
					newValue++;
				}
			}

			newValue = Mathf.Clamp(newValue, range.Min, range.Max);

			property.intValue = newValue;
		}
	}
}