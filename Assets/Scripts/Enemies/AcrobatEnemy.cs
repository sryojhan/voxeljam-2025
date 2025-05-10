using UnityEngine;

public class AcrobatEnemy : MonoBehaviour
{
    public enum Side { Left, Right };

    LineRenderer line;
    HingeJoint2D joint;
    SpriteRenderer spriteRenderer;
    Side side;
    Vector3 pivotPosition;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        line = GetComponent<LineRenderer>();
    }

    void Start()
    {
        joint = GetComponent<HingeJoint2D>();


        joint.anchor = new Vector2(-transform.position.x, 0);
        joint.connectedAnchor = new Vector2(0, transform.position.y);
    }

    public void SetSpawnSide(Side s)
    {
        side = s;
        spriteRenderer.flipX = side == Side.Right;
    }

    public void SetPivotPosition(Vector3 pivotPos)
    {
        pivotPosition = pivotPos;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Border>())
        {
            if (side == Side.Left && collision.gameObject.name == "RightBorder" ||
                side == Side.Right && collision.gameObject.name == "LeftBorder")
            {
                Destroy(gameObject);
            }
        }
    }

    private void Update()
    {
        line.SetPosition(0, pivotPosition);
        line.SetPosition(1, transform.position);
        line.startWidth = 0.1f;
        line.endWidth = 0.1f;
    }
}
