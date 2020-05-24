using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace MahApps.Metro.Controls
{
    /// <summary>
    ///   Based on Greg Schechter's Planerator
    ///   http://blogs.msdn.com/b/greg_schechter/archive/2007/10/26/enter-the-planerator-dead-simple-3d-in-wpf-with-a-stupid-name.aspx
    /// </summary>
    [ContentProperty("Child")]
    public class Planerator : FrameworkElement
    {
        /// <summary>Identifies the <see cref="RotationX"/> dependency property.</summary>
        public static readonly DependencyProperty RotationXProperty
            = DependencyProperty.Register(nameof(RotationX),
                                          typeof(double),
                                          typeof(Planerator),
                                          new UIPropertyMetadata(0.0, (d, args) => ((Planerator)d).UpdateRotation()));

        public double RotationX
        {
            get => (double)this.GetValue(RotationXProperty);
            set => this.SetValue(RotationXProperty, value);
        }

        /// <summary>Identifies the <see cref="RotationY"/> dependency property.</summary>
        public static readonly DependencyProperty RotationYProperty
            = DependencyProperty.Register(nameof(RotationY),
                                          typeof(double),
                                          typeof(Planerator),
                                          new UIPropertyMetadata(0.0, (d, args) => ((Planerator)d).UpdateRotation()));

        public double RotationY
        {
            get => (double)this.GetValue(RotationYProperty);
            set => this.SetValue(RotationYProperty, value);
        }

        /// <summary>Identifies the <see cref="RotationZ"/> dependency property.</summary>
        public static readonly DependencyProperty RotationZProperty
            = DependencyProperty.Register(nameof(RotationZ),
                                          typeof(double),
                                          typeof(Planerator),
                                          new UIPropertyMetadata(0.0, (d, args) => ((Planerator)d).UpdateRotation()));

        public double RotationZ
        {
            get => (double)this.GetValue(RotationZProperty);
            set => this.SetValue(RotationZProperty, value);
        }

        /// <summary>Identifies the <see cref="FieldOfView"/> dependency property.</summary>
        public static readonly DependencyProperty FieldOfViewProperty
            = DependencyProperty.Register(nameof(FieldOfView),
                                          typeof(double),
                                          typeof(Planerator),
                                          new UIPropertyMetadata(45.0, (d, args) => ((Planerator)d).Update3D(), (d, val) => Math.Min(Math.Max((double)val, 0.5), 179.9)));

        public double FieldOfView
        {
            get => (double)this.GetValue(FieldOfViewProperty);
            set => this.SetValue(FieldOfViewProperty, value);
        }

        // clamp to a meaningful range

        private static readonly Point3D[] Mesh =
        {
            new Point3D(0, 0, 0), new Point3D(0, 1, 0), new Point3D(1, 1, 0),
            new Point3D(1, 0, 0)
        };

        private static readonly Point[] TexCoords =
        {
            new Point(0, 1), new Point(0, 0), new Point(1, 0),
            new Point(1, 1)
        };

        private static readonly int[] Indices = { 0, 2, 1, 0, 3, 2 };
        private static readonly Vector3D XAxis = new Vector3D(1, 0, 0);
        private static readonly Vector3D YAxis = new Vector3D(0, 1, 0);
        private static readonly Vector3D ZAxis = new Vector3D(0, 0, 1);
        private readonly QuaternionRotation3D quaternionRotation = new QuaternionRotation3D();
        private readonly RotateTransform3D rotationTransform = new RotateTransform3D();
        private readonly ScaleTransform3D scaleTransform = new ScaleTransform3D();
        private Decorator logicalChild;
        private FrameworkElement originalChild;
        private Viewport3D viewport3D;
        private FrameworkElement visualChild;
        private Viewport2DVisual3D frontModel;

        public FrameworkElement Child
        {
            get => this.originalChild;
            set
            {
                if (this.originalChild == value)
                {
                    return;
                }

                this.RemoveVisualChild(this.visualChild);
                this.RemoveLogicalChild(this.logicalChild);

                // Wrap child with special decorator that catches layout invalidations. 
                this.originalChild = value;
                this.logicalChild = new LayoutInvalidationCatcher { Child = this.originalChild };
                this.visualChild = this.CreateVisualChild();

                this.AddVisualChild(this.visualChild);

                // Need to use a logical child here to make sure databinding operations get down to it,
                // since otherwise the child appears only as the Visual to a Viewport2DVisual3D, which 
                // doesn't have databinding operations pass into it from above.
                this.AddLogicalChild(this.logicalChild);
                this.InvalidateMeasure();
            }
        }

        protected override int VisualChildrenCount => this.visualChild == null ? 0 : 1;

        protected override Size MeasureOverride(Size availableSize)
        {
            if (this.logicalChild != null)
            {
                // Measure based on the size of the logical child, since we want to align with it.
                this.logicalChild.Measure(availableSize);
                var result = this.logicalChild.DesiredSize;
                this.visualChild.Measure(result);
                return result;
            }

            return new Size(0, 0);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (this.logicalChild != null)
            {
                this.logicalChild.Arrange(new Rect(finalSize));
                this.visualChild.Arrange(new Rect(finalSize));
                this.Update3D();
            }

            return base.ArrangeOverride(finalSize);
        }

        protected override Visual GetVisualChild(int index)
        {
            return this.visualChild;
        }

        private FrameworkElement CreateVisualChild()
        {
            var simpleQuad = new MeshGeometry3D
                             {
                                 Positions = new Point3DCollection(Mesh),
                                 TextureCoordinates = new PointCollection(TexCoords),
                                 TriangleIndices = new Int32Collection(Indices)
                             };

            // Front material is interactive, back material is not.
            Material frontMaterial = new DiffuseMaterial(Brushes.White);
            frontMaterial.SetValue(Viewport2DVisual3D.IsVisualHostMaterialProperty, true);

            var vb = new VisualBrush(this.logicalChild);
            this.SetCachingForObject(vb); // big perf wins by caching!!
            Material backMaterial = new DiffuseMaterial(vb);

            this.rotationTransform.Rotation = this.quaternionRotation;
            var xfGroup = new Transform3DGroup { Children = { this.scaleTransform, this.rotationTransform } };

            var backModel = new GeometryModel3D { Geometry = simpleQuad, Transform = xfGroup, BackMaterial = backMaterial };
            var m3dGroup = new Model3DGroup
                           {
                               Children =
                               {
                                   new DirectionalLight(Colors.White, new Vector3D(0, 0, -1)),
                                   new DirectionalLight(Colors.White, new Vector3D(0.1, -0.1, 1)),
                                   backModel
                               }
                           };

            // Non-interactive Visual3D consisting of the backside, and two lights.
            var mv3d = new ModelVisual3D { Content = m3dGroup };

            if (this.frontModel != null)
            {
                this.frontModel.Visual = null;
            }

            // Interactive frontside Visual3D
            this.frontModel = new Viewport2DVisual3D
                              {
                                  Geometry = simpleQuad,
                                  Visual = this.logicalChild,
                                  Material = frontMaterial,
                                  Transform = xfGroup
                              };

            // Cache the brush in the VP2V3 by setting caching on it.  Big perf wins.
            this.SetCachingForObject(this.frontModel);

            // Scene consists of both the above Visual3D's.
            this.viewport3D = new Viewport3D { ClipToBounds = false, Children = { mv3d, this.frontModel } };

            this.UpdateRotation();

            return this.viewport3D;
        }

        public void Refresh()
        {
            if (this.logicalChild != null)
            {
                // #3720 I didn't find a better solution to update the child after changing accent/theme
                this.logicalChild.Child = null;
                this.logicalChild.Child = this.Child;

                this.InvalidateVisual();
                this.InvalidateMeasure();
            }
        }

        private void SetCachingForObject(DependencyObject d)
        {
            RenderOptions.SetCachingHint(d, CachingHint.Cache);
            RenderOptions.SetCacheInvalidationThresholdMinimum(d, 0.5);
            RenderOptions.SetCacheInvalidationThresholdMaximum(d, 2.0);
        }

        private void UpdateRotation()
        {
            var qx = new Quaternion(XAxis, this.RotationX);
            var qy = new Quaternion(YAxis, this.RotationY);
            var qz = new Quaternion(ZAxis, this.RotationZ);

            this.quaternionRotation.Quaternion = qx * qy * qz;
        }

        private void Update3D()
        {
            // Use GetDescendantBounds for sizing and centering since DesiredSize includes layout whitespace, whereas GetDescendantBounds 
            // is tighter
            Rect logicalBounds = VisualTreeHelper.GetDescendantBounds(this.logicalChild);
            double w = logicalBounds.Width;
            double h = logicalBounds.Height;

            // Create a camera that looks down -Z, with up as Y, and positioned right halfway in X and Y on the element, 
            // and back along Z the right distance based on the field-of-view is the same projected size as the 2D content
            // that it's looking at.  See http://blogs.msdn.com/greg_schechter/archive/2007/04/03/camera-construction-in-parallaxui.aspx
            // for derivation of this camera.
            double fovInRadians = this.FieldOfView * (Math.PI / 180);
            double zValue = w / Math.Tan(fovInRadians / 2) / 2;
            this.viewport3D.Camera = new PerspectiveCamera(new Point3D(w / 2, h / 2, zValue), -ZAxis, YAxis, this.FieldOfView);

            this.scaleTransform.ScaleX = w;
            this.scaleTransform.ScaleY = h;
            this.rotationTransform.CenterX = w / 2;
            this.rotationTransform.CenterY = h / 2;
        }
    }
}