using System.Collections.Generic;
using UnityEngine;

public class PlayerSpriteManager : MonoBehaviour
{
    // References
    public Rigidbody2D playerRB;
    public Transform playerT;
    public SpriteRenderer playerSprite;
    public List<Sprite> sprites = new List<Sprite>();

    private bool facingLeft = true;

    void Update()
    {
        // Set sprite direction
        facingLeft = playerRB.linearVelocityX > 0 ? false : true;
        if (facingLeft) playerT.localScale = new Vector3(-1, 1, 1);
        else playerT.localScale = new Vector3(1, 1, 1);

        // Set sprite movement
        if (Mathf.Abs(playerRB.linearVelocityX) < 0.1)
        {
            playerSprite.sprite = sprites[0];
        }
        else
        {
            playerSprite.sprite = sprites[1];
        }
    }
}
