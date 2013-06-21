using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AWSResponderConsole
{
    public class AWSRegionData
    {
        public AutoScaling AS { get; set; }
        public CloudFront CF { get; set; }
        public CloudSearch CS { get; set; }
        public CloudWatch CW { get; set; }
        public DataPipeline DP { get; set; }
        public ElasticCloudComputing EC2 { get; set; }
        public ElasticMapReduce EMR { get; set; }
        public ElasticTranscoder ET { get; set; }
        public SimpleStorageSolution S3 { get; set; }
        public DirectConnect DC { get; set; }
        public DynamoDB DyDB { get; set; }
        public ElastiCache EC { get; set; }
        public ElastiCacheParameterGroupParameters ECParameters { get; set; }
        public ElastiBeanstalk EBS { get; set; }
        public ElasticLoadBalancing ELB { get; set; }
        public Glacier G { get; set; }
        public ImportExport IE { get; set; }
        public OpsWorks OW { get; set; }
        public Route53 R53 { get; set; }
        public RelationalDatabaseSystem RDS { get; set; }
        public Redshift RS { get; set; }
        public SimpleDB SDB { get; set; }
        public SimpleEmail SE { get; set; }
        public SimpleNotificationService SNS { get; set; }
        public SimpleWorkflow SW { get; set; }
        public SQSService SQS { get; set; }
    }

}
