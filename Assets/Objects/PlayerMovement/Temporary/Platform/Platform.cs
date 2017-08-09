using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer _renderer;

    [SerializeField]
    private Sprite _left;

    [SerializeField]
    private Sprite _middle;

    [SerializeField]
    private Sprite _right;

    [SerializeField]
    private Sprite _noGrass;

    [SerializeField]
    private LayerMask _layers;
	
    // Use this for initialization
	void Start ()
	{
	    var r1 = Physics2D.Raycast(transform.position, Vector2.left, _renderer.sprite.bounds.size.x/2 + .2f,_layers);
        var r2 = Physics2D.Raycast(transform.position, Vector2.right, _renderer.sprite.bounds.size.x / 2 + .2f, _layers);
        var r3 = Physics2D.Raycast(transform.position, Vector2.up, _renderer.sprite.bounds.size.x / 2 + .2f, _layers);
        var r4 = Physics2D.Raycast(transform.position, Vector2.down, _renderer.sprite.bounds.size.x / 2 + .2f, _layers);

        if ((r1.collider && r2.collider) || (!r1.collider && !r2.collider && !r3 ) || !r3 && r4)
	        _renderer.sprite = _middle;
        else if (!r3 && !r4)
            _renderer.sprite = r1.collider ? _right : _left;
        else
            _renderer.sprite = _noGrass;

	    var temp = GetComponent<TileBehaviour>();
        if(temp)
            temp.CheckSides();

	}

}
