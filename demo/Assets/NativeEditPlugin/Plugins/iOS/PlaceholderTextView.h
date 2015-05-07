#import <UIKit/UIKit.h>


@interface PlaceholderTextView : UITextView
{
    NSObject* _editBox;
}

@property(nonatomic, strong) NSString *placeholder;

@property (nonatomic, strong) UIColor *realTextColor UI_APPEARANCE_SELECTOR;
@property (nonatomic, strong) UIColor *placeholderColor UI_APPEARANCE_SELECTOR;

-(void) SetEditbox:(NSObject*) box;

@end