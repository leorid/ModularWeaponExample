using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JL
{
	public class PoolParticlesAfterPlay : MonoBehaviour
	{
		public void OnParticleSystemStopped()
		{
			ObjectPool.TryReturn(gameObject);
		}
	}
}
