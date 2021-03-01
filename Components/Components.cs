using System;
using System.Collections.Generic;
using MatrixLibrary;

namespace ComponentClass
{
    public abstract class Components
    {
        public class Terminals
        {
            public enum Polarity { negative = -1, noPol = 0, positive = 1 }
            private int node;
            internal Polarity polarity;

            public int Node
            {
                get
                {
                    return this.node;
                }
                set
                {
                    this.node = value;
                }
            }
            public Polarity Pole
            {
                get { return this.polarity; }
            }
            internal Terminals()
            {
            }
            internal Terminals(int node, Polarity pol)
            {
                this.node = node;
                this.polarity = pol;
            }
        }
                
        internal List<Terminals> terminals = new List<Terminals>();
        public Terminals this[int index]
        {
            get
            {
                return terminals[index];
            }
            set
            {
                terminals[index] = value;
            }
        }

        public int uCur;
        public string cRef;

        public Components depComp;
        public abstract void ElementStamp(Matrix matrix, int depCur = 0);
        public abstract void ElementStamp(Matrix matrix, int cur, int depCur = 0);       
        public abstract override string ToString();
    }

    /****************** PASSIVE COMPONENTS *******************/
    public class Resistor : Components
    {
        private Terminals p_term = new Terminals(0, Terminals.Polarity.positive);
        private Terminals n_term = new Terminals(0, Terminals.Polarity.negative);

        public static List<Resistor> resistors = new List<Resistor>();

        private double resistance;

        public double Resistance
        {
            get
            {
                return this.resistance;
            }
            set
            {
                this.resistance = value;
            }
        }

        public Resistor(double resistance = 1)
        {
            this.resistance = resistance;
            resistors.Add(this);
            this.cRef = $"R{resistors.IndexOf(this) + 1}";
            this.terminals.Add(this.n_term);
            this.terminals.Add(this.p_term);
        }

        public Resistor(int p_node, int n_node, double resistance = 1) : this(resistance)
        {
            this.p_term.Node = p_node;
            this.n_term.Node = n_node;
        }

        public override void ElementStamp(Matrix matrix, int depCur = 0)
        {

            matrix[this.p_term.Node, this.p_term.Node] += (1 / this.resistance);
            matrix[this.n_term.Node, this.n_term.Node] += (1 / this.resistance);
            matrix[this.n_term.Node, this.p_term.Node] += (-1 / this.resistance);
            matrix[this.p_term.Node, this.n_term.Node] += (-1 / this.resistance);

        }

        public override void ElementStamp(Matrix matrix, int cur, int depCur = 0)
        {
            matrix[this.p_term.Node, cur] += 1;
            matrix[this.n_term.Node, cur] += -1;
            matrix[cur, cur] += this.resistance;
            matrix[cur, this.p_term.Node] += -1;
            matrix[cur, this.n_term.Node] += 1;
        }

        public override string ToString()
        {
            return $"{this.cRef} N{this.p_term.Node} N{this.n_term.Node} {resistance}";
        }
    }

    /****************** ACTIVE COMPONENTS *******************/
    public class VSource : Components
    {
        private double volt;
        private Terminals p_term = new Terminals(0, Terminals.Polarity.positive);
        private Terminals n_term = new Terminals(0, Terminals.Polarity.negative);

        public static List<VSource> VSources = new List<VSource>();

        public VSource(double volt = 12)
        {
            this.volt = volt;
            VSources.Add(this);
            this.cRef = $"Vs{ VSources.IndexOf(this) + 1}";
            this.terminals.Add(this.n_term);
            this.terminals.Add(this.p_term);
        }
        public VSource(int p_node, int n_node, double volt = 12) : this(volt)
        {
            this.p_term.Node = p_node;
            this.n_term.Node = n_node;
        }

        public double Voltage
        {
            get
            {
                return this.volt;
            }
            set
            {
                this.volt = value;
            }
        }

        public override void ElementStamp(Matrix matrix, int depCur = 0)
        {            
        }

