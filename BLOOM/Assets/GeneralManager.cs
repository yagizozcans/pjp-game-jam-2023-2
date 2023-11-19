    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

public class GeneralManager : MonoBehaviour
{
    public static GeneralManager instance;

    public GameObject[,] tiles;

    public Sprite downCliff;
    public Sprite leftCliff;
    public Sprite rightCliff;
    public Sprite upperCliff;
    public Sprite upperLeftCliff;
    public Sprite upperRightCliff;
    public Sprite downRightCliff;
    public Sprite downLeftCliff;

    public GameObject upperLeftChunk;
    public GameObject upperChunk;
    public GameObject upperRightChunk;
    public GameObject rightChunk;
    public GameObject downRightChunk;
    public GameObject downChunk;
    public GameObject downLeftChunk;
    public GameObject leftChunk;
    public GameObject middleChunk;

    public Vector2 offsetUpperChunks;
    public Vector2 offsetMiddleChunks;
    public Vector2 offsetDownChunks;

    public Sprite[] grassSprites;
    public Sprite dirtParticleSprite;

    public float tileSize;
    public int chunkSizeX;
    public int chunkSizeY;
    public float dirtParticleMaxSpeed;

    [System.NonSerialized]
    public float spriteBound;

    public float gravity;

    public Text noiseText;
    public GameObject selectTile;

    GameObject sceneCamera;

    Vector2 origin;

    public float detailScale;

    public Vector2 tileOrigin;

    public Transform tilesParent;


    public void Start()
    {
        tiles = new GameObject[chunkSizeX, chunkSizeY];
        sceneCamera = Camera.main.gameObject;
        instance = this;
        spriteBound = grassSprites[0].bounds.size.x;
        DrawLR(selectTile.GetComponent<LineRenderer>(), (tileSize * spriteBound) - selectTile.GetComponent<LineRenderer>().startWidth * 2, 4, 0.08f);
        origin = new Vector2(tileSize * spriteBound * chunkSizeX / 2, tileSize * spriteBound * chunkSizeY / 2);
        Create9Chunks(0, 0);
        tileOrigin = Vector2.zero;
    }

    public void Create9Chunks(int xOrigin, int yOrigin)
    {
        for(int i = 0; i < tilesParent.childCount; i++)
        {
            Destroy(tilesParent.GetChild(i).gameObject);
        }
        middleChunk = CreatingTileChunk(xOrigin, yOrigin);
        rightChunk = CreatingTileChunk(chunkSizeX + xOrigin, yOrigin);
        leftChunk = CreatingTileChunk(-chunkSizeX + xOrigin, yOrigin);
        upperChunk = CreatingTileChunk(xOrigin, yOrigin+chunkSizeY);
        downChunk = CreatingTileChunk(xOrigin, yOrigin+-chunkSizeY);
        upperLeftChunk = CreatingTileChunk(-chunkSizeX + xOrigin, yOrigin +chunkSizeY);
        upperRightChunk = CreatingTileChunk(chunkSizeX + xOrigin, yOrigin+chunkSizeY);
        downLeftChunk = CreatingTileChunk(-chunkSizeX + xOrigin,  yOrigin+-chunkSizeY);
        downRightChunk = CreatingTileChunk(chunkSizeX+ xOrigin, yOrigin + -chunkSizeY);
    }

    private void Update()
    {
        sceneCamera.transform.position = Vector3.Lerp(sceneCamera.transform.position, new Vector3(PlayerFlower.instance.transform.position.x, PlayerFlower.instance.transform.position.y, sceneCamera.transform.position.z), 0.2f);
        if (Input.GetMouseButton(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector3.forward);
            if(hit.transform != null)
            {
                noiseText.gameObject.SetActive(true);
                selectTile.gameObject.SetActive(true);
                selectTile.transform.position = new Vector3(hit.transform.position.x, hit.transform.position.y, -3);
                noiseText.transform.position = (Vector2)hit.transform.position + Vector2.up * spriteBound;
                noiseText.text = hit.transform.GetComponent<Tile>().noise.ToString();
            }
        }
        else
        {
            noiseText.gameObject.SetActive(false);
            selectTile.gameObject.SetActive(false);
        }
    }


