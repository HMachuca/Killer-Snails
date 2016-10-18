using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class UIColorGroup
{
	public string name;
	public List<GameObject> objects;
	public Color color = Color.white;

	public void ApplyColor()
	{
		foreach(GameObject go in objects)
		{
			ApplyColor(go);
		}
	}
	
	private void ApplyColor(GameObject go)
	{
		if (go == null)
			return;
		UIButton button = go.GetComponent<UIButton>();
		if (button != null)
		{
			button.defaultColor = new Color(color.r, color.g, color.b, button.defaultColor.a);
			button.hover = new Color(color.r, color.g, color.b, button.hover.a);
			button.pressed = new Color(color.r - 0.1f, color.g - 0.1f, color.b - 0.1f, button.pressed.a);
		}
		UIWidget widget = go.GetComponent<UIWidget>();
		if (widget != null)
		{
			widget.color = new Color(color.r, color.g, color.b, widget.color.a);
		}
	}
}

[ExecuteInEditMode]
public class ColorizedUI : MonoBehaviour {

	public List<UIColorGroup> colorGroups;

	[ContextMenu("Update UI")]
	public void UpdateUI ()
	{
		foreach(UIColorGroup colorGroup in colorGroups)
		{
			colorGroup.ApplyColor();
		}
	}
//#if UNITY_EDITOR
	void Update()
	{
		UpdateUI ();
#if !UNITY_EDITOR
		this.enabled = false;
#endif
	}
//#endif

}
