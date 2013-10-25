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

            Point p1 = new Point(rightHip.Position.X * 250 + 260, rightHip.Position.Y * -250 + 250);
            Point p2 = new Point(leftHip.Position.X * 250 + 240, leftHip.Position.Y * -250 + 250);
            Line line = new Line() { X1 = p1.X, Y1 = p1.Y, X2 = p2.X, Y2 = p2.Y, Stroke = Brushes.Red, StrokeThickness = 3 };
            Point p3;
            ArrowLine directionLine = new ArrowLine();

            if (rightHip.Position.Y < range.minimum)
            {
                p3 = new Point(leftHip.Position.X * 250 + 240, leftHip.Position.Y * -250 + 200);
                directionLine = new ArrowLine() { X1 = p3.X, Y1 = p3.Y, X2 = p2.X, Y2 = p2.Y, Stroke = Brushes.Red, StrokeThickness = 2 };
                canvas.Children.Add(directionLine);
                canvas.Children.Add(line);
            }
            else if (rightHip.Position.Y > range.maximum)
            {
                p3 = new Point(rightHip.Position.X * 250 + 260, rightHip.Position.Y * -250 + 200);
                directionLine = new ArrowLine() { X1 = p3.X, Y1 = p3.Y, X2 = p1.X, Y2 = p1.Y, Stroke = Brushes.Red, StrokeThickness = 2 };
                canvas.Children.Add(directionLine);
                canvas.Children.Add(line);
            }
        }

        public void hipAlignmentZAxis()
        {
            Joint leftHip = skeleton.Joints[JointType.HipLeft];
            Joint rightHip = skeleton.Joints[JointType.HipRight];
            Range range = new Range(leftHip.Position.Z, Range.hipIntermediateRange);

            Point p1 = new Point(rightHip.Position.X * 250 + 260, rightHip.Position.Y * -250 + 250);
            Point p2 = new Point(leftHip.Position.X * 250 + 240, leftHip.Position.Y * -250 + 250);
            Line line = new Line() { X1 = p1.X, Y1 = p1.Y, X2 = p2.X, Y2 = p2.Y, Stroke = Brushes.Red, StrokeThickness = 3 };
            Point p3;
            ArrowLine directionLine = new ArrowLine();
            if (rightHip.Position.Z > range.maximum)
            {
                p3 = new Point(rightHip.Position.X * 250 + 240, rightHip.Position.Y * -250 + 280);
                directionLine = new ArrowLine() { X1 = p1.X, Y1 = p1.Y, X2 = p3.X, Y2 = p3.Y, Stroke = Brushes.Yellow, StrokeThickness = 2 };
                canvas.Children.Add(directionLine);
                canvas.Children.Add(line);
            }
            else if (rightHip.Position.Z < range.minimum)
            {
                p3 = new Point(leftHip.Position.X * 250 + 260, leftHip.Position.Y * -250 + 280);
                directionLine = new ArrowLine() { X1 = p2.X, Y1 = p2.Y, X2 = p3.X, Y2 = p3.Y, Stroke = Brushes.Yellow, StrokeThickness = 2 };
                canvas.Children.Add(directionLine);
                canvas.Children.Add(line);
            }
        }

        public void spineAlignmentYAxis()
        {
            TextBlock text = new TextBlock();
            text.FontSize = 25;
            text.Foreground = Brushes.WhiteSmoke;
            Canvas.SetTop(text, 250);
            Canvas.SetLeft(text, 100);

            Joint centerHip = skeleton.Joints[JointType.HipCenter];
            Joint centerShoulder = skeleton.Joints[JointType.ShoulderCenter];
            Range hipsRange = new Range(centerShoulder.Position.Z, Range.hipsToShouldersIntermediateRange);
            if (centerHip.Position.Z > hipsRange.maximum)
            {
                text.Inlines.Add("Your CABOOSE is sticking out");
                text.Background = Brushes.DarkCyan;
            }
            else if (centerHip.Position.Z < hipsRange.minimum)
            {
                text.Inlines.Add("Your HIPS are in front of your shoulders");
                text.Background = Brushes.Purple;
            }
            canvas.Children.Add(text);
        }

        public void shoulderAlignmentYAxis()
        {
            TextBlock text = new TextBlock();
            text.FontSize = 25;
            text.Foreground = Brushes.WhiteSmoke;
            Canvas.SetTop(text, 350);
            Canvas.SetLeft(text, 400);

            Joint leftShoulder = (skeleton.Joints[JointType.ShoulderLeft]);
            Joint rightShoulder = (skeleton.Joints[JointType.ShoulderRight]);
            Range range = new Range(leftShoulder.Position.Y, Range.shoulderYIntermediateRange);
            if (rightShoulder.Position.Y < range.minimum)
            {
                text.Text = "Your RIGHT SHOULDER is LOWER than your left";
                text.Background = Brushes.DarkCyan;
            }
            else if (rightShoulder.Position.Y > range.maximum)
            {
                text.Text = "Your LEFT SHOULDER is LOWER than your right";
                text.Background = Brushes.Purple;
            }
            canvas.Children.Add(text);
        }

        public void shoulderAlignmentZAxis()
        {
            TextBlock text = new TextBlock();
            text.FontSize = 25;
            text.Foreground = Brushes.WhiteSmoke;
            Canvas.SetTop(text, 450);
            Canvas.SetLeft(text, 100);

            Joint leftShoulder = skeleton.Joints[JointType.ShoulderLeft];
            Joint rightShoulder = skeleton.Joints[JointType.ShoulderRight];
            Range range = new Range(leftShoulder.Position.Z, Range.shoulderIntermediateRange);
            if (rightShoulder.Position.Z > range.maximum)
            {
                text.Inlines.Add("Your LEFT SHOULDER is in front of your right");
                text.Background = Brushes.DarkCyan;
            }
            else if (rightShoulder.Position.Z < range.minimum)
            {
                text.Inlines.Add("Your RIGHT SHOULDER is in front of your left");
                text.Background = Brushes.Purple;
            }
            canvas.Children.Add(text);
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