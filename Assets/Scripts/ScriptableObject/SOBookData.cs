using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SOBookData", menuName = "Scriptable Objects/SOBookData")]
public class SOBookData : ScriptableObject
{
    public string bookDescription;

    [System.Serializable]
    public struct FeelingAffinity
    {
        public SOFeeling feeling;
        [Range(0f, 1f)] public float degree;
    }

    public List<FeelingAffinity> feelingAffinities;

    public bool IsRepresentativeOfFeeling(SOFeeling targetFeeling)
    {
        foreach (var affinity in feelingAffinities)
        {
            if (affinity.feeling == targetFeeling && affinity.degree == 1f)
            {
                return true;
            }
        }

        return false;
    }
}