    public GameObject CreatingTileChunk(int currentx, int currenty)
    {
        GameObject chunk = new GameObject();
        chunk.name = $"[{currentx} , {currenty}]";
        for (int x = currentx; x < chunkSizeX + currentx; x++)
        {
            for (int y = currenty; y < currenty + chunkSizeY; y++)
            {
                float noise = generateNoise(x, y, new Vector2(x * spriteBound * tileSize, y * spriteBound * tileSize));
                if (noise > 0.3f)
                {
                    GameObject nTile = CreateTile(x, y);
                    nTile.AddComponent<SpriteRenderer>();
                    nTile.GetComponent<SpriteRenderer>().sprite = grassSprites[Random.Range(0,grassSprites.Length)];
                    nTile.AddComponent<Tile>();
                    nTile.GetComponent<Tile>().noise = noise;
                    nTile.AddComponent<BoxCollider2D>();
                    nTile.GetComponent<BoxCollider2D>().size = Vector3.one * spriteBound;
                    nTile.transform.parent = chunk.transform;
                }
                /*tile.AddComponent<BoxCollider2D>();
                tile.GetComponent<BoxCollider2D>().size = Vector2.one * spriteBound;*/
            }
        }
        chunk.transform.position -= new Vector3(origin.x, origin.y, 0);
        chunk.transform.parent = tilesParent;
        return chunk;
    }

    public float generateNoise(int x, int z, Vector2 whichTile)
    {
        float xNoise = (x + whichTile.x) / detailScale;
        float zNoise = (z + whichTile.y) / detailScale;

        return Mathf.PerlinNoise(xNoise, zNoise);
    }

    public void DrawLR(LineRenderer lr, float radius, int cornerCount, float lineWidth)
    {
        lr.positionCount = cornerCount;
        float TAU = Mathf.PI * 2;
        for (int i = 0; i < cornerCount; i++)
        {
            float currentRadian = (float)i / cornerCount * TAU;

            float xPos = Mathf.Cos(currentRadian) * radius;
            float yPos = Mathf.Sin(currentRadian) * radius;
            Vector3 pos = new Vector3(xPos, yPos, 0);
            lr.SetPosition(i, pos);
        }
        lr.loop = true;
        lr.startWidth = lineWidth;
        lr.endWidth = lineWidth;
    }

    GameObject CreateTile(int x, int y)
    {
        Vector3 pos = new Vector3(x * tileSize * spriteBound, y * tileSize * spriteBound, 5);
        GameObject tile = new GameObject();
        tile.transform.position = pos;
        tile.transform.localScale = Vector2.one * tileSize;
        return tile;
    }

