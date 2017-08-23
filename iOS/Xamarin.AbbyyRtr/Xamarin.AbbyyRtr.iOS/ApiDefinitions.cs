using System;
using CoreGraphics;
using CoreMedia;
using Foundation;
using ObjCRuntime;
using UIKit;

namespace Xamarin.AbbyyRtr.iOS
{
    // @interface RTREngine : NSObject
    [BaseType(typeof(NSObject))]
    [DisableDefaultCtor]
    interface RTREngine
    {
        // @property (readonly, nonatomic) id<RTREngineSettings> extendedSettings;
        [Export("extendedSettings")]
        RTREngineSettings ExtendedSettings { get; }

        // +(instancetype)sharedEngineWithLicenseData:(NSData *)licenseData;
        [Static]
        [Export("sharedEngineWithLicenseData:")]
        RTREngine SharedEngineWithLicenseData(NSData licenseData);

        // -(id<RTRTextCaptureService>)createTextCaptureServiceWithDelegate:(id<RTRTextCaptureServiceDelegate>)delegate;
        [Export("createTextCaptureServiceWithDelegate:")]
        RTRTextCaptureService CreateTextCaptureServiceWithDelegate(RTRTextCaptureServiceDelegate @delegate);

        // -(id<RTRTextCaptureService>)createTextCaptureServiceWithDelegate:(id<RTRTextCaptureServiceDelegate>)delegate settings:(RTRExtendedSettings *)settings;
        [Export("createTextCaptureServiceWithDelegate:settings:")]
        RTRTextCaptureService CreateTextCaptureServiceWithDelegate(RTRTextCaptureServiceDelegate @delegate, RTRExtendedSettings settings);

        // -(id<RTRDataCaptureService>)createDataCaptureServiceWithDelegate:(id<RTRDataCaptureServiceDelegate>)delegate profile:(NSString *)profile;
        [Export("createDataCaptureServiceWithDelegate:profile:")]
        RTRDataCaptureService CreateDataCaptureServiceWithDelegate(RTRDataCaptureServiceDelegate @delegate, string profile);

        // -(id<RTRDataCaptureService>)createDataCaptureServiceWithDelegate:(id<RTRDataCaptureServiceDelegate>)delegate profile:(NSString *)profile settings:(RTRExtendedSettings *)settings;
        [Export("createDataCaptureServiceWithDelegate:profile:settings:")]
        RTRDataCaptureService CreateDataCaptureServiceWithDelegate(RTRDataCaptureServiceDelegate @delegate, string profile, RTRExtendedSettings settings);

        // -(id<RTRCoreAPI>)createCoreAPI;
        [Export("createCoreAPI")]
        //[Verify (MethodToProperty)]
        RTRCoreAPI CreateCoreAPI { get; }

        // -(NSSet *)languagesAvailableForOCR;
        [Export("languagesAvailableForOCR")]
        //[Verify (MethodToProperty)]
        NSSet LanguagesAvailableForOCR { get; }
    }

    // @interface RTRCharInfo : NSObject
    [BaseType(typeof(NSObject))]
    [DisableDefaultCtor]
    interface RTRCharInfo
    {
        // @property (readonly, assign, nonatomic) CGRect rect;
        [Export("rect", ArgumentSemantic.Assign)]
        CGRect Rect { get; }

        // @property (readonly, nonatomic, strong) NSArray * quadrangle;
        [Export("quadrangle", ArgumentSemantic.Strong)]
        //[Verify (StronglyTypedNSArray)]
        NSObject[] Quadrangle { get; }

        // @property (readonly, assign, nonatomic) NSInteger foregroundColor;
        [Export("foregroundColor")]
        nint ForegroundColor { get; }

        // @property (readonly, assign, nonatomic) NSInteger backgroundColor;
        [Export("backgroundColor")]
        nint BackgroundColor { get; }

        // @property (readonly, assign, nonatomic) BOOL isItalic;
        [Export("isItalic")]
        bool IsItalic { get; }

        // @property (readonly, assign, nonatomic) BOOL isBold;
        [Export("isBold")]
        bool IsBold { get; }

        // @property (readonly, assign, nonatomic) BOOL isUnderlined;
        [Export("isUnderlined")]
        bool IsUnderlined { get; }

        // @property (readonly, assign, nonatomic) BOOL isStrikethrough;
        [Export("isStrikethrough")]
        bool IsStrikethrough { get; }

        // @property (readonly, assign, nonatomic) BOOL isSmallcaps;
        [Export("isSmallcaps")]
        bool IsSmallcaps { get; }

