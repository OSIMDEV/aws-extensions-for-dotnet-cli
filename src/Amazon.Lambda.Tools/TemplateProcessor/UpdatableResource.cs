﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace Amazon.Lambda.Tools.TemplateProcessor
{

    /// <summary>
    /// The updatable resource like a CloudFormation AWS::Lambda::Function. This class combines the UpdatableResourceDefinition
    /// which identifies the fields that can be updated and IUpdatableResourceDataSource which abstracts the JSON or YAML definition.
    /// </summary>
    public class UpdatableResource : IUpdatableResource
    {
        public string Name { get; }
        public string ResourceType { get; }
        public IList<IUpdateResourceField> Fields { get; }
        
        UpdatableResourceDefinition Definition { get; } 
        IUpdatableResourceDataSource DataSource { get; }

        public UpdatableResource(string name, UpdatableResourceDefinition definition, IUpdatableResourceDataSource dataSource)
        {
            this.Name = name;
            this.Definition = definition;
            this.DataSource = dataSource;
            
            this.Fields = new List<IUpdateResourceField>();
            foreach (var fieldDefinition in definition.Fields)
            {
                this.Fields.Add(new UpdatableResourceField(this, fieldDefinition, dataSource));
            }
        }

        public string LambdaRuntime
        {
            get
            {
                var runtime = this.DataSource.GetValue("Runtime");
                if(string.IsNullOrEmpty(runtime))
                {
                    runtime = this.DataSource.GetValueFromRoot("Globals", "Function", "Runtime");
                }

                return runtime;
            }
        }

        public string LambdaArchitecture
        {
            get
            {
                var architectures = this.DataSource.GetValueList("Architectures");
                if(architectures == null || architectures.Length == 0)
                {
                    architectures = this.DataSource.GetValueListFromRoot("Globals", "Function", "Architectures");
                }

                if(architectures == null || architectures.Length == 0)
                {
                    return LambdaConstants.ARCHITECTURE_X86_64;
                }
                else if(architectures.Length == 1)
                {
                    return architectures[0];
                }
                else
                {
                    throw new LambdaToolsException("More then one architecture was specified. .NET Lambda functions only support a single architecture value for creating a deployment bundle for the specific architecture.", Common.DotNetCli.Tools.ToolsException.CommonErrorCode.InvalidParameterValue);
                }
            }
        }

        public string[] LambdaLayers
        {
            get
            {
                var layers = new List<string>();
                
                var resourceLayers = this.DataSource.GetValueList("Layers");
                if (resourceLayers != null)
                {
                    layers.AddRange(resourceLayers);
                }
                
                
                var globalLayers = this.DataSource.GetValueListFromRoot("Globals", "Function", "Layers");
                if (globalLayers != null)
                {
                    layers.AddRange(globalLayers);
                }

                return layers.Count == 0 ? null : layers.ToArray();
            }
        }

        public CodeUploadType UploadType
        {
            get
            {
                var packageType = this.DataSource.GetValue("PackageType");
                if (string.Equals("image", packageType, StringComparison.OrdinalIgnoreCase))
                {
                    return CodeUploadType.Image;
                }

                return CodeUploadType.Zip;
            }
        }

        public void SetEnvironmentVariable(string key, string value)
        {
            this.DataSource.SetValue(value, "Environment", "Variables", key);
        }

        public class UpdatableResourceField : IUpdateResourceField
        {
            public IUpdatableResource Resource => this._resource;
            public UpdatableResource _resource;

            private UpdatableResourceDefinition.FieldDefinition Field { get; }

            private IUpdatableResourceDataSource DataSource;

            public UpdatableResourceField(UpdatableResource resource, UpdatableResourceDefinition.FieldDefinition field, IUpdatableResourceDataSource dataSource)
            {
                this._resource = resource;
                this.Field = field;
                this.DataSource = dataSource;
            }

            public string Name => this.Field.Name;

            public bool IsCode
            {
                get
                {
                    if (!this.Field.IsCode)
                    {
                        return false;
                    }
                    if(!string.IsNullOrEmpty(this._resource.DataSource.GetValue("Code", "ZipFile")))
                    {
                        // The template contains embedded code.
                        return false;
                    }                     

                    return true;
                }
            }

            public string GetLocalPath()
            {
                return this.Field.GetLocalPath(this._resource.DataSource);
            }

            public void SetS3Location(string s3Bucket, string s3Key)
            {
                this.Field.SetS3Location(this._resource.DataSource, s3Bucket, s3Key);
            }

            public void SetImageUri(string imageUri)
            {
                this.Field.SetImageUri(this._resource.DataSource, imageUri);
            }

            public string GetMetadataDockerfile()
            {
                return this.DataSource.GetValueFromResource("Metadata", "Dockerfile");
            }

            public string GetMetadataDockerTag()
            {
                return this.DataSource.GetValueFromResource("Metadata", "DockerTag");
            }
        }
    }
}
