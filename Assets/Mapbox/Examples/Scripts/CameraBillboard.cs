namespace Mapbox.Examples
{
	using UnityEngine;

	public class CameraBillboard : MonoBehaviour
	{
		private Camera _camera;

		public void Start()
		{
			_camera = GameObject.FindWithTag("MapCamera").GetComponent<Camera>();
		}

		void Update()
		{
			transform.LookAt(transform.position + _camera.transform.rotation * Vector3.forward, _camera.transform.rotation * Vector3.up);
		}
	}
}