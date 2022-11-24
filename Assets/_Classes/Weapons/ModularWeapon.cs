using System.Collections.Generic;
using JL.Weapon.Subsystem;
using UnityEngine;

namespace JL.Weapon
{
	public class ModularWeapon : MonoBehaviour
	{
		[SerializeField] List<WeaponSubsystemBase> weaponSubsystems = new();

		[SerializeField] Transform muzzleTransform;
		public Animator animator;
		// Raycast Origin
		public Transform Origin => muzzleTransform;
		public LayerMask mask;

		public WeaponInput input;

		public bool isInInventory = false;

		[SerializeField] bool _isDrawn;

		public bool WeaponDrawn { get => _isDrawn; }
		public bool WeaponHolstered { get => !_isDrawn; }


		void Start()
		{
			GetComponentsInChildren(weaponSubsystems);
			foreach (WeaponSubsystemBase weaponSubsystem in weaponSubsystems)
			{
				weaponSubsystem.InitBase(this);
			}
		}

		public void InitInput(WeaponInput input)
		{
			this.input = input;
		}

		void Update()
		{
			if (!WeaponDrawn) return;

			bool anyFailed = false;
			foreach (WeaponSubsystemBase weaponSubsystem in weaponSubsystems)
			{
				if (!weaponSubsystem.Check())
				{
					anyFailed = true;
				}
			}
			foreach (WeaponSubsystemBase weaponSubsystem in weaponSubsystems)
			{
				weaponSubsystem.OnUpdate();
			}

			if (anyFailed) return;

			foreach (WeaponSubsystemBase weaponSubsystem in weaponSubsystems)
			{
				weaponSubsystem.OnShoot();
			}
		}

		public void HolsteredUpdate()
		{
			foreach (WeaponSubsystemBase weaponSubsystem in weaponSubsystems)
			{
				weaponSubsystem.OnHolsteredUpdate();
			}
		}


		public void OnHit(RaycastHit hitInfo)
		{
			foreach (WeaponSubsystemBase weaponSubsystem in weaponSubsystems)
			{
				weaponSubsystem.OnHit(hitInfo);
			}
		}

		public void Draw()
		{
			_isDrawn = true;
			foreach (WeaponSubsystemBase weaponSubsystem in weaponSubsystems)
			{
				weaponSubsystem.OnDraw();
			}
		}
		public void Holster()
		{
			_isDrawn = false;
			foreach (WeaponSubsystemBase weaponSubsystem in weaponSubsystems)
			{
				weaponSubsystem.OnHolster();
			}
		}
	}
}
