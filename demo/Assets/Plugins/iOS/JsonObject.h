//
//  JsonObject.h
//  stockissue
//
//  Created by KYUNGMIN BANG on 12/8/14.
//  Copyright (c) 2014 Nureka Inc. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface JsonObject : NSObject

@property (nonatomic, strong) NSMutableDictionary* dict;

-(id)initWithJsonData:(NSData*) json;
-(id)initWithCmd:(NSString*) cmd;
-(id)initWithDictionary:(NSMutableDictionary*) _dict;
-(NSData*)serialize;

-(void) setInt:(NSString*) key value:(int)value;
-(void) setFloat:(NSString*) key value:(float)value;
-(void) setBool:(NSString*) key value:(BOOL)value;
-(void) setString:(NSString*) key value:(NSString*)value;
-(void) setArray:(NSString*) key value:(NSArray*) value;
-(void) setJsonObject:(NSString*) key value:(JsonObject*) value;

-(int) getInt:(NSString*) key;
-(float) getFloat:(NSString*) key;
-(BOOL) getBool:(NSString*) key;
-(NSString*) getString:(NSString*) key;
-(NSArray*) getJsonArray:(NSString*) key;
-(NSArray*) getDictArray:(NSString*) key;
-(JsonObject*) getJsonObject:(NSString*) key;

@end

