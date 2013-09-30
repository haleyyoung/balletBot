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
        private const int FRONT_VIEW = 0;
        private const int SIDE_VIEW = 1;

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

                if (pliesMode)
                {
                    MovementMode mode = new MovementMode(EllipseCanvas, skeleton);
                    mode.plies();
                }
                DrawSkeleton(skeleton, FRONT_VIEW);
                DrawSkeleton(skeleton, SIDE_VIEW);
            }
            else
            {
                System.Console.WriteLine("skeleton is null");
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

        private void PliesMode(object sender, RoutedEventArgs e)
        {
            this.pliesMode = !pliesMode;
            // TODO: change color of button
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
    }
}