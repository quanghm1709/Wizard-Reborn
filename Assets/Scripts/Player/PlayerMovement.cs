using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

[RequireComponent(typeof(PlayerController))]
public class PlayerMovement : MonoBehaviour
{
    private PlayerController player;
    private float dirX;
    private float dirY;

    private void Awake()
    {
        player = GetComponent<PlayerController>();
    }

    private void Update()
    {
        SetMoveInputs();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void SetMoveInputs()
    {
        dirX = CrossPlatformInputManager.GetAxisRaw("Horizontal");
        dirY = CrossPlatformInputManager.GetAxisRaw("Vertical");

        if (dirX != 0 || dirY != 0)
        {
            player.anim.SetBool("isMove", true);
        }
        else
        {
            player.anim.SetBool("isMove", false);
        }
        Flip();
    }

    private void Move()
    {
        if (player.canMove)
        {
            player.rb.linearVelocity = new Vector2(dirX, dirY) * player.currentSpd;
        }
        else
        {
            player.rb.linearVelocity = Vector2.zero;
        }
    }

    private void Flip()
    {
        if (dirX > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
            player.isFacingRight = true;
        }
        else if (dirX < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            player.isFacingRight = false;
        }
    }
}
