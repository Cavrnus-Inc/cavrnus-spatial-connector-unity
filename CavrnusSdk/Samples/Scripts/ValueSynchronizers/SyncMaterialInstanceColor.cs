using UnityEngine;

namespace CavrnusSdk
{
	public class SyncMaterialInstanceColor : CavrnusColorPropertySynchronizer
	{
		public override Color GetValue() { return GetComponent<MeshRenderer>().material.color; }

		public override void SetValue(Color value) { GetComponent<MeshRenderer>().material.color = value; }
	}
}