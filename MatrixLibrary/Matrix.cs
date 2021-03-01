using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; 

namespace MatrixLibrary
{
    public class Matrix
    {
        /// <summary>
        /// matrix to be operated on
        /// user to enter its elements.
        /// </summary>
        private double[,] matrix;

        /// <summary>
        /// Rank of Matrix
        /// </summary>
        private int rank;
        private bool rankFound = false;
        private bool GaussianPerformed = false;


        ///<summary>
        /// Inverse of matrix
        /// </summary>
        private Matrix inverse;

        /// <summary>
        /// If inverse has been found
        /// </summary>
        private bool inverseFound = false;

        /// <summary>
        /// property to get Number of rows in matrix
        /// </summary>
        public int NumRows
        {
            get
            {
                return matrix.GetLength(0);
            }
        }

        /// <summary>
        /// Get number of columns
        /// </summary>
        public int NumColumns
        {
            get
            {
                return matrix.GetLength(1);
            }
        }

        /// <summary>
        /// Check if matrix is square
        /// </summary>
        public bool IsSquare
        {
            get
            {
                return this.NumColumns == this.NumRows;
            }
        }

        /// <summary>
        /// Checks if matrix is a column vector
        /// </summary>
        private bool IsColVector
        {
            get
            {
                return this.NumColumns == 1;
            }
        }

        /// <summary>
        /// Checks if matrix is a column vector
        /// </summary>
        private bool IsRowVector
        {
            get
            {
                return this.NumRows == 1;
            }
        }

        /// <summary>
        /// Constructs a matrix of dimension row x col without entries
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="columns"></param>
        public Matrix(int rows, int columns)
        {
            this.matrix = new double[rows, columns];
        }

        /// <summary>
        /// Constructs a square matrix
        /// </summary>
        /// <param name="dimension"> number of rows and columns</param>
        public Matrix(int dimension) : this(dimension, dimension)
        {
        }

        /// <summary>
        /// Contructs Matrix from Array
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="columns"></param>
        /// <param name="array"></param>
        public Matrix(int rows, int columns, double[] array)
        {
            this.matrix = ToMatrix(rows, columns, array);
        }

        /// <summary>
        /// Constructs Square Matrix from Array
        /// </summary>
        /// <param name="dimension"></param>
        /// <param name="array"></param>
        public Matrix(int dimension, double[] array): this (dimension, dimension, array)
        {
        }

        /// <summary>
        /// Contructs matrix from string
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="columns"></param>
        /// <param name="matrix"></param>
        public Matrix(int rows, int columns, string matrix)
        {
            double[][] array = ToArray(matrix);
            this.matrix = MatrixElements(rows, columns, array);
        }

        /// <summary>
        /// Indexer for accesing matrix elements
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public double this[int row, int col]
        {
            get
            {
                return this.matrix[row - 1, col - 1];
            }
            set
            {
                this.matrix[row - 1, col - 1] = value;
            }
        }

        /// <summary>
        /// Converts Jagged array to matrix
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="cols"></param>
        /// <returns></returns>
        private double[,] MatrixElements(int rows, int cols, double[][] elements)
        {
            double[,] newMatrix = new double[rows, cols];
            for (int row = 1; row <= rows; row++)
            {
                for (int col = 1; col <= cols; col++)
                {
                    newMatrix[row - 1, col - 1] = elements[row - 1][col - 1];
                }
            }
            return newMatrix;
        }

