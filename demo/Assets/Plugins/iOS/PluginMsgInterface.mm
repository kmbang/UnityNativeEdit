//
//  PluginMsgHandler.m
//  Unity-iPhone
//
//  Created by KYUNGMIN BANG on 2/24/15.
//
//

#import "EditBox_iOS.h"

UIViewController* UnityGetGLViewController();

char* gCopyString(const char* string) {
    if(string == NULL) {
        return NULL;
    }
    char* ret = (char*) malloc(strlen(string) + 1); // memory will be auto-freed by unity
    strcpy(ret, string);
    return ret;
}

extern "C" {

 /*   typedef void* MonoDomain;
    typedef void* MonoAssembly;
    typedef void* MonoImage;
    typedef void* MonoClass;
    typedef void* MonoObject;
    typedef void* MonoMethodDesc;
    typedef void* MonoMethod;
    typedef void* gpointer;
    typedef int gboolean;
    
    MonoDomain *mono_domain_get();
    MonoAssembly *mono_domain_assembly_open(MonoDomain *domain, const char *assemblyName);
    MonoImage *mono_assembly_get_image(MonoAssembly *assembly);
    MonoMethodDesc *mono_method_desc_new(const char *methodString, gboolean useNamespace);
    MonoMethodDesc *mono_method_desc_free(MonoMethodDesc *desc);
    MonoMethod *mono_method_desc_search_in_image(MonoMethodDesc *methodDesc, MonoImage *image);
    MonoObject *mono_runtime_invoke(MonoMethod *method, void *obj, void **params, MonoObject **exc);
    gpointer mono_object_unbox(MonoObject *obj);
    
    void initializUnityPluginHandler()
    {
        MonoDomain *domain = mono_domain_get();
        NSString *assemblyPath = [[[NSBundle mainBundle] bundlePath]
                                  stringByAppendingPathComponent:@"Data/Managed/Assembly-CSharp.dll"];
        MonoAssembly *assembly = mono_domain_assembly_open(domain, assemblyPath.UTF8String);
        MonoImage *image = mono_assembly_get_image(assembly);

        MonoMethodDesc *desc = mono_method_desc_new("PluginMsgHandler:InitializeHandlerByNative()", FALSE);
        MonoMethod *method = mono_method_desc_search_in_image(desc, image);
        mono_runtime_invoke(method, NULL, NULL, NULL);
        mono_method_desc_free(desc);
    } */

    void _iOS_InitPluginMsgHandler(const char* unityName)
    {
        [EditBox initializeEditBox:UnityGetGLViewController() unityName:unityName];
        NSLog(@"_iOS_InitPluginMsgHandler called");
    }
    
    char* _iOS_SendUnityMsgToPlugin(int nSenderId, const char* jsonMsgApp)
    {
        NSString* strJson = [NSString stringWithUTF8String:jsonMsgApp];
        JsonObject* jsonMsg = [[JsonObject alloc] initWithJsonData:[strJson dataUsingEncoding:NSUTF8StringEncoding]];
        JsonObject* jsonRet = [EditBox processRecvJsonMsg:nSenderId msg:jsonMsg];
        
        NSString* strRet = [[NSString alloc] initWithData:[jsonRet serialize] encoding:NSUTF8StringEncoding];
        return gCopyString([strRet UTF8String]);
    }
    void _iOS_ClosePluginMsgHandler()
    {
        [EditBox finalizeEditBox];
    }
}