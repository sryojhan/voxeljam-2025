using UnityEngine;

public class Platform : MonoBehaviour
{
    public SpriteRenderer sprRenderer;

    [Header("Shake")]
    public CoroutineAnimation shake;
    public float shakeFrequency = 1;
    public float shakeIntensity = 1;


    [Header("Movement")]
    public CoroutineAnimation movement;
    public Vector2 destination;
    private Vector2 origin;

    private BoxCollider2D col;

    private void Start()
    {
        origin = transform.position;
        col = GetComponent<BoxCollider2D>();

        Move();
    }

    [EasyButtons.Button]
    public void Shake(bool move = false)
    {
        void OnUpdate(float i)
        {
            sprRenderer.transform.localPosition =
                Vector3.up *
                Mathf.PingPong(i * shakeFrequency, shakeIntensity);
        }

        void OnEnd()
        {
            sprRenderer.transform.localPosition = Vector3.zero;

            col.enabled = true;

            if (move)
                Move();
        }

        shake.Play(this, OnUpdate, null, OnEnd);
    }


    [EasyButtons.Button]
    public void Move()
    {
        col.enabled = false;

        Vector2 origin = transform.position;
        Vector2 destination = this.destination;


        void OnUpdate(float i)
        {
            transform.position = Vector2.Lerp(origin, destination, i);
        }

        void OnEnd()
        {
            transform.position = destination;

            (this.origin, this.destination) = (this.destination, this.origin);
            Shake();
        }

        movement.Play(this, OnUpdate, null, OnEnd);
    }


}
