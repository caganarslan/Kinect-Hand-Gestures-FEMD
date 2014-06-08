//------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Samples.Kinect.CoordinateMappingBasics
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Microsoft.Kinect;
    using Emgu.CV;
    using Emgu.Util;
    using Emgu.CV.Structure;
    using System.Drawing;
using Emgu.CV.CvEnum;
    using System.Windows.Forms;
    using System.Collections.Generic;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {   
        /// <summary>
        /// Format we will use for the depth stream
        /// </summary>
        private const DepthImageFormat DepthFormat = DepthImageFormat.Resolution320x240Fps30;
      //  private const DepthImageFormat DepthFormat = DepthImageFormat.Resolution640x480Fps30;

        /// <summary>
        /// Format we will use for the color stream
        /// </summary>
        private const ColorImageFormat ColorFormat = ColorImageFormat.RgbResolution640x480Fps30;
        

        /// <summary>
        /// Active Kinect sensor
        /// </summary>
        private KinectSensor sensor;

        /// <summary>
        /// Bitmap that will hold color information
        /// </summary>
        private WriteableBitmap colorBitmap;

        /// <summary>
        /// Bitmap that will hold opacity mask information
        /// </summary>
        private WriteableBitmap playerOpacityMaskImage = null;

        /// <summary>
        /// Intermediate storage for the depth data received from the sensor
        /// </summary>
        private DepthImagePixel[] depthPixels;

        /// <summary>
        /// Intermediate storage for the color data received from the camera
        /// </summary>
        private byte[] colorPixels;

        /// <summary>
        /// Intermediate storage for the player opacity mask
        /// </summary>
        private int[] playerPixelData;

        /// <summary>
        /// Intermediate storage for the depth to color mapping
        /// </summary>
        private ColorImagePoint[] colorCoordinates;

        /// <summary>
        /// Inverse scaling factor between color and depth
        /// </summary>
        private int colorToDepthDivisor;

        /// <summary>
        /// Width of the depth image
        /// </summary>
        private int depthWidth;

        /// <summary>
        /// Height of the depth image
        /// </summary>
        private int depthHeight;

        /// <summary>
        /// Indicates opaque in an opacity mask
        /// </summary>
        private int opaquePixelValue = -1;

        public int old_gNo = -1;
        public Joint myJoint;
        public float keyVal;
        public DepthImagePoint depthPoint;
        public System.Drawing.Point firstPoint;
        public double tsAngle, tsDist;
        public List<List<double[]>> references;
        

        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        private System.Windows.Point SkeletonPointToScreen(SkeletonPoint skelpoint)
        {
            // Convert point to depth space.  
            // We are not using depth directly, but we do want the points in our 640x480 output resolution.
            DepthImagePoint depthPoint = this.sensor.CoordinateMapper.MapSkeletonPointToDepthPoint(skelpoint, DepthImageFormat.Resolution640x480Fps30);
            return new System.Windows.Point(depthPoint.X, depthPoint.Y);
        }

        public double getTSAngle(System.Drawing.Point p1, System.Drawing.Point p2) {
            double dY = (double)(p1.Y - p2.Y);
            double dX = (double)(p2.X - p1.X);
            double angleDeg = Math.Atan2(dY, dX) * 180 / Math.PI;
            return angleDeg;
        }

        public double getTSDist(System.Drawing.Point p1, System.Drawing.Point p2)
        {
            double dist = 0;

            dist = Math.Sqrt(Math.Pow(p1.X - p2.X, 2)+ Math.Pow(p1.Y - p2.Y, 2)); 
            return dist;
        }

        private System.Drawing.Bitmap BitmapFromSource(BitmapSource bitmapsource)
        {
            System.Drawing.Bitmap bitmap;
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapsource));

                enc.Save(outStream);
                bitmap = new System.Drawing.Bitmap(outStream);

            }
            return bitmap;

        }

        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        /// <summary>
        /// Execute startup tasks
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            // Create the interop host control.
            System.Windows.Forms.Integration.WindowsFormsHost host =
                new System.Windows.Forms.Integration.WindowsFormsHost();

            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            this.chart1.SuspendLayout();
            this.chart1.Visible = false;
            // 
            // chart1
            // 
            chartArea2.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea2);
            legend2.Name = "Legend1";
            this.chart1.Legends.Add(legend2);
            this.chart1.Location = new System.Drawing.Point(12, 12);
            this.chart1.Name = "chart1";
            series2.ChartArea = "ChartArea1";
            series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            series2.IsXValueIndexed = false;
            series2.Legend = "Legend1";
            series2.Name = "Series1";
            this.chart1.Series.Add(series2);
            this.chart1.Size = new System.Drawing.Size(18, 13);
            this.chart1.TabIndex = 0;
            this.chart1.Text = "chart1";
          //this.chart1.Visible = false;
            

            ///*******************

            chart1.Series.SuspendUpdates();
            this.references = getReferences();

            host.Child = chart1;
            grid1.Children.Add(host);

            // Look through all sensors and start the first connected one.
            // This requires that a Kinect is connected at the time of app startup.
            // To make your app robust against plug/unplug, 
            // it is recommended to use KinectSensorChooser provided in Microsoft.Kinect.Toolkit (See components in Toolkit Browser).
            foreach (var potentialSensor in KinectSensor.KinectSensors)
            {
                if (potentialSensor.Status == KinectStatus.Connected)
                {
                    this.sensor = potentialSensor;
                    break;
                }
            }

            if (null != this.sensor)
            {
                // Turn on the depth stream to receive depth frames
                this.sensor.DepthStream.Enable(DepthFormat);

                this.depthWidth = this.sensor.DepthStream.FrameWidth;

                this.depthHeight = this.sensor.DepthStream.FrameHeight;

                this.sensor.ColorStream.Enable(ColorFormat);

                int colorWidth = this.sensor.ColorStream.FrameWidth;
                int colorHeight = this.sensor.ColorStream.FrameHeight;

                this.colorToDepthDivisor = colorWidth / this.depthWidth;

                // Turn on to get player masks
                this.sensor.SkeletonStream.Enable();

                // Allocate space to put the depth pixels we'll receive
                this.depthPixels = new DepthImagePixel[this.sensor.DepthStream.FramePixelDataLength];

                // Allocate space to put the color pixels we'll create
                this.colorPixels = new byte[this.sensor.ColorStream.FramePixelDataLength];

                this.playerPixelData = new int[this.sensor.DepthStream.FramePixelDataLength];

                this.colorCoordinates = new ColorImagePoint[this.sensor.DepthStream.FramePixelDataLength];

                // This is the bitmap we'll display on-screen
                this.colorBitmap = new WriteableBitmap(colorWidth, colorHeight, 96.0, 96.0, PixelFormats.Bgr32, null);

                // Set the image we display to point to the bitmap where we'll put the image data
                this.MaskedColor.Source = this.colorBitmap;

                // Add an event handler to be called whenever there is new depth frame data
                this.sensor.AllFramesReady += this.SensorAllFramesReady;

                this.sensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Seated;
                // Start the sensor!
                try
                {
                    this.sensor.Start();
                }
                catch (IOException)
                {
                    this.sensor = null;
                }
            }

            if (null == this.sensor)
            {
                this.statusBarText.Text = Properties.Resources.NoKinectReady;
            }
        }



        public List<double[]> getSignatures(System.Windows.Forms.DataVisualization.Charting.DataPointCollection dataPoints)
        {

            int r1 = -1, r2 = 0;
            double weight = 0, totalWeight = 0;
            List<double[]> sigList = new List<double[]>();


            //double area = dataPoints.
            int length = dataPoints.Count;
            for (int i = length - 1; i > 0; i--)
            {
                double angCurrent = dataPoints[i].YValues[0];
                if ((angCurrent > 0.01) && (r1 == -1)) { r1 = i; };
                if (r1 != -1 && angCurrent < 0.05)
                {
                    r2 = i;
                    //r1 = 0;

                }
                if ((r1 != -1 && r2 != 0) && (Math.Abs(r2 - r1) > 5))// && r2 != 0)
                {
                    dataPoints[r2].Color = System.Drawing.Color.Yellow;

                    dataPoints[r1].Color = System.Drawing.Color.Red;

                    var myMem = new MemStorage();
                    Contour<System.Drawing.PointF> myCont = new Contour<System.Drawing.PointF>(myMem);
                    System.Drawing.PointF myPoint = new System.Drawing.PointF();

                    

                    for (int t = r1 - 1; t > r2 + 1; t--)
                    {
                        myPoint.X = ((float)dataPoints[t].XValue);
                        myPoint.Y = (float)dataPoints[t].YValues[0];
                        //weight = weight + dataPoints[t].YValues[0];
                        dataPoints[t].Color = System.Drawing.Color.Silver;
                        myCont.Push(myPoint);
                    }

                    weight = myCont.Area;

                    if (weight > totalWeight)
                    {
                        totalWeight = weight;
                    }
                    //  totalWeight = totalWeight + weight;

                    myCont.Clear();

                    double[] sigArr = { 0, 0, 0 };
                    sigArr[0] = (float)dataPoints[r1].XValue;
                    sigArr[1] = (float)dataPoints[r2].XValue;
                    sigArr[2] = weight;
                    //r2_2 = r1;
                    //if (weight > 0.5) { }
                    sigList.Add(sigArr);

                    r2 = 0;
                    weight = 0;
                    r1 = -1;

                }
            }

            //totalWeight = dataPoints.FindMaxByValue().YValues[0];
           // textBox1.Text = totalWeight.ToString();

         //   foreach (double[] sign in sigList)
          //  {
            //    sign[2] = sign[2] / totalWeight;
            //}

            return sigList;
        }

        public List<double[]> loadSignature(string path)
        {

            List<double[]> signList = new List<double[]>();
            System.Xml.Serialization.XmlSerializer reader = new
               System.Xml.Serialization.XmlSerializer(signList.GetType());

            // Read the XML file.
            System.IO.StreamReader file =
               new System.IO.StreamReader(path);

            // Deserialize the content of the file into a Book object.
            signList = (List<double[]>)reader.Deserialize(file);
            //System.Windows.Forms.MessageBox.Show(introToVCS.title,
            //"Book Title");

            return signList;

        }

        public List<List<double[]>> getReferences()
        {

            string sGeste5 = "xml\\geste5_4.xml";
            string sGeste3 = "xml\\geste3.xml";
            string sGeste2 = "xml\\geste2_5.xml";
            string sGeste1 = "xml\\geste1_2.xml";
            string sGeste4 = "xml\\geste10.xml";
           

            List<double[]> lGeste5 = new List<double[]>();
            List<double[]> lGeste3 = new List<double[]>();
            List<double[]> lGeste2 = new List<double[]>();
            List<double[]> lGeste4 = new List<double[]>();
            List<double[]> lGeste1 = new List<double[]>();



            lGeste5 = loadSignature(sGeste5);
            lGeste3 = loadSignature(sGeste3);
            lGeste2 = loadSignature(sGeste2);
            lGeste4 = loadSignature(sGeste4);
 
            lGeste1 = loadSignature(sGeste1);


            List<List<double[]>> references = new List<List<double[]>>();
            
            references.Add(lGeste1);
            references.Add(lGeste2);
            references.Add(lGeste3);
            references.Add(lGeste4);
            references.Add(lGeste5);

            return references;
        }


        public int findGesture(List<double[]> lInGeste, List<List<double[]>> references)
        {

            int ind = -1;
            double minFEMD = 9999.0;
            int refIndex = 0;
            textBox1.FontSize = 36; 
            textBox1.Text = "";
            foreach (List<double[]> refGeste in references)
            {

                int nInGeste = lInGeste.Count;
                int nRefGeste = refGeste.Count;

                IntPtr dMat = CvInvoke.cvCreateMat(nInGeste, nRefGeste, Emgu.CV.CvEnum.MAT_DEPTH.CV_32F);

                Matrix<float> mFlow = new Matrix<float>(nInGeste, nRefGeste);
                Matrix<float> mS1 = new Matrix<float>(nInGeste, 1);
                Matrix<float> mS2 = new Matrix<float>(nRefGeste, 1);

                for (int i = 0; i < nInGeste; i++)
                {

                    mS1[i, 0] = (float)lInGeste[i][2];

                    for (int j = 0; j < nRefGeste; j++)
                    {

                        mS2[j, 0] = (float)refGeste[j][2];

                        double dist = Math.Min(Math.Abs(lInGeste[i][0] - refGeste[j][0]), Math.Abs(lInGeste[i][1] - refGeste[j][1]));
                        Emgu.CV.Structure.MCvScalar mDist = new Emgu.CV.Structure.MCvScalar(dist);
                        CvInvoke.cvSet2D(dMat, i, j, mDist);
                    }
                }

                try
                {
                    if ((mS1.Height != 0) && (mS2.Height != 0) && (mFlow.Height != 0))
                    {
                        float emd = CvInvoke.cvCalcEMD2(mS1, mS2, Emgu.CV.CvEnum.DIST_TYPE.CV_DIST_USER, null, dMat, mFlow, new System.IntPtr(0), new System.IntPtr(0));

                        //Console.Write("EMD: ");
                        //Console.WriteLine(emd);
                        double F = mFlow.Sum;
                        double wDiff = mS1.Sum - mS2.Sum;
                        // Console.WriteLine(wDiff);
                        //Console.WriteLine(F);

                        double FEMD = 0.5 * emd + 0.5 * Math.Abs(wDiff) / F;
                        //Console.Write("FEMD: ");
                        //Console.WriteLine(FEMD);


                        if (FEMD < minFEMD && FEMD < 0.3)
                        {
                            minFEMD = FEMD;
                            ind = refIndex + 1;
                            
                        }

                        refIndex++;
                    }
                }
                catch (Emgu.CV.Util.CvException)
                {
                }

                    
            }
           // textBox1.Text = ind.ToString();
            return ind;
        }


        /// <summary>
        /// Execute shutdown tasks
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (null != this.sensor)
            {
                this.sensor.Stop();
                this.sensor = null;
            }
        }

        /// <summary>
        /// Event handler for Kinect sensor's DepthFrameReady event
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void SensorAllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            // in the middle of shutting down, so nothing to do
            if (null == this.sensor)
            {
                return;
            }

            bool depthReceived = false;
            bool colorReceived = false;

            Skeleton[] skeletons = new Skeleton[0];

            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {


                    skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    skeletonFrame.CopySkeletonDataTo(skeletons);

                    if (skeletons.Length != 0)
                    {
                        foreach (Skeleton skel in skeletons)
                        {
                            Joint joint0 = skel.Joints[JointType.WristLeft];

                            if (joint0.Position.Z != 0)
                            {
                                myJoint = joint0;
                                // keyVal = joint0.Position.Z;
                                depthPoint = this.sensor.CoordinateMapper.MapSkeletonPointToDepthPoint(joint0.Position, DepthImageFormat.Resolution640x480Fps30);
                                keyVal = depthPoint.Depth;


                            }
                        }
                    }
                }
            }

            using (DepthImageFrame depthFrame = e.OpenDepthImageFrame())
            {
                if (null != depthFrame)
                {
                    // Copy the pixel data from the image to a temporary array
                    depthFrame.CopyDepthImagePixelDataTo(this.depthPixels);

                    depthReceived = true;
                }
            }

            using (ColorImageFrame colorFrame = e.OpenColorImageFrame())
            {
                if (null != colorFrame)
                {
                    // Copy the pixel data from the image to a temporary array
                    colorFrame.CopyPixelDataTo(this.colorPixels);

                    colorReceived = true;
                }
            }

            // do our processing outside of the using block
            // so that we return resources to the kinect as soon as possible
            if (true == depthReceived)
            {
                this.sensor.CoordinateMapper.MapDepthFrameToColorFrame(
                    DepthFormat,
                    this.depthPixels,
                    ColorFormat,
                    this.colorCoordinates);

                Array.Clear(this.playerPixelData, 0, this.playerPixelData.Length);

                // loop over each row and column of the depth
                for (int y = 0; y < this.depthHeight; ++y)
                {
                    for (int x = 0; x < this.depthWidth; ++x)
                    {
                        // calculate index into depth array
                        int depthIndex = x + (y * this.depthWidth);

                        DepthImagePixel depthPixel = this.depthPixels[depthIndex];


                        int player = depthPixel.PlayerIndex;

                        // if we're tracking a player for the current pixel, sets it opacity to full
                        //if (player > 0)
                        if (keyVal != 0 && depthPixel.Depth < (keyVal + 30) && depthPixel.Depth > (keyVal - 1000))
                        {
                            // retrieve the depth to color mapping for the current depth pixel
                            ColorImagePoint colorImagePoint = this.colorCoordinates[depthIndex];

                            // scale color coordinates to depth resolution
                            int colorInDepthX = colorImagePoint.X / this.colorToDepthDivisor;
                            int colorInDepthY = colorImagePoint.Y / this.colorToDepthDivisor;

                            // make sure the depth pixel maps to a valid point in color space
                            // check y > 0 and y < depthHeight to make sure we don't write outside of the array
                            // check x > 0 instead of >= 0 since to fill gaps we set opaque current pixel plus the one to the left
                            // because of how the sensor works it is more correct to do it this way than to set to the right
                            if (colorInDepthX > 0 && colorInDepthX < this.depthWidth && colorInDepthY >= 0 && colorInDepthY < this.depthHeight)
                            {
                                // calculate index into the player mask pixel array
                                int playerPixelIndex = colorInDepthX + (colorInDepthY * this.depthWidth);

                                // set opaque
                                this.playerPixelData[playerPixelIndex] = opaquePixelValue;

                                // compensate for depth/color not corresponding exactly by setting the pixel 
                                // to the left to opaque as well
                                this.playerPixelData[playerPixelIndex - 1] = opaquePixelValue;
                            }
                        }
                    }
                }
            }

            // do our processing outside of the using block
            // so that we return resources to the kinect as soon as possible
            if (true == colorReceived)
            {
                // Write the pixel data into our bitmap
                this.colorBitmap.WritePixels(
                    new Int32Rect(0, 0, this.colorBitmap.PixelWidth, this.colorBitmap.PixelHeight),
                    this.colorPixels,
                    this.colorBitmap.PixelWidth * sizeof(int),
                    0);

                if (this.playerOpacityMaskImage == null)
                {
                    this.playerOpacityMaskImage = new WriteableBitmap(
                        this.depthWidth,
                        this.depthHeight,
                        96,
                        96,
                        PixelFormats.Bgra32,
                        null);

                    MaskedColor.OpacityMask = new ImageBrush { ImageSource = this.playerOpacityMaskImage };
                }

                this.playerOpacityMaskImage.WritePixels(
                    new Int32Rect(0, 1, this.depthWidth, this.depthHeight-1),
                    this.playerPixelData,
                    this.depthWidth * ((this.playerOpacityMaskImage.Format.BitsPerPixel + 7) / 8),
                   0);

                Image<Gray, Byte> My_Image = new Image<Gray, byte>(BitmapFromSource(this.colorBitmap));
                
                Image<Gray, Byte> My_Mask = new Image<Gray, byte>(BitmapFromSource(this.playerOpacityMaskImage));
                
                Image<Gray,byte> armMask = My_Mask.PyrUp();

                armMask = armMask.Erode(2);
                armMask = armMask.Dilate(1);
               
                //////////////////////////////////
                Image<Gray, Byte> HandG= My_Image.Copy(armMask);

                Gray gray = new Gray(255);
                Image<Gray, Byte> iLine = HandG.ThresholdBinaryInv(new Gray(50), gray);

                armMask = armMask.Erode(4);
               // CvInvoke.cvNamedWindow("gray");
                //CvInvoke.cvShowImage("gray", armMask); 

                iLine = iLine.Copy(armMask);
                
                
                System.Windows.Point myPoint = this.SkeletonPointToScreen(myJoint.Position);
                HandG.ROI = new Rectangle((int)(myPoint.X - 70), (int)(myPoint.Y - 90), 140, 140);
                iLine.ROI = new Rectangle((int)(myPoint.X - 70), (int)(myPoint.Y - 90), 140, 140);
                iLine = iLine.Erode(1);
                //iLine = iLine.Dilate(2);
               
                //Create the window using the specific name
            
               // CvInvoke.cvNamedWindow("line");
                //CvInvoke.cvShowImage("line", iLine); 

                Image<Gray, Byte> resultImage = HandG.CopyBlank();
                Image<Gray, Single> resultImageIN = resultImage.Convert<Gray, Single>();
                Image<Gray, Byte> maskC = HandG.CopyBlank();
                Image<Bgr, Byte> iAffiche = new Image<Bgr, byte>(resultImage.Width, resultImage.Height);
               
                            
                Double Result1 = 0;
                Double Result2 = 0;

                
                HandG = HandG.ThresholdBinary(new Gray(50), gray);

                HandG = HandG.Erode(2);
                

                LineSegment2D[] lines = iLine.HoughLinesBinary(1, Math.PI / 45, 15, 5, 15)[0];
                //if (lines.Length != 0)
                //{
                //    int a1 = lines[0].P2.Y - lines[0].P1.Y;
                //    int b1 = lines[0].P1.X - lines[0].P2.X;

                //    for (int i = 0; i < HandG.Width; i++)
                //    {
                //        for (int j = 0; j < HandG.Height; j++)
                //        {
                //            if (HandG[i, j].Intensity == gray.Intensity)
                //            {
                //                int a2 = i - lines[0].P1.X;
                //                int b2 = j - lines[0].P1.Y;
                //                if (a1 * a2 + b1 * b2 < 0)
                //                {
                //                    HandG[i, j] = new Gray(0);
                //                }
                //            }
                //        }
                //    }
                //}


                
                using (var mem = new MemStorage())
                {
                    Contour<System.Drawing.Point> contour = HandG.FindContours(
                        CHAIN_APPROX_METHOD.CV_CHAIN_APPROX_NONE,
                        RETR_TYPE.CV_RETR_LIST,
                        mem);
                    Contour<System.Drawing.Point> biggestContour = null;

                    while (contour != null)
                    {
                        Result1 = contour.Area;
                        if (Result1 > Result2)
                        {
                            Result2 = Result1;
                            biggestContour = contour;
                        }
                        contour = contour.HNext;
                    }


                    if (biggestContour != null)
                    {
                       // biggestContour = biggestContour.ApproxPoly(1.5);
                       
                        resultImage.Draw(biggestContour, gray, 2);
                        maskC.Draw(biggestContour, gray, -1);
                        Emgu.CV.Image<Gray, byte> binaryIM = resultImage.ThresholdBinaryInv(new Gray(100), new Gray(255));

                        CvInvoke.cvDistTransform(binaryIM, resultImageIN, DIST_TYPE.CV_DIST_L1, 3, null, IntPtr.Zero);

                        CvInvoke.cvNormalize(resultImageIN, resultImageIN, 0, 1, Emgu.CV.CvEnum.NORM_TYPE.CV_MINMAX, IntPtr.Zero);

                        double minD = 0, maxD = 0;
                        System.Drawing.Point maxP = System.Drawing.Point.Empty, minP = System.Drawing.Point.Empty;

                        CvInvoke.cvMinMaxLoc(resultImageIN, ref minD, ref maxD, ref minP, ref maxP, maskC.Ptr);

                        iAffiche[0] = resultImage;
                        iAffiche[1] = resultImage;
                        iAffiche[2] = resultImage;


                        if (lines.Length != 0 && lines.Length<3)
                        {
                            System.Drawing.Point lineP1 = lines[0].P1;
                            iAffiche.Draw(lines[0], new Bgr(0, 255, 0), 2);
                            int sPx = lineP1.X;
                            int sPy = lineP1.Y;
                            System.Drawing.Point startP = lineP1;

                            double minDist = 999999.0;
                            System.Drawing.Point[] bContArr = biggestContour.ToArray();
                            firstPoint = bContArr[0];
                            int nbPoints = bContArr.Length;
                            double startAngle = 0;
                            double startDist = 0;
                            double rInsCircle = 99999.0;
                            

                            for (int i = 0; i < nbPoints; i++)
                            {
                                System.Drawing.Point v = bContArr[i];
                                double tempDist = ((v.X - sPx) * (v.X - sPx) + (v.Y - sPy) * (v.Y - sPy));
                                double tempCirc = Math.Sqrt((v.X - maxP.X) * (v.X - maxP.X) + (v.Y - maxP.Y) * (v.Y - maxP.Y));
                                if (tempDist < minDist)
                                {
                                    minDist = tempDist;
                                    startP = v;
                                
                                }

                                if (tempCirc < rInsCircle) { rInsCircle = tempCirc; }

                            }

                            chart1.Series.SuspendUpdates();

                            foreach (var series in chart1.Series)
                            {
                                series.Points.Clear();
                            }

                            List<double[]> pointList = new List<double[]>();
                            pointList.Clear();

                            startAngle = -1*getTSAngle(maxP, startP);
                            startDist = getTSDist(maxP, startP);
                            for (int i = 0; i < (nbPoints); i++)
                            {
                                System.Drawing.Point v = bContArr[i];

                                tsAngle = -1 * getTSAngle(maxP, v);
                                tsAngle = tsAngle - startAngle;
                                if (tsAngle < 0) { tsAngle = tsAngle + 360; }
                                tsAngle = tsAngle / 360;
                                
                                tsDist = getTSDist(maxP, v);
                                tsDist = (tsDist / (rInsCircle)) - (1.9);
                                if (tsDist < 0) { tsDist = 0.0; }

                                if (tsDist != 0 && i == 0)
                                {
                                    System.Drawing.Point a = bContArr[i];
                                    Array.Copy(bContArr, 1, bContArr, 0, bContArr.Length - 1);
                                    bContArr[bContArr.Length - 1] = a;
                                    i = -1;
                                }

                                if (i != -1)
                                {
                                    chart1.Series["Series1"].Points.AddXY(tsAngle, tsDist);
                                    double[] XY = { tsAngle, tsDist };
                                    pointList.Add(XY);
                                }

                            }

                            CircleF startPoint = new CircleF(startP, 3);

                            iAffiche.Draw(startPoint, new Bgr(0, 0, 255), 2);

                            CircleF palmCir = new CircleF(maxP, 1);
                            iAffiche.Draw(palmCir, new Bgr(255, 255, 0), 3);

                           //chart1.Series.Invalidate();
                           
                            ///////////////////SIGNATURES

                            List<double[]> inGesteSignatures = new List<double[]>();
                            inGesteSignatures.Clear();
                            inGesteSignatures = getSignatures(chart1.Series["Series1"].Points);

                            int gNo=-1;
                            if ((inGesteSignatures.Count != 0) && (inGesteSignatures.Count < 7))
                            {
                                gNo = findGesture(inGesteSignatures, references);
                             //   textBox1.Text = gNo.ToString();


                                if (old_gNo != -1 && gNo != -1 && gNo == old_gNo)
                                {
                                    textBox1.Text = gNo.ToString();
                                }
                                old_gNo = gNo;

                            }
                           //chart1.Series.Invalidate();
                         //  chart1.Series.ResumeUpdates();

                            CvInvoke.cvNamedWindow("affiche");
                            CvInvoke.cvShowImage("affiche", iAffiche); 

                        }

                    }
                }
            }
        }

        /// <summary>
        /// Handles the user clicking on the screenshot button
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void ButtonScreenshotClick(object sender, RoutedEventArgs e)
        {

        }
        
        /// <summary>
        /// Handles the checking or unchecking of the near mode combo box
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void CheckBoxNearModeChanged(object sender, RoutedEventArgs e)
        {
            if (this.sensor != null)
            {
                // will not function on non-Kinect for Windows devices
                try
                {
                    if (this.checkBoxNearMode.IsChecked.GetValueOrDefault())
                    {
                        this.sensor.DepthStream.Range = DepthRange.Near;
                    }
                    else
                    {
                        this.sensor.DepthStream.Range = DepthRange.Default;
                    }
                }
                catch (InvalidOperationException)
                {
                }
            }
        }

        private void TextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }
    }
}