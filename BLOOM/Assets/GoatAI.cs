using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoatAI : MonoBehaviour
{
    public GameObject playerflower;

    public GameObject circle;

    public class Node
    {
        public float gCost;
        public float hCost;
        public float fCost;
        public Vector2 nodePosition;
    }
    public List<Node> openNodes = new List<Node>();
    public List<Node> closedNodes = new List<Node>();


    private void Start()
    {
        StartCoroutine(startFunction());
    }

    public Vector2[] positions;

    IEnumerator startFunction()
    {
        yield return new WaitForSeconds(2);
        Node startNode = CalculateNode(transform.position.x, transform.position.y, playerflower.transform.position);
        openNodes.Clear();
        closedNodes.Clear();
        openNodes.Add(startNode);
        CreatingPath();
        Movement();
    }

    /*void CreatingPath()
    {
        int i = 0;
        GetComponent<LineRenderer>().positionCount = 1;
        GetComponent<LineRenderer>().SetPosition(0, transform.position);
        while (i < 30)
        {
            float lowest_f_cost = 10000;
            Node currentNode = new Node();
            foreach (Node node in openNodes)
            {
                if (node.fCost < lowest_f_cost)
                {
                    currentNode = node;
                    lowest_f_cost = node.fCost;
                }
            }
            List<Node> neighbours = new List<Node>();

            neighbours.Add(CalculateNode(currentNode.nodePosition.x, currentNode.nodePosition.y + GeneralManager.instance.tileSize * GeneralManager.instance.spriteBound, playerflower.transform.position));
            neighbours.Add(CalculateNode(currentNode.nodePosition.x - GeneralManager.instance.tileSize * GeneralManager.instance.spriteBound, currentNode.nodePosition.y, playerflower.transform.position));
            neighbours.Add(CalculateNode(currentNode.nodePosition.x + GeneralManager.instance.tileSize * GeneralManager.instance.spriteBound, currentNode.nodePosition.y, playerflower.transform.position));
            neighbours.Add(CalculateNode(currentNode.nodePosition.x, currentNode.nodePosition.y - GeneralManager.instance.tileSize * GeneralManager.instance.spriteBound, playerflower.transform.position));

            foreach (Node node in neighbours)
            {
                if (Physics2D.Raycast(node.nodePosition, Vector3.forward).transform != null && !closedNodes.Contains(node))
                {
                    if(node.fCost < lowest_f_cost)
                    {
                        Debug.Log(node.fCost);
                        openNodes.Add(node);
                        lowest_f_cost = node.fCost;
                        GetComponent<LineRenderer>().positionCount++;
                        GetComponent<LineRenderer>().SetPosition(i+1, node.nodePosition);
                    }
                }
            }


            openNodes.Remove(currentNode);
            closedNodes.Add(currentNode);

            i++;
        }
    }*/

    void CreatingPath()
    {
        int i = 1;
        bool currentNodeController = false;
        GetComponent<LineRenderer>().positionCount = 1;
        GetComponent<LineRenderer>().SetPosition(0, transform.position);
        Node currentNode = new Node();
        foreach(Transform trsf in transform)
        {
            Destroy(trsf.gameObject);
        }
        while (!currentNodeController)
        {
            float lowest_f_cost = 10000;
            foreach (Node node in openNodes)
            {
                if (node.fCost < lowest_f_cost)
                {
                    currentNode = node;
                    lowest_f_cost = node.fCost;
                }
            }
            openNodes.Remove(currentNode);
            closedNodes.Add(currentNode);

            List<Node> neighbours = new List<Node>();

            neighbours.Add(CalculateNode(currentNode.nodePosition.x, currentNode.nodePosition.y + GeneralManager.instance.tileSize * GeneralManager.instance.spriteBound, playerflower.transform.position));
            neighbours.Add(CalculateNode(currentNode.nodePosition.x - GeneralManager.instance.tileSize * GeneralManager.instance.spriteBound, currentNode.nodePosition.y, playerflower.transform.position));
            neighbours.Add(CalculateNode(currentNode.nodePosition.x + GeneralManager.instance.tileSize * GeneralManager.instance.spriteBound, currentNode.nodePosition.y, playerflower.transform.position));
            neighbours.Add(CalculateNode(currentNode.nodePosition.x, currentNode.nodePosition.y - GeneralManager.instance.tileSize * GeneralManager.instance.spriteBound, playerflower.transform.position));

            foreach (Node node in neighbours)
            {
                if (Physics2D.Raycast(node.nodePosition, Vector3.forward).transform != null && !closedNodes.Contains(node))
                {
                    if (node.fCost <= lowest_f_cost && !openNodes.Contains(node))
                    {
                        openNodes.Add(node);
                        lowest_f_cost = node.fCost;
                        currentNode = node;
                    }
                    else
                    {
                        closedNodes.Add(node);
                    }
                }
                else if(Physics2D.Raycast(node.nodePosition, Vector3.forward).transform == null && !closedNodes.Contains(node))
                {
                    closedNodes.Add(node);
                }
                else if(Physics2D.Raycast(node.nodePosition, Vector3.forward).transform != null && !closedNodes.Contains(node))
                {
                    closedNodes.Add(node);
                }
            }
            GetComponent<LineRenderer>().positionCount++;
            GetComponent<LineRenderer>().SetPosition(i, currentNode.nodePosition);
            if (Mathf.Abs(currentNode.nodePosition.x) < 0.1f)
            {
                break;
            }
            i++;
            if(i > 40)
            {
                break;
            }
        }
        positions = new Vector2[GetComponent<LineRenderer>().positionCount];
        for(int k = 0; k < GetComponent<LineRenderer>().positionCount; k++)
        {
            Instantiate(circle, GetComponent<LineRenderer>().GetPosition(k), Quaternion.identity, transform);
            positions[k] = GetComponent<LineRenderer>().GetPosition(k);
        }
    }

    public void Movement()
    {
        transform.position = GetComponent<LineRenderer>().GetPosition(1);
    }

    public Node CalculateNode(float xValue, float yValue, Vector2 target)
    {
        int gCost = 0;
        int hCost = 0;
        if (Mathf.Abs(xValue - target.x) <= Mathf.Abs(yValue - target.y))
        {
            int count = 0;

            for (int x = 0; x < Mathf.Abs(xValue - target.x) / (GeneralManager.instance.tileSize * GeneralManager.instance.spriteBound); x++)
            {
                count++;
                hCost += 14;
            }
            for (int y = count; y < Mathf.Abs(yValue - target.y) / (GeneralManager.instance.tileSize * GeneralManager.instance.spriteBound); y++)
            {
                count++;
                hCost += 10;
            }
        }
        else
        {
            int count = 0;

            for (int y = 0; y < Mathf.Abs(yValue - target.y) / (GeneralManager.instance.tileSize * GeneralManager.instance.spriteBound); y++)
            {
                count++;
                hCost += 14;
            }
            for (int x = count; x < Mathf.Abs(xValue - target.x) / (GeneralManager.instance.tileSize * GeneralManager.instance.spriteBound); x++)
            {
                count++;
                hCost += 10;
            }
        }

        if (Mathf.Abs((xValue-transform.position.x)) 
            < Mathf.Abs((yValue - transform.position.y)))
        {
            int count = 0;

            for (int x = 0; x < Mathf.Abs((xValue - transform.position.x) / GeneralManager.instance.tileSize * GeneralManager.instance.spriteBound); x++)
            {
                count++;
                gCost += 14;
            }
            for (int y = count; y < Mathf.Abs((yValue - transform.position.y) / GeneralManager.instance.tileSize * GeneralManager.instance.spriteBound); y++)
            {
                count++;
                gCost += 10;
            }
        }
        else
        {
            int count = 0;

            for (int y = 0; y < Mathf.Abs((yValue - transform.position.y) / GeneralManager.instance.tileSize * GeneralManager.instance.spriteBound); y++)
            {
                count++;
                gCost += 14;
            }
            for (int x = count; x < Mathf.Abs((xValue - transform.position.x) / GeneralManager.instance.tileSize * GeneralManager.instance.spriteBound); x++)
            {
                count++;
                gCost += 10;
            }
        }
        Node newNode = new Node();
        newNode.hCost = hCost;
        newNode.gCost = gCost;
        newNode.fCost = gCost + hCost;
        newNode.nodePosition.x = xValue;
        newNode.nodePosition.y = yValue;
        Debug.Log($"h cost is -> {hCost} | g cost is -> {gCost}");
        return newNode;
    }
}
/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoatAI : MonoBehaviour
{
    public GameObject playerflower;

    public GameObject circle;

    public class Node
    {
        public float gCost;
        public float hCost;
        public float fCost;
        public Vector2 nodePosition;
    }
    public List<Node> openNodes = new List<Node>();
    public List<Node> closedNodes = new List<Node>();


    private void Start()
    {
        StartCoroutine(startFunction());
    }

    IEnumerator startFunction()
    {
        yield return new WaitForEndOfFrame();
        Node startNode = CalculateNode(transform.position.x, transform.position.y, playerflower.transform.position);
        openNodes.Clear();
        closedNodes.Clear();
        openNodes.Add(startNode);

        CreatingPath();
        StartCoroutine(startFunction());
    }

    /*void CreatingPath()
    {
        int i = 0;
        GetComponent<LineRenderer>().positionCount = 1;
        GetComponent<LineRenderer>().SetPosition(0, transform.position);
        while (i < 30)
        {
            float lowest_f_cost = 10000;
            Node currentNode = new Node();
            foreach (Node node in openNodes)
            {
                if (node.fCost < lowest_f_cost)
                {
                    currentNode = node;
                    lowest_f_cost = node.fCost;
                }
            }
            List<Node> neighbours = new List<Node>();

            neighbours.Add(CalculateNode(currentNode.nodePosition.x, currentNode.nodePosition.y + GeneralManager.instance.tileSize * GeneralManager.instance.spriteBound, playerflower.transform.position));
            neighbours.Add(CalculateNode(currentNode.nodePosition.x - GeneralManager.instance.tileSize * GeneralManager.instance.spriteBound, currentNode.nodePosition.y, playerflower.transform.position));
            neighbours.Add(CalculateNode(currentNode.nodePosition.x + GeneralManager.instance.tileSize * GeneralManager.instance.spriteBound, currentNode.nodePosition.y, playerflower.transform.position));
            neighbours.Add(CalculateNode(currentNode.nodePosition.x, currentNode.nodePosition.y - GeneralManager.instance.tileSize * GeneralManager.instance.spriteBound, playerflower.transform.position));

            foreach (Node node in neighbours)
            {
                if (Physics2D.Raycast(node.nodePosition, Vector3.forward).transform != null && !closedNodes.Contains(node))
                {
                    if(node.fCost < lowest_f_cost)
                    {
                        Debug.Log(node.fCost);
                        openNodes.Add(node);
                        lowest_f_cost = node.fCost;
                        GetComponent<LineRenderer>().positionCount++;
                        GetComponent<LineRenderer>().SetPosition(i+1, node.nodePosition);
                    }
                }
            }


            openNodes.Remove(currentNode);
            closedNodes.Add(currentNode);

            i++;
        }
    }

void CreatingPath()
{
    int i = 1;
    bool currentNodeController = false;
    GetComponent<LineRenderer>().positionCount = 1;
    GetComponent<LineRenderer>().SetPosition(0, transform.position);
    Node currentNode = new Node();
    while (!currentNodeController)
    {
        float lowest_f_cost = 10000;
        foreach (Node node in openNodes)
        {
            if (node.fCost < lowest_f_cost)
            {
                currentNode = node;
                lowest_f_cost = node.fCost;
            }
        }
        openNodes.Remove(currentNode);
        closedNodes.Add(currentNode);

        List<Node> neighbours = new List<Node>();

        neighbours.Add(CalculateNode(currentNode.nodePosition.x, currentNode.nodePosition.y + GeneralManager.instance.tileSize * GeneralManager.instance.spriteBound, playerflower.transform.position));
        neighbours.Add(CalculateNode(currentNode.nodePosition.x - GeneralManager.instance.tileSize * GeneralManager.instance.spriteBound, currentNode.nodePosition.y, playerflower.transform.position));
        neighbours.Add(CalculateNode(currentNode.nodePosition.x + GeneralManager.instance.tileSize * GeneralManager.instance.spriteBound, currentNode.nodePosition.y, playerflower.transform.position));
        neighbours.Add(CalculateNode(currentNode.nodePosition.x, currentNode.nodePosition.y - GeneralManager.instance.tileSize * GeneralManager.instance.spriteBound, playerflower.transform.position));

        foreach (Node node in neighbours)
        {
            if (Physics2D.Raycast(node.nodePosition, Vector3.forward).transform != null && !closedNodes.Contains(node))
            {
                if (node.fCost <= lowest_f_cost && !openNodes.Contains(node))
                {
                    openNodes.Add(node);
                    lowest_f_cost = node.fCost;
                    GetComponent<LineRenderer>().positionCount++;
                    GetComponent<LineRenderer>().SetPosition(i, node.nodePosition);
                    currentNode = node;
                }
                else
                {
                    closedNodes.Add(node);
                }
            }
            else if (Physics2D.Raycast(node.nodePosition, Vector3.forward).transform == null && !closedNodes.Contains(node))
            {
                closedNodes.Add(node);
                GetComponent<LineRenderer>().positionCount++;
                GetComponent<LineRenderer>().SetPosition(i, currentNode.nodePosition);
            }

        }
        i++;
        if (i > 40)
        {
            break;
        }
    }
}

public Node CalculateNode(float xValue, float yValue, Vector2 target)
{
    int gCost = 0;
    int hCost = 0;
    if (Mathf.Abs(xValue - target.x) <= Mathf.Abs(yValue - target.y))
    {
        int count = 0;

        for (int x = 0; x < Mathf.Abs(xValue - target.x) / (GeneralManager.instance.tileSize * GeneralManager.instance.spriteBound); x++)
        {
            count++;
            hCost += 14;
        }
        for (int y = count; y < Mathf.Abs(yValue - target.y) / (GeneralManager.instance.tileSize * GeneralManager.instance.spriteBound); y++)
        {
            count++;
            hCost += 10;
        }
    }
    else
    {
        int count = 0;

        for (int y = 0; y < Mathf.Abs(yValue - target.y) / (GeneralManager.instance.tileSize * GeneralManager.instance.spriteBound); y++)
        {
            count++;
            hCost += 14;
        }
        for (int x = count; x < Mathf.Abs(xValue - target.x) / (GeneralManager.instance.tileSize * GeneralManager.instance.spriteBound); x++)
        {
            count++;
            hCost += 10;
        }
    }

    if (Mathf.Abs(xValue - transform.position.x) <= Mathf.Abs(yValue - transform.position.y))
    {
        int count = 0;
        for (int x = 0; x < (int)(Mathf.Abs(xValue - transform.position.x) / (GeneralManager.instance.tileSize * GeneralManager.instance.spriteBound)); x++)
        {
            count++;
            gCost += 14;
        }
        for (int y = count; y < (int)(Mathf.Abs(yValue - transform.position.y) / (GeneralManager.instance.tileSize * GeneralManager.instance.spriteBound)); y++)
        {
            count++;
            gCost += 10;

        }
    }
    else
    {
        int count = 0;

        for (int y = 0; y < (int)(Mathf.Abs(yValue - transform.position.y) / (GeneralManager.instance.tileSize * GeneralManager.instance.spriteBound)); y++)
        {
            count++;
            gCost += 14;
        }
        for (int x = count; x < (int)(Mathf.Abs(xValue - transform.position.x) / (GeneralManager.instance.tileSize * GeneralManager.instance.spriteBound)); x++)
        {
            count++;
            gCost += 10;
        }
    }
    Node newNode = new Node();
    newNode.hCost = hCost;
    newNode.gCost = gCost;
    newNode.fCost = gCost + hCost;
    newNode.nodePosition.x = xValue;
    newNode.nodePosition.y = yValue;
    Debug.Log($"h cost is -> {hCost} | g cost is -> {gCost}");
    return newNode;
}
}
*/