        // @property (readonly, assign, nonatomic) BOOL isSuperscript;
        [Export("isSuperscript")]
        bool IsSuperscript { get; }

        // @property (readonly, assign, nonatomic) BOOL isUncertain;
        [Export("isUncertain")]
        bool IsUncertain { get; }
    }

    // @interface RTRTextLine : NSObject
    [BaseType(typeof(NSObject))]
    [DisableDefaultCtor]
    interface RTRTextLine
    {
        // @property (readonly, nonatomic, strong) NSString * text;
        [Export("text", ArgumentSemantic.Strong)]
        string Text { get; }

        // @property (readonly, assign, nonatomic) CGRect rect;
        [Export("rect", ArgumentSemantic.Assign)]
        CGRect Rect { get; }

        // @property (readonly, nonatomic, strong) NSArray * quadrangle;
        [Export("quadrangle", ArgumentSemantic.Strong)]
        //[Verify (StronglyTypedNSArray)]
        NSObject[] Quadrangle { get; }

        // @property (readonly, nonatomic, strong) NSArray * charsInfo;
        [Export("charsInfo", ArgumentSemantic.Strong)]
        //[Verify (StronglyTypedNSArray)]
        NSObject[] CharsInfo { get; }
    }

    // @interface RTRTextBlock : NSObject
    [BaseType(typeof(NSObject))]
    [DisableDefaultCtor]
    interface RTRTextBlock
    {
        // @property (readonly, nonatomic, strong) NSArray<RTRTextLine *> * textLines;
        [Export("textLines", ArgumentSemantic.Strong)]
        RTRTextLine[] TextLines { get; }
    }

    // @interface RTRDataScheme : NSObject
    [BaseType(typeof(NSObject))]
    [DisableDefaultCtor]
    interface RTRDataScheme
    {
        // @property (readonly, nonatomic, strong) NSString * id;
        [Export("id", ArgumentSemantic.Strong)]
        string Id { get; }

        // @property (readonly, nonatomic, strong) NSString * name;
        [Export("name", ArgumentSemantic.Strong)]
        string Name { get; }
    }

    // @interface RTRDataField : NSObject
    [BaseType(typeof(NSObject))]
    [DisableDefaultCtor]
    interface RTRDataField
    {
        // @property (readonly, nonatomic, strong) NSString * id;
        [Export("id", ArgumentSemantic.Strong)]
        string Id { get; }

        // @property (readonly, nonatomic, strong) NSString * name;
        [Export("name", ArgumentSemantic.Strong)]
        string Name { get; }

        // @property (readonly, nonatomic, strong) NSString * text;
        [Export("text", ArgumentSemantic.Strong)]
        string Text { get; }

        // @property (readonly, nonatomic, strong) NSArray<NSValue *> * quadrangle;
        [Export("quadrangle", ArgumentSemantic.Strong)]
        NSValue[] Quadrangle { get; }

        // @property (readonly, nonatomic, strong) NSArray<RTRTextLine *> * components;
        [Export("components", ArgumentSemantic.Strong)]
        RTRTextLine[] Components { get; }
    }

    // @protocol RTRRecognitionService
    [BaseType(typeof(NSObject))]
    [Protocol, Model]
    interface RTRRecognitionService
    {
        // @required -(void)addSampleBuffer:(CMSampleBufferRef)sampleBuffer;
        [Abstract]
        [Export("addSampleBuffer:")]
        unsafe void AddSampleBuffer(IntPtr sampleBuffer);

        // @required -(void)stopTasks;
        [Abstract]
        [Export("stopTasks")]
        void StopTasks();

        // @required -(void)setAreaOfInterest:(CGRect)areaOfInterest;
        [Abstract]
        [Export("setAreaOfInterest:")]
        void SetAreaOfInterest(CGRect areaOfInterest);
    }

    // @protocol RTRTextCaptureService <RTRRecognitionService>
    [BaseType(typeof(NSObject))]
    // [Protocol, Model]
    [Protocol]
    interface RTRTextCaptureService : RTRRecognitionService
    {
        // @required -(void)setRecognitionLanguages:(NSSet *)recognitionLanguages;
        //[Abstract]
        [Export("setRecognitionLanguages:")]
        void SetRecognitionLanguages(NSSet recognitionLanguages);

        // @required -(void)setTranslationDictionary:(NSString *)dictionaryName;
        // [Abstract]
        [Export("setTranslationDictionary:")]
        void SetTranslationDictionary(string dictionaryName);
    }

