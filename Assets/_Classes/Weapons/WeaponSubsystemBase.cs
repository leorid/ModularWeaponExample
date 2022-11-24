using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JL.Weapon.Subsystem
{
	public abstract class WeaponSubsystemBase : MonoBehaviour
	{
		protected ModularWeapon weapon;

		public void InitBase(ModularWeapon weapon)
		{
			this.weapon = weapon;
			Init();
		}
		protected virtual void Init() { }
		public virtual bool Check() { return true; }
		public virtual void OnShoot() { }
		public virtual void OnHit(RaycastHit hitInfo) { }

		/// <summary>
		/// is called when the weapon is drawn
		/// </summary>
		public virtual void OnUpdate() { }
		/// <summary>
		/// is called when the weapon is holstered
		/// </summary>
		public virtual void OnHolsteredUpdate() { }
		public virtual void OnDestroy() { }

		internal virtual void OnDraw() { }

		internal virtual void OnHolster() { }
	}
}
