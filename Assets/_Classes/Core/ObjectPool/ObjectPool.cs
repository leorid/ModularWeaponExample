using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace JL
{
	public static class ObjectPool
	{
		private static Dictionary<int, Stack<GameObject>> _gameObjectPool = new Dictionary<int, Stack<GameObject>>();
		private static Dictionary<int, int> _instantiatedGameObjects = new Dictionary<int, int>();
		private static Dictionary<int, GameObject> _originalObjectIDs = new Dictionary<int, GameObject>();
		private static Dictionary<int, List<Coroutine>> _delayedCalls = new Dictionary<int, List<Coroutine>>();

		private static HashSet<int> _pooledIDs = new HashSet<int>();

		public static GameObject Get(GameObject original)
		{
			return Get(original, Vector3.zero, Quaternion.identity, null);
		}

		public static GameObject Get(GameObject original, Vector3 position, Quaternion rotation)
		{
			return Get(original, position, rotation, null);
		}

		public static GameObject Get(GameObject original, Vector3 position, Quaternion rotation, Transform parent)
		{
			var originalInstanceID = original.GetInstanceID();
			var instantiatedObject = GetObjectFromPool(originalInstanceID, position, rotation, parent);
			if (!instantiatedObject)
			{
				instantiatedObject = GameObject.Instantiate(original, position, rotation, parent);
				if (!_originalObjectIDs.ContainsKey(originalInstanceID))
				{
					_originalObjectIDs.Add(originalInstanceID, original);
				}
			}
			_instantiatedGameObjects.Add(instantiatedObject.GetInstanceID(), originalInstanceID);

			return instantiatedObject;
		}

		private static GameObject GetObjectFromPool(int originalInstanceID, Vector3 position, Quaternion rotation, Transform parent)
		{
			Stack<GameObject> pool;
			if (_gameObjectPool.TryGetValue(originalInstanceID, out pool))
			{
				while (pool.Count > 0)
				{
					var instantiatedObject = pool.Pop();
					if (!instantiatedObject)
					{
						continue;
					}
					instantiatedObject.transform.position = position;
					instantiatedObject.transform.rotation = rotation;
					instantiatedObject.transform.SetParent(parent, true);
					instantiatedObject.SetActive(true);
					return instantiatedObject;
				}
			}
			return null;
		}

		public static bool Contains(GameObject instance)
		{
			return _pooledIDs.Contains(instance.GetInstanceID());
		}

		public static bool CreatedWithPool(GameObject instantiatedObject)
		{
			return _instantiatedGameObjects.ContainsKey(instantiatedObject.GetInstanceID());
		}

		public static int OriginalInstanceID(GameObject instantiatedObject)
		{
			return OriginalInstanceIDInternal(instantiatedObject);
		}

		private static int OriginalInstanceIDInternal(GameObject instantiatedObject)
		{
			var instantiatedInstanceID = instantiatedObject.GetInstanceID();
			var originalInstanceID = -1;
			if (!_instantiatedGameObjects.TryGetValue(instantiatedInstanceID, out originalInstanceID))
			{
				Debug.LogError("Unable to get the original instance ID of " + instantiatedObject + ": has the object already been placed in the ObjectPool?");
				return -1;
			}
			return originalInstanceID;
		}

		public static void Store(GameObject instance, GameObject original)
		{
			var originalInstanceID = original.GetInstanceID();

			if (!_originalObjectIDs.ContainsKey(originalInstanceID))
			{
				_originalObjectIDs.Add(originalInstanceID, original);
			}
			// Map the instance ID to the original instance ID so when the object is returned it knows what pool to go to.
			_instantiatedGameObjects[instance.GetInstanceID()] = originalInstanceID;
			Return(instance);
		}

		public static void Return(GameObject instantiatedObject)
		{
			ReturnInternal(instantiatedObject);
		}

		public static void Return(GameObject instantiatedObject, float delay)
		{
			if (delay <= 0)
			{
				ReturnInternal(instantiatedObject);
			}
			else
			{
				Coroutine coroutine = CoroutineHelper.StartCustomCoroutine(
					ReturnAfterDelay(delay, instantiatedObject));
				int instanceID = instantiatedObject.GetInstanceID();
				List<Coroutine> coroutines;
				if (!_delayedCalls.TryGetValue(instanceID, out coroutines))
				{
					_delayedCalls[instanceID] = new List<Coroutine>();
				}
				_delayedCalls[instanceID].Add(coroutine);
			}
		}

		static IEnumerator ReturnAfterDelay(float seconds, GameObject instantiatedObject)
		{
			yield return new WaitForSeconds(seconds);
			ReturnInternal(instantiatedObject);

		}

		public static bool TryReturn(GameObject instantiatedObject)
		{
			int instantiatedInstanceID = instantiatedObject.GetInstanceID();
			int originalInstanceID;
			if (!_instantiatedGameObjects.TryGetValue(instantiatedInstanceID, out originalInstanceID))
			{
				return false;
			}

			_instantiatedGameObjects.Remove(instantiatedInstanceID);

			ReturnLocal(instantiatedObject, originalInstanceID);

			return true;
		}

		private static void ReturnInternal(GameObject instantiatedObject)
		{
			if (!instantiatedObject)
			{
				Debug.LogError("Can't pool NULL object");
			}
			var instantiatedInstanceID = instantiatedObject.GetInstanceID();
			if (_pooledIDs.Contains(instantiatedInstanceID))
			{
				Debug.LogError("Object " + instantiatedObject + " is already in Pool",
					instantiatedObject);
			}
			if (!_instantiatedGameObjects.TryGetValue(instantiatedInstanceID,
				   out int originalInstanceID))
			{
				Debug.LogError("Unable to pool " + instantiatedObject.name +
					" (instance " + instantiatedInstanceID + "): the GameObject was not " +
					"instantiated with ObjectPool.Instantiate " + Time.time, instantiatedObject);
				return;
			}

			_instantiatedGameObjects.Remove(instantiatedInstanceID);
			if (_delayedCalls.TryGetValue(instantiatedInstanceID, out List<Coroutine> coroutines))
			{
				foreach (Coroutine coroutine in coroutines)
				{
					CoroutineHelper.StopCustomCoroutine(coroutine);
				}
				_delayedCalls[instantiatedInstanceID].Clear();
			}

			ReturnLocal(instantiatedObject, originalInstanceID);
		}

		private static void ReturnLocal(GameObject instantiatedObject, int originalInstanceID)
		{
			instantiatedObject.SetActive(false);
			instantiatedObject.transform.SetParent(HolderManager.Pooled);

			Stack<GameObject> pool;
			if (_gameObjectPool.TryGetValue(originalInstanceID, out pool))
			{
				pool.Push(instantiatedObject);
			}
			else
			{
				// The pool for this GameObject type doesn't exist yet so it has to be created
				pool = new Stack<GameObject>();
				pool.Push(instantiatedObject);
				_gameObjectPool.Add(originalInstanceID, pool);
			}
		}

		public static GameObject OriginalObject(GameObject instantiatedObject)
		{
			var originalInstanceID = -1;
			if (!_instantiatedGameObjects.TryGetValue(instantiatedObject.GetInstanceID(), out originalInstanceID))
			{
				return null;
			}

			GameObject original;
			if (!_originalObjectIDs.TryGetValue(originalInstanceID, out original))
			{
				return null;
			}

			return original;
		}

		/// <summary>
		/// Reset the static variables for domain reloading
		/// </summary>
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void DomainReset()
		{
			if (_gameObjectPool != null) _gameObjectPool.Clear();
			if (_instantiatedGameObjects != null) _instantiatedGameObjects.Clear();
			if (_originalObjectIDs != null) _originalObjectIDs.Clear();
			if (_delayedCalls != null) _delayedCalls.Clear();
			if (_pooledIDs != null) _pooledIDs.Clear();
		}
	}
}