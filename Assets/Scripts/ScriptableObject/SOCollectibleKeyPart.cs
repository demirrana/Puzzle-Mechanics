using UnityEngine;

[CreateAssetMenu(fileName = "SOCollectibleKeyPart", menuName = "Scriptable Objects/SOCollectibleKeyPart")]
public class SOCollectibleKeyPart : ScriptableObject
{
    public string keyPartID;
    public string displayName;
    public Sprite inventoryIcon;
    public GameObject prefab;
}