        public override void ElementStamp(Matrix matrix, int cur, int depCur = 0)
        {

            matrix[this.p_term.Node, cur] += 1;
            matrix[cur, this.p_term.Node] += 1;
            matrix[cur, this.n_term.Node] += -1;
            matrix[this.n_term.Node, cur] += -1;

            matrix[cur, matrix.NumColumns] += volt;
        }
        public override string ToString()
        {
            return $"{this.cRef} N{this.p_term.Node} N{this.n_term.Node} {volt}";
        }
    }

    public class ISource : Components
    {
        private double Is;
        private Terminals p_term = new Terminals(0, Terminals.Polarity.positive);
        private Terminals n_term = new Terminals(0, Terminals.Polarity.negative);

        public static List<ISource> ISources = new List<ISource>();

        public double Current
        {
            get
            {
                return this.Is;
            }
            set
            {
                this.Is = value;
            }
        }
        public ISource(double Is = 1)
        {
            this.Is = Is;
            ISources.Add(this);
            this.cRef = $"Is{ ISources.IndexOf(this) + 1}";
            this.terminals.Add(this.n_term);
            this.terminals.Add(this.p_term);
        }
        public ISource(int p_node, int n_node, double Is = 1): this(Is)
        {
            this.p_term.Node = p_node;
            this.n_term.Node = n_node;
        }

        public override void ElementStamp(Matrix matrix, int depCur = 0)
        {

            matrix[this.p_term.Node, matrix.NumColumns] += -this.Is;
            matrix[this.n_term.Node, matrix.NumColumns] += this.Is;

        }

        public override void ElementStamp(Matrix matrix, int cur, int depCur = 0)
        {
            matrix[this.p_term.Node, cur] += 1;
            matrix[this.n_term.Node, cur] += -1;
            matrix[cur, cur] += 1;
            matrix[cur, matrix.NumColumns] = this.Current;
        }

        public override string ToString()
        {
            return $" {this.cRef} N{this.p_term.Node} N{this.n_term.Node} {Is}";
        }
    }

    /*********************** Dependent Sources ******************/
    public class VCVS : Components
    {
        private double gain;
        private Terminals p_term = new Terminals(0, Terminals.Polarity.positive);
        private Terminals n_term = new Terminals(0, Terminals.Polarity.negative);


        public Terminals depP_term = new Terminals();
        public Terminals depN_term = new Terminals();
        public static List<VCVS> VCVSources = new List<VCVS>();

        public VCVS(double gain = 1)
        {
            this.gain = gain;
            VCVSources.Add(this);
            this.cRef = $"VCVS{VCVSources.IndexOf(this) + 1}";
            this.terminals.Add(this.n_term);
            this.terminals.Add(this.p_term);
            this.terminals.Add(this.depN_term);
            this.terminals.Add(this.depP_term);
        }

        public double Gain
        {
            get
            {
                return this.gain;
            }
            set
            {
                this.gain = value;
            }
        }

        public override void ElementStamp(Matrix matrix, int depCur = 0)
        {
        }

        public override void ElementStamp(Matrix matrix, int cur, int depCur = 0)
        {
            matrix[this.p_term.Node, cur] += 1;
            matrix[cur, this.p_term.Node] += 1;
            matrix[cur, this.n_term.Node] += -1;
            matrix[this.n_term.Node, cur] += -1;
            matrix[cur, this.depP_term.Node] += -Gain;
            matrix[cur, this.depN_term.Node] += Gain;
        }

        public override string ToString()
        {
            return $"{this.cRef} N{this.p_term.Node} N{this.n_term.Node} {gain}Vx";
        }
    }

    public class CCVS : Components
    {
        private double gain;
        private Terminals p_term = new Terminals(0, Terminals.Polarity.positive);
        private Terminals n_term = new Terminals(0, Terminals.Polarity.negative);

        public static List<CCVS> CCVSources = new List<CCVS>();

        public CCVS(double gain = 1)
        {
            this.gain = gain;
            CCVSources.Add(this);
            this.cRef = $"VCVS{CCVSources.IndexOf(this) + 1}";
            this.terminals.Add(this.n_term);
            this.terminals.Add(this.p_term);
        }

        public double Gain
        {
            get
            {
                return this.gain;
            }
            set
            {
                this.gain = value;
            }
        }

        public override void ElementStamp(Matrix matrix, int depCur = 0)
        {
        }

