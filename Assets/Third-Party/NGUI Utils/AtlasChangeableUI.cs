using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class AtlasChangeableUI : MonoBehaviour {

	public List<UIAtlas> atlases = new List<UIAtlas>();
	
	[ContextMenu("Find All Atlases")]
	public void FindAllAtlases ()
	{
		//atlases.Clear();
		//foreach(UISprite sprite in GetComponentsInChildren<UISprite>(true))
		//{
		//	if (sprite.atlas != null && sprite.GetComponent<ToyDependantAtlas>() == null && !atlases.Contains(sprite.atlas))
		//		atlases.Add(sprite.atlas);
		//}
	}

	[ContextMenu("Update Atlases")]
	public void UpdateAtlases ()
	{
		foreach(UIAtlas atlas in atlases)
		{
			foreach(UISprite sprite in GetComponentsInChildren<UISprite>(true))
			{
				if (sprite.atlas.name == atlas.name && sprite.atlas != atlas)
					sprite.atlas = atlas;
			}
		}
	}

}
