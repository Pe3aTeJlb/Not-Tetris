using UnityEngine;

namespace UnitySpriteCutter.Tools {

	public class RendererParametersRepresentation {
		Material[] sharedMaterials;
		public Material[] materials;
		int sortingOrder;
		int sortingLayerID;
		HideFlags hideFlags;
		bool enabled;

		Sprite sprite;
		Texture2D texture;
		
		public void CopyFrom( SpriteRenderer from ) {
			sharedMaterials = from.sharedMaterials;
			materials[0] = from.materials[0];
			sortingOrder = from.sortingOrder;
			sortingLayerID = from.sortingLayerID;
			hideFlags = from.hideFlags;
			enabled = from.enabled;
			sprite = from.sprite;
			texture = from.sprite.texture;
		}
		
		public void CopyFrom( MeshRenderer from ) {
			sharedMaterials = from.sharedMaterials;
			materials[0] = from.materials[0];
			sortingOrder = from.sortingOrder;
			sortingLayerID = from.sortingLayerID;
			hideFlags = from.hideFlags;
			enabled = from.enabled;
			sprite = null;
			texture = from.material.GetTexture( "_MainTex" ) as Texture2D;
		}
		
		public void PasteTo( SpriteRenderer to ) {
			to.sharedMaterials = sharedMaterials;
			to.materials = materials;
			to.sortingOrder = sortingOrder;
			to.sortingLayerID = sortingLayerID;
			to.hideFlags = hideFlags;
			to.sprite = sprite;
		}
		
		public void PasteTo( MeshRenderer to ) {
			to.sharedMaterials = sharedMaterials;
			to.materials[0] = materials[0];
			to.sortingOrder = sortingOrder;
			to.sortingLayerID = sortingLayerID;
			to.hideFlags = hideFlags;
			to.material.SetTexture( "_MainTex", texture );
			to.enabled = enabled;
		}
	}

}