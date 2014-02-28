using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media.Media3D;

namespace Microsoft.DwayneNeed.Extensions
{
    public static class MeshGeometry3DExtensions
    {
        public static IEnumerable<Tuple<Point3D, Point3D, Point3D>> GetTrianglePositions(this MeshGeometry3D _this)
        {
            int numTriangles = _this.TriangleIndices.Count/3;

            for (int i = 0; i < numTriangles; i++)
            {
                int j = i*3;
                yield return new Tuple<Point3D, Point3D, Point3D>(_this.Positions[_this.TriangleIndices[j+0]],
                                                                  _this.Positions[_this.TriangleIndices[j+1]],
                                                                  _this.Positions[_this.TriangleIndices[j+2]]);
            }
        }

        public static IEnumerable<Tuple<int, int, int>> GetTriangleIndices(this MeshGeometry3D _this)
        {
            int numTriangles = _this.TriangleIndices.Count / 3;

            for (int i = 0; i < numTriangles; i++)
            {
                int j = i * 3;
                yield return new Tuple<int, int, int>(_this.TriangleIndices[j + 0],
                                                      _this.TriangleIndices[j + 1],
                                                      _this.TriangleIndices[j + 2]);
            }
        }

        public static IEnumerable<Tuple<int, int>> GetTriangleEdgeIndices(this MeshGeometry3D _this)
        {
            int numTriangles = _this.TriangleIndices.Count / 3;

            for (int i = 0; i < numTriangles; i++)
            {
                int j = i * 3;
                yield return new Tuple<int, int>(_this.TriangleIndices[j + 0], _this.TriangleIndices[j + 1]);
                yield return new Tuple<int, int>(_this.TriangleIndices[j + 1], _this.TriangleIndices[j + 2]);
                yield return new Tuple<int, int>(_this.TriangleIndices[j + 2], _this.TriangleIndices[j + 0]);
            }
        }

        public static Size CalculateIdealTextureSize(this MeshGeometry3D _this)
        {
            Size size = new Size();

            foreach (Tuple<int, int> edgeIndices in _this.GetTriangleEdgeIndices())
            {
                Vector edgeTexture = _this.TextureCoordinates[edgeIndices.Item1] - _this.TextureCoordinates[edgeIndices.Item2];
                Vector3D edgeModel = _this.Positions[edgeIndices.Item1] - _this.Positions[edgeIndices.Item2];
                double scale = edgeModel.Length / edgeTexture.Length;

                double widthModel = (edgeTexture.X * scale) / edgeTexture.Length;
                double heightModel = (edgeTexture.Y * scale) / edgeTexture.Length;

                size.Width = Math.Max(size.Width, Math.Abs(widthModel));
                size.Height = Math.Max(size.Height, Math.Abs(heightModel));
            }

            return size;
        }
    }
}
