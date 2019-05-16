// handle exception for stop and start

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Threading;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System.IO;
using System.Diagnostics;

namespace CamCaptureUsingOpenCVSharp
{


    public partial class Form1 : Form
    {
        public void wait(int milliseconds)
        {
            System.Windows.Forms.Timer timer1 = new System.Windows.Forms.Timer();
            if (milliseconds == 0 || milliseconds < 0) return;
            //Console.WriteLine("start wait timer");
            timer1.Interval = milliseconds;
            timer1.Enabled = true;
            timer1.Start();
            timer1.Tick += (s, e) =>
            {
                timer1.Enabled = false;
                timer1.Stop();
                //Console.WriteLine("stop wait timer");
            };
            while (timer1.Enabled)
            {
                Application.DoEvents();
            }
        }
        // Create class-level accesible variables
        VideoCapture capture;
        Mat frame;
        Bitmap image;
        Bitmap snapshot;
        private Thread camera;
        bool isCameraRunning = false;
        bool isSnapTaken = false;

        // Declare required methods
        private void CaptureCamera()
        {
            camera = new Thread(new ThreadStart(CaptureCameraCallback));
            camera.Start();
        }
        private void CaptureCameraCallback()
        {

            frame = new Mat();
            capture = new VideoCapture(0);
            capture.Open(0);

            if (capture.IsOpened())
            {
                while (isCameraRunning)
                {
                    if (isSnapTaken)
                    {
                        image = snapshot;
                        pictureBox1.Image = image;
                        break;
                    }
                    capture.Read(frame);
                    image = BitmapConverter.ToBitmap(frame);
                    if (pictureBox1.Image != null)
                    {
                        pictureBox1.Image = null;
                    }
                    pictureBox1.Image = image;
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text.Equals("Start"))
            {
                CaptureCamera();
                button1.Text = "Stop";
                isCameraRunning = true;
            }
            else
            {
                isSnapTaken = false;
                capture.Release();
                button1.Text = "Start";
                isCameraRunning = false;
                pictureBox1.Image.Dispose();
                pictureBox1.Image = null;
                button2.Text = "Take a Snapshot";
                textBox1.Text = "";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (isCameraRunning)
            {
                if (button2.Text == "Save" && textBox1.Text != "Write name here") //  use his name
                {
                    Name = textBox1.Text;
                    textBox1.Text = "saving...";
                    wait(500);
                    button2.Text = "Take a Snapshot";
                    Directory.CreateDirectory(@"C:\ProgramData\AttendanceSys\images\");
                    snapshot.Save(@"C:\ProgramData\AttendanceSys\images\" + Name + ".png", ImageFormat.Png);
                    textBox1.Text = "Saved!";
                    wait(1000);
                    //initialize the whole program
                    isSnapTaken = false;
                    capture.Release();
                    pictureBox1.Image.Dispose();
                    pictureBox1.Image = null;
                    button2.Text = "Take a Snapshot";
                    textBox1.Text = "";
                    CaptureCamera();
                    isCameraRunning = true;


                    // invoke python script to make encoding here
                }
                else
                {
                    snapshot = new Bitmap(pictureBox1.Image);
                    button2.Text = "Save";
                    textBox1.Text = "Write name here";
                    isSnapTaken = true;
                }

                //snapshot.Save(@"C:\\mysnapshot.png", ImageFormat.Png);
                //snapshot.Save(string.Format(@"C:\\{0}.png", Guid.NewGuid()), ImageFormat.Png);
            }
            else
            {
                Console.WriteLine("Cannot take picture if the camera isn't capturing image!");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (isCameraRunning)
            {
                if (button3.Text != "") //  use his name
                {
                    textBox2.Text = "Marking...";
                    wait(500);
                    Name = "ImageToMark";
                    Directory.CreateDirectory(@"C:\ProgramData\AttendanceSys\images\");
                    snapshot.Save(@"C:\ProgramData\AttendanceSys\images\" + Name + ".png", ImageFormat.Png);
                    wait(1000);

                    // 1) Create Process Info
                    var psi = new ProcessStartInfo();
                    psi.FileName = @"C:\Users\Adeel\AppData\Local\Programs\Python\Python37\python.exe";

                    // 2) Provide script and arguments
                    var script = "face_recog.py";
                    var start = "";
                    var end = "";

                    psi.Arguments = $"\"{script}\" \"{start}\" \"{end}\"";

                    // 3) Process configuration
                    psi.UseShellExecute = false;
                    psi.CreateNoWindow = true;
                    psi.RedirectStandardOutput = true;
                    psi.RedirectStandardError = true;

                    // 4) Execute process and get output
                    var errors = "";
                    var results = "";

                    using (var process = Process.Start(psi))
                    {
                        errors = process.StandardError.ReadToEnd();
                        results = process.StandardOutput.ReadToEnd();
                    }
                    if (results == "1")
                    {
                        textBox2.Text = "Attendance Marked";
                    }
                    else
                    {
                        textBox2.Text = "Attendance Marked";
                    }
                    //textBox2.Text = results;
                    textBox2.Text = errors;
                    wait(2000);
                    isSnapTaken = false;
                    capture.Release();
                    pictureBox1.Image.Dispose();
                    pictureBox1.Image = null;
                    button2.Text = "Take a Snapshot";
                    textBox1.Text = "";
                    CaptureCamera();
                    isCameraRunning = true;







                    //initialize the whole program



                    // invoke python script to make encoding here



                }
                else
                {
                    Console.WriteLine("Cannot take picture if the camera isn't capturing image!");
                }
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
