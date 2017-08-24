using UnityEngine;


/// <summary>
/// Purpose:
/// Creator:
/// </summary>
public class Climbable : MonoBehaviour
{
    [SerializeField]
    private float resistance;

    [SerializeField]
    private GameObject _top;

    public float Resistance
    {
        get
        {
            return resistance;
        }

        set
        {
            resistance = value;
        }
    }

    public GameObject Top
    {
        get { return _top; }
        set { _top = value; }
    }
}