using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Pathfinding_AStar : PathFinding
{
    [System.Serializable]
    class NodeInformation
    {
        public GridNode node;
        public NodeInformation parent;
        public float gCost;
        public float hCost;
        public float fCost;
        
        public NodeInformation(GridNode node, NodeInformation parent, float gCost, float hCost)
        {
            this.node = node;
            this.parent = parent;
            this.gCost = gCost;
            this.hCost = hCost;
            fCost = gCost + hCost;
        }
        //Add node information to open and closed lists.
        //currentnodeinfo will later include lowest instead of null, and have manhattan and euclidian.
        public void UpdateNodeInformation(NodeInformation parent, float gCost, float hCost)
        {
            this.parent = parent;
            this.gCost = gCost;
            this.hCost = hCost;
            fCost = gCost + hCost;
        }
    }

    public Pathfinding_AStar(bool allowDiagonal, bool cutCorners) : base(allowDiagonal, cutCorners)
    {
        m_AllowDiagonal = allowDiagonal;
        m_CutCorners = cutCorners;
    }
    //https://ebookcentral.proquest.com/lib/staffordshire/reader.action?docID=7007113&ppg=100
    //https://staffsuniversity.sharepoint.com/:p:/s/TheVault/ESug2A2TnKlEr4hbLUBox-IBwq7XnfaP69lC5liqZcG-nw?e=MfKiLO
    public override void GeneratePath(GridNode start, GridNode end)
    {


        List<NodeInformation> openList = new List<NodeInformation>();
        List<NodeInformation> closedList = new List<NodeInformation>();
        List<NodeInformation> pathNodes = new List<NodeInformation>();
        List<Vector2> path = new List<Vector2>();
        NodeInformation nStart = new(start, null, 0f , Heuristic_Manhattan(start, end) ); 
        openList.Add(nStart);
        NodeInformation bestNode = null;
        bool isInLists = false;
        //bool firstLoop = true;
        
        while (openList.Count > 0)
        {
            //if (firstLoop)
            //{
            //    bestNode = openList[0];
            //    firstLoop = false;
            //}

            //foreach (NodeInformation node in openList)
            //{
            //    if (node == null)
            //    {
            //        openList.Remove(node);
            //        continue;
            //    }

            //    if (bestNode.fCost >= node.fCost)
            //    {
            //        bestNode = node;
            //    }
            //}

            //openList.Remove(bestNode);

            //removing from open list WAS HERE

            openList = openList.OrderBy(node => node.fCost).ToList();
            //openList.Reverse();
            //openList.OrderByDescending(node => node.fCost).ToList();
            bestNode = openList[0];

            if (bestNode.node == end)
            {
                bool loop = false;
                while (!loop)
                {
                    path.Add(bestNode.node.transform.position);

                    //Purely for the green path visual.
                    pathNodes.Add(bestNode);


                    if (bestNode.parent != null)
                    {
                        bestNode = bestNode.parent;
                    }

                    if (bestNode.node == start)
                    {
                        break;
                    }

                    //bestNode = bestNode.parent;
                    //if (bestNode.parent == null)
                    //{
                    //    break;
                    //}

                }

                m_Path = path;
                m_Path.Reverse();


                //path.Add((Vector2)bestNode.node.transform.position);
                break;
            }

            if (openList.Count < 1)
            {
                break;
            }


            //For each neighbour adjacent to b
            for (int i = 0; i < bestNode.node.Neighbours.Count(); i++)
            {

                isInLists = false;

                GridNode neighbour = bestNode.node.Neighbours[i];

                if (!neighbour.m_Walkable)
                {
                    continue;
                }

                if (!m_AllowDiagonal)
                {
                    if (i == 1 || i == 3 || i == 5 || i == 7)
                    {
                        continue;
                    }
                }

                if (!m_CutCorners)
                {
                    
                }
                NodeInformation c = new NodeInformation(neighbour, bestNode, Heuristic_Euclidean(bestNode.node, neighbour) + bestNode.gCost, Heuristic_Euclidean(neighbour, end));
                //If open or closed list contain c.

                //Check Open list
                for (int j = 0; j < openList.Count; j++)
                {

                    if (openList[j].node.transform.position == neighbour.transform.position)
                    {
                        isInLists = true;

                        if (c.fCost < openList[j].fCost)
                        {
                            //c.fCost = openList[j].fCost;
                            //c.gCost = openList[j].gCost;
                            //c.hCost = openList[j].hCost;
                            //c.parent = openList[j].parent;

                            openList[j].fCost = c.fCost;
                            openList[j].gCost = c.gCost;
                            openList[j].hCost = c.hCost;
                            openList[j].parent = c.parent;
                            continue;
                        }
                    }
                }

                //Check Closed list.
                for (int j = 0; j < closedList.Count; j++)
                {
                    if (closedList[j].node.transform.position == neighbour.transform.position)
                    {
                        isInLists = true;

                        if (c.fCost < closedList[j].fCost)
                        {
                            //c.fCost = closedList[j].fCost;
                            //c.gCost = closedList[j].gCost;
                            //c.hCost = closedList[j].hCost;
                            //c.parent = closedList[j].parent;

                            closedList[j].fCost = c.fCost;
                            closedList[j].gCost = c.gCost;
                            closedList[j].hCost = c.hCost;
                            closedList[j].parent = c.parent;
                            continue;
                        }
                    }

                }

                if (!isInLists)
                {
                    openList.Add(c);

                }
                //Not in OPEN or CLOSED list.
            }


            openList.RemoveAt(0);
            closedList.Add(bestNode);
        }

        //while (bestNode != null)
        //{
        //    path.Add((Vector2)bestNode.node.transform.position);
        //    bestNode = bestNode.parent;
        //}



            //drawPath
        Grid.ResetGridNodeColours();

		foreach (NodeInformation node in closedList)
		{
			node.node.SetClosedInPathFinding();
		}

		foreach (NodeInformation node in openList)
		{
			node.node.SetOpenInPathFinding();
		}

		foreach (NodeInformation node in pathNodes)
		{
			node.node.SetPathInPathFinding();
		}
        //m_Path = path;
    }
}