    // typedef BOOL (^RTRFieldPredicateBlock)(NSString *);
    delegate bool RTRFieldPredicateBlock(string arg0);

    // @protocol RTRDataFieldBuilder
    [BaseType(typeof(NSObject))]
    [Protocol, Model]
    interface RTRDataFieldBuilder
    {
        // @required -(id<RTRDataFieldBuilder>)setName:(NSString *)name;
        [Abstract]
        [Export("setName:")]
        RTRDataFieldBuilder SetName(string name);

        // @required -(id<RTRDataFieldBuilder>)setRegEx:(NSString *)regEx;
        [Abstract]
        [Export("setRegEx:")]
        RTRDataFieldBuilder SetRegEx(string regEx);

        // @required -(id<RTRDataFieldBuilder>)setPredicateBlock:(RTRFieldPredicateBlock)predicateBlock;
        [Abstract]
        [Export("setPredicateBlock:")]
        RTRDataFieldBuilder SetPredicateBlock(RTRFieldPredicateBlock predicateBlock);
    }

    // @protocol RTRDataSchemeBuilder
    [BaseType(typeof(NSObject))]
    [Protocol, Model]
    interface RTRDataSchemeBuilder
    {
        // @required -(id<RTRDataSchemeBuilder>)setName:(NSString *)name;
        [Abstract]
        [Export("setName:")]
        RTRDataSchemeBuilder SetName(string name);

        // @required -(id<RTRDataFieldBuilder>)addField:(NSString *)id;
        [Abstract]
        [Export("addField:")]
        RTRDataFieldBuilder AddField(string id);
    }

    // @protocol RTRDataCaptureProfileBuilder
    [BaseType(typeof(NSObject))]
    [Protocol, Model]
    interface RTRDataCaptureProfileBuilder
    {
        // @required -(id<RTRDataCaptureProfileBuilder>)setRecognitionLanguages:(NSSet *)languages;
        [Abstract]
        [Export("setRecognitionLanguages:")]
        RTRDataCaptureProfileBuilder SetRecognitionLanguages(NSSet languages);

        // @required -(id<RTRDataSchemeBuilder>)addScheme:(NSString *)id;
        [Abstract]
        [Export("addScheme:")]
        RTRDataSchemeBuilder AddScheme(string id);

        // @required -(NSError *)checkAndApply;
        [Abstract]
        [Export("checkAndApply")]
        //[Verify (MethodToProperty)]
        NSError CheckAndApply { get; }
    }

    // @protocol RTRDataCaptureService <RTRRecognitionService>
    [BaseType(typeof(NSObject))]
    [Protocol, Model]
    interface RTRDataCaptureService : RTRRecognitionService
    {
        // @required -(id<RTRDataCaptureProfileBuilder>)configureDataCaptureProfile;
        [Abstract]
        [Export("configureDataCaptureProfile")]
        //[Verify (MethodToProperty)]
        RTRDataCaptureProfileBuilder ConfigureDataCaptureProfile { get; }
    }

    // @protocol RTRRecognitionServiceDelegate <NSObject>
    [Protocol, Model]
    [BaseType(typeof(NSObject))]
    interface IRTRRecognitionServiceDelegate
    {
        // @required -(void)onError:(NSError *)error;
        [Abstract]
        [Export("onError:")]
        // TODO: Find a way to make it required on child objects
        void OnError(NSError error);

        // @optional -(void)onWarning:(RTRCallbackWarningCode)warningCode;
        [Export("onWarning:")]
        void OnWarning(RTRCallbackWarningCode warningCode);
    }

    // @protocol RTRTextCaptureServiceDelegate <RTRRecognitionServiceDelegate>
    [BaseType(typeof(NSObject))]
    [Protocol, Model]
    interface RTRTextCaptureServiceDelegate : IRTRRecognitionServiceDelegate
    {
        // @required -(void)onBufferProcessedWithTextLines:(NSArray<RTRTextLine *> *)textLines resultStatus:(RTRResultStabilityStatus)resultStatus;
        [Abstract]
        [Export("onBufferProcessedWithTextLines:resultStatus:")]
        void ResultStatus(RTRTextLine[] textLines, RTRResultStabilityStatus resultStatus);
    }

