using UnityEngine;
using System.Collections;

public class CancelPhysicalSizeEnforcer : MonoBehaviour {

	Transform root;
	public Vector3 referenceSize = Vector3.one;
	private Vector3 lastSize;

	void Awake () 
	{
		root = NGUITools.GetRoot(gameObject).transform;
	}

	void Start()
	{
		UpdateSize();
		lastSize = root.localScale;
	}

	void Update ()
	{
		if (root.localScale != lastSize)
		{
			UpdateSize();
			lastSize = root.localScale;
		}
	}

	void UpdateSize()
	{
		transform.localScale = new Vector3(referenceSize.x / root.localScale.x, referenceSize.y / root.localScale.y, referenceSize.z / root.localScale.z);
	}

	[ContextMenu("Initialize with current size")]
	void InitWithCurrentSize()
	{
		Transform root = NGUITools.GetRoot(gameObject).transform;
		referenceSize = root.localScale;
	}
}
