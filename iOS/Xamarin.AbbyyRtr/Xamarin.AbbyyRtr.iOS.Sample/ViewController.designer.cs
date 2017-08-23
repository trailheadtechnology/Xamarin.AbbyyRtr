// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace Xamarin.AbbyyRtr.iOS.Sample
{
    [Register ("ViewController")]
    partial class ViewController
    {
        [Outlet]
        UIKit.UIButton captureButton { get; set; }

        [Outlet]
        UIKit.UITableView languagesTableView { get; set; }

        [Outlet]
        Xamarin.AbbyyRtr.iOS.Sample.RTRSelectedAreaView overlayView { get; set; }

        [Outlet]
        UIKit.UIView previewView { get; set; }

        [Outlet]
        UIKit.UIBarButtonItem recognizeLanguageButton { get; set; }

        [Outlet]
        UIKit.UIView whiteBackgroundView { get; set; }
        
        void ReleaseDesignerOutlets ()
        {
            if (captureButton != null) {
                captureButton.Dispose ();
                captureButton = null;
            }

            if (languagesTableView != null) {
                languagesTableView.Dispose ();
                languagesTableView = null;
            }

            if (overlayView != null) {
                overlayView.Dispose ();
                overlayView = null;
            }

            if (previewView != null) {
                previewView.Dispose ();
                previewView = null;
            }

            if (whiteBackgroundView != null) {
                whiteBackgroundView.Dispose ();
                whiteBackgroundView = null;
            }

            if (recognizeLanguageButton != null) {
                recognizeLanguageButton.Dispose ();
                recognizeLanguageButton = null;
            }
        }
    }
}