    public void CheckOtherTiles(Transform forWhichObject, int x, int y)
    {
        /*Debug.Log($"{x +" , "+y}");
        if(x >= 0 && y-1 >= 0)
        {
            if (tiles[x, y - 1].transform.GetComponent<Tile>().noise < 0.3f)
            {
                GameObject newTile = CreateTile(x, y, forWhichObject);
                newTile.AddComponent<SpriteRenderer>();
                newTile.transform.GetComponent<SpriteRenderer>().sprite = leftCliff;

            }
        }

        if(x >= 0 && y+1 < chunkSizeY)
        {
            if (tiles[x, y + 1].transform.GetComponent<Tile>().noise < 0.3f)
            {
                GameObject newTile = CreateTile(x, y, forWhichObject);
                newTile.AddComponent<SpriteRenderer>();
                newTile.transform.GetComponent<SpriteRenderer>().sprite = rightCliff;
            }
        }

        if(x+1 < chunkSizeX && y >= 0)
        {
            if (tiles[x + 1, y].transform.GetComponent<Tile>().noise < 0.3f)
            {
                GameObject newTile = CreateTile(x, y, forWhichObject);
                newTile.AddComponent<SpriteRenderer>();
                newTile.transform.GetComponent<SpriteRenderer>().sprite = upperCliff;
            }
        }

        if(x-1 >= 0 && y>=0)
        {
            if (tiles[x - 1, y].transform.GetComponent<Tile>().noise < 0.3f)
            {
                GameObject newTile = CreateTile(x, y, forWhichObject);
                newTile.AddComponent<SpriteRenderer>();
                newTile.transform.GetComponent<SpriteRenderer>().sprite = downCliff;
            }
        }*/
        if (
        generateNoise(x, y + 1, new Vector2(x * tileSize * spriteBound, (y + 1) * tileSize * spriteBound)) < 0.3f &&
        generateNoise(x, y - 1, new Vector2(x * tileSize * spriteBound, (y - 1) * tileSize * spriteBound)) > 0.3f &&
        generateNoise(x + 1, y, new Vector2((x + 1) * tileSize * spriteBound, y * tileSize * spriteBound)) > 0.3f &&
        generateNoise(x - 1, y, new Vector2((x - 1) * tileSize * spriteBound, y * tileSize * spriteBound)) > 0.3f &&
        generateNoise(x - 1, y - 1, new Vector2((x - 1) * tileSize * spriteBound, (y - 1) * tileSize * spriteBound)) > 0.3f &&
        generateNoise(x + 1, y - 1, new Vector2((x + 1) * tileSize * spriteBound, (y - 1) * tileSize * spriteBound)) > 0.3f &&
        generateNoise(x - 1, y + 1, new Vector2((x - 1) * tileSize * spriteBound, (y + 1) * tileSize * spriteBound)) > 0.3f &&
        generateNoise(x + 1, y + 1, new Vector2((x + 1) * tileSize * spriteBound, (y + 1) * tileSize * spriteBound)) > 0.3f
            )
        {
            GameObject newTile = CreateTile(x, y);
            newTile.AddComponent<SpriteRenderer>();
            newTile.transform.GetComponent<SpriteRenderer>().sprite = upperCliff;

        }
        else if (
        generateNoise(x, y - 1, new Vector2(x * tileSize * spriteBound, (y - 1) * tileSize * spriteBound)) < 0.3f &&
        generateNoise(x, y + 1, new Vector2(x * tileSize * spriteBound, (y + 1) * tileSize * spriteBound)) > 0.3f &&
        generateNoise(x + 1, y, new Vector2((x + 1) * tileSize * spriteBound, y * tileSize * spriteBound)) > 0.3f &&
        generateNoise(x - 1, y, new Vector2((x - 1) * tileSize * spriteBound, y * tileSize * spriteBound)) > 0.3f &&
        generateNoise(x - 1, y - 1, new Vector2((x - 1) * tileSize * spriteBound, (y - 1) * tileSize * spriteBound)) > 0.3f &&
        generateNoise(x + 1, y - 1, new Vector2((x + 1) * tileSize * spriteBound, (y - 1) * tileSize * spriteBound)) > 0.3f &&
        generateNoise(x - 1, y + 1, new Vector2((x - 1) * tileSize * spriteBound, (y + 1) * tileSize * spriteBound)) > 0.3f &&
        generateNoise(x + 1, y + 1, new Vector2((x + 1) * tileSize * spriteBound, (y + 1) * tileSize * spriteBound)) > 0.3f
        )
        {
            GameObject newTile = CreateTile(x, y);
            newTile.AddComponent<SpriteRenderer>();
            newTile.transform.GetComponent<SpriteRenderer>().sprite = downCliff;
        }
        else if (
        generateNoise(x + 1, y, new Vector2((x + 1) * tileSize * spriteBound, (y) * tileSize * spriteBound)) < 0.3f &&
        generateNoise(x, y + 1, new Vector2(x * tileSize * spriteBound, (y + 1) * tileSize * spriteBound)) > 0.3f &&
        generateNoise(x, y - 1, new Vector2(x * tileSize * spriteBound, (y - 1) * tileSize * spriteBound)) > 0.3f &&
        generateNoise(x - 1, y, new Vector2((x - 1) * tileSize * spriteBound, y * tileSize * spriteBound)) > 0.3f &&
        generateNoise(x - 1, y - 1, new Vector2((x - 1) * tileSize * spriteBound, (y - 1) * tileSize * spriteBound)) > 0.3f &&
        generateNoise(x + 1, y - 1, new Vector2((x + 1) * tileSize * spriteBound, (y - 1) * tileSize * spriteBound)) > 0.3f &&
        generateNoise(x - 1, y + 1, new Vector2((x - 1) * tileSize * spriteBound, (y + 1) * tileSize * spriteBound)) > 0.3f &&
        generateNoise(x + 1, y + 1, new Vector2((x + 1) * tileSize * spriteBound, (y + 1) * tileSize * spriteBound)) > 0.3f
        )
        {
            GameObject newTile = CreateTile(x, y);
            newTile.AddComponent<SpriteRenderer>();
            newTile.transform.GetComponent<SpriteRenderer>().sprite = rightCliff;
        }
        else if (
         generateNoise(x - 1, y, new Vector2((x - 1) * tileSize * spriteBound, y * tileSize * spriteBound)) < 0.3f &&
         generateNoise(x, y + 1, new Vector2(x * tileSize * spriteBound, (y + 1) * tileSize * spriteBound)) > 0.3f &&
         generateNoise(x + 1, y, new Vector2((x + 1) * tileSize * spriteBound, y * tileSize * spriteBound)) > 0.3f &&
         generateNoise(x, y - 1, new Vector2((x) * tileSize * spriteBound, (y - 1) * tileSize * spriteBound)) > 0.3f &&
         generateNoise(x - 1, y - 1, new Vector2((x - 1) * tileSize * spriteBound, (y - 1) * tileSize * spriteBound)) > 0.3f &&
         generateNoise(x + 1, y - 1, new Vector2((x + 1) * tileSize * spriteBound, (y - 1) * tileSize * spriteBound)) > 0.3f &&
         generateNoise(x - 1, y + 1, new Vector2((x - 1) * tileSize * spriteBound, (y + 1) * tileSize * spriteBound)) > 0.3f &&
         generateNoise(x + 1, y + 1, new Vector2((x + 1) * tileSize * spriteBound, (y + 1) * tileSize * spriteBound)) > 0.3f
         )
        {
            GameObject newTile = CreateTile(x, y);
            newTile.AddComponent<SpriteRenderer>();
            newTile.transform.GetComponent<SpriteRenderer>().sprite = leftCliff;
        }


        /*if (x - 1 >= 0 && y - 1 >= 0)
        {
            if (tiles[x - 1, y].transform.GetComponent<Tile>().noise > 0.3f)
            {
                GameObject newTile = CreateTile(x, y, forWhichObject);
                newTile.AddComponent<SpriteRenderer>();
                newTile.transform.GetComponent<SpriteRenderer>().sprite = downCliff;
            }
        }

        if (x - 1 >= 0 && y + 1 < chunkSizeY)
        {
            if (tiles[x - 1, y].transform.GetComponent<Tile>().noise > 0.3f)
            {
                tiles[x - 1, y].transform.GetComponent<SpriteRenderer>().sprite = downCliff;
            }
        }

        if (x + 1 < chunkSizeX && y + 1 < chunkSizeY)
        {
            if (tiles[x - 1, y].transform.GetComponent<Tile>().noise > 0.3f)
            {
                tiles[x - 1, y].transform.GetComponent<SpriteRenderer>().sprite = downCliff;
            }
        }

        if (x + 1 < chunkSizeX && y - 1 >= 0)
        {
            if (tiles[x - 1, y].transform.GetComponent<Tile>().noise > 0.3f)
            {
                tiles[x - 1, y].transform.GetComponent<SpriteRenderer>().sprite = downCliff;
            }
        }*/
    }
}

    /*
     * using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class GeneralManager : MonoBehaviour
    {
        public static GeneralManager instance;

        public GameObject[,] tiles;

        public Sprite downCliff;
        public Sprite leftCliff;
        public Sprite rightCliff;
        public Sprite upperCliff;
        public Sprite upperLeftCliff;
        public Sprite upperRightCliff;
        public Sprite downRightCliff;
        public Sprite downLeftCliff;


        public Sprite[] grassSprites;
        public Sprite dirtParticleSprite;

        public float tileSize;
        public int chunkSizeX;
        public int chunkSizeY;
        public float dirtParticleMaxSpeed;

        [System.NonSerialized]
        public float spriteBound;

        public float gravity;

        public Text noiseText;
        public GameObject selectTile;

        GameObject sceneCamera;

        Vector2 origin;

        public void Start()
        {
            tiles = new GameObject[chunkSizeX, chunkSizeY];
            sceneCamera = Camera.main.gameObject;
            instance = this;
            spriteBound = grassSprites[0].bounds.size.x;
            DrawLR(selectTile.GetComponent<LineRenderer>(), (tileSize * spriteBound) - selectTile.GetComponent<LineRenderer>().startWidth * 2, 4,0.08f);
            origin = new Vector2(Camera.main.orthographicSize * Screen.width / Screen.height, Camera.main.orthographicSize);
            PlayerFlower.instance.transform.position = -origin + Vector2.up * PlayerFlower.instance.transform.GetComponentInChildren<SpriteRenderer>().sprite.bounds.size.y/2;
            CreatingTiles(0);
        }

        private void Update()
        {
            sceneCamera.transform.position = Vector3.Lerp(sceneCamera.transform.position, new Vector3(PlayerFlower.instance.transform.position.x,PlayerFlower.instance.transform.position.y,sceneCamera.transform.position.z), 0.2f);
            if(Input.GetMouseButton(0))
            {
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector3.forward);
                if(hit.transform.tag == "main")
                {
                    GameObject sproutWave = new GameObject();
                    sproutWave.AddComponent<LineRenderer>();
                }
                else
                {
                    noiseText.gameObject.SetActive(true);
                    selectTile.gameObject.SetActive(true);
                    selectTile.transform.position = new Vector3(hit.transform.position.x, hit.transform.position.y, -3);
                    noiseText.transform.position = (Vector2)hit.transform.position + Vector2.up * spriteBound;
                    noiseText.text = hit.transform.GetComponent<Tile>().noise.ToString();
                }
            }
            else
            {
                noiseText.gameObject.SetActive(false);
                selectTile.gameObject.SetActive(false);
            }
        }


        public void CreatingTiles(float height)
        {
            for(int x = 0; x < chunkSizeX; x++)
            {
                for(int y = 0; y < chunkSizeY; y++)
                {
                    GameObject tile = CreateTileSprite(x,y);
                    float noise = generateNoise(x, y, 1f, tile);
                    tile.AddComponent<Tile>();
                    tile.GetComponent<Tile>().noise = noise;
                    if(noise > 0.3f)
                    {
                        CreatingTiles(tile,x,y);
                        GameObject mainTile = CreateTileSprite(x, y);
                        mainTile.AddComponent<SpriteRenderer>();
                        mainTile.GetComponent<SpriteRenderer>().sprite = grassSprites[Random.Range(0,grassSprites.Length)];
                    }
                    tile.AddComponent<BoxCollider2D>();
                    tile.GetComponent<BoxCollider2D>().size = Vector2.one * spriteBound;
                    tiles[x, y] = tile;
                }
            }
        }

        private float generateNoise(int x, int z,float detailScale,GameObject whichTile)
        {
            float xNoise = (x + whichTile.transform.position.x) / detailScale;
            float zNoise = (z + whichTile.transform.position.y) / detailScale;

            return Mathf.PerlinNoise(xNoise, zNoise);
        }

        GameObject CreateTileSprite(int x, int y)
        {
            Vector3 pos = new Vector3(y * tileSize * spriteBound - origin.x, x * tileSize * spriteBound - origin.y, 5);
            GameObject tile = new GameObject();
            tile.transform.position = pos;
            tile.transform.localScale = Vector2.one * tileSize;
            return tile;
        }

        void DrawLR(LineRenderer lr,float radius,int cornerCount,float lineWidth)
        {
            lr.positionCount = cornerCount;
            float TAU = Mathf.PI * 2;
            for (int i = 0; i < cornerCount; i++)
            {
                float currentRadian = (float)i / cornerCount * TAU;

                float xPos = Mathf.Cos(currentRadian) * radius;
                float yPos = Mathf.Sin(currentRadian) * radius;
                Vector3 pos = new Vector3(xPos, yPos, 0);
                lr.SetPosition(i, pos);
            }
            lr.loop = true;
            lr.startWidth = lineWidth;
            lr.endWidth = lineWidth;
        }

        public void CreatingTiles(GameObject forWhichObject,int x, int y)
        {
            if(generateNoise(x+1,y,1f,forWhichObject) < 0.3f)
            {
                GameObject newTile = CreateTileSprite(x,y);
                newTile.AddComponent<SpriteRenderer>();
                newTile.GetComponent<SpriteRenderer>().sprite = rightCliff;
                newTile.transform.parent = forWhichObject.transform;
            }
            if (generateNoise(x-1, y, 1f, forWhichObject) < 0.3f)
            {
                GameObject newTile = CreateTileSprite(x, y);
                newTile.AddComponent<SpriteRenderer>();
                newTile.GetComponent<SpriteRenderer>().sprite = leftCliff;
                newTile.transform.parent = forWhichObject.transform;
            }
            if (generateNoise(x, y+1, 1f, forWhichObject) < 0.3f)
            {
                GameObject newTile = CreateTileSprite(x, y);
                newTile.AddComponent<SpriteRenderer>();
                newTile.GetComponent<SpriteRenderer>().sprite = upperCliff;
                newTile.transform.parent = forWhichObject.transform;
            }
            if (generateNoise(x, y-1, 1f, forWhichObject) < 0.3f)
            {
                GameObject newTile = CreateTileSprite(x , y);
                newTile.AddComponent<SpriteRenderer>();
                newTile.GetComponent<SpriteRenderer>().sprite = downCliff;
                newTile.transform.parent = forWhichObject.transform;
            }
            if (generateNoise(x - 1 , y - 1, 1f, forWhichObject) < 0.3f)
            {
                GameObject newTile = CreateTileSprite(x, y);
                newTile.AddComponent<SpriteRenderer>();
                newTile.GetComponent<SpriteRenderer>().sprite = downLeftCliff;
                newTile.transform.parent = forWhichObject.transform;
            }
            if (generateNoise(x - 1, y + 1, 1f, forWhichObject) < 0.3f)
            {
                GameObject newTile = CreateTileSprite(x, y);
                newTile.AddComponent<SpriteRenderer>();
                newTile.GetComponent<SpriteRenderer>().sprite = upperLeftCliff;
                newTile.transform.parent = forWhichObject.transform;
            }
            if (generateNoise(x + 1, y + 1, 1f, forWhichObject) < 0.3f)
            {
                GameObject newTile = CreateTileSprite(x, y);
                newTile.AddComponent<SpriteRenderer>();
                newTile.GetComponent<SpriteRenderer>().sprite = upperRightCliff;
                newTile.transform.parent = forWhichObject.transform;
            }
            if (generateNoise(x+1, y - 1, 1f, forWhichObject) < 0.3f)
            {
                GameObject newTile = CreateTileSprite(x, y);
                newTile.AddComponent<SpriteRenderer>();
                newTile.GetComponent<SpriteRenderer>().sprite = upperLeftCliff;
                newTile.transform.parent = forWhichObject.transform;
            }
        }
    }
    */

