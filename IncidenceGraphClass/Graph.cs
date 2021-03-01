using System;
using MatrixLibrary;

namespace IncidenceGraphClass
{    
    public class Graph
    {
        
        public Matrix IMatrix;

        /// <summary>
        /// Number of nodes
        /// </summary>
        public int nodeNum = 0;
        /// <summary>
        /// Number of edges
        /// </summary>
        public int edgeNum = 0;

        /// <summary>
        /// Constructs graph with matrix dimensions
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="edges"></param>
        public Graph(int nodes, int edges)
        {
            this.nodeNum = nodes;
            this.edgeNum = edges;
            this.IMatrix = new Matrix(this.nodeNum, this.edgeNum);
        }

        public Graph()
        {
            this.IMatrix = new Matrix(this.nodeNum, this.edgeNum);
        }

        /// <summary>
        /// Adds an edge to graph
        /// </summary>
        public void AddEdge()
        {
            this.edgeNum++;
            this.nodeNum += 2;
            IMatrix = IMatrix.UpdateMatrix(nodeNum, edgeNum);
            this.Connect(edgeNum, this.nodeNum - 1, -1);
            this.Connect(edgeNum, this.nodeNum, 1);
        }

        ///// <summary>
        ///// Adds an edge to graph
        ///// </summary>
        //public void AddEdge(int node, char end)
        //{
        //    this.AddEdge(); 
        //    if (end != 'T' || end != 'H')
        //    {
        //        end = 'T';                
        //    }                     
        //    this.Connect(edgeNum, node, end);
           
        //}


        ///// <summary>
        ///// Add new edge to graph connecting its head and tail to specified nodes
        ///// </summary>
        ///// <param name="tail"></param>
        ///// <param name="head"></param>
        //public void AddEdge(int tail, int head)
        //{
        //    edgeNum++;
        //    IMatrix = IMatrix.UpdateMatrix(nodeNum, edgeNum);
        //    this.Connect(this.edgeNum, tail, head);
        //}
                

        public void Connect(int edge, int node, int end)
        {
            // Check if edge is connected to any node and disconnect
            for (int row = 1; row <= this.nodeNum; row++)
            {
                if (row == node)
                {
                    continue;
                }
                if (this.IsConnected(edge, row) == end)
                {
                    this.DisconnectEdgeEnd(edge, row);
                }
            }
            IMatrix[node, edge] = end;
        }

        /// <summary>
        /// To Nodes
        /// </summary>
        /// <param name="edge"></param>
        /// <param name="head"></param>
        /// <param name="tail"></param>
        //public void Connect(int edge, int tail, int head)
        //{
        //    // Check if edge is connected to any node and disconnect
        //    for (int row = 1; row <= this.nodeNum; row++)
        //    {
        //        if (row == tail || row == head)
        //        {
        //            continue;
        //        }
        //        if (this.IsConnected(edge, row) != 0)
        //        {
        //            this.DisconnectEdgeEnd(edge, row);
        //        }
        //    }

        //    //Check if new Node is to be created
        //    bool newHead = false;
        //    bool newTail = false;
        //    if (tail > IMatrix.NumRows)
        //    {
        //        nodeNum++;
        //        IMatrix = IMatrix.UpdateMatrix(nodeNum, edgeNum);
        //        newTail = true;
        //    }
        //    if (head > IMatrix.NumRows)
        //    {
        //        nodeNum++;
        //        IMatrix = IMatrix.UpdateMatrix(nodeNum, edgeNum);
        //        newHead = true;
        //    }
        //    if (newHead || newTail)
        //    {
        //        if (newHead && newTail)
        //        {
        //            if (head < tail)
        //            {
        //                IMatrix[nodeNum - 1, edge] = -1;
        //                IMatrix[nodeNum, edge] = 1;
        //            }
        //            else
        //            {
        //                IMatrix[nodeNum, edge] = -1;
        //                IMatrix[nodeNum - 1, edge] = 1;
        //            }
        //        }
        //        else if (newHead)
        //        {
        //            if (tail == 0)
        //            {
        //                IMatrix[nodeNum, edge] = -1;
        //            }
        //            else
        //            {
        //                IMatrix[nodeNum, edge] = -1;
        //                IMatrix[tail, edge] = 1;
        //            }
        //        }
        //        else if (newTail)
        //        {
        //            if (head == 0)
        //            {
        //                IMatrix[nodeNum, edge] = 1;
        //            }

        //            else
        //            {
        //                IMatrix[head, edge] = -1;
        //                IMatrix[nodeNum, edge] = 1;
        //            }
        //        }

        //    }
        //    //If no node is to be created
        //    else
        //    {
        //        if (head == 0)
        //        {
        //            IMatrix[tail, edge] = 1;
        //        }
        //        else if (tail == 0)
        //        {
        //            IMatrix[head, edge] = -1;
        //        }
        //        else
        //        {
        //            IMatrix[head, edge] = -1;
        //            IMatrix[tail, edge] = 1;
        //        }
        //    }
        //}

        /// <summary>
        /// Forms node
        /// </summary>
        public void FormNode()
        {
            this.nodeNum++;
            IMatrix = IMatrix.UpdateMatrix(nodeNum, edgeNum);
        }
    
        /// <summary>
        /// Checks if Node has atleast one edge
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public bool HasEdge(int node)
        {
            bool hasEdge = false;
            for (int edge = 1; edge <= IMatrix.NumColumns; edge++)
            {
                if(IMatrix[node, edge] != 0)
                {
                    hasEdge = true;
                    break;
                }
            }
            return hasEdge;
        }

        /// <summary>
        /// Deletes nodes without edges
        /// </summary>
        public void NoEdges()
        {
            for (int row = 1; row <= nodeNum; row++)
            {
                if (!this.HasEdge(row))
                {
                    nodeNum--;
                    IMatrix.DeleteRow(row);
                }
            }
            for (int row = nodeNum; row > 0; row--)
            {
                if (!this.HasEdge(row))
                {
                    nodeNum--;
                    IMatrix.DeleteRow(row);
                }
            }
        }

        /// <summary>
        /// Removes edge
        /// </summary>
        /// <param name="edge"></param>
        public void RemoveEdge(int edge)
        {
            edgeNum--;
            IMatrix.DeleteCol(edge);
            this.NoEdges();
        }
                

        /// <summary>
        /// Check if edge is connected to a node
        /// </summary>
        /// <param name="edge"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public int IsConnected(int edge, int end)
        {
            int isConnected = 0;
            if (end > this.nodeNum)
            {
                return isConnected;
            }
            if (IMatrix[end, edge] > 0)
            {
                isConnected = 1;
                
            }
            else if (IMatrix[end, edge] < 0)
            {
                isConnected = -1;
            }      
            return isConnected;
        }
        
        /// <summary>
        /// Returns the node a particular edge is connected to
        /// </summary>
        /// <param name="end"></param>
        /// <returns></returns>
        public int NodeConnected(int edge, int end)
        {
            int node;
            for (node = 0; node<=this.nodeNum; node++ )
            {
                if(IsConnected(edge, node) == end)
                {
                    return node;
                }
                
            }
            return 0;
        }

        /// <summary>
        /// Disconnencts a particular end of an edge from the node
        /// </summary>
        /// <param name="edge"></param>
        /// <param name="end"></param>
        public void DisconnectEdgeEnd(int edge, int end)
        {            
            this.IMatrix[end, edge] = 0;
            //this.NoEdges();
        }
    }
}
