#import <Foundation/Foundation.h>
#import "NativeCallProxy.h"


@implementation FrameworkLibAPI

id<NativeCallsProtocol> api = NULL;
+(void) registerAPIforNativeCalls:(id<NativeCallsProtocol>) aApi
{
    api = aApi;
}

@end


extern "C" {
    void Unity2NativeMsgIOS(const char* opJson);//xiugou do stuff here? }
}

void Unity2NativeMsgIOS(const char * value){
    
    return [api Unity2NativeMsgIOS:[NSString stringWithUTF8String:value]];
    
}
