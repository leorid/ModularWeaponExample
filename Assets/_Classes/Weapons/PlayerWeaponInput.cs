using System;
using UnityEngine;

namespace JL.Weapon
{
	[Serializable]
	public class WeaponInput
	{
		[SerializeField] bool debugNoInputReset;

		public bool shootButton;
		public bool shootButtonDown;
		public bool shootButtonUp;

		public bool altShootButton;
		public bool altShootButtonDown;
		public bool altShootButtonUp;

		public bool reloadButton;
		public bool reloadButtonDown;
		public bool reloadButtonUp;

		public void ResetValues()
		{
			if (debugNoInputReset) return;
			shootButton = false;
			shootButtonDown = false;
			shootButtonUp = false;

			altShootButton = false;
			altShootButtonDown = false;
			altShootButtonUp = false;

			reloadButton = false;
			reloadButtonDown = false;
			reloadButtonUp = false;
		}
	}
}
