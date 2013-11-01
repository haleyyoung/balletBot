using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Kinect;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls;
using Petzold.Media2D;

namespace kinectTutorial
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public class MovementMode
    {
        public Canvas canvas;
        public Skeleton skeleton;
        private SolidColorBrush correctionLineColor = Brushes.Red;
        private SolidColorBrush frontBackColor = Brushes.Yellow;
        private SolidColorBrush sideSideColor = Brushes.Orange;
        private SolidColorBrush upDownColor = Brushes.Green;

        public MovementMode(Canvas canvas, Skeleton skeleton)
        {
            this.canvas = canvas;
            this.skeleton = skeleton;
        }

        public void plies()
        {
            if (skeleton != null)
            {
                hipAlignmentYAxis();
                hipAlignmentZAxis();
                spineAlignmentYAxis();
                shoulderAlignmentYAxis();
                shoulderAlignmentZAxis();
            }
        }

        public void hipAlignmentYAxis()
        {
            Joint leftHip = (skeleton.Joints[JointType.HipLeft]);
            Joint rightHip = (skeleton.Joints[JointType.HipRight]);
            Range range = new Range(leftHip.Position.Y, Range.hipIntermediateRange);

            Point p1 = new Point(rightHip.Position.X * 250 + 250, rightHip.Position.Y * -250 + 250);
            Point p2 = new Point(leftHip.Position.X * 250 + 250, leftHip.Position.Y * -250 + 250);
            Line line = new Line() { X1 = p1.X, Y1 = p1.Y, X2 = p2.X, Y2 = p2.Y, Stroke = correctionLineColor, StrokeThickness = 3 };
            Point p3;
            ArrowLine directionLine = new ArrowLine();

            if (rightHip.Position.Y < range.minimum)
            {
                p3 = new Point(leftHip.Position.X * 250 + 250, leftHip.Position.Y * -250 + 300);
                directionLine = new ArrowLine() { X1 = p2.X, Y1 = p2.Y, X2 = p3.X, Y2 = p3.Y, Stroke = upDownColor, StrokeThickness = 2 };
                canvas.Children.Add(line);
                canvas.Children.Add(directionLine);
            }
            else if (rightHip.Position.Y > range.maximum)
            {
                p3 = new Point(rightHip.Position.X * 250 + 250, rightHip.Position.Y * -250 + 300);
                directionLine = new ArrowLine() { X1 = p1.X, Y1 = p1.Y, X2 = p3.X, Y2 = p3.Y, Stroke = upDownColor, StrokeThickness = 2 };
                canvas.Children.Add(line);
                canvas.Children.Add(directionLine);
            }
        }

        public void hipAlignmentZAxis()
        {
            Joint leftHip = skeleton.Joints[JointType.HipLeft];
            Joint rightHip = skeleton.Joints[JointType.HipRight];
            Range range = new Range(leftHip.Position.Z, Range.hipIntermediateRange);

            Point p1 = new Point(rightHip.Position.X * 250 + 250, rightHip.Position.Y * -250 + 250);
            Point p2 = new Point(leftHip.Position.X * 250 + 250, leftHip.Position.Y * -250 + 250);
            Line line = new Line() { X1 = p1.X, Y1 = p1.Y, X2 = p2.X, Y2 = p2.Y, Stroke = correctionLineColor, StrokeThickness = 3 };
            Point p3;
            ArrowLine directionLine = new ArrowLine();
            if (rightHip.Position.Z > range.maximum)
            {
                p3 = new Point(rightHip.Position.X * 250 + 250, rightHip.Position.Y * -250 + 300);
                directionLine = new ArrowLine() { X1 = p1.X, Y1 = p1.Y, X2 = p3.X, Y2 = p3.Y, Stroke = frontBackColor, StrokeThickness = 2 };
                canvas.Children.Add(line);
                canvas.Children.Add(directionLine);
            }
            else if (rightHip.Position.Z < range.minimum)
            {
                p3 = new Point(leftHip.Position.X * 250 + 250, leftHip.Position.Y * -250 + 300);
                directionLine = new ArrowLine() { X1 = p2.X, Y1 = p2.Y, X2 = p3.X, Y2 = p3.Y, Stroke = frontBackColor, StrokeThickness = 2 };
                canvas.Children.Add(line);
                canvas.Children.Add(directionLine);
            }
        }

        public void spineAlignmentYAxis()
        {
            Joint centerHip = skeleton.Joints[JointType.HipCenter];
            Joint centerShoulder = skeleton.Joints[JointType.ShoulderCenter];
            Range hipsRange = new Range(centerShoulder.Position.Z, Range.hipsToShouldersIntermediateRange);

            Point p1 = new Point(centerHip.Position.X * 250 + 250, centerHip.Position.Y * -250 + 250);
            Point p2 = new Point(centerShoulder.Position.X * 250 + 250, centerShoulder.Position.Y * -250 + 250);
            Line line = new Line() { X1 = p1.X, Y1 = p1.Y, X2 = p2.X, Y2 = p2.Y, Stroke = correctionLineColor, StrokeThickness = 3 };
            Point p3;
            ArrowLine directionLine = new ArrowLine();

            if (centerHip.Position.Z > hipsRange.maximum)
            {
                p3 = new Point(centerShoulder.Position.X * 250 + 250, centerShoulder.Position.Y * -250 + 200);
                directionLine = new ArrowLine() { X1 = p2.X, Y1 = p2.Y, X2 = p3.X, Y2 = p3.Y, Stroke = frontBackColor, StrokeThickness = 2 };
                canvas.Children.Add(line);
                canvas.Children.Add(directionLine);
            }
            else if (centerHip.Position.Z < hipsRange.minimum)
            {
                p3 = new Point(centerShoulder.Position.X * 250 + 250, centerShoulder.Position.Y * -250 + 300);
                directionLine = new ArrowLine() { X1 = p2.X, Y1 = p2.Y, X2 = p3.X, Y2 = p3.Y, Stroke = frontBackColor, StrokeThickness = 2 };
                canvas.Children.Add(line);
                canvas.Children.Add(directionLine);
            }
        }

        public void shoulderAlignmentYAxis()
        {
            Joint leftShoulder = (skeleton.Joints[JointType.ShoulderLeft]);
            Joint rightShoulder = (skeleton.Joints[JointType.ShoulderRight]);
            Range range = new Range(leftShoulder.Position.Y, Range.shoulderYIntermediateRange);

            Point p1 = new Point(leftShoulder.Position.X * 250 + 250, leftShoulder.Position.Y * -250 + 250);
            Point p2 = new Point(rightShoulder.Position.X * 250 + 250, rightShoulder.Position.Y * -250 + 250);
            Line line = new Line() { X1 = p1.X, Y1 = p1.Y, X2 = p2.X, Y2 = p2.Y, Stroke = correctionLineColor, StrokeThickness = 3 };
            Point p3;
            ArrowLine directionLine = new ArrowLine();

            if (rightShoulder.Position.Y < range.minimum)
            {
                p3 = new Point(leftShoulder.Position.X * 250 + 250, leftShoulder.Position.Y * -250 + 300);
                directionLine = new ArrowLine() { X1 = p1.X, Y1 = p1.Y, X2 = p3.X, Y2 = p3.Y, Stroke = upDownColor, StrokeThickness = 2 };
                canvas.Children.Add(line);
                canvas.Children.Add(directionLine);
            }
            else if (rightShoulder.Position.Y > range.maximum)
            {
                p3 = new Point(rightShoulder.Position.X * 250 + 250, rightShoulder.Position.Y * -250 + 300);
                directionLine = new ArrowLine() { X1 = p2.X, Y1 = p2.Y, X2 = p3.X, Y2 = p3.Y, Stroke = upDownColor, StrokeThickness = 2 };
                canvas.Children.Add(line);
                canvas.Children.Add(directionLine);
            }
        }

        public void shoulderAlignmentZAxis()
        {
            Joint leftShoulder = skeleton.Joints[JointType.ShoulderLeft];
            Joint rightShoulder = skeleton.Joints[JointType.ShoulderRight];
            Range range = new Range(leftShoulder.Position.Z, Range.shoulderIntermediateRange);

            Point p1 = new Point(leftShoulder.Position.X * 250 + 250, leftShoulder.Position.Y * -250 + 250);
            Point p2 = new Point(rightShoulder.Position.X * 250 + 250, rightShoulder.Position.Y * -250 + 250);
            Line line = new Line() { X1 = p1.X, Y1 = p1.Y, X2 = p2.X, Y2 = p2.Y, Stroke = correctionLineColor, StrokeThickness = 3 };
            Point p3;
            ArrowLine directionLine = new ArrowLine();

            if (rightShoulder.Position.Z > range.maximum)
            {
                p3 = new Point(leftShoulder.Position.X * 250 + 250, leftShoulder.Position.Y * -250 + 200);
                directionLine = new ArrowLine() { X1 = p1.X, Y1 = p1.Y, X2 = p3.X, Y2 = p3.Y, Stroke = frontBackColor, StrokeThickness = 2 };
                canvas.Children.Add(line);
                canvas.Children.Add(directionLine);
            }
            else if (rightShoulder.Position.Z < range.minimum)
            {
                p3 = new Point(rightShoulder.Position.X * 250 + 250, rightShoulder.Position.Y * -250 + 300);
                directionLine = new ArrowLine() { X1 = p2.X, Y1 = p2.Y, X2 = p3.X, Y2 = p3.Y, Stroke = frontBackColor, StrokeThickness = 2 };
                canvas.Children.Add(line);
                canvas.Children.Add(directionLine);
            }
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

        public float minimum;
        public float maximum;

        public Range(float rangeBasis, float level)
        {
            this.minimum = rangeBasis - level;
            this.maximum = rangeBasis + level;
        }
    }
}