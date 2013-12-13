using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Kinect;

namespace balletBot
{
    /// <summary>
    /// This partial class contains all the helper methods for drawing out skeletons
    /// on the MainWindow canvas as well as tracking skeletons. This partial class is
    /// mostly used for visual elements of the application rather than functional
    /// elements.
    /// </summary>
    public partial class MainWindow: Window
    {
        public static int jointDiameter = 10;
        int boneThickness = 7;
        Brush jointColor = Brushes.BlueViolet;
        Brush boneFrontColor = Brushes.MediumOrchid;
        Brush boneSideColor = Brushes.CornflowerBlue;

        public static double jointRadius = jointDiameter / 2;
        public static double windowHeightRatio = 1.25;
        public static double windowWidthRatio = 4;

        private void DrawSkeleton(Skeleton skeleton, int view)
        {
            // Sadly, the human body does not have an Euler Path, so this is the most efficient
            // (least redundant) path to draw the body
            Joint[] topOfBodyDrawOrder = {
              skeleton.Joints[JointType.HandLeft],
              skeleton.Joints[JointType.WristLeft],
              skeleton.Joints[JointType.ElbowLeft],
              skeleton.Joints[JointType.ShoulderLeft],
              skeleton.Joints[JointType.ShoulderCenter],
              skeleton.Joints[JointType.Head],
              skeleton.Joints[JointType.ShoulderCenter],
              skeleton.Joints[JointType.ShoulderRight],
              skeleton.Joints[JointType.ElbowRight],
              skeleton.Joints[JointType.WristRight],
              skeleton.Joints[JointType.HandRight]
            };

            Joint[] bottomOfBodyDrawOrder = {
              skeleton.Joints[JointType.FootLeft],
              skeleton.Joints[JointType.AnkleLeft],
              skeleton.Joints[JointType.KneeLeft],
              skeleton.Joints[JointType.HipLeft],
              skeleton.Joints[JointType.HipCenter],
              skeleton.Joints[JointType.Spine],
              skeleton.Joints[JointType.ShoulderCenter],
              skeleton.Joints[JointType.Spine],
              skeleton.Joints[JointType.HipCenter],
              skeleton.Joints[JointType.HipRight],
              skeleton.Joints[JointType.KneeRight],
              skeleton.Joints[JointType.AnkleRight],
              skeleton.Joints[JointType.FootRight]
            };

            // Draw the joints
            foreach (Joint joint in skeleton.Joints)
            {
                if (jointTrackedOrInfered(joint))
                {
                    if (view == FRONT_VIEW)
                    {
                        DrawJointFrontView(joint);
                    }
                    else if (view == SIDE_VIEW)
                    {
                        DrawJointSideView(joint);
                    }
                }
            }

            // Draw the bones
            for (int i = 0; i < topOfBodyDrawOrder.Length - 1; i++)
            {
                if (jointTrackedOrInfered(topOfBodyDrawOrder[i]) &&
                    jointTrackedOrInfered(topOfBodyDrawOrder[i + 1])
                )
                {
                    if (view == FRONT_VIEW)
                    {
                        DrawBoneFrontView(topOfBodyDrawOrder[i], topOfBodyDrawOrder[i + 1]);
                    }
                    else if (view == SIDE_VIEW)
                    {
                        DrawBoneSideView(topOfBodyDrawOrder[i], topOfBodyDrawOrder[i + 1]);
                    }
                }
            }
            for (int i = 0; i < bottomOfBodyDrawOrder.Length - 1; i++)
            {
                if (jointTrackedOrInfered(bottomOfBodyDrawOrder[i]) &&
                    jointTrackedOrInfered(bottomOfBodyDrawOrder[i + 1])
                )
                {
                    if (view == FRONT_VIEW)
                    {
                        DrawBoneFrontView(bottomOfBodyDrawOrder[i], bottomOfBodyDrawOrder[i + 1]);
                    }
                    else if (view == SIDE_VIEW)
                    {
                        DrawBoneSideView(bottomOfBodyDrawOrder[i], bottomOfBodyDrawOrder[i + 1]);
                    }
                }
            }
        }

