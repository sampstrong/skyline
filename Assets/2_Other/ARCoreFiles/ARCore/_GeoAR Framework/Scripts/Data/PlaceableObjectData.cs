using UnityEngine;

namespace Buck
{
    [System.Serializable]
    public class PlaceableObjectData
    {
        public int PrefabIndex;
        public Pose LocalPose;
        public string FileName;
        public string PrizeName;
        public string AuxData;
        

        public PlaceableObjectData
        (
            int prefabIndex, 
            Pose localPose, 
            string fileName = null, 
            string prizeName = null, 
            string auxData = null
        )
        {
            PrefabIndex = prefabIndex;
            LocalPose = localPose;
            FileName = fileName;
            PrizeName = prizeName;
            AuxData = auxData;
        }
    }
    
    // need to test if inheritance will work to add PrizeName
    // save prizename on application close
    // load prize from mysterybox class on init
    /*
    [System.Serializable]
    public class MysteryBoxData : PlaceableObjectData
    {
        public string PrizeName;
        
        public MysteryBoxData(int prefabIndex, Pose localPose, string fileName = null, string auxData = null, string prizeName = null) :
            base(prefabIndex, localPose, fileName, auxData)
        {
            PrizeName = prizeName;
        }
    }
    */
}
