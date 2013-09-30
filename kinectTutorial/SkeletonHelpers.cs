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
    public partial class MainWindow: Window
    {
        private void DrawSkeleton(Skeleton skeleton, int view)
        {
            foreach (Joint joint in skeleton.Joints)
            {
                if (joint.TrackingState == JointTrackingState.Tracked)
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
            // TODO find a way to efficiently check if these are tracked
            if (view == FRONT_VIEW)
            {
                DrawBoneFrontView(skeleton.Joints[JointType.Head], skeleton.Joints[JointType.ShoulderCenter]);
                // Right arm
                DrawBoneFrontView(skeleton.Joints[JointType.ShoulderCenter], skeleton.Joints[JointType.ShoulderRight]);
                DrawBoneFrontView(skeleton.Joints[JointType.ShoulderRight], skeleton.Joints[JointType.ElbowRight]);
                DrawBoneFrontView(skeleton.Joints[JointType.ElbowRight], skeleton.Joints[JointType.WristRight]);
                DrawBoneFrontView(skeleton.Joints[JointType.WristRight], skeleton.Joints[JointType.HandRight]);
                // Left arm
                DrawBoneFrontView(skeleton.Joints[JointType.ShoulderCenter], skeleton.Joints[JointType.ShoulderLeft]);
                DrawBoneFrontView(skeleton.Joints[JointType.ShoulderLeft], skeleton.Joints[JointType.ElbowLeft]);
                DrawBoneFrontView(skeleton.Joints[JointType.ElbowLeft], skeleton.Joints[JointType.WristLeft]);
                DrawBoneFrontView(skeleton.Joints[JointType.WristLeft], skeleton.Joints[JointType.HandLeft]);

                DrawBoneFrontView(skeleton.Joints[JointType.ShoulderCenter], skeleton.Joints[JointType.Spine]);
                DrawBoneFrontView(skeleton.Joints[JointType.Spine], skeleton.Joints[JointType.HipCenter]);
                // Right Leg
                DrawBoneFrontView(skeleton.Joints[JointType.HipCenter], skeleton.Joints[JointType.HipRight]);
                DrawBoneFrontView(skeleton.Joints[JointType.HipRight], skeleton.Joints[JointType.KneeRight]);
                DrawBoneFrontView(skeleton.Joints[JointType.KneeRight], skeleton.Joints[JointType.AnkleRight]);
                DrawBoneFrontView(skeleton.Joints[JointType.AnkleRight], skeleton.Joints[JointType.FootRight]);
                // Left Leg
                DrawBoneFrontView(skeleton.Joints[JointType.HipCenter], skeleton.Joints[JointType.HipLeft]);
                DrawBoneFrontView(skeleton.Joints[JointType.HipLeft], skeleton.Joints[JointType.KneeLeft]);
                DrawBoneFrontView(skeleton.Joints[JointType.KneeLeft], skeleton.Joints[JointType.AnkleLeft]);
                DrawBoneFrontView(skeleton.Joints[JointType.AnkleLeft], skeleton.Joints[JointType.FootLeft]);
            }
            else if (view == SIDE_VIEW)
            {
                DrawBoneSideView(skeleton.Joints[JointType.Head], skeleton.Joints[JointType.ShoulderCenter]);
                // Right arm
                DrawBoneSideView(skeleton.Joints[JointType.ShoulderCenter], skeleton.Joints[JointType.ShoulderRight]);
                DrawBoneSideView(skeleton.Joints[JointType.ShoulderRight], skeleton.Joints[JointType.ElbowRight]);
                DrawBoneSideView(skeleton.Joints[JointType.ElbowRight], skeleton.Joints[JointType.WristRight]);
                DrawBoneSideView(skeleton.Joints[JointType.WristRight], skeleton.Joints[JointType.HandRight]);
                // Left arm
                DrawBoneSideView(skeleton.Joints[JointType.ShoulderCenter], skeleton.Joints[JointType.ShoulderLeft]);
                DrawBoneSideView(skeleton.Joints[JointType.ShoulderLeft], skeleton.Joints[JointType.ElbowLeft]);
                DrawBoneSideView(skeleton.Joints[JointType.ElbowLeft], skeleton.Joints[JointType.WristLeft]);
                DrawBoneSideView(skeleton.Joints[JointType.WristLeft], skeleton.Joints[JointType.HandLeft]);

                DrawBoneSideView(skeleton.Joints[JointType.ShoulderCenter], skeleton.Joints[JointType.Spine]);
                DrawBoneSideView(skeleton.Joints[JointType.Spine], skeleton.Joints[JointType.HipCenter]);
                // Right Leg
                DrawBoneSideView(skeleton.Joints[JointType.HipCenter], skeleton.Joints[JointType.HipRight]);
                DrawBoneSideView(skeleton.Joints[JointType.HipRight], skeleton.Joints[JointType.KneeRight]);
                DrawBoneSideView(skeleton.Joints[JointType.KneeRight], skeleton.Joints[JointType.AnkleRight]);
                DrawBoneSideView(skeleton.Joints[JointType.AnkleRight], skeleton.Joints[JointType.FootRight]);
                // Left Leg
                DrawBoneSideView(skeleton.Joints[JointType.HipCenter], skeleton.Joints[JointType.HipLeft]);
                DrawBoneSideView(skeleton.Joints[JointType.HipLeft], skeleton.Joints[JointType.KneeLeft]);
                DrawBoneSideView(skeleton.Joints[JointType.KneeLeft], skeleton.Joints[JointType.AnkleLeft]);
                DrawBoneSideView(skeleton.Joints[JointType.AnkleLeft], skeleton.Joints[JointType.FootLeft]);
            }
        }

        private void DrawJointFrontView(Joint joint)
        {
            if (joint.TrackingState == JointTrackingState.Tracked)
            {
                Ellipse follow = new Ellipse() {Height = 5, Width = 5, Fill = Brushes.BlueViolet};
                EllipseCanvas.Children.Add(follow);
                Canvas.SetTop(follow, joint.Position.Y * -250 + 247.5);
                Canvas.SetLeft(follow, joint.Position.X * 250 + 247.5);
            }
        }

        private void DrawJointSideView(Joint joint)
        {
            if (joint.TrackingState == JointTrackingState.Tracked)
            {
                Ellipse sideJoint = new Ellipse() {Height = 5, Width  = 5, Fill = Brushes.BlueViolet};
                EllipseCanvas.Children.Add(sideJoint);
                Canvas.SetTop(sideJoint, joint.Position.Y * -250 + 247.5);
                Canvas.SetLeft(sideJoint, joint.Position.Z * 200 + 147.5);
            }
        }

        private void DrawBoneFrontView(Joint start, Joint end)
        {
            if (start.TrackingState == JointTrackingState.Tracked && end.TrackingState == JointTrackingState.Tracked)
            {
                Point p1 = new Point(start.Position.X * 250 + 250, start.Position.Y * -250 + 250);
                Point p2 = new Point(end.Position.X * 250 + 250, end.Position.Y * -250 + 250);
                Line line = new Line() {X1 = p1.X, Y1 = p1.Y, X2 = p2.X, Y2 = p2.Y, Stroke = Brushes.MediumOrchid, StrokeThickness = 3};
                EllipseCanvas.Children.Add(line);
            }
        }

        private void DrawBoneSideView(Joint start, Joint end)
        {
            if (start.TrackingState == JointTrackingState.Tracked && end.TrackingState == JointTrackingState.Tracked)
            {
                Point p1 = new Point(start.Position.Z * 200 + 150, start.Position.Y * -250 + 250);
                Point p2 = new Point( end.Position.Z * 200 + 150, end.Position.Y * -250 + 250);
                Line line = new Line() { X1 = p1.X, Y1 = p1.Y, X2 = p2.X, Y2 = p2.Y, Stroke = Brushes.CornflowerBlue, StrokeThickness = 3 };
                EllipseCanvas.Children.Add(line);
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
    }
}
