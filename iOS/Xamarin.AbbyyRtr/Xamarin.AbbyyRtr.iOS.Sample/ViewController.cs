using System;
using System.Diagnostics;
using System.IO;
using AVFoundation;
using CoreAnimation;
using CoreFoundation;
using CoreGraphics;
using CoreVideo;
using Foundation;
using UIKit;

namespace Xamarin.AbbyyRtr.iOS.Sample
{
    public partial class ViewController : UIViewController, IAVCaptureVideoDataOutputSampleBufferDelegate
    {
        bool capture;

        const string RTRTextRegionsLayerName = "TextRegionsLayer";
        CGSize ImageBufferSize = new CGSize(720, 1280);

        RTREngine engine;
        RTRTextCaptureService textCaptureService;
        AVCaptureSession session;
        NSString sessionPreset = AVCaptureSession.Preset1280x720;
        AVCaptureVideoPreviewLayer previewLayer;

        TextCaptureServiceDelegate textCaptureServiceDelegate;

        CGRect _selectedArea;
        CGRect SelectedArea
        {
            get { return _selectedArea; }
            set
            {
                _selectedArea = value;
                overlayView.SelectedArea = SelectedArea;
            }
        }

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();
            if (previewLayer != null)
            {
                UpdatePreviewLayerFrame();
            }
        }

        protected ViewController(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            captureButton.TouchUpInside += CapturePressed;
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            session.StopRunning();
            captureButton.Selected = false;
            capture = false;
            captureButton.TouchUpInside -= CapturePressed;
        }

        public async override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var licensePath = Path.Combine(NSBundle.MainBundle.BundlePath, "License/AbbyyRtrSdk.license");

            engine = RTREngine.SharedEngineWithLicenseData(NSData.FromFile(licensePath));

            if (engine == null)
            {
                throw new InvalidOperationException("tmp");
            }

            textCaptureServiceDelegate = new TextCaptureServiceDelegate(this);
            textCaptureService = engine.CreateTextCaptureServiceWithDelegate(textCaptureServiceDelegate);

            NSSet selectedRecognitionLanguages = new NSSet("English");
            textCaptureService.SetRecognitionLanguages(selectedRecognitionLanguages);

            //self.languagesTableView.register(UITableViewCell.self, forCellReuseIdentifier: RTRVideoScreenCellName)
            //self.languagesTableView.tableFooterView = UIView(frame: CGRect.zero)
            languagesTableView.Hidden = true;

            captureButton.Selected = false;
            captureButton.SetTitle("Stop", UIControlState.Selected);
            captureButton.SetTitle("Start", UIControlState.Normal);

            //let recognizeLanguageButtonTitle = self.languagesButtonTitle()
            //self.recognizeLanguageButton.title = recognizeLanguageButtonTitle

            var status = AVCaptureDevice.GetAuthorizationStatus(AVMediaType.Video);

            switch (status)
            {
                case AVAuthorizationStatus.Authorized:
                    ConfigureCompletionAccess(true);
                    break;
                case AVAuthorizationStatus.NotDetermined:
                    var granted = await AVCaptureDevice.RequestAccessForMediaTypeAsync(AVMediaType.Video);
                    ConfigureCompletionAccess(granted);
                    break;
                case AVAuthorizationStatus.Restricted:
                case AVAuthorizationStatus.Denied:
                    ConfigureCompletionAccess(false);
                    break;
            }
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

        void ConfigureCompletionAccess(bool accessGranted)
        {
            if (!UIImagePickerController.IsCameraDeviceAvailable(UIImagePickerControllerCameraDevice.Rear))
            {
                captureButton.Enabled = false;
                Console.WriteLine("Device has no camera");
                return;
            }

            if (!accessGranted)
            {
                captureButton.Enabled = false;
                Console.WriteLine("Camera access denied");
                return;
            }

            ConfigureAVCaptureSession();
            ConfigurePreviewLayer();
            session.StartRunning();

            //NotificationCenter.default.addObserver(self, selector:#selector(RTRViewController.avSessionFailed(_:)), name: NSNotification.Name.AVCaptureSessionRuntimeError, object: nil)
            //NotificationCenter.default.addObserver(self, selector:#selector(RTRViewController.applicationDidEnterBackground(_:)), name: NSNotification.Name.UIApplicationDidEnterBackground, object: nil)
            //NotificationCenter.default.addObserver(self, selector:#selector(RTRViewController.applicationWillEnterForeground(_:)), name: NSNotification.Name.UIApplicationWillEnterForeground, object: nil)

            //self.capturePressed("" as AnyObject)
        }

