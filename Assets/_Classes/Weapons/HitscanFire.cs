
using UnityEngine;

namespace JL.Weapon.Subsystem
{
	public class HitscanFire : WeaponSubsystemBase
	{
		public float maxShootDistance = 100;

		public override void OnShoot()
		{
			Ray ray = new Ray(weapon.Origin.position,
							  weapon.Origin.forward);

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