        private void DrawJointFrontView(Joint joint)
        {
            double windowWidth = Canvas.ActualWidth;
            double windowHeight = Canvas.ActualHeight;
            if (jointTrackedOrInfered(joint))
            {
                Ellipse frontJoint = new Ellipse() {Height = jointDiameter, Width = jointDiameter, Fill = jointColor};
                EllipseCanvas.Children.Add(frontJoint);
                Canvas.SetTop(frontJoint, (windowHeight / windowHeightRatio - jointRadius) + joint.Position.Y * -(windowHeight / 2));
                Canvas.SetLeft(frontJoint, joint.Position.X * (windowWidth / 2) + (windowWidth / 2 - jointRadius));
            }
        }

        private void DrawJointSideView(Joint joint)
        {
            double windowWidth = Canvas.ActualWidth;
            double windowHeight = Canvas.ActualHeight;
            if (jointTrackedOrInfered(joint))
            {
                Ellipse sideJoint = new Ellipse() {Height = jointDiameter, Width  = jointDiameter, Fill = jointColor};
                EllipseCanvas.Children.Add(sideJoint);
                Canvas.SetTop(sideJoint, joint.Position.Y * -(windowHeight / 2) + (windowHeight / windowHeightRatio - jointRadius));
                Canvas.SetLeft(sideJoint, joint.Position.Z * (windowWidth / windowWidthRatio) - jointRadius);
            }
        }

        private void DrawBoneFrontView(Joint start, Joint end)
        {
            double windowWidth = Canvas.ActualWidth;
            double windowHeight = Canvas.ActualHeight;
            if (jointTrackedOrInfered(start) &&
                jointTrackedOrInfered(end)
            )
            {
                Point p1 = new Point(start.Position.X * (windowWidth / 2) + (windowWidth / 2), (windowHeight / windowHeightRatio) + start.Position.Y * -(windowHeight / 2));
                Point p2 = new Point(end.Position.X * (windowWidth / 2) + (windowWidth / 2), (windowHeight / windowHeightRatio) + end.Position.Y * -(windowHeight / 2));
                Line frontBone = new Line() {X1 = p1.X, Y1 = p1.Y, X2 = p2.X, Y2 = p2.Y, Stroke = boneFrontColor, StrokeThickness = boneThickness};
                EllipseCanvas.Children.Add(frontBone);
            }
        }

        private void DrawBoneSideView(Joint start, Joint end)
        {
            double windowWidth = Canvas.ActualWidth;
            double windowHeight = Canvas.ActualHeight;
            if (jointTrackedOrInfered(start) &&
                jointTrackedOrInfered(end))
            {
                Point p1 = new Point(start.Position.Z * (windowWidth / windowWidthRatio), start.Position.Y * -(windowHeight / 2) + (windowHeight / windowHeightRatio));
                Point p2 = new Point(end.Position.Z * (windowWidth / windowWidthRatio), end.Position.Y * -(windowHeight / 2) + (windowHeight / windowHeightRatio));
                Line sideBone = new Line() { X1 = p1.X, Y1 = p1.Y, X2 = p2.X, Y2 = p2.Y, Stroke = boneSideColor, StrokeThickness = boneThickness};
                EllipseCanvas.Children.Add(sideBone);
            }
        }

        private void TrackClosestSkeleton()
        {
            if (this.kinect != null && this.kinect.SkeletonStream != null)
            {
                if (!this.kinect.SkeletonStream.AppChoosesSkeletons)
                {
                    this.kinect.SkeletonStream.AppChoosesSkeletons = true; // Ensure AppChoosesSkeletons is set
                }

                float closestDistance = 10000f; // Start with a far enough distance
                int closestID = 0;

                foreach (Skeleton skeleton in this.skeletonData.Where(s => s.TrackingState != SkeletonTrackingState.NotTracked))
                {
                    if (skeleton.Position.Z < closestDistance)
                    {
                        closestID = skeleton.TrackingId;
                        closestDistance = skeleton.Position.Z;
                    }
                }

                if (closestID > 0)
                {
                    this.kinect.SkeletonStream.ChooseSkeletons(closestID); // Track this skeleton
                }
            }
        }

        public Boolean jointTrackedOrInfered(Joint joint)
        {
            return (joint.TrackingState == JointTrackingState.Tracked ||
                joint.TrackingState == JointTrackingState.Inferred);
        }
    }
}
