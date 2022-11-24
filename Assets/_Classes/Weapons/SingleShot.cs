using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JL.Weapon.Subsystem
{
	public class SingleShot : WeaponSubsystemBase
	{
		[SerializeField, Min(0)] float _fireRate = 0.2f;
		float _lastFireTime = 0;

		public override bool Check()
		{
			return weapon.input.shootButtonDown && 
				Time.time > _lastFireTime + _fireRate;
		}
		public override void OnShoot()
		{
			_lastFireTime = Time.time;
		}
	}
}
