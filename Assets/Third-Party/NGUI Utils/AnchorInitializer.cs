using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent(typeof(UIWidget))]
/// <summary>
/// Use this class to automatically initialize the anchors of a UIWidget.
/// For use on a prefab meant to be added to a UIRoot, so it can be properly linked with the root automatically.
/// For now it always anchors to fill the screen, but options could be added.
/// </summary>
public class AnchorInitializer : MonoBehaviour {

	UIWidget _widget;
	UIWidget widget
	{
		get
		{
			if (_widget == null)
				_widget = GetComponent<UIWidget>();
			return _widget;
		}
	}

	void Update () 
	{
		if (widget == null || widget.isAnchored)
			return;
		Initialize();
	}

	[ContextMenu("Update Anchor")]
	void Initialize()
	{
		widget.SetAnchor(NGUITools.GetRoot(gameObject).GetComponentInChildren<UICamera>().gameObject);
		widget.leftAnchor.relative = 0f;
		widget.rightAnchor.relative = 1f;
		widget.bottomAnchor.relative = 0f;
		widget.topAnchor.relative = 1f;
		widget.leftAnchor.absolute = widget.rightAnchor.absolute = widget.bottomAnchor.absolute = widget.topAnchor.absolute = 0;
		transform.localScale = Vector3.one;
	}
}
