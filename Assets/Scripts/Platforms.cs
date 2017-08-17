using UnityEngine;

public class Platforms : MonoBehaviour
{

    public int NumberOfTiles { get; private set; }
    public int NumberOfPlatforms { get; private set; }
    public int NumberOfVerticals { get; private set; }
    public int NumberOfHorizontals { get; private set; }

    public void ParentToThis(Transform child, int numberOfTiles, bool isVertical)
    {
        child.SetParent(transform);
        NumberOfTiles += numberOfTiles;
        NumberOfPlatforms++;

        if (isVertical)
            NumberOfVerticals++;
        else
            NumberOfHorizontals++;
    }
}
