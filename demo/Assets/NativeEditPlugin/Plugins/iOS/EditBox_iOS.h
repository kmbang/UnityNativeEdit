#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>
#import "JsonObject.h"

/// UnityEditBox Plugin
/// Written by bkmin 2014/11 Nureka Inc. 

#define MSG_CREATE @"CreateEdit"
#define MSG_REMOVE @"RemoveEdit"
#define MSG_SET_TEXT @"SetText"
#define MSG_GET_TEXT @"GetText"
#define MSG_SET_RECT @"SetRect"
#define MSG_SET_FOCUS @"SetFocus"
#define MSG_SET_VISIBLE @"SetVisible"
#define MSG_TEXT_CHANGE @"TextChange"
#define MSG_TEXT_END_EDIT @"TextEndEdit"

@interface EditBoxHoldView : UIView
{

}

-(id) initHoldView:(CGRect) frame;

@end

@interface EditBox : NSObject<UITextFieldDelegate, UITextViewDelegate>
{
    UIView*     editView;
    UIViewController* viewController;
    CGRect rectKeyboardFrame;
    UIToolbar* keyboardDoneButtonView;
    UIBarButtonItem* doneButton;
    int tag;
}

+(void) initializeEditBox:(UIViewController*) _unityViewController unityName:(const char*) unityName;
+(JsonObject*) processRecvJsonMsg:(int)nSenderId msg:(JsonObject*) jsonMsg;
+(void) finalizeEditBox;

-(void) showKeyboard:(bool)isShow;
-(BOOL) IsFocused;
@end
