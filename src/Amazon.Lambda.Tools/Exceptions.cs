﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Common.DotNetCli.Tools;
using Amazon.Runtime;

namespace Amazon.Lambda.Tools
{
    /// <summary>
    /// The deploy tool exception. This is used to throw back an error to the user but is considerd a known error
    /// so the stack trace will not be displayed.
    /// </summary>
    public class LambdaToolsException : ToolsException
    {
        public enum LambdaErrorCode {

            CloudFormationCreateChangeSet,
            CloudFormationCreateStack,
            CloudFormationDeleteStack,
            CloudFormationDescribeChangeSet,
            CloudFormationDescribeStack,
            CloudFormationDescribeStackEvents,

            InvalidPackage,
            FrameworkNewerThanRuntime,

            AspNetCoreAllValidation,

            IAMAttachRole,
            IAMCreateRole,
            IAMGetRole,

            LambdaFunctionUrlGet,
            LambdaFunctionUrlCreate,
            LambdaFunctionUrlUpdate,
            LambdaFunctionUrlDelete,

            LambdaCreateFunction,
            LambdaDeleteFunction,
            LambdaGetConfiguration,
            LambdaInvokeFunction,
            LambdaListFunctions,
            LambdaUpdateFunctionCode,
            LambdaUpdateFunctionConfiguration,
            LambdaPublishFunction,
            LambdaTaggingFunction,
            LambdaPublishLayerVersion,
            LambdaListLayers,
            LambdaListLayerVersions,
            LambdaGetLayerVersionDetails,
            LambdaDeleteLayerVersion,
            ParseLayerVersionArnFail,
            LambdaWaitTillFunctionAvailable,

            UnknownLayerType,
            StoreCommandError,
            FailedToFindArtifactZip,
            LayerPackageManifestNotFound,
            UnsupportedOptimizationPlatform,

            ServerlessTemplateNotFound,
            ServerlessTemplateParseError,
            ServerlessTemplateMissingResourceSection,
            ServerlessTemplateSubstitutionError,
            ServerlessTemplateMissingLocalPath,
            ServerlessTemplateUnknownActionForLocalPath,
            WaitingForStackError,

            FailedToFindZipProgram,
            FailedToDetectSdkVersion,
            LayerNetSdkVersionMismatch,

            DisabledSupportForNET31Layers
        }

        public LambdaToolsException(string message, LambdaErrorCode code) : base(message, code.ToString(), null)
        {
        }

        public LambdaToolsException(string message, CommonErrorCode code) : base(message, code.ToString(), null)
        {
        }

        public LambdaToolsException(string message, LambdaErrorCode code, Exception e) : base(message, code.ToString(), e)
        {
        }
    }


}
