
using UnityEngine;

namespace JL.Weapon.Subsystem
{
	public class HitscanFire_Spread : WeaponSubsystemBase
	{
		public float maxShootDistance = 100;
		public int nrOfShots = 5;
		public float spreadAngle = 50;

		public override void OnShoot()
		{
			float halfAngle = spreadAngle / 2f;
			for (int i = 0; i < nrOfShots; i++)
			{
				// get random rotation within limits
				float rndXAngle = Random.Range(-halfAngle, halfAngle);
				float rndZAngle = Random.Range(0, 360);

				Quaternion rot = weapon.Origin.rotation * 
							Quaternion.Euler(0, 0, rndZAngle) * 
							Quaternion.Euler(rndXAngle, 0, 0);

				RaycastInDirection(rot * Vector3.forward);
			}
		}

		void RaycastInDirection(Vector3 direction)
		{
			Ray ray = new Ray(weapon.Origin.position,
							  direction);

			RaycastHit hit;
			if (Physics.Raycast(ray, out hit,
					maxShootDistance, weapon.mask))
			{
				weapon.OnHit(hit);

				Debug.DrawLine(weapon.Origin.position, hit.point, Color.red, 0.1f);
				Toolbox.DrawAxisCross(hit.point, 0.2f, Color.red, 0.1f);
			}
		}
	}
}

