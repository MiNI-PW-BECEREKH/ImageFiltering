using System.ComponentModel;
using System.Drawing;
using System.Runtime.Serialization;
using System.Windows.Controls;
using System.Xml.Serialization;

namespace ImageFiltering.Common.Models
{
    public class Kernel
    {
        public int Height { get; set; }

        public int Width { get; set; }

        public int IntensityOffset { get; set; }

        public List<List<int>> matrixForSerializtion { get; set; } = new();

        public Point Anchor { get; set; }

        public int D { get; set; }

        [XmlIgnore]
        public int[,] Matrix { get; set; }
        public Kernel()
        {

        }
        public Kernel(int[,] matrix, Point anchor, int intensityOffset = 0)
        {
            Anchor = anchor;
            Width = matrix.GetLength(1);
            Height = matrix.GetLength(0);
            Matrix = matrix;
            IntensityOffset = intensityOffset;
            D = 0;

            if (anchor.X < 0 || anchor.X > Width)
                throw new ArgumentException("Anchor coordinate X is not in Kernel");
            if (anchor.Y < 0 || anchor.Y > Height)
                throw new ArgumentException("Anchor coordinate Y is not in Kernel");

            ReComputeD();

        }

        public void ReComputeD()
        {
            D = 0;
            for (int kernelCol = 0; kernelCol < Width; kernelCol++)
            {
                for (int kernelRow = 0; kernelRow < Height; kernelRow++)
                {
                    D += Matrix[kernelRow, kernelCol];
                }
            }
            D = D == 0 ? 1 : D;
        }

        public void EraseRow()
        {
            if (3 >= Height )
                return;
            var newMatrix = new int[Height - 1, Width];
            for (int kernelCol = 0; kernelCol < Width; kernelCol++)
            {
                for (int kernelRow = 0; kernelRow < Height - 1; kernelRow++)
                {
                    newMatrix[kernelRow, kernelCol] = Matrix[kernelRow, kernelCol];
                }
            }

            Matrix = newMatrix;
            Width = newMatrix.GetLength(1);
            Height = newMatrix.GetLength(0);

            if (Anchor.X >= Width || Anchor.Y >= Height)
                Anchor = new Point(Width/2, Height /2);

            ReComputeD();

        }

        public void AddRow()
        {
            var newMatrix = new int[Height + 1, Width];
            for (int kernelCol = 0; kernelCol < Width; kernelCol++)
            {
                for (int kernelRow = 0; kernelRow < Height + 1; kernelRow++)
                {
                    if (kernelRow >= Height)
                        newMatrix[kernelRow, kernelCol] = 0;
                    else
                        newMatrix[kernelRow, kernelCol] = Matrix[kernelRow, kernelCol];
                }
            }

            Matrix = newMatrix;
            Width = newMatrix.GetLength(1);
            Height = newMatrix.GetLength(0);
            ReComputeD();

        }

        public void EraseColumn()
        {
            if (3 >= Width )
                return;
            var newMatrix = new int[Height, Width - 1];
            for (int kernelCol = 0; kernelCol < Width - 1; kernelCol++)
            {
                for (int kernelRow = 0; kernelRow < Height; kernelRow++)
                {
                    newMatrix[kernelRow, kernelCol] = Matrix[kernelRow, kernelCol];
                }
            }

            Matrix = newMatrix;
            Width = newMatrix.GetLength(1);
            Height = newMatrix.GetLength(0);

            if (Anchor.X >= Width || Anchor.Y >= Height)
                Anchor = new Point(Width / 2, Height / 2);

            ReComputeD();

        }

        public void AddColumn()
        {
            var newMatrix = new int[Height, Width + 1];
            for (int kernelCol = 0; kernelCol < Width + 1; kernelCol++)
            {
                for (int kernelRow = 0; kernelRow < Height; kernelRow++)
                {
                    if (kernelCol >= Width)
                        newMatrix[kernelRow, kernelCol] = 0;
                    else
                        newMatrix[kernelRow, kernelCol] = Matrix[kernelRow, kernelCol];
                }
            }

            Matrix = newMatrix;
            Width = newMatrix.GetLength(1);
            Height = newMatrix.GetLength(0);
            ReComputeD();
        }

        public void PrepareSerialization()
        {
            for (int kernelCol = 0; kernelCol < Width; kernelCol++)
            {
                var innerList = new List<int>();
                for (int kernelRow = 0; kernelRow < Height; kernelRow++)
                {
                    innerList.Add(Matrix[kernelRow, kernelCol]);
                }
                matrixForSerializtion.Add(innerList);
            }
        }

        public void ApplyDeserialization()
        {
            Matrix = new int[Height, Width];
            for (int kernelCol = 0; kernelCol < Width; kernelCol++)
            {
                for (int kernelRow = 0; kernelRow < Height; kernelRow++)
                {
                    Matrix[kernelRow, kernelCol] = matrixForSerializtion[kernelCol][kernelRow];
                }
                
            }
        }
    }
}
