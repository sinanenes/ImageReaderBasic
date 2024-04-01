using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using OpenCvSharp;
using Tesseract;

internal class Program
{
    static void Main(string[] args)
    {
        // Access the camera and capture an image
        Bitmap capturedImage = CaptureImage();
        // Extract text from the captured image
        string extractedText = ExtractText(capturedImage);
        // Display the extracted text
        Console.WriteLine("Extracted Text:");
        Console.WriteLine(extractedText);
    }
    static Bitmap CaptureImage()
    {
        // Code to capture an image from the camera using OpenCVSharp or AForge.NET
        // Implement your camera capture logic here
        // Example:
        //  var capturedImage = CameraCaptureLibrary.CaptureImage();
        //  return capturedImage;
        // For demonstration purposes, I'll use a placeholder
        // Bitmap placeholderImage = new Bitmap("path_to_your_image.jpg");
        // return placeholderImage;

        // Access the camera and capture an image
        Mat capturedImage = CaptureImagee();

        // Convert OpenCV Mat to Bitmap
        //Bitmap bitmap = BitmapConverter.ToBitmap(capturedImage);
        Bitmap bitmap = ConvertMatToBitmap(capturedImage);

        // Further processing, text extraction, etc.

        return bitmap;
    }
    static Bitmap ConvertMatToBitmap(Mat mat)
    {
        // Convert Mat to Bitmap
        Bitmap bitmap = new Bitmap(mat.Width, mat.Height, PixelFormat.Format24bppRgb);
        BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, mat.Width, mat.Height), ImageLockMode.WriteOnly, bitmap.PixelFormat);
        IntPtr ptr = bmpData.Scan0;

        int bytes = Math.Abs(bmpData.Stride) * bitmap.Height;
        byte[] rgbValues = new byte[bytes];
        Marshal.Copy(mat.Data, rgbValues, 0, bytes);

        Marshal.Copy(rgbValues, 0, ptr, bytes);
        bitmap.UnlockBits(bmpData);

        return bitmap;
    }
    static Mat CaptureImagee()
    {
        using (var capture = new VideoCapture(0)) // Open default camera
        {
            if (!capture.IsOpened())
            {
                // Camera not found or unable to connect
                throw new Exception("Unable to connect to camera.");
            }

            // Capture a frame asynchronously
            Mat frame = new Mat();
            Console.WriteLine("Kaydet? Y/N");
            if (Console.ReadLine() == "Y")
            {                
                capture.Read(frame);
            }                        

            return frame;
        }
    }
    static string ExtractText(Bitmap image)
    {
        // Perform OCR on the captured image using Tesseract
        using (var engine = new TesseractEngine(@"C:\OCR\Tesseract\tessdata\", "tur", EngineMode.Default))
        {
            // using (var img = Pix.LoadFromFile(image).LoadFromFile("path_to_your_image.jpg")) // Load image directly into Pix
            // {
                ImageConverter converter = new ImageConverter();
                var imgByteArray = (byte[])converter.ConvertTo(image, typeof(byte[]));
                var img = Pix.LoadFromMemory(imgByteArray);


                using (var page = engine.Process(img))
                {
                    return page.GetText();
                }
            // }
        }
    }
}