        /// <summary>
        /// Converts Array to Matrix
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="columns"></param>
        /// <param name="array"></param>
        /// <returns></returns>
        private double[,] ToMatrix(int rows, int columns, double[] array)
        {
            if ((rows * columns) != array.Length)
            {
                throw new IndexOutOfRangeException("Matrix dimension and array length do not conform!");
            }

            double[,] newMatrix = new double[rows, columns];
            int item = 0;
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < columns; col++)
                {
                    newMatrix[row, col] = array[item];
                    item++;
                }
            }
            return newMatrix;
        }

        // <summary>
        /// Converts String to a jagged array of numbers
        /// </summary>
        /// <param name="sMatrix"></param>
        /// <returns></returns>
        private double[][] ToArray(string sMatrix)
        {
            string[] rows = sMatrix.Split(new[] { Environment.NewLine, "\n", "\r", ":", ";" }, StringSplitOptions.RemoveEmptyEntries);
            double[][] matrix = new double[rows.Length][];
            int i = 0;
            foreach (string row in rows)
            {
                string[] cols = row.Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
                matrix[i] = new double[cols.Length];
                int j = 0;
                foreach (string col in cols)
                {
                    matrix[i][j] = double.TryParse(col, out double num) ? num : 0;
                    j++;
                }
                i++;
            }

            return matrix;
        }

        /// <summary>
        /// Prints a matrix
        /// </summary>
        public void PrintMatrix()
        {
            for (int row = 1; row <= this.NumRows; row++)
            {
                for (int col = 1; col <= this.NumColumns; col++)
                {
                    Console.Write("{0,-4}", this[row, col]);
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Returns the rank of a matrix
        /// </summary>
        public int Rank
        {
            get
            {
                if (rankFound)
                {
                    return this.rank;
                }
                this.Gaussian();
                this.rankFound = true;
                return this.rank;
            }
        }

        /// <summary>
        /// Finds Determinant of Matrix
        /// </summary>
        /// <returns></returns>
        public double Determinant
        {
            get
            {
                if (!this.IsSquare)
                {
                    throw new Exception("Matrix Must be square!");
                }
                double determinant = 1;
                Matrix U = this.Gaussian();
                for (int row = 1; row <= this.NumRows; row++)
                {
                    determinant *= U[row, row];
                }
                return determinant;
            }
        }

        /// <summary>
        /// Gets a particular column from a matrix
        /// </summary>
        /// <param name="col">Column selected</param>
        /// <returns>Selected Column</returns>
        public Matrix GetColumn(int col)
        {
            if (col > this.NumColumns)
            {
                throw new ArgumentException("Invalid Column Selected");
            }
            Matrix column = new Matrix(this.NumRows, 1);
            for (int i = 1; i <= this.NumRows; i++)
            {
                column[i, 1] = this[i, col];
            }
            return column;
        }

        /// <summary>
        /// Gets a particular row from a matrix
        /// </summary>
        /// <param name="row"></param>
        /// <returns>Selected Row</returns>
        public Matrix GetRow(int row)
        {
            if (row > this.NumRows)
            {
                throw new ArgumentException("Invalid Row Selected");
            }
            Matrix mRow = new Matrix(1, this.NumColumns);
            for (int i = 1; i <= this.NumColumns; i++)
            {
                mRow[1, i] = this[row, i];
            }
            return mRow;
        }


        /// <summary>
        /// returns the inverse of the matrix
        /// </summary>
        public Matrix Inverse()
        {
            if (this.IsSquare)
            {
                if (inverseFound)
                {
                    return this.inverse;
                }
                this.inverse = new Matrix(NumRows);
                Matrix identity = Matrix.Identity(NumRows);
                Matrix aug = this.AugmentMatrix(identity); ;
                aug  = aug.GaussJordan();
                for (int row = 1; row <= this.NumRows; row++)
                {
                    for (int col = 1; col <= this.NumColumns; col++)
                    {
                        this.inverse[row, col] = aug[row, NumColumns + col];
                    }
                }
                this.inverseFound = true;
                return this.inverse;
            }
            else
            {
                throw new Exception("Matrix must be square");
            }
        }

        /// <summary>
        /// Copy Matrix into another
        /// </summary>
        /// <param name="copy"></param>
        public void Copy(Matrix copy)
        {
            if (this.NumColumns != copy.NumColumns || this.NumRows != copy.NumRows)
            {
                this.matrix = new double[copy.NumRows, copy.NumColumns];
            }

            for (int row = 1; row <= this.NumRows; row++)
            {
                for (int col = 1; col <= this.NumColumns; col++)
                {
                    this[row, col] = copy[row, col];
                }
            }
            this.GaussianPerformed = copy.GaussianPerformed;
            this.rank = copy.rank;
        }

        /// <summary>
        /// Matrix Addition
        /// </summary>
        /// <param name="matrix">Matrix to be added</param>
        /// <returns>Their Addtion</returns>
        public Matrix Add(Matrix matrix)
        {
            if (this.NumColumns != matrix.NumColumns || this.NumRows != matrix.NumRows)
            {
                throw new Exception("Invalid operation! Matrices Must have the same dimension");
            }
            Matrix newMatrix = new Matrix(this.NumRows, this.NumColumns);

            for (int row = 1; row <= this.NumRows; row++)
            {
                for (int col = 1; col <= this.NumColumns; col++)
                {
                    newMatrix[row, col] = this[row, col] + matrix[row, col];
                }
            }
            return newMatrix;
        }

        /// <summary>
        /// Subtracts one matrix from another
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public Matrix Subtract(Matrix matrix)
        {
            if (this.NumColumns != matrix.NumColumns || this.NumRows != matrix.NumRows)
            {
                throw new Exception("Invalid operation! Matrices Must have the same dimension");
            }
            Matrix newMatrix = new Matrix(this.NumRows, this.NumColumns);

            for (int row = 1; row <= this.NumRows; row++)
            {
                for (int col = 1; col <= this.NumColumns; col++)
                {
                    newMatrix[row, col] = this[row, col] - matrix[row, col];
                }
            }
            return newMatrix;
        }

        /// <summary>
        /// Creates a identity matrix
        /// </summary>
        /// <param name="dimension"></param>
        /// <returns>identity matrix</returns>
        static public Matrix Identity(int dimension)
        {
            Matrix identity = new Matrix(dimension);
            for (int row = 1; row <= identity.NumRows; row++)
            {
                for (int col = 1; col <= identity.NumColumns; col++)
                {
                    if (row == col)
                    {
                        identity[row, col] = 1;
                    }
                    else
                    {
                        identity[row, col] = 0;
                    }
                }
            }
            return identity;
        }

        /// <summary>
        /// Matrix-Matrix Multiplication
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public Matrix Multiply(Matrix matrix)
        {
            if (this.NumColumns != matrix.NumRows)
            {
                throw new Exception("Invalid Matrix Multiplication");
            }
            Matrix newMatrix = new Matrix(this.NumRows, matrix.NumColumns);
            for (int row = 1; row <= newMatrix.NumRows; row++)
            {
                for (int col = 1; col <= newMatrix.NumColumns; col++)
                {
                    for (int i = 1; i <= this.NumColumns; i++)
                    {
                        newMatrix[row, col] += this[row, i] * matrix[i, col];
                    }
                }
            }

            return newMatrix;
        }

        /// <summary>
        /// Multiplies a matrix by a scalar
        /// </summary>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public Matrix ScalarMult(double scalar)
        {
            Matrix newMatrix = new Matrix(this.NumRows, this.NumColumns);
            for (int row = 1; row <= this.NumRows; row++)
            {
                for (int col = 1; col <= this.NumColumns; col++)
                {
                    newMatrix[row, col] = scalar * this[row, col];
                }
            }
            return newMatrix;
        }

        /// <summary>
        /// Dot Product of two vectors
        /// </summary>
        /// <param name="vector"></param>
        /// <returns>A scalar</returns>
        public double DotProduct(Matrix vector)
        {
            if (!this.IsColVector && !vector.IsColVector)
            {
                throw new Exception("Argument is not a Vector");
            }
            double dot = 0;
            for (int i = 1; i <= this.NumRows; i++)
            {
                dot += (this[i, 1] * vector[i, 1]);
            }
            return dot;
        }

        /// <summary>
        /// Transposes a matrix
        /// </summary>
        public Matrix Transpose()
        {
            Matrix newMatrix = new Matrix(this.NumColumns, this.NumRows);
            for (int row = 1; row <= newMatrix.NumRows; row++)
            {
                for (int col = 1; col <= newMatrix.NumColumns; col++)
                {
                    newMatrix[row, col] = this[col, row];
                }
            }
            return newMatrix;
        }

        /// <summary>
        /// Generates a random square matrix
        /// </summary>
        /// <param name="dimension"></param>
        /// <returns>random matrix</returns>
        static public Matrix RandomMatrix(int dimension)
        {
            Matrix newMatrix = new Matrix(dimension);
            Random rand = new Random();
            for (int row = 1; row <= newMatrix.NumRows; row++)
            {
                for (int col = 1; col <= newMatrix.NumColumns; col++)
                {

                    newMatrix[row, col] = rand.Next(9);
                }
            }
            return newMatrix;
        }

        /// <summary>
        /// Generates rectangular random matrix
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="columns"></param>
        /// <returns>Random Matrix</returns>
        static public Matrix RandomMatrix(int rows, int columns)
        {
            Matrix newMatrix = new Matrix(rows, columns);
            Random rand = new Random();
            for (int row = 1; row <= newMatrix.NumRows; row++)
            {
                for (int col = 1; col <= newMatrix.NumColumns; col++)
                {
                    newMatrix[row, col] = rand.Next(9);
                }
            }
            return newMatrix;
        }

        /// <summary>
        /// Zeros entries below an pivot
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        private void ZeroEntry(int row, int col)
        {
            double multiplier = 0;
            for (int i = row + 1; i <= this.NumRows; i++)
            {
                if (this[i, col] == 0)
                {
                    continue;
                }
                multiplier = this[i, col] / this[row, col];
                for (int j = col; j <= this.NumColumns; j++)
                {
                    this[i, j] = this[i, j] - (multiplier * this[row, j]);
                }
            }
        }

        /// <summary>
        /// Exchanges two rows
        /// </summary>
        /// <param name="row1"></param>
        /// <param name="row2"></param>
        public void SwapRows(int row1, int row2)
        {
            for (int col = 1; col <= this.NumColumns; col++)
            {
                double temp = this[row1, col];
                this[row1, col] = this[row2, col];
                this[row2, col] = temp;
            }
        }

        /// <summary>
        /// Reduces matrix to echelon form
        /// </summary>
        public Matrix Gaussian()
        {
            Matrix newMatrix = new Matrix(this.NumRows, this.NumColumns);
            newMatrix.Copy(this);
            for (int row = 1; row <= newMatrix.NumRows; row++)
            {
                for (int col = row; col <= newMatrix.NumColumns; col++)
                {
                    if (newMatrix[row, col] == 0)
                    {
                        for (int i = row + 1; i <= newMatrix.NumRows; i++)
                        {
                            if (newMatrix[i, col] != 0)
                            {
                                newMatrix.SwapRows(row, i);
                                break;
                            }
                        }
                    }
                    if (newMatrix[row, col] != 0)
                    {
                        newMatrix.ZeroEntry(row, col);
                        if (!GaussianPerformed)
                        {
                            this.rank++;
                        }
                        break;
                    }
                }
            }
            GaussianPerformed = true;
            return newMatrix;
        }

        /// <summary>
        /// Zeroes entries above an pivot
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        private void ZeroEntryAbove(int row, int col)
        {
            double multiplier = 0;
            for (int i = row - 1; i >= 1; i--)
            {
                if (this[i, col] == 0)
                {
                    continue;
                }
                multiplier = this[i, col] / this[row, col];
                for (int j = i; j <= this.NumColumns; j++)
                {
                    this[i, j] = this[i, j] - (multiplier * this[row, j]);
                }
            }
        }

        /// <summary>
        /// Divides rows by pivot 
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        public void DPivot(int row, int col)
        {
            double multiplier = 1 / this[row, col];
            for (int i = row; i <= this.NumColumns; i++)
            {
                this[row, i] = multiplier * this[row, i];
            }
        }

        /// <summary>
        /// Reduces matrix to row reduced echelon form
        /// </summary>
        public Matrix GaussJordan()
        {
            Matrix newMatrix = new Matrix(this.NumRows, this.NumRows);
            newMatrix = this.Gaussian();
            for (int row = newMatrix.NumRows; row >= 1; row--)
            {
                for (int col = row; col <= newMatrix.NumColumns; col++)
                {
                    if (newMatrix[row, col] != 0)
                    {
                        newMatrix.ZeroEntryAbove(row, col);
                        break;
                    }
                }
            }
            for (int row = 1; row <= newMatrix.NumRows; row++)
            {
                for (int col = row; col <= newMatrix.NumColumns; col++)
                {
                    if (newMatrix[row, col] != 0)
                    {
                        newMatrix.DPivot(row, col);
                        break;
                    }

                }
            }
            return newMatrix;
        }

        /// <summary>
        /// Augments a matrix with another
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public Matrix AugmentMatrix(Matrix matrix)
        {
            if (this.NumRows != matrix.NumRows)
            {
                throw new Exception("Cannot Augment Matrix! Rows of matrices must be equal");
            }
            Matrix newMatrix = new Matrix(this.NumRows, this.NumColumns + matrix.NumColumns);
            for (int row = 1; row <= this.NumRows; row++)
            {
                for (int col = 1; col <= this.NumColumns; col++)
                {
                    newMatrix[row, col] = this[row, col];
                }
            }
            for (int row = 1; row <= matrix.NumRows; row++)
            {
                for (int col = 1; col <= matrix.NumColumns; col++)
                {
                    newMatrix[row, this.NumColumns + col] = matrix[row, col];
                }
            }
            return newMatrix;
        }

        /// <summary>
        /// Augment Matrix Rows with another
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public Matrix AugmentMatrixRow(Matrix matrix)
        {
            if (this.NumColumns != matrix.NumColumns)
            {
                throw new Exception("Cannot Augment Matrix! Columns of matrices must be equal");
            }
            Matrix newMatrix = new Matrix(this.NumRows + matrix.NumRows, this.NumColumns);
            for (int row = 1; row <= this.NumRows; row++)
            {
                for (int col = 1; col <= this.NumColumns; col++)
                {
                    newMatrix[row, col] = this[row, col];
                }
            }
            for (int row = 1; row <= matrix.NumRows; row++)
            {
                for (int col = 1; col <= matrix.NumColumns; col++)
                {
                    newMatrix[this.NumRows + row, col] = matrix[row, col];
                }
            }
            return newMatrix;
        }

        /// <summary>
        /// Delete rows from matrix
        /// </summary>
        /// <param name="rows">List of rows to be deleted</param>
        /// <returns></returns>
        public void DeleteRow(params int[] rows)
        {
            foreach (int item in rows)
            {
                if (item > this.NumRows)
                {
                    throw new ArgumentOutOfRangeException("Argument Out of Bounds of Array");
                }
            }

            Matrix newMatrix = new Matrix(this.NumRows - rows.Length, this.NumColumns);
            int deleted = 0;
            for (int row = 1; row <= this.NumRows; row++)
            {
                if (rows.Contains(row))
                {
                    deleted++;
                    continue;
                }
                for (int col = 1; col <= this.NumColumns; col++)
                {
                    newMatrix[row - deleted, col] = this[row, col];
                }
            }
            this.Copy(newMatrix);
        }

        /// <summary>
        /// Delete cols from matrix
        /// </summary>
        /// <param name="rows">List of cols to be deleted</param>
        /// <returns></returns>
        public void DeleteCol(params int[] cols)
        {
            foreach (int item in cols)
            {
                if (item > this.NumColumns)
                {
                    throw new ArgumentOutOfRangeException("Argument Out of Bounds of Array");
                }
            }

            Matrix newMatrix = new Matrix(this.NumRows, this.NumColumns - cols.Length);
            int deleted = 0;
            for (int row = 1; row <= this.NumRows; row++)
            {
                deleted = 0;
                for (int col = 1; col <= this.NumColumns; col++)
                {
                    if (cols.Contains(col))
                    {
                        deleted++;
                        continue;
                    }
                    newMatrix[row, col - deleted] = this[row, col];
                }
            }
            this.Copy(newMatrix);
        }

        /// <summary>
        /// Updates Matrix Dimension With new rows and columns;
        /// </summary>
        /// <param name="rows">Number of rows for new matrix</param>
        /// <param name="cols">Number of cols for new matrix</param>
        /// <returns></returns>
        public Matrix UpdateMatrix(int rows, int cols)
        {
            Matrix a = new Matrix(rows - this.NumRows, this.NumColumns);

            Matrix aug = this.AugmentMatrixRow(a);
            Matrix b = new Matrix(aug.NumRows, cols - aug.NumColumns);
            aug = aug.AugmentMatrix(b);
            return aug;
        }


        /// <summary>
        /// Decomposes Matrices to LU
        /// </summary>
        /// <returns>A tuple containing L, U, b</returns>
        private Tuple<Matrix, Matrix, Matrix> LUFactor()
        {
            Matrix L = new Matrix(this.NumRows);
            Matrix U = new Matrix(this.NumRows);
            for (int row = 1; row <= U.NumRows; row++)
            {
                for (int col = 1; col <= U.NumColumns; col++)
                {
                    U[row, col] = this[row, col];
                }
            }
            Matrix b = new Matrix(this.NumRows, 1);
            for (int rows = 1; rows <= b.NumRows; rows++)
            {
                b[rows, 1] = this[rows, this.NumColumns];
            }

            for (int row = 1; row <= U.NumRows; row++)
            {
                for (int col = row; col <= U.NumColumns; col++)
                {
                    if (U[row, col] == 0)
                    {
                        for (int i = row + 1; i <= U.NumRows; i++)
                        {
                            if (U[i, col] != 0)
                            {
                                U.SwapRows(row, i);
                                L.SwapRows(row, i);
                                b.SwapRows(row, i);
                                break;
                            }
                        }
                    }

                    if (U[row, col] != 0)
                    {
                        double multiplier = 0;

                        for (int i = row + 1; i <= U.NumRows; i++)
                        {
                            if (U[i, col] == 0)
                            {
                                continue;
                            }
                            multiplier = U[i, col] / U[row, col];
                            L[i, col] = multiplier;
                            for (int j = col; j <= U.NumColumns; j++)
                            {
                                U[i, j] = U[i, j] - (multiplier * U[row, j]);
                            }
                        }
                        break;
                    }
                }

            }
            for (int row = 1; row <= L.NumRows; row++)
            {
                for (int col = 1; col <= L.NumColumns; col++)
                {
                    if (row == col)
                    {
                        L[row, col] = 1;
                    }
                }
            }
            
            return new Tuple<Matrix, Matrix, Matrix>(L, U, b);
        }

        /// <summary>
        /// Perfoms Foward Substitution
        /// </summary>
        /// <param name="L"></param>
        /// <param name="b"></param>
        /// <returns>Solution</returns>
        private Matrix ForwardSub(Matrix L, Matrix b)
        {
            Matrix y = new Matrix(this.NumRows, 1);
            for (int row = 1; row <= y.NumRows; row++)
            {
                y[row, 1] = b[row, 1];
                for (int col = row; col > 1; col--)
                {
                    y[row, 1] -= y[col - 1, 1] * L[row, col - 1];
                }
                y[row, 1] /= L[row, row];
            }
            return y;
        }

        /// <summary>
        /// Perfoms Back Substitution
        /// </summary>
        /// <param name="U"></param>
        /// <param name="y"></param>
        /// <returns>Solution</returns>
        private Matrix BackSub(Matrix U, Matrix y)
        {
            Matrix x = new Matrix(this.NumRows, 1);
            for (int row = x.NumRows; row >= 1; row--)
            {
                double a = y[row, 1];
                x[row, 1] = a;
                for (int col = row; col < U.NumColumns; col++)
                {
                    x[row, 1] -= x[col + 1, 1] * U[row, col + 1];
                }
                x[row, 1] /= U[row, row];
            }
            return x;
        }

        /// <summary>
        /// Solves Linear equations using LU Decomposition
        /// </summary>
        /// <returns></returns>
        public Matrix SolveLU()
        {
            Tuple<Matrix, Matrix, Matrix> LU = this.LUFactor();
            Matrix L = LU.Item1;
            Matrix U = LU.Item2;
            Matrix b = LU.Item3;

            Matrix y = ForwardSub(L, b);
            Matrix x = BackSub(U, y);
            return x;
        }
        /********************** OPERATOR OVERLOADING ***********************/
        /// <summary>
        /// Positve
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static Matrix operator +(Matrix matrix)
            => matrix;
        /// <summary>
        /// Negate
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static Matrix operator -(Matrix matrix)
            => matrix.ScalarMult(-1);
        /// <summary>
        /// Addition
        /// </summary>
        /// <param name="matrix1"></param>
        /// <param name="matrix2"></param>
        /// <returns></returns>
        public static Matrix operator +(Matrix matrix1, Matrix matrix2)
            => matrix1.Add(matrix2);
        /// <summary>
        /// Subtraction
        /// </summary>
        /// <param name="matrix1"></param>
        /// <param name="matrix2"></param>
        /// <returns></returns>
        public static Matrix operator -(Matrix matrix1, Matrix matrix2)
            => matrix1.Subtract(matrix2);
        /// <summary>
        /// Multiplication
        /// </summary>
        /// <param name="matrix1"></param>
        /// <param name="matrix2"></param>
        /// <returns></returns>
        public static Matrix operator *(Matrix matrix1, Matrix matrix2)
            => matrix1.Multiply(matrix2);
        /// <summary>
        /// Power
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="pow"></param>
        /// <returns></returns>
        public static Matrix operator ^(Matrix matrix, int pow)
            => matrix.Inverse();

        /// <summary>
        /// Scalar Multilication
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public static Matrix operator *(Matrix matrix, int scalar)
            => matrix.ScalarMult(scalar);
        public static Matrix operator *(int scalar, Matrix matrix)
            => matrix.ScalarMult(scalar);

        /// <summary>
        /// Matrix Matrix Divsion
        /// </summary>
        /// <param name="matrix1"></param>
        /// <param name="matrix2"></param>
        /// <returns></returns>
        public static Matrix operator /(Matrix matrix1, Matrix matrix2)
            => matrix2.Inverse().Multiply(matrix1);
    }

}
