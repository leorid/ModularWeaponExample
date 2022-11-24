
using UnityEngine;

namespace JL.Weapon.Subsystem
{
	public class HitParticles : WeaponSubsystemBase
	{
		public GameObject particlesPrefab;

		public override void OnHit(RaycastHit hitInfo)
		{
			Vector3 position = hitInfo.point;
			Quaternion rotation =
				Quaternion.FromToRotation(Vector3.forward, hitInfo.normal);

			// "HolderManager" creates Parent-Transforms (Holders)
			// on demand to keep the hierarchy clean (custom Tool)
			Transform parent = HolderManager.Particles;

			// "ObjectPool" is a simple static pooling class (custom Tool)
			ObjectPool.Get(particlesPrefab, position, rotation, parent);
		}
	}
}

