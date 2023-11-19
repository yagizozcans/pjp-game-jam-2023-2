using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFlower : MonoBehaviour
{
    public static PlayerFlower instance;

    public float movementSpeed;

    bool isMoving;
    bool isMoved;

    Vector2 whichWayToMove;

    public GameObject circle;

    int currentX = 0;
    int currentY = 0;

    bool isTileFull = false;
    void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        Movement();
    }

    void Movement()
    {
        if((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) && !isMoving)
        {
            MovementActive(Vector2.up);
            RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position + whichWayToMove * GeneralManager.instance.tileSize * GeneralManager.instance.spriteBound, Vector3.forward);
            if(hit.transform != null && hit.transform.tag != "turret")
            {
                GeneralManager.instance.tileOrigin += Vector2.up;
                currentY++;
                if (currentY >= GeneralManager.instance.chunkSizeY)
                {
                    GeneralManager.instance.Create9Chunks((int)GeneralManager.instance.tileOrigin.x, (int)GeneralManager.instance.tileOrigin.y);
                    currentY = 0;
                    currentX = 0;
                }
            }


            //GeneralManager.instance.CreatingTiles();
        }
        if ((Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) && !isMoving)
        {
            MovementActive(Vector2.down);
            RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position + whichWayToMove * GeneralManager.instance.tileSize * GeneralManager.instance.spriteBound, Vector3.forward);
            if(hit.transform != null && hit.transform.tag != "turret")
            {
                GeneralManager.instance.tileOrigin += Vector2.down;
                currentY--;
                if (currentY <= -GeneralManager.instance.chunkSizeY)
                {
                    GeneralManager.instance.Create9Chunks((int)GeneralManager.instance.tileOrigin.x, (int)GeneralManager.instance.tileOrigin.y);
                    currentY = 0;
                    currentX = 0;
                }
            }

            //GeneralManager.instance.CreatingTileRow((int)GeneralManager.instance.tileOrigin.y);
        }
        if ((Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow) && !isMoving))
        {
            MovementActive(Vector2.right);
            RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position + whichWayToMove * GeneralManager.instance.tileSize * GeneralManager.instance.spriteBound, Vector3.forward);
            if(hit.transform!= null && hit.transform.tag != "turret")
            {
                GeneralManager.instance.tileOrigin += Vector2.right;
                currentX++;
                if (currentX >= GeneralManager.instance.chunkSizeX)
                {

                    GeneralManager.instance.Create9Chunks((int)GeneralManager.instance.tileOrigin.x, (int)GeneralManager.instance.tileOrigin.y);
                    currentY = 0;
                    currentX = 0;
                }
            }
        }
        if ((Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) && !isMoving && GeneralManager.instance)
        {
            MovementActive(Vector2.left);
            RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position + whichWayToMove * GeneralManager.instance.tileSize * GeneralManager.instance.spriteBound, Vector3.forward);
            if(hit.transform != null && hit.transform.tag != "turret")
            {
                GeneralManager.instance.tileOrigin += Vector2.left;
                currentX--;
                if (currentX <= -GeneralManager.instance.chunkSizeX)
                {
                    GeneralManager.instance.Create9Chunks((int)GeneralManager.instance.tileOrigin.x, (int)GeneralManager.instance.tileOrigin.y);
                    currentY = 0;
                    currentX = 0;
                }
            }
        }
        
        if (isMoving && !isMoved)
        {
            transform.localScale = (Vector2)transform.localScale - Vector2.one * movementSpeed * Time.deltaTime;
            if(transform.localScale.x <= 0)
            {
                transform.localScale = Vector2.zero;
                isMoved = true;
                RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position + whichWayToMove * GeneralManager.instance.tileSize * GeneralManager.instance.spriteBound, Vector3.forward);
                if (hit.transform.tag != "turret" && !isTileFull)
                {
                    transform.position = (Vector2)transform.position + whichWayToMove * GeneralManager.instance.tileSize * GeneralManager.instance.spriteBound;
                }
                CreateDirtParticles(10);
            }
        }
        if(isMoving && isMoved)
        {
            transform.localScale = (Vector2)transform.localScale + Vector2.one * movementSpeed * Time.deltaTime;
            if(transform.localScale.x >= 1)
            {
                transform.localScale = Vector2.one;
                isMoving = false;
                isMoved = true;
                isTileFull = false;
            }
        }
    }
    
    void MovementActive(Vector2 whichWay)
    {
        isMoving = true;
        isMoved = false;
        whichWayToMove = whichWay;
        RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position + whichWayToMove * GeneralManager.instance.tileSize * GeneralManager.instance.spriteBound, Vector3.forward);
        if(hit.transform.tag == "turret")
        {
            isTileFull = true;
        }

        CreateDirtParticles(10);
    }

    void CreateDirtParticles(int count)
    {
        for(int i = 0; i < count; i++)
        {
            GameObject dirtParticle = new GameObject();
            float randomAngle = Random.Range(-45f, 45f);
            dirtParticle.transform.position = transform.position;
            dirtParticle.AddComponent<DirtParticle>();
            dirtParticle.AddComponent<SpriteRenderer>();
            dirtParticle.GetComponent<SpriteRenderer>().sprite = GeneralManager.instance.dirtParticleSprite;
            dirtParticle.GetComponent<DirtParticle>().velocity = new Vector2(Mathf.Cos(randomAngle) * GeneralManager.instance.dirtParticleMaxSpeed, Mathf.Sin(randomAngle) * GeneralManager.instance.dirtParticleMaxSpeed);
        }
    }
}
