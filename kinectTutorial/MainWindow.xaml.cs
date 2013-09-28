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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// Most of the code to get the kinect set up and tracking a skeleton came
    /// from http://channel9.msdn.com/Series/KinectQuickstart/
    /// </summary>
    public partial class MainWindow : Window
    {
        KinectSensor kinect = null;
        Skeleton[] skeletonData;
        Skeleton skeleton;
        Boolean pliesMode = false;
        public MainWindow()
        {
            InitializeComponent();
            StartKinectST();
        }

        void StartKinectST()
        {
            kinect = KinectSensor.KinectSensors.FirstOrDefault(s => s.Status == KinectStatus.Connected); // Get first Kinect Sensor

            var parameters = new TransformSmoothParameters
            {
                Smoothing = 0.1f,
                Correction = 0.0f,
                Prediction = 0.0f,
                JitterRadius = 1.0f,
                MaxDeviationRadius = 0.5f
            };

            kinect.SkeletonStream.Enable(parameters); // Enable skeletal tracking

            skeletonData = new Skeleton[kinect.SkeletonStream.FrameSkeletonArrayLength]; // Allocate ST data

            kinect.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(SensorSkeletonFrameReady); // Get Ready for Skeleton Ready Events

            kinect.Start(); // Start Kinect sensor
        }

        private void SensorSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame()) // Open the Skeleton frame
            {
                if (skeletonFrame != null && this.skeletonData != null) // check that a frame is available
                {
                    skeletonFrame.CopySkeletonDataTo(this.skeletonData); // get the skeletal information in this frame
                }
            }

            skeleton = this.skeletonData.Where(s => s.TrackingState == SkeletonTrackingState.Tracked).FirstOrDefault();
            Render();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void Render()
        {
            if (skeleton != null)
            {
                EllipseCanvas.Children.Clear();
                Joint head = skeleton.Joints[JointType.Head];
                if (pliesMode)
                {
                    MovementMode mode = new MovementMode(EllipseCanvas, skeleton);
                    mode.Plies();
                }

                if (head.TrackingState == JointTrackingState.Tracked)
                {

                    foreach (Joint joint in skeleton.Joints)
                    {
                       DrawJoint(joint);
                    }
                    DrawBone(skeleton.Joints[JointType.Head], skeleton.Joints[JointType.ShoulderCenter]);
                    // Right arm
                    DrawBone(skeleton.Joints[JointType.ShoulderCenter], skeleton.Joints[JointType.ShoulderRight]);
                    DrawBone(skeleton.Joints[JointType.ShoulderRight], skeleton.Joints[JointType.ElbowRight]);
                    DrawBone(skeleton.Joints[JointType.ElbowRight], skeleton.Joints[JointType.WristRight]);
                    DrawBone(skeleton.Joints[JointType.WristRight], skeleton.Joints[JointType.HandRight]);
                    // Left arm
                    DrawBone(skeleton.Joints[JointType.ShoulderCenter], skeleton.Joints[JointType.ShoulderLeft]);
                    DrawBone(skeleton.Joints[JointType.ShoulderLeft], skeleton.Joints[JointType.ElbowLeft]);
                    DrawBone(skeleton.Joints[JointType.ElbowLeft], skeleton.Joints[JointType.WristLeft]);
                    DrawBone(skeleton.Joints[JointType.WristLeft], skeleton.Joints[JointType.HandLeft]);

                    DrawBone(skeleton.Joints[JointType.ShoulderCenter], skeleton.Joints[JointType.Spine]);
                    DrawBone(skeleton.Joints[JointType.Spine], skeleton.Joints[JointType.HipCenter]);
                    // Right Leg
                    DrawBone(skeleton.Joints[JointType.HipCenter], skeleton.Joints[JointType.HipRight]);
                    DrawBone(skeleton.Joints[JointType.HipRight], skeleton.Joints[JointType.KneeRight]);
                    DrawBone(skeleton.Joints[JointType.KneeRight], skeleton.Joints[JointType.AnkleRight]);
                    DrawBone(skeleton.Joints[JointType.AnkleRight], skeleton.Joints[JointType.FootRight]);
                    // Left Leg
                    DrawBone(skeleton.Joints[JointType.HipCenter], skeleton.Joints[JointType.HipLeft]);
                    DrawBone(skeleton.Joints[JointType.HipLeft], skeleton.Joints[JointType.KneeLeft]);
                    DrawBone(skeleton.Joints[JointType.KneeLeft], skeleton.Joints[JointType.AnkleLeft]);
                    DrawBone(skeleton.Joints[JointType.AnkleLeft], skeleton.Joints[JointType.FootLeft]);
                }
            }
            else
            {
                System.Console.WriteLine("skeleton is null");
            }
        }

        private void DrawJoint(Joint joint)
        {
            if (joint.TrackingState == JointTrackingState.Tracked)
            {
                Ellipse follow = new Ellipse() {Height = 5, Width = 5, Fill = Brushes.MediumOrchid};
                EllipseCanvas.Children.Add(follow);
                Canvas.SetTop(follow, joint.Position.Y * -250 + 247.5);
                Canvas.SetLeft(follow, joint.Position.X * 250 + 247.5);
            }
        }

        private void DrawBone(Joint start, Joint end)
        {
            if (start.TrackingState == JointTrackingState.Tracked && end.TrackingState == JointTrackingState.Tracked)
            {
                Point p1 = new Point(start.Position.X * 250 + 250, start.Position.Y * -250 + 250);
                Point p2 = new Point(end.Position.X * 250 + 250, end.Position.Y * -250 + 250);
                Line line = new Line() {X1 = p1.X, Y1 = p1.Y, X2 = p2.X, Y2 = p2.Y, Stroke = Brushes.Red, StrokeThickness = 3};
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

        // this event fires when Color/Depth/Skeleton are syncronized
        void _sensor_AllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            using (ColorImageFrame colorFrame = e.OpenColorImageFrame())
            {
                if (colorFrame == null)
                {
                    return;
                }
                byte[] pixels = new byte[colorFrame.PixelDataLength];
                colorFrame.CopyPixelDataTo(pixels);

                int stride = colorFrame.Width * 4;
                image1.Source = BitmapSource.Create(colorFrame.Width, colorFrame.Height, 96,
                    96, PixelFormats.Bgr32, null, pixels, stride);
            }
        }

        void StopKinect(KinectSensor sensor)
        {
            if (sensor != null)
            {
                sensor.Stop();
                sensor.AudioSource.Stop();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs c)
        {
            Application.Current.Shutdown();
        }

        private void PliesMode(object sender, RoutedEventArgs e)
        {
            this.pliesMode = !pliesMode;
            // TODO: change color of button
        }
    }
}