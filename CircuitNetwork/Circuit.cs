using System;
using System.Collections.Generic;
using IncidenceGraphClass;
using MatrixLibrary;
using ComponentClass;

namespace CircuitNetwork
{
    public class Circuit
    {
        private Graph network;
        private List<Components> components = new List<Components>();
        private List<Components> findCurr = new List<Components>();
        private List<Components> depComps = new List<Components>();

        public Circuit()
        {
            network = new Graph();
            this.AddGround();
        }

        public void PrintNetwork()        
         =>   this.network.IMatrix.PrintMatrix();
        
        private void Add2TComp(Components comp)
        {
            network.AddEdge();
            comp[0].Node = network.nodeNum - 1;
            comp[1].Node = network.nodeNum;
            components.Add(comp);
        }
        public void AddRes()
        {
            Resistor res = new Resistor();
            Add2TComp(res);
        }
        public void AddVs()
        {
            VSource vSource = new VSource();
            Add2TComp(vSource);
        }
        public void AddIs()
        {
            ISource iSource = new ISource();
            Add2TComp(iSource);
        }

        public void AddRes(double resistance)
        {
            Resistor res = new Resistor(resistance);
            Add2TComp(res);
        }
        public void AddVs(double volt)
        {
            VSource vSource = new VSource(volt);
            Add2TComp(vSource);
        }
        public void AddIs(double cur)
        {
            ISource iSource = new ISource(cur);
            Add2TComp(iSource);
        }
        public void AddVCVS(double gain)
        {
            VCVS vCVS = new VCVS(gain);
            Add2TComp(vCVS);
        }
        public void AddVCCS(double gain)
        {
            VCCS vCCS = new VCCS(gain);
            Add2TComp(vCCS);
        }

        public void AddCCVS(double gain) 
        {
            CCVS cCVS = new CCVS(gain);
            Add2TComp(cCVS);
        }
        public void AddCCCS(double gain)
        {
            CCCS cCCS = new CCCS(gain);
            Add2TComp(cCCS);
        }

        private void AddGround()
        {
            Ground ground = new Ground();
            network.FormNode();
            ground.term.Node = network.nodeNum;
            components.Add(ground);
        }
                
        public void AddG()
        {
            Ground ground = new Ground();
            ground.term.Node = 1;
        }

        public Components GetComp(string compRef)
        {
            foreach(Components component in this.components)
            {
                if(component.cRef == compRef)
                {
                    return component;
                }
            }
            return null;
        }

        public void Connect(string compRef, Components.Terminals term, int node)
        {
            Components comp = GetComp(compRef);
            network.Connect(components.IndexOf(comp), node, (int)term.Pole);
            if (term.Pole == Components.Terminals.Polarity.negative)
            {
                comp[0].Node = node;
            }
            else if (term.Pole == Components.Terminals.Polarity.positive)
            {
                comp[1].Node = node;
            }

        }

        public void DepConnect(string compRef, string depCompRef)
        {
            Components depComp = GetComp(depCompRef);
            Components comp = GetComp(compRef);
            Type type = comp.GetType();
            if (type == typeof(VCCS) || type == typeof(VCVS))
            {
                comp[2].Node = depComp[0].Node;
                comp[3].Node = depComp[1].Node;
            }
            else
            {
                comp.depComp = depComp;
                Type dType = comp.depComp.GetType();
                if (dType != typeof(VSource) || dType != typeof(VCCS) || dType != typeof(VCVS))
                {
                    depComps.Add(depComp);
                }
            }
        }

        public void DeleteComp(string compRef)
        {
            Components comp = GetComp(compRef);
            network.RemoveEdge(components.IndexOf(comp));
            components.Remove(comp);
            Type type = comp.GetType();
            if (type == typeof(Resistor))
            {
                Resistor.resistors.Remove((Resistor)comp);
            }
            else if (type == typeof(VSource))
            {
                VSource.VSources.Remove((VSource)comp);
            }
            else if (type == typeof(ISource))
            {
                ISource.ISources.Remove((ISource)comp);
            }
            else if (type == typeof(VCVS))
            {
                VCVS.VCVSources.Remove((VCVS)comp);
            }
            else if (type == typeof(CCVS))
            {
                CCVS.CCVSources.Remove((CCVS)comp);
            }
            else if (type == typeof(CCCS))
            {
                CCCS.CCCSources.Remove((CCCS)comp);
            }

            else if (type == typeof(VCCS))
            {
                VCCS.VCCSources.Remove((VCCS)comp);
            }
            else if (type == typeof(Ground))
            {
                Ground.grounds.Remove((Ground)comp);
            }            
        }

