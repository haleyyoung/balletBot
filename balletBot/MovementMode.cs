using System;
using Microsoft.Kinect;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls;
using Petzold.Media2D;

namespace balletBot
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public class MovementMode
    {
        public Canvas canvas;
        public Skeleton skeleton;
        private double windowWidth;
        private double windowHeight;
        private SolidColorBrush correctionLineColor = Brushes.Red;
        private int correctionLineThickness = 6;
        private int correctionArrowThickness = 4;
        private int correctionArrowLength = 50;
        private SolidColorBrush frontBackColor = Brushes.Yellow;
        private SolidColorBrush sideSideColor = Brushes.Orange;
        private SolidColorBrush upDownColor = Brushes.Green;

        public MovementMode(Canvas canvas, Skeleton skeleton)
        {
            this.canvas = canvas;
            this.skeleton = skeleton;
            this.windowWidth = canvas.ActualWidth;
            this.windowHeight = canvas.ActualHeight;
            this.canvas.Children.Add(drawKey());
        }

        public Canvas drawKey()
        {
            Canvas canvas = new Canvas();
            TextBlock upDownMovement = new TextBlock();
            upDownMovement.FontSize = 20;
            upDownMovement.Foreground = Brushes.WhiteSmoke;
            upDownMovement.Text = "Move up or down";
            TextBlock sideToSideMovement = new TextBlock();
            sideToSideMovement.FontSize = 20;
            sideToSideMovement.Foreground = Brushes.WhiteSmoke;
            sideToSideMovement.Text = "Move left or right";
            TextBlock forwardBackwardMovement = new TextBlock();
            forwardBackwardMovement.FontSize = 20;
            forwardBackwardMovement.Foreground = Brushes.WhiteSmoke;
            forwardBackwardMovement.Text = "Move front or back";

            Point p1 = new Point(windowWidth - 250, 10);
            Point p2 = new Point(windowWidth - 250, 30);
            ArrowLine downArrow = new ArrowLine() { X1 = p1.X, Y1 = p1.Y, X2 = p2.X, Y2 = p2.Y, Stroke = upDownColor, StrokeThickness = correctionArrowThickness };
            ArrowLine upArrow = new ArrowLine() { X1 = p2.X + 20, Y1 = p2.Y, X2 = p1.X + 20, Y2 = p1.Y, Stroke = upDownColor, StrokeThickness = correctionArrowThickness };
            Point p3 = new Point(windowWidth - 250, 50);
            Point p4 = new Point(windowWidth - 230, 50);
            ArrowLine rightArrow = new ArrowLine() { X1 = p3.X, Y1 = p3.Y, X2 = p4.X, Y2 = p4.Y, Stroke = sideSideColor, StrokeThickness = correctionArrowThickness };
            ArrowLine leftArrow = new ArrowLine() { X1 = p4.X + 30, Y1 = p4.Y, X2 = p3.X + 30, Y2 = p3.Y, Stroke = sideSideColor, StrokeThickness = correctionArrowThickness };
            Point p5 = new Point(windowWidth - 250, 80);
            Point p6 = new Point(windowWidth - 230, 100);
            ArrowLine frontArrow = new ArrowLine() { X1 = p5.X, Y1 = p5.Y, X2 = p6.X, Y2 = p6.Y, Stroke = frontBackColor, StrokeThickness = correctionArrowThickness };
            ArrowLine backArrow = new ArrowLine() { X1 = p6.X + 30, Y1 = p6.Y, X2 = p5.X + 30, Y2 = p5.Y, Stroke = frontBackColor, StrokeThickness = correctionArrowThickness };

            canvas.Children.Add(upDownMovement);
            canvas.Children.Add(sideToSideMovement);
            canvas.Children.Add(forwardBackwardMovement);
            canvas.Children.Add(downArrow);
            canvas.Children.Add(upArrow);
            canvas.Children.Add(rightArrow);
            canvas.Children.Add(leftArrow);
            canvas.Children.Add(frontArrow);
            canvas.Children.Add(backArrow);
            Canvas.SetTop(upDownMovement, 10);
            Canvas.SetLeft(upDownMovement, windowWidth - 190);
            Canvas.SetTop(sideToSideMovement, 40);
            Canvas.SetLeft(sideToSideMovement, windowWidth - 190);
            Canvas.SetTop(forwardBackwardMovement, 80);
            Canvas.SetLeft(forwardBackwardMovement, windowWidth - 190);

            return canvas;
        }

        public void hipAlignmentYAxis()
        {
            Joint leftHip = (skeleton.Joints[JointType.HipLeft]);
            Joint rightHip = (skeleton.Joints[JointType.HipRight]);
            Range range = new Range(leftHip.Position.Y, Range.hipIntermediateRange);

            Point p1 = new Point(rightHip.Position.X * (windowWidth / 2) + (windowWidth / 2), rightHip.Position.Y * -(windowHeight / 2) + (windowHeight / MainWindow.windowHeightRatio));
            Point p2 = new Point(leftHip.Position.X * (windowWidth / 2) + (windowWidth / 2), leftHip.Position.Y * -(windowHeight / 2) + (windowHeight / MainWindow.windowHeightRatio));
            Line line = new Line() { X1 = p1.X, Y1 = p1.Y, X2 = p2.X, Y2 = p2.Y, Stroke = correctionLineColor, StrokeThickness = correctionLineThickness };
            Point p3;
            ArrowLine directionLine = new ArrowLine();

            if (rightHip.Position.Y < range.minimum)
            {
                p3 = new Point(leftHip.Position.X * (windowWidth / 2) + (windowWidth / 2), leftHip.Position.Y * -(windowHeight / 2) + (windowHeight / MainWindow.windowHeightRatio + correctionArrowLength));
                directionLine = new ArrowLine() { X1 = p2.X, Y1 = p2.Y, X2 = p3.X, Y2 = p3.Y, Stroke = upDownColor, StrokeThickness = correctionLineThickness };
                canvas.Children.Add(line);
                canvas.Children.Add(directionLine);
            }
            else if (rightHip.Position.Y > range.maximum)
            {
                p3 = new Point(rightHip.Position.X * (windowWidth / 2) + (windowWidth / 2), rightHip.Position.Y * -(windowHeight / 2) + (windowHeight / MainWindow.windowHeightRatio + correctionArrowLength));
                directionLine = new ArrowLine() { X1 = p1.X, Y1 = p1.Y, X2 = p3.X, Y2 = p3.Y, Stroke = upDownColor, StrokeThickness = correctionLineThickness };
                canvas.Children.Add(line);
                canvas.Children.Add(directionLine);
            }
        }

        public void hipAlignmentZAxis()
        {
            Joint leftHip = skeleton.Joints[JointType.HipLeft];
            Joint rightHip = skeleton.Joints[JointType.HipRight];
            Range range = new Range(leftHip.Position.Z, Range.hipIntermediateRange);

            Point p1 = new Point(rightHip.Position.X * (windowWidth / 2) + (windowWidth / 2), rightHip.Position.Y * -(windowHeight / 2) + (windowHeight / MainWindow.windowHeightRatio));
            Point p2 = new Point(leftHip.Position.X * (windowWidth / 2) + (windowWidth / 2), leftHip.Position.Y * -(windowHeight / 2) + (windowHeight / MainWindow.windowHeightRatio));
            Line line = new Line() { X1 = p1.X, Y1 = p1.Y, X2 = p2.X, Y2 = p2.Y, Stroke = correctionLineColor, StrokeThickness = correctionLineThickness };
            Point p3;
            ArrowLine directionLine = new ArrowLine();
            if (rightHip.Position.Z > range.maximum)
            {
                p3 = new Point(rightHip.Position.X * (windowWidth / 2) + (windowWidth / 2), rightHip.Position.Y * -(windowHeight / 2) + (windowHeight / MainWindow.windowHeightRatio + correctionArrowLength));
                directionLine = new ArrowLine() { X1 = p1.X, Y1 = p1.Y, X2 = p3.X, Y2 = p3.Y, Stroke = frontBackColor, StrokeThickness = correctionArrowThickness };
                canvas.Children.Add(line);
                canvas.Children.Add(directionLine);
            }
            else if (rightHip.Position.Z < range.minimum)
            {
                p3 = new Point(leftHip.Position.X * (windowWidth / 2) + (windowWidth / 2), leftHip.Position.Y * -(windowHeight / 2) + (windowHeight / MainWindow.windowHeightRatio + correctionArrowLength));
                directionLine = new ArrowLine() { X1 = p2.X, Y1 = p2.Y, X2 = p3.X, Y2 = p3.Y, Stroke = frontBackColor, StrokeThickness = correctionArrowThickness };
                canvas.Children.Add(line);
                canvas.Children.Add(directionLine);
            }
        }

        public void spineAlignmentYAxis()
        {
            Joint centerHip = skeleton.Joints[JointType.HipCenter];
            Joint centerShoulder = skeleton.Joints[JointType.ShoulderCenter];
            Range hipsRange = new Range(centerShoulder.Position.Z, Range.hipsToShouldersIntermediateRange);

            Point p1 = new Point(centerHip.Position.X * (windowWidth / 2) + (windowWidth / 2), centerHip.Position.Y * -(windowHeight / 2) + (windowHeight / MainWindow.windowHeightRatio));
            Point p2 = new Point(centerShoulder.Position.X * (windowWidth / 2) + (windowWidth / 2), centerShoulder.Position.Y * -(windowHeight / 2) + (windowHeight / MainWindow.windowHeightRatio));
            Line line = new Line() { X1 = p1.X, Y1 = p1.Y, X2 = p2.X, Y2 = p2.Y, Stroke = correctionLineColor, StrokeThickness = correctionLineThickness };
            Point p3;
            ArrowLine directionLine = new ArrowLine();

            if (centerHip.Position.Z > hipsRange.maximum)
            {
                p3 = new Point(centerShoulder.Position.X * (windowWidth / 2) + (windowWidth / 2), centerShoulder.Position.Y * -(windowHeight / 2) + (windowHeight / MainWindow.windowHeightRatio - correctionArrowLength));
                directionLine = new ArrowLine() { X1 = p2.X, Y1 = p2.Y, X2 = p3.X, Y2 = p3.Y, Stroke = frontBackColor, StrokeThickness = correctionArrowThickness };
                canvas.Children.Add(line);
                canvas.Children.Add(directionLine);
            }
            else if (centerHip.Position.Z < hipsRange.minimum)
            {
                p3 = new Point(centerShoulder.Position.X * (windowWidth / 2) + (windowWidth / 2), centerShoulder.Position.Y * -(windowHeight / 2) + (windowHeight / MainWindow.windowHeightRatio + correctionArrowLength));
                directionLine = new ArrowLine() { X1 = p2.X, Y1 = p2.Y, X2 = p3.X, Y2 = p3.Y, Stroke = frontBackColor, StrokeThickness = correctionArrowThickness };
                canvas.Children.Add(line);
                canvas.Children.Add(directionLine);
            }
        }

        public void shoulderAlignmentYAxis()
        {
            Joint leftShoulder = (skeleton.Joints[JointType.ShoulderLeft]);
            Joint rightShoulder = (skeleton.Joints[JointType.ShoulderRight]);
            Range range = new Range(leftShoulder.Position.Y, Range.shoulderYIntermediateRange);

            Point p1 = new Point(leftShoulder.Position.X * (windowWidth / 2) + (windowWidth / 2), leftShoulder.Position.Y * -(windowHeight / 2) + (windowHeight / MainWindow.windowHeightRatio));
            Point p2 = new Point(rightShoulder.Position.X * (windowWidth / 2) + (windowWidth / 2), rightShoulder.Position.Y * -(windowHeight / 2) + (windowHeight / MainWindow.windowHeightRatio));
            Line line = new Line() { X1 = p1.X, Y1 = p1.Y, X2 = p2.X, Y2 = p2.Y, Stroke = correctionLineColor, StrokeThickness = correctionLineThickness };
            Point p3;
            ArrowLine directionLine = new ArrowLine();

            if (rightShoulder.Position.Y < range.minimum)
            {
                p3 = new Point(leftShoulder.Position.X * (windowWidth / 2) + (windowWidth / 2), leftShoulder.Position.Y * -(windowHeight / 2) + (windowHeight / MainWindow.windowHeightRatio + correctionArrowLength));
                directionLine = new ArrowLine() { X1 = p1.X, Y1 = p1.Y, X2 = p3.X, Y2 = p3.Y, Stroke = upDownColor, StrokeThickness = correctionArrowThickness };
                canvas.Children.Add(line);
                canvas.Children.Add(directionLine);
            }
            else if (rightShoulder.Position.Y > range.maximum)
            {
                p3 = new Point(rightShoulder.Position.X * (windowWidth / 2) + (windowWidth / 2), rightShoulder.Position.Y * -(windowHeight / 2) + (windowHeight / MainWindow.windowHeightRatio + correctionArrowLength));
                directionLine = new ArrowLine() { X1 = p2.X, Y1 = p2.Y, X2 = p3.X, Y2 = p3.Y, Stroke = upDownColor, StrokeThickness = correctionArrowThickness };
                canvas.Children.Add(line);
                canvas.Children.Add(directionLine);
            }
        }

        public void shoulderAlignmentZAxis()
        {
            Joint leftShoulder = skeleton.Joints[JointType.ShoulderLeft];
            Joint rightShoulder = skeleton.Joints[JointType.ShoulderRight];
            Range range = new Range(leftShoulder.Position.Z, Range.shoulderIntermediateRange);

            Point p1 = new Point(leftShoulder.Position.X * (windowWidth / 2) + (windowWidth / 2), leftShoulder.Position.Y * -(windowHeight / 2) + (windowHeight / MainWindow.windowHeightRatio));
            Point p2 = new Point(rightShoulder.Position.X * (windowWidth / 2) + (windowWidth / 2), rightShoulder.Position.Y * -(windowHeight / 2) + (windowHeight / MainWindow.windowHeightRatio));
            Line line = new Line() { X1 = p1.X, Y1 = p1.Y, X2 = p2.X, Y2 = p2.Y, Stroke = correctionLineColor, StrokeThickness = correctionLineThickness };
            Point p3;
            ArrowLine directionLine = new ArrowLine();

            if (rightShoulder.Position.Z > range.maximum)
            {
                p3 = new Point(leftShoulder.Position.X * (windowWidth / 2) + (windowWidth / 2), leftShoulder.Position.Y * -(windowHeight / 2) + (windowHeight / MainWindow.windowHeightRatio - correctionArrowLength));
                directionLine = new ArrowLine() { X1 = p1.X, Y1 = p1.Y, X2 = p3.X, Y2 = p3.Y, Stroke = frontBackColor, StrokeThickness = correctionArrowThickness };
                canvas.Children.Add(line);
                canvas.Children.Add(directionLine);
            }
            else if (rightShoulder.Position.Z < range.minimum)
            {
                p3 = new Point(rightShoulder.Position.X * (windowWidth / 2) + (windowWidth / 2), rightShoulder.Position.Y * -(windowHeight / 2) + (windowHeight / MainWindow.windowHeightRatio + correctionArrowLength));
                directionLine = new ArrowLine() { X1 = p2.X, Y1 = p2.Y, X2 = p3.X, Y2 = p3.Y, Stroke = frontBackColor, StrokeThickness = correctionArrowThickness };
                canvas.Children.Add(line);
                canvas.Children.Add(directionLine);
            }
        }

        public Boolean turnoutCheck()
        {
            Joint leftAnkle = skeleton.Joints[JointType.AnkleLeft];
            Joint leftFoot = skeleton.Joints[JointType.FootLeft];
            Joint rightAnkle = skeleton.Joints[JointType.AnkleRight];
            Joint rightFoot = skeleton.Joints[JointType.FootRight];

            if (rightFoot.Position.X <= rightAnkle.Position.X &&
                leftFoot.Position.X >= leftAnkle.Position.X)
            {
                return true;
            }
            return false;
        }
    }

    public class Range
    {
        // Hips y-axis and z-axis alignment
        public const float hipEasyRange = 0.005f;
        public const float hipIntermediateRange = 0.003f;
        public const float hipHardRange = 0.001f;

        // Hips to shoulders y-axis alignment
        public const float hipsToShouldersEasyRange = 0.05f;
        public const float hipsToShouldersIntermediateRange = 0.01f;
        public const float hipsToShouldersHardRange = 0.005f;

        // Shoulders y-axis alignment
        public const float shoulderYEasyRange = 0.035f;
        public const float shoulderYIntermediateRange = 0.001f;
        public const float shoulderYHardRange = 0.0005f;

        // Shoulders z-axis alignment
        public const float shoulderEasyRange = 0.005f;
        public const float shoulderIntermediateRange = 0.003f;
        public const float shoulderHardRange = 0.001f;

        // Knees y-axis alignment
        public const float kneeEasyRange = 0.001f;
        public const float kneeIntermediateRange = 0.003f;

        // Feet x-axis alignment
        public const float footEasyRange = 0.005f;

        // Ankles x-axis alignment
        public const float ankleEasyRange = 0.005f;

        public float minimum;
        public float maximum;

        public Range(float rangeBasis, float level)
        {
            this.minimum = rangeBasis - level;
            this.maximum = rangeBasis + level;
        }

        public Boolean inRange(float position)
        {
            return (position >= this.minimum && position <= this.maximum);
        }
    }
}