        void ConfigureAVCaptureSession()
        {
            session = new AVCaptureSession();
            session.SessionPreset = sessionPreset;

            var device = AVCaptureDevice.GetDefaultDevice(AVMediaType.Video);

            try
            {
                var input = new AVCaptureDeviceInput(device, out var error);

                if (error != null)
                {
                    throw new Exception(error.LocalizedDescription);
                }

                session.AddInput(input);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            var videoDataOutput = new AVCaptureVideoDataOutput();
            var videoDataOutputQueue = new DispatchQueue("videodataqueue", false);
            videoDataOutput.SetSampleBufferDelegateQueue(this, videoDataOutputQueue);


            var settings = new AVVideoSettingsUncompressed()
            {
                PixelFormatType = CVPixelFormatType.CV32BGRA
            };
            videoDataOutput.WeakVideoSettings = settings.Dictionary;
            session.AddOutput(videoDataOutput);

            var connection = videoDataOutput.ConnectionFromMediaType(AVMediaType.Video);
            connection.Enabled = true;
        }

        void ConfigurePreviewLayer()
        {
            previewLayer = new AVCaptureVideoPreviewLayer(session);
            previewLayer.BackgroundColor = UIColor.Black.CGColor;
            previewLayer.VideoGravity = AVLayerVideoGravity.Resize;

            var rootLayer = previewView.Layer;
            rootLayer.InsertSublayer(previewLayer, 0);
            UpdatePreviewLayerFrame();
        }

        private void UpdatePreviewLayerFrame()
        {
            var orientation = UIApplication.SharedApplication.StatusBarOrientation;

            previewLayer.Connection.VideoOrientation = VideoOrientation(orientation);

            var viewBounds = View.Bounds;
            previewLayer.Frame = viewBounds;
            // selectedArea = viewBounds.InsetBy(dx: viewBounds.width / 8.0, dy: viewBounds.height / 3.0)
            UpdateAreaOfInterest();

        }

        private AVCaptureVideoOrientation VideoOrientation(UIInterfaceOrientation orientation)
        {
            switch (orientation)
            {
                case UIInterfaceOrientation.Portrait:
                    return AVCaptureVideoOrientation.Portrait;
                case UIInterfaceOrientation.PortraitUpsideDown:
                    return AVCaptureVideoOrientation.PortraitUpsideDown;
                case UIInterfaceOrientation.LandscapeLeft:
                    return AVCaptureVideoOrientation.LandscapeLeft;
                case UIInterfaceOrientation.LandscapeRight:
                    return AVCaptureVideoOrientation.LandscapeRight;
                default:
                    return AVCaptureVideoOrientation.Portrait;
            }
        }

        void CapturePressed(object sender, EventArgs args)
        {
            if (!captureButton.Enabled) // TODO: why?
            {
                return;
            }

            captureButton.Selected = !captureButton.Selected;
            capture = captureButton.Selected;

            textCaptureService.StopTasks();

            if (capture)
            {
                ClearScreenFromRegions();
                whiteBackgroundView.Hidden = true;
                session.StartRunning();
            }
        }

        void ClearScreenFromRegions()
        {
            // Get all visible regions
            var sublayers = previewView.Layer.Sublayers;

            foreach (var layer in sublayers)
            {
                if (layer.Name == RTRTextRegionsLayerName)
                {
                    layer.RemoveFromSuperLayer();
                }
            }
        }

		[Export("captureOutput:didOutputSampleBuffer:fromConnection:")]
		public virtual void DidOutputSampleBuffer(AVCaptureOutput captureOutput, CoreMedia.CMSampleBuffer sampleBuffer, AVCaptureConnection connection)
		{
            if (!capture)
            {
                sampleBuffer.Dispose();
                return;
            }

            InvokeOnMainThread(() =>
            {
                // Image is prepared
                var orientation = UIApplication.SharedApplication.StatusBarOrientation;
                connection.VideoOrientation = VideoOrientation(orientation);

                textCaptureService.AddSampleBuffer(sampleBuffer.Handle);

                sampleBuffer.Dispose();
            });
        }

		void UpdateAreaOfInterest()
		{
            // var affineTransform = CGAffineTransform.MakeScale((nfloat)(ImageBufferSize.Width * 1.0 / overlayView.Frame.Width), (nfloat)(ImageBufferSize.Height * 1.0 / overlayView.Frame.Height));
			// var selectedRect = selectedArea.applying(affineTransform); // how to stransform
			//textCaptureService.SetAreaOfInterest(selectedRect); 

			textCaptureService.SetAreaOfInterest(SelectedArea);
	    }

        public void OnResult(RTRTextLine[] textLines, RTRResultStabilityStatus resultStatus)
        {
            InvokeOnMainThread(() =>
            {
                if (resultStatus == RTRResultStabilityStatus.Stable)
                {
                    captureButton.Selected = false;
                    capture = false;
                    whiteBackgroundView.Hidden = false;
                }

                DrawTextLines(textLines, resultStatus);
            });
        }

        void DrawTextLines(RTRTextLine[] textLines, RTRResultStabilityStatus resultStatus)
        {
            ClearScreenFromRegions();

            var textRegionsLayer = new CALayer()
            {
                Frame = previewLayer.Frame,
                Name = RTRTextRegionsLayerName
            };

            foreach (var textLine in textLines)
            {
                DrawTextLine(textLine, textRegionsLayer, resultStatus);
            }

            previewView.Layer.AddSublayer(textRegionsLayer);
        }

		void DrawTextLine(RTRTextLine textLine, CALayer layer, RTRResultStabilityStatus progress)
		{
            var topLeft = ScaledPoint(textLine.Quadrangle[0] as NSValue);
            var bottomLeft = ScaledPoint(textLine.Quadrangle[1] as NSValue);
            var bottomRight = ScaledPoint(textLine.Quadrangle[2] as NSValue);
            var topRight = ScaledPoint(textLine.Quadrangle[3] as NSValue);

            DrawQuadrangle(topLeft, bottomLeft, bottomRight, topRight, layer, progress);
            var recognizedString = textLine.Text;

            var textLayer = new CATextLayer();
            var textWidth = DistanceBetween(topLeft, topRight);
            var textHeight = DistanceBetween(topLeft, bottomLeft);

            var rectForTextLayer = new CGRect(bottomLeft.X, bottomLeft.Y, textWidth, textHeight);

            // Selecting the initial font size by rectangle
            var textFont = GetFont(recognizedString, rectForTextLayer);
            textLayer.SetFont(CGFont.CreateWithFontName(textFont.FontDescriptor.Name));
            textLayer.FontSize = textFont.PointSize;
            textLayer.ForegroundColor = ProgressColor(progress).CGColor;
            textLayer.AlignmentMode = CATextLayer.AlignmentCenter;
            textLayer.String = recognizedString;
            textLayer.Frame = rectForTextLayer;

            // Rotate the text layer
            var angle = (float)Math.Asin((bottomRight.Y - bottomLeft.Y) / DistanceBetween(bottomLeft, bottomRight));
            textLayer.AnchorPoint = new CGPoint(0, 0);
            textLayer.Position = bottomLeft;
            //textLayer.Transform = new CATransform3DRotate(CATransform3DIdentity, angle, 0, 0, 1);
            textLayer.Transform = CATransform3D.MakeRotation(angle, 0, 0, 1);

            layer.AddSublayer(textLayer);
		}

        void DrawQuadrangle(CGPoint p0, CGPoint p1, CGPoint p2, CGPoint p3, CALayer layer, RTRResultStabilityStatus progress)
        {
            var area = new CAShapeLayer();
            var recognizedAreaPath = new UIBezierPath();
            recognizedAreaPath.MoveTo(p0);
            recognizedAreaPath.AddLineTo(p1);
            recognizedAreaPath.AddLineTo(p2);
            recognizedAreaPath.AddLineTo(p3);
            recognizedAreaPath.ClosePath();
            area.Path = recognizedAreaPath.CGPath;
            area.StrokeColor = ProgressColor(progress).CGColor;
            area.FillColor = UIColor.Clear.CGColor;
            layer.AddSublayer(area);
        }

        UIColor ProgressColor(RTRResultStabilityStatus progress)
        {
            switch (progress)
            {
                case RTRResultStabilityStatus.NotReady:
                case RTRResultStabilityStatus.Tentative:
                    return ColorHelper.FromHexString("#FF6500");
                case RTRResultStabilityStatus.Verified:
                    return ColorHelper.FromHexString("#C96500");
                case RTRResultStabilityStatus.Available:
                    return ColorHelper.FromHexString("#886500");
                case RTRResultStabilityStatus.TentativelyStable:
                    return ColorHelper.FromHexString("#4B6500");
                case RTRResultStabilityStatus.Stable:
                    return ColorHelper.FromHexString("#006500");
            }

            throw new ArgumentException("Unknown status");
        }

        UIFont GetFont(string text, CGRect rect)
        {

            var minFontSize = 0.1f;
            var maxFontSize = 72.0f;
            var fontSize = minFontSize;

            var rectSize = rect.Size;

            while (true)
            {
                var labelSize = text.StringSize(UIFont.BoldSystemFontOfSize(fontSize));

                if (rectSize.Height - labelSize.Height > 0)
                {
                    minFontSize = fontSize;

                    if (rectSize.Height * 0.99 - labelSize.Height < 0)
                    {
                        break;
                    }
                }
                else
                {
                    maxFontSize = fontSize;
                }

                if (Math.Abs(minFontSize - maxFontSize) < 0.01)
                {
                    break;
                }

                fontSize = (minFontSize + maxFontSize) / 2;
            }

            return UIFont.BoldSystemFontOfSize(fontSize);
        }

		CGPoint ScaledPoint(NSValue mocrPoint /*cMocrPoint mocrPoint: NSValue*/)
        {
            var layerWidth = previewLayer.Bounds.Width;
            var layerHeight = previewLayer.Bounds.Height;

            var widthScale = layerWidth / ImageBufferSize.Width;
            var heightScale = layerHeight / ImageBufferSize.Height;

            var point = mocrPoint.CGPointValue;
            point.X *= widthScale;
            point.Y *= heightScale;

            return point;
        }

        float DistanceBetween(CGPoint p1, CGPoint p2)
        {
            var vector = new CGVector(p2.X - p1.X, p2.Y - p1.Y);
            return (float)Math.Sqrt(vector.dx * vector.dx + vector.dy * vector.dy);
        }

		public void OnError(NSError error)
		{
            Debug.WriteLine(error.LocalizedDescription);
		}

        public void OnWarning(RTRCallbackWarningCode warningCode)
        {
            if (warningCode == RTRCallbackWarningCode.TextTooSmall)
            {
                Debug.WriteLine("Text is too small");   
            }
		}

		class TextCaptureServiceDelegate : RTRTextCaptureServiceDelegate
		{
			readonly ViewController _viewController;

			public TextCaptureServiceDelegate(ViewController viewController)
			{
				_viewController = viewController;
			}

			public override void ResultStatus(RTRTextLine[] textLines, RTRResultStabilityStatus resultStatus)
			{
				_viewController.OnResult(textLines, resultStatus);
			}

			public override void OnError(NSError error)
			{
				_viewController.OnError(error);
			}

			public override void OnWarning(RTRCallbackWarningCode warningCode)
			{
				_viewController.OnWarning(warningCode);
			}
		}
	}