        public void Disconnect(string compRef, Components.Terminals term)
        {
            Components comp = GetComp(compRef);
            network.DisconnectEdgeEnd(components.IndexOf(comp), term.Node);
            network.FormNode();
            term.Node = network.nodeNum;
        }

        public Matrix Generate_Ab()
        {
            int dim = this.network.nodeNum + VSource.VSources.Count
                + VCVS.VCVSources.Count + CCVS.CCVSources.Count + depComps.Count + findCurr.Count;
            Matrix matrix = new Matrix(dim, dim + 1);

            foreach (VSource comp in VSource.VSources)
            {
                comp.uCur = network.nodeNum + VSource.VSources.IndexOf(comp) + 1;
            }
            foreach (VCVS comp in VCVS.VCVSources)
            {
                comp.uCur = network.nodeNum + VSource.VSources.Count +  VCVS.VCVSources.IndexOf(comp) + 1;
            }

            foreach (CCVS comp in CCVS.CCVSources)
            {
                comp.uCur = network.nodeNum + VSource.VSources.Count + CCVS.CCVSources.IndexOf(comp) + 1;
            }
            foreach (Components comp in depComps)
            {
                comp.uCur = network.nodeNum + VSource.VSources.Count + VCVS.VCVSources.Count 
                    + CCVS.CCVSources.Count + depComps.IndexOf(comp) + 1;
            }

            foreach (Components comp in findCurr)
            {
                comp.uCur = network.nodeNum + VSource.VSources.Count + VCVS.VCVSources.Count
                    + CCVS.CCVSources.Count + depComps.Count + findCurr.IndexOf(comp) + 1;
            }
            foreach (Components comp in components)
            {
                if (comp.GetType() == typeof(Ground))
                {
                    continue;
                }
                if (findCurr.Contains(comp))
                {
                    continue;
                }

                if (comp.GetType() == typeof(VSource) || comp.GetType() == typeof(VCVS))
                {
                    comp.ElementStamp(matrix, comp.uCur, 0);
                }
                else if (comp.GetType() == typeof(CCVS))
                {
                    comp.ElementStamp(matrix, comp.uCur, comp.depComp.uCur);
                }
                else if (comp.GetType() == typeof(CCCS))
                {

                    if (!depComps.Contains(comp))
                    {
                        comp.ElementStamp(matrix, comp.depComp.uCur);
                    }
                    else
                    {
                        comp.ElementStamp(matrix, comp.uCur, comp.depComp.uCur);
                    }
                }
                else
                {
                    if (depComps.Contains(comp))
                    {
                        comp.ElementStamp(matrix, comp.uCur, 0);
                    }
                    else
                    {
                        comp.ElementStamp(matrix);
                    }
                }
            }
            foreach(Components comp in findCurr)
            {
                Type dType = comp.GetType();
                if (dType != typeof(CCCS) || dType != typeof(CCVS))
                {
                    comp.ElementStamp(matrix, comp.uCur, 0);
                }
                else
                {
                    comp.ElementStamp(matrix, comp.uCur, comp.depComp.uCur);
                }
            }

            matrix.DeleteCol(1);
            matrix.DeleteRow(1);

            return matrix;
        }

        public void GetUnknownCurr(params string[] comps)
        {
            foreach (string compRef in comps)
            {
                Components comp = GetComp(compRef);
                Type dType = comp.GetType();
                if (dType != typeof(VSource) || dType != typeof(VCCS) || dType != typeof(VCVS) || !depComps.Contains(comp))
                {
                    findCurr.Add(comp);
                }
            }
        }
        public void ClearEmptyNodes()
        {
            this.network.NoEdges();
        }

        public string GetNetlist()
        {
            string netlist = "";
            foreach(Components comp in components)
            {
                Console.WriteLine(comp);
                netlist += comp + "\n";
            }
            return netlist;
        }
        public override string ToString()
        {
            return GetNetlist();
        }
    }
}
