using UnityEngine;

	public class PooledObject : MonoBehaviour {
		public ObjectPool pool { get; set; }

		public virtual void OnRecycled() {}
	}