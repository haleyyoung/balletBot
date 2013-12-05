using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Kinect;
using kinectTutorial;

namespace kinectTutorial
{
    class Position
    {
        public Boolean gestureComplete = false;
        public int positionBufferFrames = 0;
        public int positionCompleteBannerFrames = 0;
        public Skeleton skeleton = null;
        public Canvas canvas;
        public Range leftFootXRange;
        public Range leftFootYRange;
        public Range leftFootZRange;
        public Range rightFootXRange;
        public Range rightFootYRange;
        public Range rightFootZRange;

        public Position(Canvas canvas, Skeleton skeleton)
        {
            this.skeleton = skeleton;
            this.canvas = canvas;

            this.leftFootXRange = new Range(skeleton.Joints[JointType.FootLeft].Position.X, Range.hipEasyRange);
            this.leftFootYRange = new Range(skeleton.Joints[JointType.FootLeft].Position.Y, Range.hipEasyRange);
            this.leftFootZRange = new Range(skeleton.Joints[JointType.FootLeft].Position.Z, Range.hipEasyRange);
            this.rightFootXRange = new Range(skeleton.Joints[JointType.FootRight].Position.X, Range.hipEasyRange);
            this.rightFootYRange = new Range(skeleton.Joints[JointType.FootRight].Position.Y, Range.hipEasyRange);
            this.rightFootZRange = new Range(skeleton.Joints[JointType.FootRight].Position.Z, Range.hipEasyRange);

        }

        public Boolean firstPosition()
        {
            if (skeleton == null)
            {
                return false;
            }

            checkAlignment();

            TextBlock blah = new TextBlock();
            blah.Background = Brushes.Pink;
            blah.FontSize = 50;
            canvas.Children.Add(blah);
            Canvas.SetTop(blah, 300);
            Canvas.SetLeft(blah, 100);
            Joint rightAnkle = this.skeleton.Joints[JointType.AnkleRight];
            Joint leftAnkle = this.skeleton.Joints[JointType.AnkleLeft];
            Joint rightHip = this.skeleton.Joints[JointType.HipRight];
            Joint leftHip = this.skeleton.Joints[JointType.HipLeft];

            // Make sure feet are equally positioned on the floor (one foot isn't in front of the other)
            Range rightAnkleYComparison = new Range(rightAnkle.Position.Y, Range.hipEasyRange);
            Range rightAnkleZComparison = new Range(rightAnkle.Position.Z, Range.hipEasyRange);
            // Make sure feet are not wider than the hips
            Range rightHipXComparison = new Range(rightHip.Position.X, Range.hipEasyRange);
            Range leftHipXComparison = new Range(leftHip.Position.X, Range.hipEasyRange);

            blah.Text = "" + (rightAnkleYComparison.inRange(leftAnkle.Position.Y) +"\n"+
                    (rightAnkleZComparison.inRange(leftAnkle.Position.Z)) +"\n"+
                    (rightAnkle.Position.X <= rightHipXComparison.maximum) + "\n" +
                    (leftAnkle.Position.X >= leftHipXComparison.minimum) + "\n" +
                    rightFootStable() + "\n" +
                    leftFootStable() + "\n" +
                    rightFootTurnout() + "\n" +
                    leftFootTurnout());

            return (rightAnkleYComparison.inRange(leftAnkle.Position.Y) &&
                    rightAnkleZComparison.inRange(leftAnkle.Position.Z) &&
                    rightAnkle.Position.X <= rightHipXComparison.maximum &&
                    leftAnkle.Position.X >= leftHipXComparison.minimum &&
                    rightFootStable() &&
                    leftFootStable() &&
                    rightFootTurnout() &&
                    leftFootTurnout()
            );
        }

        public Boolean secondPosition()
        {
            return false;
        }

        public void checkAlignment()
        {
            // Alignment checks
            MovementMode position = new MovementMode(this.canvas, this.skeleton);
            position.hipAlignmentYAxis();
            position.hipAlignmentZAxis();
            position.spineAlignmentYAxis();
            position.shoulderAlignmentYAxis();
            position.shoulderAlignmentZAxis();
        }

        public Boolean leftFootStable()
        {
            TextBlock blah = new TextBlock();
            blah.Background = Brushes.Orange;
            canvas.Children.Add(blah);
            Canvas.SetTop(blah, 200);
            Joint leftFoot = this.skeleton.Joints[JointType.FootLeft];

            if (this.leftFootXRange.minimum <= leftFoot.Position.X &&
                this.leftFootXRange.maximum >= leftFoot.Position.X &&
                this.leftFootYRange.minimum <= leftFoot.Position.Y &&
                this.leftFootYRange.maximum >= leftFoot.Position.Y &&
                this.leftFootZRange.minimum <= leftFoot.Position.Z &&
                this.leftFootZRange.maximum >= leftFoot.Position.Z
            )
            {
                blah.Text = "stable";
                return true;
            }
            blah.Text = "left foot NOT stable";
            return false;
        }

        public Boolean rightFootStable()
        {
            TextBlock blah = new TextBlock();
            blah.Background = Brushes.Orange;
            canvas.Children.Add(blah);
            Canvas.SetTop(blah, 300);
            Joint rightFoot = this.skeleton.Joints[JointType.FootRight];

            if (this.rightFootXRange.minimum <= rightFoot.Position.X &&
                this.rightFootXRange.maximum >= rightFoot.Position.X &&
                this.rightFootYRange.minimum <= rightFoot.Position.Y &&
                this.rightFootYRange.maximum >= rightFoot.Position.Y &&
                this.rightFootZRange.minimum <= rightFoot.Position.Z &&
                this.rightFootZRange.maximum >= rightFoot.Position.Z
            )
            {
                blah.Text = "stable";
                return true;
            }
            blah.Text = "right foot NOT stable";
            return false;
        }

        public Boolean leftFootTurnout()
        {
            TextBlock blah = new TextBlock();
            blah.Background = Brushes.Orange;
            canvas.Children.Add(blah);
            Canvas.SetTop(blah, 400);
            Joint leftFoot = this.skeleton.Joints[JointType.FootLeft];
            Joint leftAnkle = this.skeleton.Joints[JointType.AnkleLeft];

            if (leftFoot.Position.X <= leftAnkle.Position.X)
            {
                blah.Text = "turned out";
                return true;
            }
            blah.Text = "left foot NOT turned out";
            return false;
        }

        public Boolean rightFootTurnout()
        {
            TextBlock blah = new TextBlock();
            blah.Background = Brushes.Orange;
            canvas.Children.Add(blah);
            Canvas.SetTop(blah, 500);
            Joint rightFoot = this.skeleton.Joints[JointType.FootRight];
            Joint rightAnkle = this.skeleton.Joints[JointType.AnkleRight];

            if (rightFoot.Position.X >= rightAnkle.Position.X)
            {
                blah.Text = "turned out";
                return true;
            }
            blah.Text = "right foot NOT turned out";
            return false;
        }

        public Boolean showSuccessBanner()
        {
            Image imageToShow = (Image)this.canvas.FindName("moveCompletedImage");
            TextBlock textToShow = (TextBlock)this.canvas.FindName("plieCompletedText");

            this.positionCompleteBannerFrames++;

            if (this.positionCompleteBannerFrames < 120)
            {
                imageToShow.Visibility = Visibility.Visible;
                textToShow.Visibility = Visibility.Visible;
            }
            else
            {
                imageToShow.Visibility = Visibility.Hidden;
                textToShow.Visibility = Visibility.Hidden;
                return true;
            }
            return false;
        }
    }
}
