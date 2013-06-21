using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AWSResponderConsole
{
    public class AmazonObjectReader
    {
        private Amazon.Runtime.SessionAWSCredentials _credentials;
        public AmazonObjectReader(Amazon.Runtime.SessionAWSCredentials AWSCredentials)
        {
            _credentials = AWSCredentials;
        }
        public List<string> GetObjects(List<string> path)
        {
            List<string> result = new List<string>();
            switch (path[0])
            {
                case "IdentityAccountManagement":
                    result = GetIdentityAccountManagementObjects(path);
                    break;
                case "AutoScaling":
                    result = GetAutoScalingObjects(path);
                    break;
                case "CloudFront":
                    result= GetCloudFrontObjects(path);
                    break;
                case "CloudSearch":
                    result=GetCloudSearchObjects(path);
                    break;
                case "DataPipeline":
                    result = GetDataPipelineObjects(path);
                    break;
                case "ElasticCloudComputing":
                    result=GetElasticCloudComputingObjects(path);
                    break;
                case "ElasticMapReduce":
                    result=GetElasticMapReduceObjects(path);
                    break;
                case "ElasticTranscoder":
                    result = GetElasticTranscoderObjects(path);
                    break;
                case "DirectConnect":
                    result= GetDirectConnectObjects(path);
                    break;
                case "DynamoDB":
                    result = GetDynamoDBObjects(path);
                    break;
                case "ElastiCache":
                    result=GetElastiCacheObjects(path);
                    break;
                case "ElastiBeanstalk":
                    result = GetElastiBeanstalkObjects(path);
                    break;
                case "ElasticLoadBalancing":
                    result=GetElasticLoadBalancingObjects(path);
                    break;
                case "Glacier":
                    result=GetGlacierObjects(path);
                    break;
                case "ImportExport":
                    result = GetImportExportObjects(path);
                    break;
                case "OpsWorks":
                    result = GetOpsWorksObjects(path);
                    break;
                case "Route53":
                    result = GetRoute53Objects(path);
                    break;
                case "Redshift":
                    result =GetRedshiftObjects(path);
                    break;
                case "SimpleEmail":
                    result=GetSimpleEmailObjects(path);
                    break;
                case "SimpleNotificationService":
                    result=GetSimpleNotificationServiceObjects(path);
                    break;
                case "SimpleWorkflow":
                    result=GetSimpleWorkflowObjects(path);
                    break;
                case "SQSService":
                    result=GetSQSServiceObjects(path);
                    break;
            }

            return result;
        }

        public List<string> GetIdentityAccountManagementObjects(List<string> path)
        {
            List<string> result = new List<string>();
            
            return result;
        }
        public List<string> GetAutoScalingObjects(List<string> path)
        {
            List<string> result = new List<string>();
            return result;
        }
        public List<string> GetCloudFrontObjects(List<string> path)
        {
            List<string> result = new List<string>();
            return result;
        }
        public List<string> GetCloudSearchObjects(List<string> path)
        {
            List<string> result = new List<string>();
            return result;
        }
        public List<string> GetDataPipelineObjects(List<string> path)
        {
            List<string> result = new List<string>();
            return result;
        }
        public List<string> GetElasticCloudComputingObjects(List<string> path)
        {
            List<string> result = new List<string>();
            return result;
        }
        public List<string> GetElasticMapReduceObjects(List<string> path)
        {
            List<string> result = new List<string>();
            return result;
        }
        public List<string> GetElasticTranscoderObjects(List<string> path)
        {
            List<string> result = new List<string>();
            return result;
        }
        public List<string> GetDirectConnectObjects(List<string> path)
        {
            List<string> result = new List<string>();
            return result;
        }
        public List<string> GetDynamoDBObjects(List<string> path)
        {
            List<string> result = new List<string>();
            return result;
        }
        public List<string> GetElastiCacheObjects(List<string> path)
        {
            List<string> result = new List<string>();
            return result;
        }
        public List<string> GetElastiBeanstalkObjects(List<string> path)
        {
            List<string> result = new List<string>();
            return result;
        }
        public List<string> GetElasticLoadBalancingObjects(List<string> path)
        {
            List<string> result = new List<string>();
            return result;
        }
        public List<string> GetGlacierObjects(List<string> path)
        {
            List<string> result = new List<string>();
            return result;
        }
        public List<string> GetImportExportObjects(List<string> path)
        {
            List<string> result = new List<string>();
            return result;
        }
        public List<string> GetOpsWorksObjects(List<string> path)
        {
            List<string> result = new List<string>();
            return result;
        }
        public List<string> GetRoute53Objects(List<string> path)
        {
            List<string> result = new List<string>();
            return result;
        }
        public List<string> GetRedshiftObjects(List<string> path)
        {
            List<string> result = new List<string>();
            return result;
        }
        public List<string> GetSimpleEmailObjects(List<string> path)
        {
            List<string> result = new List<string>();
            return result;
        }
        public List<string> GetSimpleNotificationServiceObjects(List<string> path)
        {
            List<string> result = new List<string>();
            return result;
        }
        public List<string> GetSimpleWorkflowObjects(List<string> path)
        {
            List<string> result = new List<string>();
            return result;
        }
        public List<string> GetSQSServiceObjects(List<string> path)
        {
            List<string> result = new List<string>();
            return result;
        }

    }
}
