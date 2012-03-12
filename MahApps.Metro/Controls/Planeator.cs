namespace MahApps.Metro.Controls
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;

    /// <summary>
    ///   Based on Greg Schechter's Planeator
    ///   http://blogs.msdn.com/b/greg_schechter/archive/2007/10/26/enter-the-planerator-dead-simple-3d-in-wpf-with-a-stupid-name.aspx
    /// </summary>
    [ContentProperty("Child")]
    public class Planerator : FrameworkElement
    {
        #region Constants and Fields

        public static readonly DependencyProperty FieldOfViewProperty = DependencyProperty.Register(
            "FieldOfView",
            typeof(double),
            typeof(Planerator),
            new UIPropertyMetadata(
                45.0, (d, args) => ((Planerator)d).Update3D(), (d, val) => Math.Min(Math.Max((double)val, 0.5), 179.9)));

        public static readonly DependencyProperty RotationXProperty = DependencyProperty.Register(
            "RotationX",
            typeof(double),
            typeof(Planerator),
            new UIPropertyMetadata(0.0, (d, args) => ((Planerator)d).UpdateRotation()));

        public static readonly DependencyProperty RotationYProperty = DependencyProperty.Register(
            "RotationY",
            typeof(double),
            typeof(Planerator),
            new UIPropertyMetadata(0.0, (d, args) => ((Planerator)d).UpdateRotation()));

        public static readonly DependencyProperty RotationZProperty = DependencyProperty.Register(
            "RotationZ",
            typeof(double),
            typeof(Planerator),
            new UIPropertyMetadata(0.0, (d, args) => ((Planerator)d).UpdateRotation()));

        private static readonly int[] Indices = new[] { 0, 2, 1, 0, 3, 2 };

        // clamp to a meaningful range

        private static readonly Point3D[] Mesh = new[]
            { new Point3D(0, 0, 0), new Point3D(0, 1, 0), new Point3D(1, 1, 0), new Point3D(1, 0, 0) };

        private static readonly Point[] TexCoords = new[]
            { new Point(0, 1), new Point(0, 0), new Point(1, 0), new Point(1, 1) };

        private static readonly Vector3D XAxis = new Vector3D(1, 0, 0);

        private static readonly Vector3D YAxis = new Vector3D(0, 1, 0);

        private static readonly Vector3D ZAxis = new Vector3D(0, 0, 1);

        private readonly QuaternionRotation3D _quaternionRotation = new QuaternionRotation3D();

        private readonly RotateTransform3D _rotationTransform = new RotateTransform3D();

        private readonly ScaleTransform3D _scaleTransform = new ScaleTransform3D();

        private Viewport2DVisual3D _frontModel;

        private FrameworkElement _logicalChild;

        private FrameworkElement _originalChild;

        private Viewport3D _viewport3D;

        private FrameworkElement _visualChild;

        #endregion

        #region Public Properties

        public FrameworkElement Child
        {
            get
            {
                return this._originalChild;
            }
            set
            {
                if (this._originalChild == value)
                {
                    return;
                }
                this.RemoveVisualChild(this._visualChild);
                this.RemoveLogicalChild(this._logicalChild);

                // Wrap child with special decorator that catches layout invalidations. 
                this._originalChild = value;
                this._logicalChild = new LayoutInvalidationCatcher { Child = this._originalChild };
                this._visualChild = this.CreateVisualChild();

                this.AddVisualChild(this._visualChild);

                // Need to use a logical child here to make sure databinding operations get down to it,
                // since otherwise the child appears only as the Visual to a Viewport2DVisual3D, which 
                // doesn't have databinding operations pass into it from above.
                this.AddLogicalChild(this._logicalChild);
                this.InvalidateMeasure();
            }
        }

        public double FieldOfView
        {
            get
            {
                return (double)this.GetValue(FieldOfViewProperty);
            }
            set
            {
                this.SetValue(FieldOfViewProperty, value);
            }
        }

        public double RotationX
        {
            get
            {
                return (double)this.GetValue(RotationXProperty);
            }
            set
            {
                this.SetValue(RotationXProperty, value);
            }
        }

        public double RotationY
        {
            get
            {
                return (double)this.GetValue(RotationYProperty);
            }
            set
            {
                this.SetValue(RotationYProperty, value);
            }
        }

        public double RotationZ
        {
            get
            {
                return (double)this.GetValue(RotationZProperty);
            }
            set
            {
                this.SetValue(RotationZProperty, value);
            }
        }

        #endregion

        #region Properties

        protected override int VisualChildrenCount
        {
            get
            {
                return this._visualChild == null ? 0 : 1;
            }
        }

        #endregion

        #region Methods

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (this._logicalChild != null)
            {
                this._logicalChild.Arrange(new Rect(finalSize));
                this._visualChild.Arrange(new Rect(finalSize));
                this.Update3D();
            }
            return base.ArrangeOverride(finalSize);
        }

        protected override Visual GetVisualChild(int index)
        {
            return this._visualChild;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            Size result;
            if (this._logicalChild != null)
            {
                // Measure based on the size of the logical child, since we want to align with it.
                this._logicalChild.Measure(availableSize);
                result = this._logicalChild.DesiredSize;
                this._visualChild.Measure(result);
            }
            else
            {
                result = new Size(0, 0);
            }
            return result;
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

            var vb = new VisualBrush(this._logicalChild);
            this.SetCachingForObject(vb); // big perf wins by caching!!
            Material backMaterial = new DiffuseMaterial(vb);

            this._rotationTransform.Rotation = this._quaternionRotation;
            var xfGroup = new Transform3DGroup { Children = { this._scaleTransform, this._rotationTransform } };

            var backModel = new GeometryModel3D
                { Geometry = simpleQuad, Transform = xfGroup, BackMaterial = backMaterial };
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

            if (this._frontModel != null)
            {
                this._frontModel.Visual = null;
            }

            // Interactive frontside Visual3D
            this._frontModel = new Viewport2DVisual3D
                { Geometry = simpleQuad, Visual = this._logicalChild, Material = frontMaterial, Transform = xfGroup };

            // Cache the brush in the VP2V3 by setting caching on it.  Big perf wins.
            this.SetCachingForObject(this._frontModel);

            // Scene consists of both the above Visual3D's.
            this._viewport3D = new Viewport3D { ClipToBounds = false, Children = { mv3d, this._frontModel } };

            this.UpdateRotation();

            return this._viewport3D;
        }

        private void SetCachingForObject(DependencyObject d)
        {
            RenderOptions.SetCachingHint(d, CachingHint.Cache);
            RenderOptions.SetCacheInvalidationThresholdMinimum(d, 0.5);
            RenderOptions.SetCacheInvalidationThresholdMaximum(d, 2.0);
        }

        private void Update3D()
        {
            // Use GetDescendantBounds for sizing and centering since DesiredSize includes layout whitespace, whereas GetDescendantBounds 
            // is tighter
            var logicalBounds = VisualTreeHelper.GetDescendantBounds(this._logicalChild);
            var w = logicalBounds.Width;
            var h = logicalBounds.Height;

            // Create a camera that looks down -Z, with up as Y, and positioned right halfway in X and Y on the element, 
            // and back along Z the right distance based on the field-of-view is the same projected size as the 2D content
            // that it's looking at.  See http://blogs.msdn.com/greg_schechter/archive/2007/04/03/camera-construction-in-parallaxui.aspx
            // for derivation of this camera.
            var fovInRadians = this.FieldOfView * (Math.PI / 180);
            var zValue = w / Math.Tan(fovInRadians / 2) / 2;
            this._viewport3D.Camera = new PerspectiveCamera(
                new Point3D(w / 2, h / 2, zValue), -ZAxis, YAxis, this.FieldOfView);

            this._scaleTransform.ScaleX = w;
            this._scaleTransform.ScaleY = h;
            this._rotationTransform.CenterX = w / 2;
            this._rotationTransform.CenterY = h / 2;
        }

        private void UpdateRotation()
        {
            var qx = new Quaternion(XAxis, this.RotationX);
            var qy = new Quaternion(YAxis, this.RotationY);
            var qz = new Quaternion(ZAxis, this.RotationZ);

            this._quaternionRotation.Quaternion = qx * qy * qz;
        }

        #endregion
    }
}