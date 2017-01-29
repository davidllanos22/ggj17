using UnityEngine;

[CreateAssetMenu()]
public class DataBetweenScenes : ScriptableObject {
	public bool[] playerPlaying;
    public int[] playerControllerIds;
    public int[] playerInputTypes;
}
