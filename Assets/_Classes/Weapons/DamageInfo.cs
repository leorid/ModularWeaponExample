using UnityEngine;

namespace JL.Weapon
{
	[System.Serializable]
	public struct DamageInfo
	{
		public int damage;
		public DamageType damageType;
		public Collider collider;
		public object sender;
		public Vector3 point;
		public Vector3 direction;
		public float force;
	}
	public enum DamageType
	{
		Melee,
		Knockout,
		Bullet,
		Explosion
	}
}
