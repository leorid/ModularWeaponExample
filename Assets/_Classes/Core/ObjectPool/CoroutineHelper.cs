using System.Collections;
using UnityEngine;

namespace JL
{
	public class CoroutineHelper : MonoBehaviour
	{
		static CoroutineHelper _instance;
		static CoroutineHelper Instance
		{
			get
			{
				if (!_instance)
				{
					GameObject go = new GameObject("Coroutine Helper");
					go.transform.parent = HolderManager.HolderHead;
					_instance = go.AddComponent<CoroutineHelper>();
				}
				return _instance;
			}
		}


		public static Coroutine StartCustomCoroutine(IEnumerator coroutine)
		{
			return Instance.ExecuteStartCustomCoroutine(coroutine);
		}

		public static void StopCustomCoroutine(Coroutine coroutine)
		{
			Instance.ExecuteStopCustomCoroutine(coroutine);
		}


		Coroutine ExecuteStartCustomCoroutine(IEnumerator coroutine)
		{
			return StartCoroutine(coroutine);
		}
		void ExecuteStopCustomCoroutine(Coroutine coroutine)
		{
			StopCoroutine(coroutine);
		}
	}
}