    /*
// ABBYY® Real-Time Recognition SDK 1 © 2016 ABBYY Production LLC.
// ABBYY is either a registered trademark or a trademark of ABBYY Software Ltd.

import UIKit
import AVFoundation

class RTRViewController: UIViewController, UITableViewDelegate, UITableViewDataSource, AVCaptureVideoDataOutputSampleBufferDelegate, RTRTextCaptureServiceDelegate {

    /// Cell ID for languagesTableView
    private let RTRVideoScreenCellName = "VideoScreenCell"
    private let RTRTextRegionsLayerName = "TextRegionsLayer"

    /// View with video preview layer
    @IBOutlet weak var previewView: UIView!
    /// Stop/Start capture button
    @IBOutlet weak var captureButton: UIButton!

    /// Recognition languages table
    @IBOutlet weak var languagesTableView: UITableView!
    @IBOutlet weak var recognizeLanguageButton: UIBarButtonItem!

    @IBOutlet weak var whiteBackgroundView: UIView!
    @IBOutlet weak var overlayView: RTRSelectedAreaView!

    private var session: AVCaptureSession?
    private var previewLayer: AVCaptureVideoPreviewLayer?
    private var engine: RTREngine?
    private var textCaptureService: RTRTextCaptureService?
    private var selectedRecognitionLanguages = Set(["English"])

    private let SessionPreset = AVCaptureSessionPreset1280x720
    private let ImageBufferSize = CGSize(width: 720, height: 1280)
    
    private let RecognitionLanguages = ["English",
                                        "French",
                                        "German",
                                        "Italian",
                                        "Polish",
                                        "PortugueseBrazilian",
                                        "Spanish"]

    

        //# MARK: - LifeCycle

    deinit {
        NotificationCenter.default.removeObserver(self)
    }

    override var prefersStatusBarHidden: Bool {
        return true
    }

//# MARK: - Private

    private func languagesButtonTitle() -> String {
        if self.selectedRecognitionLanguages.count == 1 {
            return self.selectedRecognitionLanguages.first!
        }

        var languageCodes = [String]()

        for language in self.selectedRecognitionLanguages {
            let index = language.index(language.startIndex, offsetBy: 2)
            languageCodes.append(language.substring(to: index))
        }

        return languageCodes.joined(separator: " ")
    }

    private func tryToCloseLanguagesTable() {
        if self.selectedRecognitionLanguages.isEmpty {
            return
        }

        self.textCaptureService?.setRecognitionLanguages(self.selectedRecognitionLanguages)
        self.capturePressed("" as AnyObject)
        self.languagesTableView.isHidden = true
    }

//# MARK: - Drawing result

//# MARK: - UITableViewDelegate

    func tableView(_ tableView: UITableView, didSelectRowAt indexPath: IndexPath) {
        let language = RecognitionLanguages[indexPath.row]
        if !self.selectedRecognitionLanguages.contains(language) {
            self.selectedRecognitionLanguages.insert(language)
        } else {
            self.selectedRecognitionLanguages.remove(language)
        }

        self.recognizeLanguageButton.title = self.languagesButtonTitle()
        tableView .reloadRows(at: [indexPath], with: UITableViewRowAnimation.automatic)
    }

//# MARK: - UITableViewDatasource

    func tableView(_ tableView: UITableView, numberOfRowsInSection section: Int) -> Int {
        return RecognitionLanguages.count
    }

    func tableView(_ tableView: UITableView, cellForRowAt indexPath: IndexPath) -> UITableViewCell {
        let cell = UITableViewCell(style: UITableViewCellStyle.default, reuseIdentifier: nil)
        let language = RecognitionLanguages[indexPath.row]
        cell.textLabel?.text = language
        cell.accessoryType = self.selectedRecognitionLanguages.contains(language) ? UITableViewCellAccessoryType.checkmark : UITableViewCellAccessoryType.none
        return cell
    }

//# MARK: - Notifications

    func avSessionFailed(_ notification: NSNotification) {
        let alertView = UIAlertView(title: "AVSession Failed!", message: nil, delegate: nil, cancelButtonTitle:"OK")
        alertView.show()
    }

    func applicationDidEnterBackground(_ notification: NSNotification) {
        self.session?.stopRunning()
        self.clearScreenFromRegions()
        self.whiteBackgroundView.isHidden = true
        self.textCaptureService?.stopTasks()
        self.captureButton.isSelected = true
    }

    func applicationWillEnterForeground(_ notification: NSNotification) {
        self.session?.startRunning()
    }


//# MARK: - Actions

    @IBAction func onReconitionLanguages(_ sender: AnyObject) {
        if self.languagesTableView.isHidden {
            self.captureButton.isSelected = false
            self.languagesTableView.reloadData()
            self.languagesTableView.isHidden = false
        } else {
            self.tryToCloseLanguagesTable()
        }
    }

    
}
*/

}
