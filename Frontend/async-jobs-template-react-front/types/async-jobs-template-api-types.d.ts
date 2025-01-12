/**
 * This file was auto-generated by openapi-typescript.
 * Do not make direct changes to the file.
 */

export interface paths {
    "/": {
        parameters: {
            query?: never;
            header?: never;
            path?: never;
            cookie?: never;
        };
        /** Get basic information about the app */
        get: {
            parameters: {
                query?: never;
                header?: never;
                path?: never;
                cookie?: never;
            };
            requestBody?: never;
            responses: {
                /** @description Correct response */
                200: {
                    headers: {
                        [name: string]: unknown;
                    };
                    content: {
                        "application/json": components["schemas"]["GetAppInfoResponse"];
                    };
                };
            };
        };
        put?: never;
        post?: never;
        delete?: never;
        options?: never;
        head?: never;
        patch?: never;
        trace?: never;
    };
    "/health-check": {
        parameters: {
            query?: never;
            header?: never;
            path?: never;
            cookie?: never;
        };
        /** Check health */
        get: {
            parameters: {
                query?: never;
                header: {
                    "api-key": string;
                };
                path?: never;
                cookie?: never;
            };
            requestBody?: never;
            responses: {
                /** @description OK */
                200: {
                    headers: {
                        [name: string]: unknown;
                    };
                    content: {
                        "application/json": components["schemas"]["CheckHealthResponse"];
                    };
                };
                /** @description Service Unavailable */
                503: {
                    headers: {
                        [name: string]: unknown;
                    };
                    content: {
                        "application/json": components["schemas"]["CheckHealthResponse"];
                    };
                };
            };
        };
        put?: never;
        post?: never;
        delete?: never;
        options?: never;
        head?: never;
        patch?: never;
        trace?: never;
    };
    "/jobs/{jobCategoryName}": {
        parameters: {
            query?: never;
            header?: never;
            path?: never;
            cookie?: never;
        };
        get?: never;
        put?: never;
        /** Trigger a job with a JSON payload */
        post: {
            parameters: {
                query?: never;
                header?: never;
                path: {
                    jobCategoryName: string;
                };
                cookie?: never;
            };
            requestBody?: {
                content: {
                    "application/json": unknown;
                    "text/json": unknown;
                    "application/*+json": unknown;
                };
            };
            responses: {
                /** @description Correct response */
                200: {
                    headers: {
                        [name: string]: unknown;
                    };
                    content: {
                        "application/json": components["schemas"]["TriggerJobResponse"];
                    };
                };
                /** @description Incorrect request */
                400: {
                    headers: {
                        [name: string]: unknown;
                    };
                    content: {
                        "application/json": components["schemas"]["TriggerJobResponse"];
                    };
                };
                /** @description Internal error */
                500: {
                    headers: {
                        [name: string]: unknown;
                    };
                    content: {
                        "application/json": components["schemas"]["TriggerJobResponse"];
                    };
                };
            };
        };
        delete?: never;
        options?: never;
        head?: never;
        patch?: never;
        trace?: never;
    };
    "/jobs/{jobCategoryName}/file": {
        parameters: {
            query?: never;
            header?: never;
            path?: never;
            cookie?: never;
        };
        get?: never;
        put?: never;
        /** Trigger a job with a file */
        post: {
            parameters: {
                query?: never;
                header?: never;
                path: {
                    jobCategoryName: string;
                };
                cookie?: never;
            };
            requestBody?: {
                content: {
                    "multipart/form-data": {
                        /** Format: binary */
                        file?: string;
                    };
                };
            };
            responses: {
                /** @description Correct response */
                200: {
                    headers: {
                        [name: string]: unknown;
                    };
                    content: {
                        "application/json": components["schemas"]["TriggerJobResponse"];
                    };
                };
                /** @description Incorrect request */
                400: {
                    headers: {
                        [name: string]: unknown;
                    };
                    content: {
                        "application/json": components["schemas"]["TriggerJobResponse"];
                    };
                };
                /** @description Internal error */
                500: {
                    headers: {
                        [name: string]: unknown;
                    };
                    content: {
                        "application/json": components["schemas"]["TriggerJobResponse"];
                    };
                };
            };
        };
        delete?: never;
        options?: never;
        head?: never;
        patch?: never;
        trace?: never;
    };
    "/jobs/{jobId}": {
        parameters: {
            query?: never;
            header?: never;
            path?: never;
            cookie?: never;
        };
        /** Get a job */
        get: {
            parameters: {
                query?: never;
                header?: never;
                path: {
                    jobId: string;
                };
                cookie?: never;
            };
            requestBody?: never;
            responses: {
                /** @description Correct response */
                200: {
                    headers: {
                        [name: string]: unknown;
                    };
                    content: {
                        "application/json": components["schemas"]["GetJobResult"];
                    };
                };
                /** @description Job doesn't exist */
                404: {
                    headers: {
                        [name: string]: unknown;
                    };
                    content: {
                        "application/json": components["schemas"]["GetJobResult"];
                    };
                };
                /** @description Internal Server Error */
                500: {
                    headers: {
                        [name: string]: unknown;
                    };
                    content?: never;
                };
            };
        };
        put?: never;
        post?: never;
        delete?: never;
        options?: never;
        head?: never;
        patch?: never;
        trace?: never;
    };
    "/jobs": {
        parameters: {
            query?: never;
            header?: never;
            path?: never;
            cookie?: never;
        };
        /** Get jobs */
        get: {
            parameters: {
                query?: {
                    page?: number;
                    pageSize?: number;
                };
                header?: never;
                path?: never;
                cookie?: never;
            };
            requestBody?: never;
            responses: {
                /** @description Correct response */
                200: {
                    headers: {
                        [name: string]: unknown;
                    };
                    content: {
                        "application/json": components["schemas"]["GetJobsResult"];
                    };
                };
            };
        };
        put?: never;
        post?: never;
        delete?: never;
        options?: never;
        head?: never;
        patch?: never;
        trace?: never;
    };
    "/jobs/{jobId}/file": {
        parameters: {
            query?: never;
            header?: never;
            path?: never;
            cookie?: never;
        };
        /** Download a job file */
        get: {
            parameters: {
                query?: never;
                header?: never;
                path: {
                    jobId: string;
                };
                cookie?: never;
            };
            requestBody?: never;
            responses: {
                /** @description OK */
                200: {
                    headers: {
                        [name: string]: unknown;
                    };
                    content?: never;
                };
                /** @description Job file doesn't exist */
                404: {
                    headers: {
                        [name: string]: unknown;
                    };
                    content?: never;
                };
            };
        };
        put?: never;
        post?: never;
        delete?: never;
        options?: never;
        head?: never;
        patch?: never;
        trace?: never;
    };
}
export type webhooks = Record<string, never>;
export interface components {
    schemas: {
        Assembly: {
            readonly definedTypes?: components["schemas"]["TypeInfo"][] | null;
            readonly exportedTypes?: components["schemas"]["Type"][] | null;
            /** @deprecated */
            readonly codeBase?: string | null;
            entryPoint?: components["schemas"]["MethodInfo"];
            readonly fullName?: string | null;
            readonly imageRuntimeVersion?: string | null;
            readonly isDynamic?: boolean;
            readonly location?: string | null;
            readonly reflectionOnly?: boolean;
            readonly isCollectible?: boolean;
            readonly isFullyTrusted?: boolean;
            readonly customAttributes?: components["schemas"]["CustomAttributeData"][] | null;
            /** @deprecated */
            readonly escapedCodeBase?: string | null;
            manifestModule?: components["schemas"]["Module"];
            readonly modules?: components["schemas"]["Module"][] | null;
            /** @deprecated */
            readonly globalAssemblyCache?: boolean;
            /** Format: int64 */
            readonly hostContext?: number;
            securityRuleSet?: components["schemas"]["SecurityRuleSet"];
        };
        /**
         * Format: int32
         * @enum {integer}
         */
        CallingConventions: 1 | 2 | 3 | 32 | 64;
        CheckHealthResponse: {
            entries?: {
                [key: string]: components["schemas"]["HealthReportEntryDto"];
            } | null;
            status?: string | null;
            /** Format: date-span */
            totalDuration?: string;
        };
        ConstructorInfo: {
            readonly name?: string | null;
            declaringType?: components["schemas"]["Type"];
            reflectedType?: components["schemas"]["Type"];
            module?: components["schemas"]["Module"];
            readonly customAttributes?: components["schemas"]["CustomAttributeData"][] | null;
            readonly isCollectible?: boolean;
            /** Format: int32 */
            readonly metadataToken?: number;
            attributes?: components["schemas"]["MethodAttributes"];
            methodImplementationFlags?: components["schemas"]["MethodImplAttributes"];
            callingConvention?: components["schemas"]["CallingConventions"];
            readonly isAbstract?: boolean;
            readonly isConstructor?: boolean;
            readonly isFinal?: boolean;
            readonly isHideBySig?: boolean;
            readonly isSpecialName?: boolean;
            readonly isStatic?: boolean;
            readonly isVirtual?: boolean;
            readonly isAssembly?: boolean;
            readonly isFamily?: boolean;
            readonly isFamilyAndAssembly?: boolean;
            readonly isFamilyOrAssembly?: boolean;
            readonly isPrivate?: boolean;
            readonly isPublic?: boolean;
            readonly isConstructedGenericMethod?: boolean;
            readonly isGenericMethod?: boolean;
            readonly isGenericMethodDefinition?: boolean;
            readonly containsGenericParameters?: boolean;
            methodHandle?: components["schemas"]["RuntimeMethodHandle"];
            readonly isSecurityCritical?: boolean;
            readonly isSecuritySafeCritical?: boolean;
            readonly isSecurityTransparent?: boolean;
            memberType?: components["schemas"]["MemberTypes"];
        };
        CustomAttributeData: {
            attributeType?: components["schemas"]["Type"];
            constructor?: {
                readonly name?: string | null;
                declaringType?: components["schemas"]["Type"];
                reflectedType?: components["schemas"]["Type"];
                module?: components["schemas"]["Module"];
                readonly customAttributes?: components["schemas"]["CustomAttributeData"][] | null;
                readonly isCollectible?: boolean;
                /** Format: int32 */
                readonly metadataToken?: number;
                attributes?: components["schemas"]["MethodAttributes"];
                methodImplementationFlags?: components["schemas"]["MethodImplAttributes"];
                callingConvention?: components["schemas"]["CallingConventions"];
                readonly isAbstract?: boolean;
                readonly isConstructor?: boolean;
                readonly isFinal?: boolean;
                readonly isHideBySig?: boolean;
                readonly isSpecialName?: boolean;
                readonly isStatic?: boolean;
                readonly isVirtual?: boolean;
                readonly isAssembly?: boolean;
                readonly isFamily?: boolean;
                readonly isFamilyAndAssembly?: boolean;
                readonly isFamilyOrAssembly?: boolean;
                readonly isPrivate?: boolean;
                readonly isPublic?: boolean;
                readonly isConstructedGenericMethod?: boolean;
                readonly isGenericMethod?: boolean;
                readonly isGenericMethodDefinition?: boolean;
                readonly containsGenericParameters?: boolean;
                methodHandle?: components["schemas"]["RuntimeMethodHandle"];
                readonly isSecurityCritical?: boolean;
                readonly isSecuritySafeCritical?: boolean;
                readonly isSecurityTransparent?: boolean;
                memberType?: components["schemas"]["MemberTypes"];
            };
            readonly constructorArguments?: components["schemas"]["CustomAttributeTypedArgument"][] | null;
            readonly namedArguments?: components["schemas"]["CustomAttributeNamedArgument"][] | null;
        };
        CustomAttributeNamedArgument: {
            memberInfo?: components["schemas"]["MemberInfo"];
            typedValue?: components["schemas"]["CustomAttributeTypedArgument"];
            readonly memberName?: string | null;
            readonly isField?: boolean;
        };
        CustomAttributeTypedArgument: {
            argumentType?: components["schemas"]["Type"];
            value?: unknown;
        };
        /**
         * Format: int32
         * @enum {integer}
         */
        ErrorType: 0 | 1;
        /**
         * Format: int32
         * @enum {integer}
         */
        EventAttributes: 0 | 512 | 1024;
        EventInfo: {
            readonly name?: string | null;
            declaringType?: components["schemas"]["Type"];
            reflectedType?: components["schemas"]["Type"];
            module?: components["schemas"]["Module"];
            readonly customAttributes?: components["schemas"]["CustomAttributeData"][] | null;
            readonly isCollectible?: boolean;
            /** Format: int32 */
            readonly metadataToken?: number;
            memberType?: components["schemas"]["MemberTypes"];
            attributes?: components["schemas"]["EventAttributes"];
            readonly isSpecialName?: boolean;
            addMethod?: components["schemas"]["MethodInfo"];
            removeMethod?: components["schemas"]["MethodInfo"];
            raiseMethod?: components["schemas"]["MethodInfo"];
            readonly isMulticast?: boolean;
            eventHandlerType?: components["schemas"]["Type"];
        };
        Exception: {
            targetSite?: components["schemas"]["MethodBase"];
            readonly message?: string | null;
            readonly data?: {
                [key: string]: unknown;
            } | null;
            innerException?: components["schemas"]["Exception"];
            helpLink?: string | null;
            source?: string | null;
            /** Format: int32 */
            hResult?: number;
            readonly stackTrace?: string | null;
        };
        /**
         * Format: int32
         * @enum {integer}
         */
        FieldAttributes: 0 | 1 | 2 | 3 | 4 | 5 | 6 | 7 | 16 | 32 | 64 | 128 | 256 | 512 | 1024 | 4096 | 8192 | 32768 | 38144;
        FieldInfo: {
            readonly name?: string | null;
            declaringType?: components["schemas"]["Type"];
            reflectedType?: components["schemas"]["Type"];
            module?: components["schemas"]["Module"];
            readonly customAttributes?: components["schemas"]["CustomAttributeData"][] | null;
            readonly isCollectible?: boolean;
            /** Format: int32 */
            readonly metadataToken?: number;
            memberType?: components["schemas"]["MemberTypes"];
            attributes?: components["schemas"]["FieldAttributes"];
            fieldType?: components["schemas"]["Type"];
            readonly isInitOnly?: boolean;
            readonly isLiteral?: boolean;
            /** @deprecated */
            readonly isNotSerialized?: boolean;
            readonly isPinvokeImpl?: boolean;
            readonly isSpecialName?: boolean;
            readonly isStatic?: boolean;
            readonly isAssembly?: boolean;
            readonly isFamily?: boolean;
            readonly isFamilyAndAssembly?: boolean;
            readonly isFamilyOrAssembly?: boolean;
            readonly isPrivate?: boolean;
            readonly isPublic?: boolean;
            readonly isSecurityCritical?: boolean;
            readonly isSecuritySafeCritical?: boolean;
            readonly isSecurityTransparent?: boolean;
            fieldHandle?: components["schemas"]["RuntimeFieldHandle"];
        };
        /**
         * Format: int32
         * @enum {integer}
         */
        GenericParameterAttributes: 0 | 1 | 2 | 3 | 4 | 8 | 16 | 28 | 32;
        GetAppInfoResponse: {
            name?: string | null;
            version?: string | null;
        };
        GetJobResult: {
            status?: string | null;
            outputData?: unknown;
            outputFileReference?: string | null;
        };
        GetJobsResult: {
            jobs?: components["schemas"]["JobPaginatedList"];
        };
        HealthReportEntryDto: {
            data?: {
                [key: string]: unknown;
            } | null;
            description?: string | null;
            /** Format: date-span */
            duration?: string;
            exceptionInfo?: components["schemas"]["HealthReportEntryExceptionInfoDto"];
            status?: string | null;
            tags?: string[] | null;
        };
        HealthReportEntryExceptionInfoDto: {
            message?: string | null;
            stackTrace?: string | null;
        };
        ICustomAttributeProvider: Record<string, never>;
        IntPtr: Record<string, never>;
        Job: {
            /** Format: uuid */
            jobId?: string;
            jobCategoryName?: string | null;
            status?: components["schemas"]["JobStatus"];
            inputData?: unknown;
            inputFileReference?: string | null;
            outputData?: unknown;
            outputFileReference?: string | null;
            errors?: components["schemas"]["JobError"][] | null;
            /** Format: date-time */
            createdAt?: string;
            /** Format: date-time */
            lastUpdatedAt?: string | null;
        };
        JobError: {
            message?: string | null;
            exception?: components["schemas"]["Exception"];
            errorCode?: string | null;
            type?: components["schemas"]["ErrorType"];
        };
        JobPaginatedList: {
            data?: components["schemas"]["Job"][] | null;
            /** Format: int64 */
            count?: number;
        };
        /**
         * Format: int32
         * @enum {integer}
         */
        JobStatus: 0 | 1 | 2 | 3 | 4;
        /**
         * Format: int32
         * @enum {integer}
         */
        LayoutKind: 0 | 2 | 3;
        MemberInfo: {
            memberType?: components["schemas"]["MemberTypes"];
            readonly name?: string | null;
            declaringType?: components["schemas"]["Type"];
            reflectedType?: components["schemas"]["Type"];
            module?: components["schemas"]["Module"];
            readonly customAttributes?: components["schemas"]["CustomAttributeData"][] | null;
            readonly isCollectible?: boolean;
            /** Format: int32 */
            readonly metadataToken?: number;
        };
        /**
         * Format: int32
         * @enum {integer}
         */
        MemberTypes: 1 | 2 | 4 | 8 | 16 | 32 | 64 | 128 | 191;
        /**
         * Format: int32
         * @enum {integer}
         */
        MethodAttributes: 0 | 1 | 2 | 3 | 4 | 5 | 6 | 7 | 8 | 16 | 32 | 64 | 128 | 256 | 512 | 1024 | 2048 | 4096 | 8192 | 16384 | 32768 | 53248;
        MethodBase: {
            memberType?: components["schemas"]["MemberTypes"];
            readonly name?: string | null;
            declaringType?: components["schemas"]["Type"];
            reflectedType?: components["schemas"]["Type"];
            module?: components["schemas"]["Module"];
            readonly customAttributes?: components["schemas"]["CustomAttributeData"][] | null;
            readonly isCollectible?: boolean;
            /** Format: int32 */
            readonly metadataToken?: number;
            attributes?: components["schemas"]["MethodAttributes"];
            methodImplementationFlags?: components["schemas"]["MethodImplAttributes"];
            callingConvention?: components["schemas"]["CallingConventions"];
            readonly isAbstract?: boolean;
            readonly isConstructor?: boolean;
            readonly isFinal?: boolean;
            readonly isHideBySig?: boolean;
            readonly isSpecialName?: boolean;
            readonly isStatic?: boolean;
            readonly isVirtual?: boolean;
            readonly isAssembly?: boolean;
            readonly isFamily?: boolean;
            readonly isFamilyAndAssembly?: boolean;
            readonly isFamilyOrAssembly?: boolean;
            readonly isPrivate?: boolean;
            readonly isPublic?: boolean;
            readonly isConstructedGenericMethod?: boolean;
            readonly isGenericMethod?: boolean;
            readonly isGenericMethodDefinition?: boolean;
            readonly containsGenericParameters?: boolean;
            methodHandle?: components["schemas"]["RuntimeMethodHandle"];
            readonly isSecurityCritical?: boolean;
            readonly isSecuritySafeCritical?: boolean;
            readonly isSecurityTransparent?: boolean;
        };
        /**
         * Format: int32
         * @enum {integer}
         */
        MethodImplAttributes: 0 | 1 | 2 | 3 | 4 | 8 | 16 | 32 | 64 | 128 | 256 | 512 | 4096 | 65535;
        MethodInfo: {
            readonly name?: string | null;
            declaringType?: components["schemas"]["Type"];
            reflectedType?: components["schemas"]["Type"];
            module?: components["schemas"]["Module"];
            readonly customAttributes?: components["schemas"]["CustomAttributeData"][] | null;
            readonly isCollectible?: boolean;
            /** Format: int32 */
            readonly metadataToken?: number;
            attributes?: components["schemas"]["MethodAttributes"];
            methodImplementationFlags?: components["schemas"]["MethodImplAttributes"];
            callingConvention?: components["schemas"]["CallingConventions"];
            readonly isAbstract?: boolean;
            readonly isConstructor?: boolean;
            readonly isFinal?: boolean;
            readonly isHideBySig?: boolean;
            readonly isSpecialName?: boolean;
            readonly isStatic?: boolean;
            readonly isVirtual?: boolean;
            readonly isAssembly?: boolean;
            readonly isFamily?: boolean;
            readonly isFamilyAndAssembly?: boolean;
            readonly isFamilyOrAssembly?: boolean;
            readonly isPrivate?: boolean;
            readonly isPublic?: boolean;
            readonly isConstructedGenericMethod?: boolean;
            readonly isGenericMethod?: boolean;
            readonly isGenericMethodDefinition?: boolean;
            readonly containsGenericParameters?: boolean;
            methodHandle?: components["schemas"]["RuntimeMethodHandle"];
            readonly isSecurityCritical?: boolean;
            readonly isSecuritySafeCritical?: boolean;
            readonly isSecurityTransparent?: boolean;
            memberType?: components["schemas"]["MemberTypes"];
            returnParameter?: components["schemas"]["ParameterInfo"];
            returnType?: components["schemas"]["Type"];
            returnTypeCustomAttributes?: components["schemas"]["ICustomAttributeProvider"];
        };
        Module: {
            assembly?: components["schemas"]["Assembly"];
            readonly fullyQualifiedName?: string | null;
            readonly name?: string | null;
            /** Format: int32 */
            readonly mdStreamVersion?: number;
            /** Format: uuid */
            readonly moduleVersionId?: string;
            readonly scopeName?: string | null;
            moduleHandle?: components["schemas"]["ModuleHandle"];
            readonly customAttributes?: components["schemas"]["CustomAttributeData"][] | null;
            /** Format: int32 */
            readonly metadataToken?: number;
        };
        ModuleHandle: {
            /** Format: int32 */
            readonly mdStreamVersion?: number;
        };
        /**
         * Format: int32
         * @enum {integer}
         */
        ParameterAttributes: 0 | 1 | 2 | 4 | 8 | 16 | 4096 | 8192 | 16384 | 32768 | 61440;
        ParameterInfo: {
            attributes?: components["schemas"]["ParameterAttributes"];
            member?: components["schemas"]["MemberInfo"];
            readonly name?: string | null;
            parameterType?: components["schemas"]["Type"];
            /** Format: int32 */
            readonly position?: number;
            readonly isIn?: boolean;
            readonly isLcid?: boolean;
            readonly isOptional?: boolean;
            readonly isOut?: boolean;
            readonly isRetval?: boolean;
            readonly defaultValue?: unknown;
            readonly rawDefaultValue?: unknown;
            readonly hasDefaultValue?: boolean;
            readonly customAttributes?: components["schemas"]["CustomAttributeData"][] | null;
            /** Format: int32 */
            readonly metadataToken?: number;
        };
        /**
         * Format: int32
         * @enum {integer}
         */
        PropertyAttributes: 0 | 512 | 1024 | 4096 | 8192 | 16384 | 32768 | 62464;
        PropertyInfo: {
            readonly name?: string | null;
            declaringType?: components["schemas"]["Type"];
            reflectedType?: components["schemas"]["Type"];
            module?: components["schemas"]["Module"];
            readonly customAttributes?: components["schemas"]["CustomAttributeData"][] | null;
            readonly isCollectible?: boolean;
            /** Format: int32 */
            readonly metadataToken?: number;
            memberType?: components["schemas"]["MemberTypes"];
            propertyType?: components["schemas"]["Type"];
            attributes?: components["schemas"]["PropertyAttributes"];
            readonly isSpecialName?: boolean;
            readonly canRead?: boolean;
            readonly canWrite?: boolean;
            getMethod?: components["schemas"]["MethodInfo"];
            setMethod?: components["schemas"]["MethodInfo"];
        };
        RuntimeFieldHandle: {
            value?: components["schemas"]["IntPtr"];
        };
        RuntimeMethodHandle: {
            value?: components["schemas"]["IntPtr"];
        };
        RuntimeTypeHandle: {
            value?: components["schemas"]["IntPtr"];
        };
        /**
         * Format: int32
         * @enum {integer}
         */
        SecurityRuleSet: 0 | 1 | 2;
        StructLayoutAttribute: {
            readonly typeId?: unknown;
            value?: components["schemas"]["LayoutKind"];
        };
        TriggerJobResponse: {
            result?: boolean;
            /** Format: uuid */
            jobId?: string | null;
        };
        Type: {
            readonly name?: string | null;
            readonly customAttributes?: components["schemas"]["CustomAttributeData"][] | null;
            readonly isCollectible?: boolean;
            /** Format: int32 */
            readonly metadataToken?: number;
            memberType?: components["schemas"]["MemberTypes"];
            readonly namespace?: string | null;
            readonly assemblyQualifiedName?: string | null;
            readonly fullName?: string | null;
            assembly?: components["schemas"]["Assembly"];
            module?: components["schemas"]["Module"];
            readonly isInterface?: boolean;
            readonly isNested?: boolean;
            declaringType?: components["schemas"]["Type"];
            declaringMethod?: components["schemas"]["MethodBase"];
            reflectedType?: components["schemas"]["Type"];
            underlyingSystemType?: components["schemas"]["Type"];
            readonly isTypeDefinition?: boolean;
            readonly isArray?: boolean;
            readonly isByRef?: boolean;
            readonly isPointer?: boolean;
            readonly isConstructedGenericType?: boolean;
            readonly isGenericParameter?: boolean;
            readonly isGenericTypeParameter?: boolean;
            readonly isGenericMethodParameter?: boolean;
            readonly isGenericType?: boolean;
            readonly isGenericTypeDefinition?: boolean;
            readonly isSZArray?: boolean;
            readonly isVariableBoundArray?: boolean;
            readonly isByRefLike?: boolean;
            readonly isFunctionPointer?: boolean;
            readonly isUnmanagedFunctionPointer?: boolean;
            readonly hasElementType?: boolean;
            readonly genericTypeArguments?: components["schemas"]["Type"][] | null;
            /** Format: int32 */
            readonly genericParameterPosition?: number;
            genericParameterAttributes?: components["schemas"]["GenericParameterAttributes"];
            attributes?: components["schemas"]["TypeAttributes"];
            readonly isAbstract?: boolean;
            readonly isImport?: boolean;
            readonly isSealed?: boolean;
            readonly isSpecialName?: boolean;
            readonly isClass?: boolean;
            readonly isNestedAssembly?: boolean;
            readonly isNestedFamANDAssem?: boolean;
            readonly isNestedFamily?: boolean;
            readonly isNestedFamORAssem?: boolean;
            readonly isNestedPrivate?: boolean;
            readonly isNestedPublic?: boolean;
            readonly isNotPublic?: boolean;
            readonly isPublic?: boolean;
            readonly isAutoLayout?: boolean;
            readonly isExplicitLayout?: boolean;
            readonly isLayoutSequential?: boolean;
            readonly isAnsiClass?: boolean;
            readonly isAutoClass?: boolean;
            readonly isUnicodeClass?: boolean;
            readonly isCOMObject?: boolean;
            readonly isContextful?: boolean;
            readonly isEnum?: boolean;
            readonly isMarshalByRef?: boolean;
            readonly isPrimitive?: boolean;
            readonly isValueType?: boolean;
            readonly isSignatureType?: boolean;
            readonly isSecurityCritical?: boolean;
            readonly isSecuritySafeCritical?: boolean;
            readonly isSecurityTransparent?: boolean;
            structLayoutAttribute?: components["schemas"]["StructLayoutAttribute"];
            typeInitializer?: components["schemas"]["ConstructorInfo"];
            typeHandle?: components["schemas"]["RuntimeTypeHandle"];
            /** Format: uuid */
            readonly guid?: string;
            baseType?: components["schemas"]["Type"];
            /** @deprecated */
            readonly isSerializable?: boolean;
            readonly containsGenericParameters?: boolean;
            readonly isVisible?: boolean;
        };
        /**
         * Format: int32
         * @enum {integer}
         */
        TypeAttributes: 0 | 1 | 2 | 3 | 4 | 5 | 6 | 7 | 8 | 16 | 24 | 32 | 128 | 256 | 1024 | 2048 | 4096 | 8192 | 16384 | 65536 | 131072 | 196608 | 262144 | 264192 | 1048576 | 12582912;
        TypeInfo: {
            readonly name?: string | null;
            readonly customAttributes?: components["schemas"]["CustomAttributeData"][] | null;
            readonly isCollectible?: boolean;
            /** Format: int32 */
            readonly metadataToken?: number;
            memberType?: components["schemas"]["MemberTypes"];
            readonly namespace?: string | null;
            readonly assemblyQualifiedName?: string | null;
            readonly fullName?: string | null;
            assembly?: components["schemas"]["Assembly"];
            module?: components["schemas"]["Module"];
            readonly isInterface?: boolean;
            readonly isNested?: boolean;
            declaringType?: components["schemas"]["Type"];
            declaringMethod?: components["schemas"]["MethodBase"];
            reflectedType?: components["schemas"]["Type"];
            underlyingSystemType?: components["schemas"]["Type"];
            readonly isTypeDefinition?: boolean;
            readonly isArray?: boolean;
            readonly isByRef?: boolean;
            readonly isPointer?: boolean;
            readonly isConstructedGenericType?: boolean;
            readonly isGenericParameter?: boolean;
            readonly isGenericTypeParameter?: boolean;
            readonly isGenericMethodParameter?: boolean;
            readonly isGenericType?: boolean;
            readonly isGenericTypeDefinition?: boolean;
            readonly isSZArray?: boolean;
            readonly isVariableBoundArray?: boolean;
            readonly isByRefLike?: boolean;
            readonly isFunctionPointer?: boolean;
            readonly isUnmanagedFunctionPointer?: boolean;
            readonly hasElementType?: boolean;
            readonly genericTypeArguments?: components["schemas"]["Type"][] | null;
            /** Format: int32 */
            readonly genericParameterPosition?: number;
            genericParameterAttributes?: components["schemas"]["GenericParameterAttributes"];
            attributes?: components["schemas"]["TypeAttributes"];
            readonly isAbstract?: boolean;
            readonly isImport?: boolean;
            readonly isSealed?: boolean;
            readonly isSpecialName?: boolean;
            readonly isClass?: boolean;
            readonly isNestedAssembly?: boolean;
            readonly isNestedFamANDAssem?: boolean;
            readonly isNestedFamily?: boolean;
            readonly isNestedFamORAssem?: boolean;
            readonly isNestedPrivate?: boolean;
            readonly isNestedPublic?: boolean;
            readonly isNotPublic?: boolean;
            readonly isPublic?: boolean;
            readonly isAutoLayout?: boolean;
            readonly isExplicitLayout?: boolean;
            readonly isLayoutSequential?: boolean;
            readonly isAnsiClass?: boolean;
            readonly isAutoClass?: boolean;
            readonly isUnicodeClass?: boolean;
            readonly isCOMObject?: boolean;
            readonly isContextful?: boolean;
            readonly isEnum?: boolean;
            readonly isMarshalByRef?: boolean;
            readonly isPrimitive?: boolean;
            readonly isValueType?: boolean;
            readonly isSignatureType?: boolean;
            readonly isSecurityCritical?: boolean;
            readonly isSecuritySafeCritical?: boolean;
            readonly isSecurityTransparent?: boolean;
            structLayoutAttribute?: components["schemas"]["StructLayoutAttribute"];
            typeInitializer?: components["schemas"]["ConstructorInfo"];
            typeHandle?: components["schemas"]["RuntimeTypeHandle"];
            /** Format: uuid */
            readonly guid?: string;
            baseType?: components["schemas"]["Type"];
            /** @deprecated */
            readonly isSerializable?: boolean;
            readonly containsGenericParameters?: boolean;
            readonly isVisible?: boolean;
            readonly genericTypeParameters?: components["schemas"]["Type"][] | null;
            readonly declaredConstructors?: components["schemas"]["ConstructorInfo"][] | null;
            readonly declaredEvents?: components["schemas"]["EventInfo"][] | null;
            readonly declaredFields?: components["schemas"]["FieldInfo"][] | null;
            readonly declaredMembers?: components["schemas"]["MemberInfo"][] | null;
            readonly declaredMethods?: components["schemas"]["MethodInfo"][] | null;
            readonly declaredNestedTypes?: components["schemas"]["TypeInfo"][] | null;
            readonly declaredProperties?: components["schemas"]["PropertyInfo"][] | null;
            readonly implementedInterfaces?: components["schemas"]["Type"][] | null;
        };
    };
    responses: never;
    parameters: never;
    requestBodies: never;
    headers: never;
    pathItems: never;
}
export type $defs = Record<string, never>;
export type operations = Record<string, never>;
