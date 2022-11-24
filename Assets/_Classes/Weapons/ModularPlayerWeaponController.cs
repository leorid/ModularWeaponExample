using JL.Weapon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JL.Player.Weapon
{
	public class ModularPlayerWeaponController : MonoBehaviour
	{
		[SerializeField] List<ModularWeapon> weapons;

		public LayerMask weaponLayerMask = ~0;

		int selectedWeaponIndex;
		ModularWeapon SelectedWeapon => weapons[selectedWeaponIndex];

		int lastWeaponIndex = -1;

		[SerializeField] WeaponInput input = new WeaponInput();

		bool _canChangeWeapon = true;
		bool _canFire = true;

		private void OnEnable()
		{
			UpdateWeaponList();
		}

		private void Awake()
		{
			UpdateWeaponList();
			foreach (ModularWeapon weapon in weapons)
			{
				weapon.mask = weaponLayerMask;
				weapon.InitInput(input);
			}
			weapons[0].isInInventory = true;
			WeaponVisibilityOnOff(0);
			SelectedWeapon.Draw();
		}

		void Update()
		{
			RemoveDrawnWeaponIfCantUse();

			SelectWeaponOnInput();

			PrepareAndSendInput();

			UpdateHolsteredWeapons();

			UpdateWeaponSelection();
		}

		void SelectWeaponOnInput()
		{
			if (!_canChangeWeapon) return;

			float mouseWheel;

			mouseWheel = Input.GetAxis("Mouse ScrollWheel");

			if (mouseWheel < -0.02f)
			{
				NextWeapon();
			}
			if (mouseWheel > 0.02f)
			{
				PreviousWeapon();
			}

			if (Input.GetKeyDown(KeyCode.Alpha1)) SetWeapon(0);
			if (Input.GetKeyDown(KeyCode.Alpha2)) SetWeapon(1);
			if (Input.GetKeyDown(KeyCode.Alpha3)) SetWeapon(2);
			if (Input.GetKeyDown(KeyCode.Alpha4)) SetWeapon(3);
			if (Input.GetKeyDown(KeyCode.Alpha5)) SetWeapon(4);
			if (Input.GetKeyDown(KeyCode.Alpha6)) SetWeapon(5);
			if (Input.GetKeyDown(KeyCode.Alpha7)) SetWeapon(6);
			if (Input.GetKeyDown(KeyCode.Alpha8)) SetWeapon(7);
			if (Input.GetKeyDown(KeyCode.Alpha9)) SetWeapon(8);
		}

		void RemoveDrawnWeaponIfCantUse()
		{
			if (!CanUseWeapon(SelectedWeapon))
			{
				NextWeapon();
			}
		}

		void PrepareAndSendInput()
		{
			input.ResetValues();

			if (!_canFire) return;


			input.shootButton = Input.GetButton("Fire1");
			input.shootButtonDown = Input.GetButtonDown("Fire1");
			input.shootButtonUp = Input.GetButtonUp("Fire1");

			input.altShootButton = Input.GetButton("Fire2");
			input.altShootButtonDown = Input.GetButtonDown("Fire2");
			input.altShootButtonUp = Input.GetButtonUp("Fire2");

			input.reloadButton = Input.GetButton("Reload");
			input.reloadButtonDown = Input.GetButtonDown("Reload");
			input.reloadButtonUp = Input.GetButtonUp("Reload");
		}

		void UpdateHolsteredWeapons()
		{
			foreach (ModularWeapon weapon in weapons)
			{
				if (weapon.WeaponHolstered)
				{
					weapon.HolsteredUpdate();
				}
			}
		}

		void UpdateWeaponSelection()
		{
			if (selectedWeaponIndex != lastWeaponIndex)
			{
				if (lastWeaponIndex != -1)
				{
					weapons[lastWeaponIndex].Holster();
				}
				if (lastWeaponIndex == -1 || weapons[lastWeaponIndex].WeaponHolstered)
				{
					WeaponVisibilityOnOff(selectedWeaponIndex);
					lastWeaponIndex = selectedWeaponIndex;
					SelectedWeapon.Draw();
				}
			}

			if (lastWeaponIndex == selectedWeaponIndex && !SelectedWeapon.WeaponDrawn)
			{
				SelectedWeapon.Draw();
			}
		}

		void WeaponVisibilityOnOff(int index)
		{
			if (index < 0 || index > weapons.Count - 1) return;

			for (int i = 0; i < weapons.Count; i++)
			{
				weapons[i].gameObject.SetActive(i == index);
			}
		}

		bool CanUseWeapon(ModularWeapon weapon)
		{
			if (!weapon.isInInventory) return false;
			return true;
		}


		ModularWeapon GetWeaponAtIndex(int index, bool logOutOfRange)
		{
			if (index >= weapons.Count || index < 0)
			{
				if (logOutOfRange)
				{
					Debug.LogError("No weapon found at index: " + index +
						", weapon Count is: " + weapons.Count);
				}
				return null;
			}
			return weapons[index];
		}


		public bool SetWeapon(int index)
		{
			if (!_canChangeWeapon) return false;

			if (!GetWeaponAtIndex(index, false)) return false;
			if (!CanUseWeapon(weapons[index]))
			{
				return false;
			}
			selectedWeaponIndex = index;
			return true;
		}
		public void NextWeapon()
		{
			if (!_canChangeWeapon) return;

			int substract = 0;
			for (int i = 1; i < 10; i++)
			{
				int temp = selectedWeaponIndex - substract + i;
				if (temp >= weapons.Count)
				{
					substract = weapons.Count + 1;
				}
				if (SetWeapon(temp))
				{
					break;
				}
			}
		}
		void PreviousWeapon()
		{
			if (!_canChangeWeapon) return;

			int add = 0;
			for (int i = 1; i < 10; i++)
			{
				int temp = selectedWeaponIndex + add - i;
				if (temp < 0)
				{
					add = weapons.Count + 1;
				}
				if (SetWeapon(temp))
				{
					break;
				}
			}
		}

		void UpdateWeaponList()
		{
			transform.root.GetComponentsInChildren<ModularWeapon>(true, weapons);
		}
	}
}
