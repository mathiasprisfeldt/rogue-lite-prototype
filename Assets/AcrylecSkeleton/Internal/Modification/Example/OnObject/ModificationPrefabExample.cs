using UnityEngine;

namespace AcrylecSkeleton.ModificationSystem.Examples
{
    [RequireComponent(typeof(ModificationHandler))]
    public class ModificationPrefabExample : MonoBehaviour {

        // Use this for initialization
        void Start ()
        {
            var temp = GetComponent<ModificationHandler>();
            temp.AddModification(new ModificationExample(1,0,Modification.ModificationTypeEnum.Timed));
        }
    }
}