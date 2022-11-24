
using UnityEngine;

namespace JL.Weapon.Subsystem
{
	public class ShootAnimation : WeaponSubsystemBase
	{
		[SerializeField] GameObject _muzzleFlash;
		public override void OnShoot()
		{
			weapon.animator.SetTrigger("Shoot");
			_muzzleFlash.SetActive(true);
		}
	}
}

