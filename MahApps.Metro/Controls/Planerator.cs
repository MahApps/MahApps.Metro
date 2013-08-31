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
        public static readonly DependencyProperty RotationXProperty =
            DependencyProperty.Register("RotationX", typeof(double), typeof(Planerator),
                                        new UIPropertyMetadata(0.0, (d, args) => ((Planerator)d).UpdateRotation()));

        public static readonly DependencyProperty RotationYProperty =
            DependencyProperty.Register("RotationY", typeof(double), typeof(Planerator),
                                        new UIPropertyMetadata(0.0, (d, args) => ((Planerator)d).UpdateRotation()));

        public static readonly DependencyProperty RotationZProperty =
            DependencyProperty.Register("RotationZ", typeof(double), typeof(Planerator),
                                        new UIPropertyMetadata(0.0, (d, args) => ((Planerator)d).UpdateRotation()));

        public static readonly DependencyProperty FieldOfViewProperty =
            DependencyProperty.Register("FieldOfView", typeof(double), typeof(Planerator),
                                        new UIPropertyMetadata(45.0, (d, args) => ((Planerator)d).Update3D(),
                                                               (d, val) => Math.Min(Math.Max((double)val, 0.5), 179.9)));
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
        private readonly QuaternionRotation3D _quaternionRotation = new QuaternionRotation3D();
        private readonly RotateTransform3D _rotationTransform = new RotateTransform3D();
        private readonly ScaleTransform3D _scaleTransform = new ScaleTransform3D();
        private FrameworkElement _logicalChild;
        private FrameworkElement _originalChild;
        private Viewport3D _viewport3D;
        private FrameworkElement _visualChild;
        private Viewport2DVisual3D _frontModel;

        public double RotationX
        {
            get { return (double)GetValue(RotationXProperty); }
            set { SetValue(RotationXProperty, value); }
        }

        public double RotationY
        {
            get { return (double)GetValue(RotationYProperty); }
            set { SetValue(RotationYProperty, value); }
        }

        public double RotationZ
        {
            get { return (double)GetValue(RotationZProperty); }
            set { SetValue(RotationZProperty, value); }
        }

        public double FieldOfView
        {
            get { return (double)GetValue(FieldOfViewProperty); }
            set { SetValue(FieldOfViewProperty, value); }
        }

        public FrameworkElement Child
        {
            get { return _originalChild; }
            set
            {
                if (_originalChild == value)
                    return;
                RemoveVisualChild(_visualChild);
                RemoveLogicalChild(_logicalChild);

                // Wrap child with special decorator that catches layout invalidations. 
                _originalChild = value;
                _logicalChild = new LayoutInvalidationCatcher { Child = _originalChild };
                _visualChild = CreateVisualChild();

                AddVisualChild(_visualChild);

                // Need to use a logical child here to make sure databinding operations get down to it,
                // since otherwise the child appears only as the Visual to a Viewport2DVisual3D, which 
                // doesn't have databinding operations pass into it from above.
                AddLogicalChild(_logicalChild);
                InvalidateMeasure();
            }
        }

        protected override int VisualChildrenCount
        {
            get { return _visualChild == null ? 0 : 1; }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            Size result;
            if (_logicalChild != null)
            {
                // Measure based on the size of the logical child, since we want to align with it.
                _logicalChild.Measure(availableSize);
                result = _logicalChild.DesiredSize;
                _visualChild.Measure(result);
            }
            else
            {
                result = new Size(0, 0);
            }
            return result;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (_logicalChild != null)
            {
                _logicalChild.Arrange(new Rect(finalSize));
                _visualChild.Arrange(new Rect(finalSize));
                Update3D();
            }
            return base.ArrangeOverride(finalSize);
        }

        protected override Visual GetVisualChild(int index)
        {
            return _visualChild;
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

            var vb = new VisualBrush(_logicalChild);
            SetCachingForObject(vb); // big perf wins by caching!!
            Material backMaterial = new DiffuseMaterial(vb);

            _rotationTransform.Rotation = _quaternionRotation;
            var xfGroup = new Transform3DGroup { Children = { _scaleTransform, _rotationTransform } };

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

            if (_frontModel != null)
                _frontModel.Visual = null;

            // Interactive frontside Visual3D
            _frontModel = new Viewport2DVisual3D
            {
                Geometry = simpleQuad,
                Visual = _logicalChild,
                Material = frontMaterial,
                Transform = xfGroup
            };

            // Cache the brush in the VP2V3 by setting caching on it.  Big perf wins.
            SetCachingForObject(_frontModel);

            // Scene consists of both the above Visual3D's.
            _viewport3D = new Viewport3D { ClipToBounds = false, Children = { mv3d, _frontModel } };

            UpdateRotation();

            return _viewport3D;
        }

        private void SetCachingForObject(DependencyObject d)
        {
            RenderOptions.SetCachingHint(d, CachingHint.Cache);
            RenderOptions.SetCacheInvalidationThresholdMinimum(d, 0.5);
            RenderOptions.SetCacheInvalidationThresholdMaximum(d, 2.0);
        }

        private void UpdateRotation()
        {
            var qx = new Quaternion(XAxis, RotationX);
            var qy = new Quaternion(YAxis, RotationY);
            var qz = new Quaternion(ZAxis, RotationZ);

            _quaternionRotation.Quaternion = qx * qy * qz;
        }

        private void Update3D()
        {
            // Use GetDescendantBounds for sizing and centering since DesiredSize includes layout whitespace, whereas GetDescendantBounds 
            // is tighter
            Rect logicalBounds = VisualTreeHelper.GetDescendantBounds(_logicalChild);
            double w = logicalBounds.Width;
            double h = logicalBounds.Height;

            // Create a camera that looks down -Z, with up as Y, and positioned right halfway in X and Y on the element, 
            // and back along Z the right distance based on the field-of-view is the same projected size as the 2D content
            // that it's looking at.  See http://blogs.msdn.com/greg_schechter/archive/2007/04/03/camera-construction-in-parallaxui.aspx
            // for derivation of this camera.
            double fovInRadians = FieldOfView * (Math.PI / 180);
            double zValue = w / Math.Tan(fovInRadians / 2) / 2;
            _viewport3D.Camera = new PerspectiveCamera(new Point3D(w / 2, h / 2, zValue), -ZAxis, YAxis, FieldOfView);

            _scaleTransform.ScaleX = w;
            _scaleTransform.ScaleY = h;
            _rotationTransform.CenterX = w / 2;
            _rotationTransform.CenterY = h / 2;
        }
    }
}
