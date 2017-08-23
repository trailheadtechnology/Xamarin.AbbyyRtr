using System;
using Foundation;
using ObjCRuntime;

namespace Xamarin.AbbyyRtr.iOS
{
    [BaseType(typeof(NSObject))]
    [Native]
    public enum RTRResultStabilityStatus : long
    {
        NotReady,
        Tentative,
        Verified,
        Available,
        TentativelyStable,
        Stable
    }

    [BaseType(typeof(NSObject))]
    [Native]
    public enum RTRCallbackWarningCode : long
    {
        NoWarning,
        RecognitionIsSlow,
        ProbablyLowQualityImage,
        ProbablyWrongLanguage,
        WrongLanguage,
        TextTooSmall
    }
}
