using System;
using Niantic.Lightship.Maps;

namespace Mapbox.Examples
{
	using UnityEngine;
	using Mapbox.Utils;
	using Mapbox.Unity.Map;
	using Mapbox.Unity.MeshGeneration.Factories;
	using Mapbox.Unity.Utilities;
	using System.Collections.Generic;

	public class SpawnOnMap : MonoBehaviour
	{
		public List<GameObject> SpawnedObjects => _spawnedObjects;

		[SerializeField]
		AbstractMap _map;

		[SerializeField]
		[Geocode]
		string[] _locationStrings;
		Vector2d[] _locations;

		[SerializeField]
		float _spawnScale = 100f;

		[SerializeField]
		GameObject _markerPrefab;

		List<GameObject> _spawnedObjects = new List<GameObject>();

		private List<MapTracker> _mapTrackers = new List<MapTracker>();

		//public event Action onAllPrefabsSpawned;

		// can probably remove
		void Start()
		{
			if (_locationStrings.Length <= 0) return;
			_locations = new Vector2d[_locationStrings.Length];
			_spawnedObjects = new List<GameObject>();
			for (int i = 0; i < _locationStrings.Length; i++)
			{
				var locationString = _locationStrings[i];
				_locations[i] = Conversions.StringToLatLon(locationString);
				var instance = Instantiate(_markerPrefab);
				instance.transform.localPosition = _map.GeoToWorldPosition(_locations[i], true);
				instance.transform.localScale = new Vector3(_spawnScale, _spawnScale, _spawnScale);
				_spawnedObjects.Add(instance);
			}
			
			//onAllPrefabsSpawned?.Invoke();
		}

		private void Update()
		{
			if (_mapTrackers.Count <= 0) return;
			foreach (var tracker in _mapTrackers)
			{
				var location = new Vector2d(tracker.Coordinates.Latitude, tracker.Coordinates.Longitude);
				tracker.transform.localPosition = _map.GeoToWorldPosition(location, true);
				tracker.transform.localScale = new Vector3(_spawnScale, _spawnScale, _spawnScale);
			}
		}

		public MapTracker SpawnMapTracker(ITimelineObject building)
		{
			var mapCoords = new Vector2d(building.Coordinates.Latitude, building.Coordinates.Longitude);
			
			var obj = Instantiate(_markerPrefab);
			obj.transform.localPosition = _map.GeoToWorldPosition(mapCoords, true);
			obj.transform.localScale = new Vector3(_spawnScale, _spawnScale, _spawnScale);

			var mapTracker = obj.GetComponent<MapTracker>();
			mapTracker.Init(building);
			_mapTrackers.Add(mapTracker);
			
			Debug.Log($"Map Tracker Spawned: {obj.name}");

			return mapTracker;
		}

		public void SpawnMyLocationTracker()
		{
			
		}
	}
}