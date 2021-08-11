using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MagnumFoudation;

/*
public class s_node {
    public int g_cost, h_cost;
    public int f_cost{
        get {
            return g_cost + h_cost;
        }
    }
    public bool walkable = true;
    public Vector2 realPosition;
    public s_node parent;
    public o_collidableobject.COLLISION_T collisionType;
}

public class s_nodegraph : MagnumFoudation.s_nodegraph {

    public s_node[,] nodeGraph;
    public s_node[] nodeGraph2;
    const float tileSize = 25f;
    int xsi = 75, ysi = 75;


    public List<s_node> path = new List<s_node>();
    public void SetNodeSize(int x, int y)
    {
        xsi = x;
        ysi = y;
    }
    public void SetNode(int x, int y, o_collidableobject.COLLISION_T collisionType)
    {
        nodeGraph2[x + (y * xsi)].COLTYPE = (int)collisionType;
    }

    void ResetNodes()
    {
        for (int x = 0; x < xsi; x++)
        {
            for (int y = 0; y < ysi; y++)
            {
                nodeGraph[x, y].h_cost = 0;
                nodeGraph[x, y].g_cost = 0;
                nodeGraph[x, y].parent = null;
            }
        }
    }
    public float CheckYFall(s_node node)
    {
        s_node nextnode = node;
        Vector2Int v = PosToVec(node.realPosition);
        int n = 0;
        while (nextnode != null)
        {
            n++;
            print(v.x + ((v.y - n) * xsi));
            if (v.x + ((v.y + n) * xsi) > nodeGraph2.Length ||
                v.x + ((v.y + n) * xsi) < 0)
                break;
            nextnode = nodeGraph2[v.x + ((v.y - n) * xsi)];
            if (nextnode == null ||
                nextnode.COLTYPE != (int)o_collidableobject.COLLISION_T.FALLING)
                break;
        }
        print((v.y + n) * tileSize);
        return (v.y - n) * tileSize;
    }

    public s_node[] CreateNodeArray(s_map.s_tileobj[] ti)
    {
        s_node[] arr = new s_node[xsi * ysi];
        //o_collidableobject[] col = GameObject.Find("Tiles").GetComponentsInChildren<o_collidableobject>();
        
        int x = 0, y = 0;
        for (int i = 0; i < arr.Length; i++)
        {
            x++;
            if (x == xsi)
            {
                y++;
                x = 0;
            }
            arr[i] = new s_node();
            arr[i].realPosition = new Vector2((x * tileSize), (y * tileSize));
            arr[i].COLTYPE = (int)o_collidableobject.COLLISION_T.NONE;
        }
        foreach (s_map.s_tileobj c in ti)
        {
            Vector2Int v = PosToVec(new Vector3(c.pos_x, c.pos_y));

            if (arr.Length < v.x + (v.y * xsi))
                continue;
            if (0 > v.x + (v.y * xsi))
                continue;
            
            arr[v.x + (v.y * xsi)].realPosition = new Vector2(c.pos_x, c.pos_y);
            arr[v.x + (v.y * xsi)].COLTYPE = (int)c.enumthing;

            if (c.TYPENAME == "teleport_object")
            {
                arr[v.x + (v.y * xsi)].COLTYPE = (int)o_collidableobject.COLLISION_T.NONE;
                arr[v.x + (v.y * xsi)].walkable = true;
            }
            arr[v.x + (v.y * xsi)].walkable = true;
            
        }

        return arr;
    }
    public void CreateNodeGraph(List<s_map.s_tileobj> tiles)
    {
        nodeGraph = new s_node[xsi, ysi];
        for (int x = 0; x < xsi; x++)
        {
            for (int y = 0; y < ysi; y++)
            {
                nodeGraph[x, y] = new s_node();
                nodeGraph[x, y].realPosition = new Vector2(tileSize * x, tileSize * y);
                nodeGraph[x, y].walkable = true;
            }
        }
    }

    public Vector2Int PosToVec(int x_p, int y_p)
    {
        int x = (int)(x_p / tileSize);
        int y = (int)(y_p / tileSize);
        //print("x: " + x + " y: " + y);

        return new Vector2Int(x, y);
    }
    public Vector2Int PosToVec(Vector3 vec)
    {
        int x = (int)(vec.x / tileSize);
        int y = (int)(vec.y / tileSize);
        //print("x: " + x + " y: " + y);

        return new Vector2Int(x, y);
    }
    public s_node PosToNode(Vector2 vec)
    {
        int x = (int)(vec.x / tileSize);
        int y = (int)(vec.y / tileSize);
        if ((x + (y * xsi)) < 0 || (x + (y * xsi)) > nodeGraph2.Length - 1)
        {
            print("Sorry! x: " + x + " y: " + y);
            return null;
        }
        return nodeGraph2[x + (y * xsi)];
    }

    public void PathFind(s_node goal, s_node start)
    {
        List<s_node> OpenList = new List<s_node>();
        HashSet<s_node> ClosedList = new HashSet<s_node>();

        if (!start.walkable || !goal.walkable)
            return;
        OpenList.Add(start);
        path = new List<s_node>();
        
        start.h_cost = Mathf.RoundToInt(HerusticVal(start.realPosition, goal.realPosition));

        while (OpenList.Count > 0) {

            s_node currentNode = OpenList[0];
            for (int i = 1; i < OpenList.Count; i++) {
                s_node cur = OpenList[i];
                if (cur.f_cost < currentNode.f_cost && cur.h_cost < currentNode.h_cost) {
                        currentNode = cur;
                }
            }
            OpenList.Remove(currentNode);
            ClosedList.Add(currentNode);

            if (currentNode == goal)
            {
                path = RetracePath(goal, start);
                ResetNodes();
                print("Gocha");
                return;
            }

            Vector2Int nodepos = PosToVec(currentNode.realPosition);
            List<s_node> neighbours = new List<s_node>();
            for (int x = nodepos.x - 1; x < nodepos.x + 1; x++)
            {
                for (int y = nodepos.y - 1; y < nodepos.y + 1; y++)
                {
                    if (x > xsi || y > ysi || y < 0 || x < 0)
                        continue;
                    
                    if (y == nodepos.y - 1 && x == nodepos.x + 1 ||
                        y == nodepos.y + 1 && x == nodepos.x + 1 ||
                        y == nodepos.y + 1 && x == nodepos.x - 1 ||
                        y == nodepos.y - 1 && x == nodepos.x - 1)
                        continue;
                    
                    s_node targnod = nodeGraph[x, y];
                    
                    neighbours.Add(targnod);
                }
            }
            
            foreach (s_node no in neighbours)
            {
                if (!no.walkable || ClosedList.Contains(no))
                    continue;

                path.Add(no);
                int movcost = currentNode.g_cost + Mathf.RoundToInt(HerusticVal(currentNode.realPosition, no.realPosition)) ;
                if (movcost < no.g_cost ||
                    !OpenList.Contains(no))
                {
                    no.g_cost = movcost;
                    no.h_cost = Mathf.RoundToInt(HerusticVal(no.realPosition, goal.realPosition));
                    no.parent = currentNode;

                    if (!OpenList.Contains(no))
                        OpenList.Add(no);
                }
            }

        }
        print("OOF");
        ResetNodes();
    }
    float HerusticVal(Vector2 a, Vector2 b)
    {
        float distx = Mathf.Abs(a.x - b.x);
        float disty = Mathf.Abs(a.y - b.y);

        
        return Vector2.Distance(a, b) / tileSize;
        //D * (distx + dist)

    }

    public List<s_node> RetracePath(s_node goal, s_node start)
    {
        int i = 0;
        List<s_node> route = new List<s_node>();
        s_node current = goal;
        while (current != start)
        {
            route.Add(current);
            current = current.parent;
            i++;
            if (i == int.MaxValue)
                return route;
        }
        route.Reverse();
        return route;
    }

    
    private void OnDrawGizmos()
    {
        if(nodeGraph2 != null)
            for (int x = 0; x < xsi; x++)
            {
                for (int y = 0; y < ysi; y++)
                {
                    s_node nod = nodeGraph2[x + (y * xsi)];
                    if (nod != null)
                    {
                        Vector2 pos = new Vector2((x * tileSize) + tileSize, (y * tileSize) + tileSize);
                        switch ((o_collidableobject.COLLISION_T)nod.COLTYPE)
                        {
                            default:

                                Gizmos.color = Color.white;
                                Gizmos.DrawWireCube(pos, new Vector2(tileSize, tileSize));
                                break;

                            case o_collidableobject.COLLISION_T.DITCH:

                                Gizmos.color = Color.cyan;
                                Gizmos.DrawWireCube(pos, new Vector2(tileSize, tileSize));
                                break;
                            case o_collidableobject.COLLISION_T.WALL:

                                Gizmos.color = Color.magenta;
                                Gizmos.DrawWireCube(pos, new Vector2(tileSize, tileSize));
                                break;
                        }
                    }

                    if (path.Find(n => n == nodeGraph2[x + (y * xsi)]) != null)
                    {
                        Gizmos.color = Color.cyan;
                        Gizmos.DrawCube(new Vector2(x * tileSize, y * tileSize), new Vector2(tileSize, tileSize));
                    }
                    Vector2Int vec = PosToVec(GameObject.Find("Player").transform.position);
                    if (vec != new Vector2Int(0,0))
                        Gizmos.DrawCube(new Vector2(vec.x * tileSize, vec.y * tileSize), new Vector2(tileSize, tileSize));
                    else
                        Gizmos.color = Color.white;

                    if (nodeGraph[x, y].walkable)
                    {
                        Gizmos.DrawWireCube(new Vector2(x * tileSize, y * tileSize), new Vector2(tileSize, tileSize));
                    }

                }
            }
    }
}
*/