using UnityEngine;

// V1.01: Updated to allow for grid scale values > 1

public class YuME_GizmoGrid : MonoBehaviour
{
    [HideInInspector]
    public float tileSize = 1;
    [HideInInspector]
    public int gridWidth = 40;
    [HideInInspector]
    public int gridDepth = 40;

    [HideInInspector]
    public float gridHeight
    {
        get { return _gridHeight; }
        set { _gridHeight = value; }
    }

    float _gridHeight = 0;

    [HideInInspector]
    public bool toolEnabled = true;

	public float gridOffset = 0.01f;
    float tileOffset = 0.5f;

    [HideInInspector]
    public Color gridColorNormal = Color.white;
    [HideInInspector]
    public Color gridColorBorder = Color.green;
    [HideInInspector]
    public Color gridColorFill = new Color(1, 0, 0, 0.5F);

    float gridWidthOffset;
    float gridDepthOffset;

    Vector3 gridColliderPosition;

    void OnEnable()
    {
        gameObject.SetActive(false);
    }

    Vector3 gridMin;
    Vector3 gridMax;

    void OnDrawGizmos()
    {
        if (toolEnabled)
        {
            tileOffset = tileSize / 2;
            gridWidthOffset = gridWidth * tileSize / 2;
            gridDepthOffset = gridDepth * tileSize / 2;
            gridMin.x = gameObject.transform.position.x - gridWidthOffset - tileOffset;
            gridMin.y = gameObject.transform.position.y + gridHeight - tileOffset - gridOffset;
            gridMin.z = gameObject.transform.position.z - gridDepthOffset - tileOffset;
            gridMax.x = gridMin.x + (tileSize * gridWidth);
            gridMax.z = gridMin.z + (tileSize * gridDepth);
            gridMax.y = gridMin.y;

            drawGridBase();
            drawMainGrid();
            drawGridBorder();

            moveGrid();
        }
    }

    public void moveGrid()
    {
        gridColliderPosition = gameObject.GetComponent<BoxCollider>().center;
        gridColliderPosition.y = gridHeight - 0.5f;
        gameObject.GetComponent<BoxCollider>().center = gridColliderPosition;
    }


    private void drawGridBorder() // fixed for scale
    {
        Gizmos.color = gridColorBorder;

        // left side
        Gizmos.DrawLine(new Vector3(gridMin.x, gridMin.y, gridMin.z), new Vector3(gridMin.x, gridMin.y, gridMax.z));

        //bottom
        Gizmos.DrawLine(new Vector3(gridMin.x, gridMin.y, gridMin.z), new Vector3(gridMax.x, gridMin.y, gridMin.z ));

        // Right side
        Gizmos.DrawLine(new Vector3(gridMax.x, gridMin.y, gridMin.z), new Vector3(gridMax.x, gridMin.y, gridMax.z));

        //top
        Gizmos.DrawLine(new Vector3(gridMin.x, gridMin.y, gridMax.z), new Vector3(gridMax.x, gridMin.y, gridMax.z));
    }

    private void drawGridBase() // fixed for scale
    {
        Gizmos.color = gridColorFill;
        Gizmos.DrawCube(new Vector3(gameObject.transform.position.x - tileOffset, gameObject.transform.position.y + gridHeight - tileOffset - gridOffset, gameObject.transform.position.z - tileOffset), 
            new Vector3((gridWidth * tileSize), 0.01f, (gridDepth * tileSize)));
    }

    private void drawMainGrid() // fixed for scale
    {
        Gizmos.color = gridColorNormal;

        if (tileSize != 0)
        {
            for (float i = tileSize; i < (gridWidth * tileSize); i += tileSize)
            {
                Gizmos.DrawLine(
					new Vector3((float)i + gridMin.x, gridMin.y, gridMin.z),
					new Vector3((float)i + gridMin.x, gridMin.y, gridMax.z)
                    );
            }
        }

        if (tileSize != 0)
        {
            for (float j = tileSize; j < (gridDepth * tileSize) ; j += tileSize)
            {
                Gizmos.DrawLine(
					new Vector3(gridMin.x, gridMin.y, j + gridMin.z),
					new Vector3(gridMax.x, gridMin.y, j + gridMin.z)
                    );
            }
        }
    }
}
