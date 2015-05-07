//
//  JsonObject.m
//  stockissue
//
//  Created by KYUNGMIN BANG on 12/8/14.
//  Copyright (c) 2014 Nureka Inc. All rights reserved.
//

#import "JsonObject.h"


@implementation JsonObject
@synthesize dict;

-(void) dealloc
{
    dict = nil;
}

-(id)init
{
    self = [super init];
    if (self)
    {
        dict = [[NSMutableDictionary alloc] init];
        
    }
    return self;
}

-(id)initWithDictionary:(NSMutableDictionary*) _dict
{
    self = [super init];
    if (self)
    {
        dict = _dict;
        
    }
    return self;
}

-(id)initWithJsonData:(NSData*) json
{
    self = [super init];
    if (self)
    {
        NSError *error;
        
        dict = [NSJSONSerialization
                JSONObjectWithData:json
                options:NSJSONReadingMutableContainers
                error:&error];
        if (dict == nil)
        {
            NSLog(@"Json parse error %@",[error description]);
        }
    }
    return self;
}
-(id)initWithCmd:(NSString*) cmd
{
    self = [self init];
    if (dict)
    {
        dict[@"cmd"] = cmd;
    }
    return self;
}

-(NSData*)serialize
{
    NSError *error;
    NSData *jsonData = [NSJSONSerialization dataWithJSONObject:dict
                                            options:0
                                            error:&error];
    return  jsonData;
}

-(void) setInt:(NSString*) key value:(int)value
{
    dict[key] = [NSNumber numberWithInt:value];
}
-(void) setFloat:(NSString*) key value:(float)value
{
    dict[key] = [NSNumber numberWithFloat:value];
}
-(void) setBool:(NSString*) key value:(BOOL)value
{
    dict[key] = [NSNumber numberWithBool:value];
}
-(void) setString:(NSString*) key value:(NSString*)value
{
    dict[key] = value;
}
-(void) setArray:(NSString*) key value:(NSArray*) value
{
    dict[key] = value;
}
-(void) setJsonObject:(NSString*) key value:(JsonObject*) value
{
    dict[key] = value.dict;
}

-(int) getInt:(NSString*) key
{
    id obj = [dict objectForKey:key];
    if (obj == nil || obj == (id)[NSNull null]) return 0;
    return [obj intValue];
}
-(float) getFloat:(NSString*) key
{
    id obj = [dict objectForKey:key];
    if (obj == nil || obj == (id)[NSNull null]) return 0.0f;
    return [obj floatValue];
}
-(BOOL) getBool:(NSString*) key
{
    id obj = [dict objectForKey:key];
    if (obj == nil || obj == (id)[NSNull null]) return NO;
    return [obj boolValue];
}
-(NSString*) getString:(NSString*) key
{
    id obj = [dict objectForKey:key];
    if (!obj || obj == (id)[NSNull null]) return @"";
    return (NSString*) obj;
}
-(NSArray*) getDictArray:(NSString*) key
{
    id obj = [dict objectForKey:key];
    if (obj == nil || obj == (id)[NSNull null]) return [[NSArray alloc] init];
    return (NSArray*) obj;
}
-(NSArray*) getJsonArray:(NSString*) key
{
    id obj = [dict objectForKey:key];
    NSMutableArray* arr = [[NSMutableArray alloc] init];
    if (obj == nil || obj == (id)[NSNull null]) return arr;
    for (id arrObj in (NSMutableArray*) obj) {
        JsonObject* newObj = [[JsonObject alloc] initWithDictionary:(NSMutableDictionary*) arrObj];
        [arr addObject:newObj];
    }
    return (NSArray*) arr;
}
-(JsonObject*) getJsonObject:(NSString*) key
{
    id obj = [dict objectForKey:key];
    JsonObject* newObj = [[JsonObject alloc] init];
    if (obj == nil || obj == (id)[NSNull null]) return newObj;
    newObj.dict = (NSMutableDictionary*) obj;
    return newObj;
}

@end
