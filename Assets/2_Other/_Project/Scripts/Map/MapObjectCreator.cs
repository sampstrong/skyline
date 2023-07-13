using System.Collections.Generic;
using Niantic.Lightship.Maps;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PlanetX.Map
{
   public class MapObjectCreator : MonoBehaviour
   {
      [Header("Testing")]
      [SerializeField] private ModelAsset _testModelAsset;

      [Header("Prefabs")]
      [SerializeField] private GameObject _defaultHighLODPrefab;
      [SerializeField] private GameObject _defaultMidLODPrefab;
      [SerializeField] private GameObject _defaultLowLODPrefab;
      [SerializeField] private GameObject _bubblePrefab;
      
      [Header("Common Objects: My Models")]
      [SerializeField] private Material _myGlowMaterial;
      [SerializeField] private Sprite _myMidLODSprite;
      [SerializeField] private Sprite _myLowLODSprite;

      [Header("Common Objects: Other Models")] 
      [SerializeField] private Material _otherGlowMaterial;
      [SerializeField] private Sprite _otherMidLODSprite;
      [SerializeField] private Sprite _otherLowLODSPrite;

      public static CommonObjects commonObjects;
      
      public struct CommonObjects
      {
         public Material MyGlowMaterial;
         public Sprite MyMidLODSprite;
         public Sprite MyLowLODSprite;

         public Material OtherGlowMaterial;
         public Sprite OtherMidLODSprite;
         public Sprite OtherLowLODSprite;

         public CommonObjects(
            Material myGlowMaterial, 
            Material otherGlowMaterial, 
            Sprite myMidLODSprite,
            Sprite otherMidLODSprite, 
            Sprite myLowLODSprite, 
            Sprite otherLowLODSprite)
         {
            MyGlowMaterial = myGlowMaterial;
            MyMidLODSprite = myMidLODSprite;
            MyLowLODSprite = myLowLODSprite;

            OtherGlowMaterial = otherGlowMaterial;
            OtherMidLODSprite = otherMidLODSprite;
            OtherLowLODSprite = otherLowLODSprite;
         }
      }

      private void Awake()
      {
         commonObjects = new CommonObjects(_myGlowMaterial, _otherGlowMaterial, 
            _myMidLODSprite, _otherMidLODSprite,
            _myLowLODSprite, _otherLowLODSPrite);
      }
   
      [Button]
      public void CreateNewMapObjectGroup(LatLng coordinates, ModelAsset model)
      {
         // Create group
         var group = CreateMapObjectGroup();

         // Create objects for each level of detail
         var highLODObjects = CreateMapObjects(_defaultHighLODPrefab, coordinates, group, model);
         var midLODObjects = CreateMapObjects(_defaultMidLODPrefab, coordinates, group, model);
         var lowLODObjects = CreateMapObjects(_defaultLowLODPrefab, coordinates, group, model);
         
         // Create 3D Objects
         var objects3D = CreateMapObjects(_bubblePrefab, coordinates, group, model);
         
         // assign variables in MapGroupObject
         group.Initialize(highLODObjects, midLODObjects, lowLODObjects, objects3D);
      }
      
      private MapObjectGroup CreateMapObjectGroup()
      {
         GameObject obj = new GameObject();
         obj.name = $"MapObjectGroup{obj.GetInstanceID()}";
         var group = obj.AddComponent<MapObjectGroup>();
   
         return group;
      }

      private List<MapObject> CreateMapObjects(GameObject prefab, LatLng coordinates, MapObjectGroup group, ModelAsset model = null)
      {
         var obj = Instantiate(prefab);
         obj.transform.SetParent(group.transform, false);
         var objects = GetMapObjectList(obj);
         InitializeObjects(objects, coordinates, model);
         return objects;
      }

      
      private List<MapObject> GetMapObjectList(GameObject baseObject)
      {
         var mapObjectArray = baseObject.GetComponentsInChildren<MapObject>();

         List<MapObject> mapObjList = new List<MapObject>();
         mapObjList.AddRange(mapObjectArray);

         return mapObjList;
      }

      private void InitializeObjects(List<MapObject> objects, LatLng coordinates, ModelAsset model = null)
      {
         foreach (var obj in objects)
         {
            obj.Initialize(coordinates, model);
         }
      }

      public void SaveMapObjectGroupPrefab(GameObject obj)
      {
         // create prefab and save to references folder
      }
   }
}