    // @protocol RTRDataCaptureServiceDelegate <RTRRecognitionServiceDelegate>
    [BaseType(typeof(NSObject))]
    [Protocol, Model]
    interface RTRDataCaptureServiceDelegate : IRTRRecognitionServiceDelegate
    {
        // @required -(void)onBufferProcessedWithDataScheme:(RTRDataScheme *)dataScheme dataFields:(NSArray<RTRDataField *> *)dataFields resultStatus:(RTRResultStabilityStatus)resultStatus;
        [Abstract]
        [Export("onBufferProcessedWithDataScheme:dataFields:resultStatus:")]
        void DataFields(RTRDataScheme dataScheme, RTRDataField[] dataFields, RTRResultStabilityStatus resultStatus);
    }

    // @interface RTRExtendedSettings : NSObject
    [BaseType(typeof(NSObject))]
    interface RTRExtendedSettings
    {
        // @property (assign, nonatomic) NSInteger processingThreadsCount;
        [Export("processingThreadsCount")]
        nint ProcessingThreadsCount { get; set; }

        // @property (getter = isFrameMergingEnabled, assign, nonatomic) BOOL frameMergingEnabled;
        [Export("frameMergingEnabled")]
        bool FrameMergingEnabled { [Bind("isFrameMergingEnabled")] get; set; }

        // @property (getter = isCJKVerticalTextEnabled, assign, nonatomic) BOOL CJKVerticalTextEnabled;
        [Export("CJKVerticalTextEnabled")]
        bool CJKVerticalTextEnabled { [Bind("isCJKVerticalTextEnabled")] get; set; }
    }

    // @protocol RTREngineSettings
    [BaseType(typeof(NSObject))]
    [Protocol, Model]
    interface RTREngineSettings
    {
        // @required @property (copy, nonatomic) NSString * _Nullable externalAssetsPath;
        [Abstract]
        [NullAllowed, Export("externalAssetsPath")]
        string ExternalAssetsPath { get; set; }
    }

    // typedef BOOL (^RTRProgressCallbackBlock)(NSInteger, RTRCallbackWarningCode);
    delegate bool RTRProgressCallbackBlock(nint arg0, RTRCallbackWarningCode arg1);

    // typedef void (^RTRTextOrientationDetectedBlock)(NSInteger);
    delegate void RTRTextOrientationDetectedBlock(nint arg0);

    // @protocol RTRCoreAPI
    [BaseType(typeof(NSObject))]
    [Protocol, Model]
    interface RTRCoreAPI
    {
        // @required @property (readonly, nonatomic) id<RTRCoreAPIProcessingSettings> processingSettings;
        [Abstract]
        [Export("processingSettings")]
        RTRCoreAPIProcessingSettings ProcessingSettings { get; }

        // @required @property (readonly, nonatomic) id<RTRCoreAPITextRecognitionSettings> textRecognitionSettings;
        [Abstract]
        [Export("textRecognitionSettings")]
        RTRCoreAPITextRecognitionSettings TextRecognitionSettings { get; }

        // @required -(NSArray<RTRTextBlock *> *)recognizeTextOnImage:(UIImage *)image onProgress:(RTRProgressCallbackBlock)progressCallback onTextOrientationDetected:(RTRTextOrientationDetectedBlock)textOrientationDetectedCallback error:(NSError **)error;
        [Abstract]
        [Export("recognizeTextOnImage:onProgress:onTextOrientationDetected:error:")]
        RTRTextBlock[] OnProgress(UIImage image, RTRProgressCallbackBlock progressCallback, RTRTextOrientationDetectedBlock textOrientationDetectedCallback, out NSError error);
    }

    // @protocol RTRCoreAPIProcessingSettings
    [BaseType(typeof(NSObject))]
    [Protocol, Model]
    interface RTRCoreAPIProcessingSettings
    {
        // @required @property (assign, nonatomic) NSInteger processingThreadsCount;
        [Abstract]
        [Export("processingThreadsCount")]
        nint ProcessingThreadsCount { get; set; }
    }

    // @protocol RTRCoreAPITextRecognitionSettings
    [BaseType(typeof(NSObject))]
    [Protocol, Model]
    interface RTRCoreAPITextRecognitionSettings
    {
        // @required -(void)setAreaOfInterest:(CGRect)areaOfInterest;
        [Abstract]
        [Export("setAreaOfInterest:")]
        void SetAreaOfInterest(CGRect areaOfInterest);

        // @required -(void)setRecognitionLanguages:(NSSet<NSString *> *)recognitionLanguages;
        [Abstract]
        [Export("setRecognitionLanguages:")]
        void SetRecognitionLanguages(NSSet<NSString> recognitionLanguages);
    }
}
