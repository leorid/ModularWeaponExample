using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JL.Weapon.Subsystem
{
	public class Damage : WeaponSubsystemBase
	{
		public int damage = 10;
		public float knockbackForce = 1;
		public DamageType damageType = DamageType.Bullet;
		public override void OnHit(RaycastHit hitInfo)
		{
			DamageInfo damageInfo = new DamageInfo
			{
				damage = damage,
				damageType = damageType,
				collider = hitInfo.collider,
				point = hitInfo.point,
				direction = -hitInfo.normal,
				force = knockbackForce,
				sender = this
			};

			GameObject receiver = hitInfo.collider.gameObject;
			// The EventSystem will be released with my video on this topic :)
			// my YouTube Channel: https://www.youtube.com/@johnleorid
			// EL.Interaction.damage.Execute(this, receiver, damageInfo); 
		}
	}
}
