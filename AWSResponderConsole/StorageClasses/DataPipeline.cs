using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AWSResponderConsole
{
    public class DataPipeline
    {
        /// <summary>
        /// A list of all the pipeline identifiers that your account has permission to access. If you require additional information about the pipelines, you can use these identifiers to call DescribePipelines and GetPipelineDefinition. 
        /// </summary>
        public ListComparisonResults<Amazon.DataPipeline.Model.PipelineIdName> PipelineIdList { get; set; }
        public ListComparisonResults<Amazon.DataPipeline.Model.PipelineDescription> PipelineDescriptionList { get; set; }
        public ListComparisonResults<DataPipelineObject> DataPipelineObjects { get; set; }
    }
    public class DataPipelineObject
    {
        public List<Amazon.DataPipeline.Model.PipelineObject> PipelineObjects { get; set; }
        public Amazon.DataPipeline.Model.PipelineIdName IdName { get; set; }
        public DataPipelineObject(
            Amazon.DataPipeline.Model.PipelineIdName idName,
            List<Amazon.DataPipeline.Model.PipelineObject> pipelineObjects
            )
        {
            PipelineObjects = pipelineObjects;
            IdName = idName;
        }
    }
}
