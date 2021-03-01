using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MatrixLibrary;
using IncidenceGraphClass;
using ComponentClass;
using CircuitNetwork;

namespace ConsoleApp1
{

    class Program
    {
        static void Main(string[] args)
        {
            //Matrix mat = new Matrix(5, 6, new double[] { 1.0 / 20, -1 / 20.0, 0, 1, 0, 0, -1.0 / 20, 1.0 / 20 + 1.0 / 8 + 1.0 / 4, -1.0 / 4, 0, 0, 0, 0, -1.0 / 4, 1.0 / 4, 0, 1, 0, 1, 0, 0, 0, 0, 32, 0, 0, 1, 0, 0, 20 });

            Matrix mat = new Matrix(4, 5, new double[] { 0, 0, 1, 1, 0, 0, 1, -3, 0, 0, -1, 1, 1, 0, 0, 1, 0, 0, 0, 3 });
            mat.PrintMatrix();
            mat.SolveLU().PrintMatrix();


            Circuit circuit = new Circuit();
            circuit.AddVs(2);
            circuit.AddRes(10000);
            circuit.AddRes(2000000);
            circuit.AddRes(20000);
            circuit.AddRes(50);
            circuit.AddVCVS(200000);
            // Circuit Connection
            circuit.Connect("Vs1", circuit.GetComp("Vs1")[0], 1);
            circuit.Connect("Vs1", circuit.GetComp("Vs1")[1], 2);
            circuit.Connect("R1", circuit.GetComp("R1")[0], 3);
            circuit.Connect("R1", circuit.GetComp("R1")[1], 2);
            circuit.Connect("R2", circuit.GetComp("R2")[0], 3);
            circuit.Connect("R2", circuit.GetComp("R2")[1], 1);
            circuit.Connect("R3", circuit.GetComp("R3")[0], 5);
            circuit.Connect("R3", circuit.GetComp("R3")[1], 3);
            circuit.Connect("R4", circuit.GetComp("R4")[0], 4);
            circuit.Connect("R4", circuit.GetComp("R4")[1], 5);
            circuit.Connect("VCVS1", circuit.GetComp("VCVS1")[0], 1);
            circuit.Connect("VCVS1", circuit.GetComp("VCVS1")[1], 4);
            // Asign dependent source to its controlling Element
            circuit.DepConnect("VCVS1", "R2");
            //Additional Current to be found
            circuit.GetUnknownCurr("R3");
            circuit.PrintNetwork();
            circuit.ClearEmptyNodes();
            circuit.PrintNetwork();
            circuit.Generate_Ab().PrintMatrix();
            circuit.Generate_Ab().SolveLU().PrintMatrix();

            Console.WriteLine(circuit);
            //circuit.AddVs(3);
            //circuit.AddRes(1);
            //circuit.AddRes(1);
            //circuit.AddCCCS(2);
            //circuit.Connect("Vs1", circuit.GetComp("Vs1")[0], 1);
            //circuit.Connect("Vs1", circuit.GetComp("Vs1")[1], 2);
            //circuit.Connect("R1", circuit.GetComp("R1")[0], 3);
            //circuit.Connect("R1", circuit.GetComp("R1")[1], 2);
            //circuit.Connect("R2", circuit.GetComp("R2")[0], 1);
            //circuit.Connect("R2", circuit.GetComp("R2")[1], 3);
            //circuit.Connect("CCCS1", circuit.GetComp("CCCS1")[0], 3);
            //circuit.Connect("CCCS1", circuit.GetComp("CCCS1")[1], 1);
            //circuit.DepConnect("CCCS1", "R1");


            //circuit.AddVs(32);
            //circuit.AddRes(20);
            //circuit.AddRes(4);
            //circuit.AddRes(8);
            //circuit.AddVs(20);
            //circuit.Connect("Vs1", circuit.GetComp("Vs1")[0], 1);
            //circuit.Connect("Vs1", circuit.GetComp("Vs1")[1], 2);
            //circuit.Connect("R1", circuit.GetComp("R1")[0], 2);
            //circuit.Connect("R1", circuit.GetComp("R1")[1], 3);
            //circuit.Connect("R2", circuit.GetComp("R2")[0], 3);
            //circuit.Connect("R2", circuit.GetComp("R2")[1], 4);
            //circuit.Connect("R3", circuit.GetComp("R3")[0], 3);
            //circuit.Connect("R3", circuit.GetComp("R3")[1], 1);
            //circuit.Connect("Vs2", circuit.GetComp("Vs2")[0], 1);
            //circuit.Connect("Vs2", circuit.GetComp("Vs2")[1], 4);
            //obj.Add(3);
            //Matrix x = A.SolveLU();
            //x.PrintMatrix();
            //Graph graph = new Graph();
            //graph.AddEdge(1, 2);
            //graph.AddEdge(2, 3);
            //graph.AddEdge(1, 3);
            ////graph.Connect(1, 4, 3);
            ////graph.Connect(3, 2, 1);
            ////graph.Connect(2, 3, 1);
            //graph.IMatrix.PrintMatrix();
            //graph.Connect(1, 4, 5);

            //graph.IMatrix.PrintMatrix();
            //graph.DisconnectEdgeEnd(1, 4);
            //graph.IMatrix.PrintMatrix();
            //graph.RemoveEdge(3);
            //graph.IMatrix.PrintMatrix();
            //Matrix A = Matrix.RandomMatrix(10, 10);
            //Matrix b = Matrix.RandomMatrix(10, 1);

            //Matrix newMatrix = A.AugmentMatrix(b);

            //Console.WriteLine(newMatrix.Rank);
            //newMatrix.SolveLU().PrintMatrix();

            //Matrix Ainverse = A.Inverse();
            //Matrix sol = Ainverse.Multiply(b);
            //sol.PrintMatrix();x                           
            //newMatrix.PrintMatrix();

        }
    }
}
      
   
