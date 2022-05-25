using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ValakiUvScrollDemo : MonoBehaviour
{
    enum MoveType
    {
        LeftToRight,
        RightToLeft,
        BottomToTop,
        TopToBottom,
    }

    [SerializeField] private MoveType _moveType;

    [SerializeField] private float speed = 0.1f;
    private Mesh mesh;
    private List<Vector2> uvs = new List<Vector2>();

    // Start is called before the first frame update
    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        uvs = mesh.uv.ToList();
    }

    // Update is called once per frame

    private void LateUpdate()
    {
        for (int i = 0; i < uvs.Count; i++)
        {
            Vector2 v = mesh.uv[i];
            switch (_moveType)
            {
                case MoveType.LeftToRight:
                    v.x -= Time.deltaTime * speed;
                    break;
                case MoveType.RightToLeft:
                    v.x += Time.deltaTime * speed;
                    break;
                case MoveType.TopToBottom:
                    v.y += Time.deltaTime * speed;
                    break;
                case MoveType.BottomToTop:
                    v.y -= Time.deltaTime * speed;
                    break;
            }

            uvs[i] = v;
        }

        mesh.SetUVs(0, uvs);
    }
}