        public override void ElementStamp(Matrix matrix, int cur, int depCur)
        {

            matrix[this.p_term.Node, cur] += 1;
            matrix[cur, this.p_term.Node] += 1;
            matrix[cur, this.n_term.Node] += -1;
            matrix[this.n_term.Node, cur] += -1;
            matrix[cur, depCur] += -Gain;
        }

        public override string ToString()
        {
            return $"{this.cRef} N{this.p_term.Node} N{this.n_term.Node} {depComp.cRef} {gain}Ix";
        }
    }

    public class VCCS: Components
    {
        private double gain;
        private Terminals p_term = new Terminals(0, Terminals.Polarity.positive);
        private Terminals n_term = new Terminals(0, Terminals.Polarity.negative);

        public Terminals depP_term = new Terminals();
        public Terminals depN_term = new Terminals();
        public static List<VCCS> VCCSources = new List<VCCS>();

        public VCCS(double gain = 1)
        {
            this.gain = gain;
            VCCSources.Add(this);
            this.cRef = $"VCCS{VCCSources.IndexOf(this) + 1}";
            this.terminals.Add(this.n_term);
            this.terminals.Add(this.p_term);
            this.terminals.Add(this.depN_term);
            this.terminals.Add(this.depP_term);
        }

        public double Gain
        {
            get
            {
                return this.gain;
            }
            set
            {
                this.gain = value;
            }
        }

        public override void ElementStamp(Matrix matrix, int depCur = 0)
        {
            matrix[this.p_term.Node, this.depP_term.Node] += Gain;
            matrix[this.n_term.Node, this.depN_term.Node] += Gain;
            matrix[this.n_term.Node, this.depP_term.Node] += -Gain;
            matrix[this.p_term.Node, this.depN_term.Node] += -Gain;
        }

        public override void ElementStamp(Matrix matrix, int cur, int depCur = 0)
        {
            matrix[this.p_term.Node, cur] += 1;
            matrix[this.n_term.Node, cur] += -1;
            matrix[cur, this.depP_term.Node] += -Gain;
            matrix[cur, this.depN_term.Node] += Gain;
            matrix[cur, cur] += 1;

        }

        public override string ToString()
        {
            return $"{this.cRef} N{this.p_term.Node} N{this.n_term.Node} {gain}Vx";
        }
    }

    public class CCCS: Components
    {
        private double gain;
        private Terminals p_term = new Terminals(0, Terminals.Polarity.positive);
        private Terminals n_term = new Terminals(0, Terminals.Polarity.negative);

        public static List<CCCS> CCCSources = new List<CCCS>();
    
        public CCCS(double gain = 1)
        {
            this.gain = gain;
            CCCSources.Add(this);
            this.cRef = $"CCCS{CCCSources.IndexOf(this) + 1}";
            this.terminals.Add(this.n_term);
            this.terminals.Add(this.p_term);
        }

        public double Gain
        {
            get
            {
                return this.gain;
            }
            set
            {
                this.gain = value;
            }
        }

        public override void ElementStamp(Matrix matrix, int depCur)
        {
            matrix[this.p_term.Node, depCur] += Gain;
            matrix[this.n_term.Node, depCur] += -Gain;
        }

        public override void ElementStamp(Matrix matrix, int cur, int depCur)
        {
            matrix[this.p_term.Node, cur] += 1;
            matrix[this.n_term.Node, cur] += -1;
            matrix[cur, depCur] += -Gain;
            matrix[cur, cur] += 1;
        }

        public override string ToString()
        {
            return $"{this.cRef} N{this.p_term.Node} N{this.n_term.Node} {depComp.cRef} {gain}Ix";
        }
    }

    public class Ground: Components
    {
        public Terminals term = new Terminals(0, Terminals.Polarity.noPol);
        public static List<Ground> grounds = new List<Ground>();

        public Ground()
        {
            this.cRef = $"G{ grounds.IndexOf(this) + 1}";
            grounds.Add(this);
        }
        public override void ElementStamp(Matrix matrix, int depCur = 0)
        {
        }

        public override void ElementStamp(Matrix matrix, int cur, int depCur = 0)
        { }
        public override string ToString()
        {
            return $"{this.cRef} N{this.term.Node}";
        }
    }
}