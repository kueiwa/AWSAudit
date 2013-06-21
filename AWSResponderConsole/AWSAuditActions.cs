using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Amazon;
using Amazon.EC2;
using Amazon.EC2.Model;
using Amazon.SimpleDB;
using Amazon.SimpleDB.Model;
using Amazon.S3;
using Amazon.S3.Model;


using log4net;
using log4net.Config;

namespace AWSResponderConsole
{
    public class AWSAuditActions
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Audit Functions
        public AWSAuditActions()
        {
            log4net.ThreadContext.Properties["SessionID"] = Environment.UserDomainName + "\\" + Environment.UserName;
            log4net.Config.XmlConfigurator.Configure();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public  AutoScaling ReadAutoScaling(AuditParams audit_params)
        {

            BaselineAuditor Auditor = new BaselineAuditor();

            AutoScaling ASData = new AutoScaling();
            StringBuilder sb = new StringBuilder(1024);
            using (StringWriter sr = new StringWriter(sb))
            {
                Amazon.AutoScaling.AmazonAutoScalingClient ASClient = new Amazon.AutoScaling.AmazonAutoScalingClient(audit_params.AWSCredentials, audit_params.AWSRegion);

                ASData.AdjustmentTypes = new ListComparisonResults<Amazon.AutoScaling.Model.AdjustmentType>();
                Amazon.AutoScaling.Model.DescribeAdjustmentTypesResponse adjresp =
                    ASClient.DescribeAdjustmentTypes();
                ASData.AdjustmentTypes.Current.AddRange(adjresp.DescribeAdjustmentTypesResult.AdjustmentTypes);
                //check for changes
                ASData.AdjustmentTypes = Auditor.CheckCIBaseline<Amazon.AutoScaling.Model.AdjustmentType>(audit_params, ASData.AdjustmentTypes.Current, "");

                //This includes running instances and will kick off false postives to an extent.
                ASData.AutoScalingGroups = new ListComparisonResults<Amazon.AutoScaling.Model.AutoScalingGroup>();
                Amazon.AutoScaling.Model.DescribeAutoScalingGroupsResponse asgresp =
                    ASClient.DescribeAutoScalingGroups();
                ASData.AutoScalingGroups.Current.AddRange(asgresp.DescribeAutoScalingGroupsResult.AutoScalingGroups);
                while (asgresp.DescribeAutoScalingGroupsResult.NextToken != null)
                {
                    asgresp = ASClient.DescribeAutoScalingGroups(new Amazon.AutoScaling.Model.DescribeAutoScalingGroupsRequest()
                        .WithNextToken(asgresp.DescribeAutoScalingGroupsResult.NextToken));
                    ASData.AutoScalingGroups.Current.AddRange(asgresp.DescribeAutoScalingGroupsResult.AutoScalingGroups);
                }
                ASData.AutoScalingGroups = Auditor.CheckCIBaseline<Amazon.AutoScaling.Model.AutoScalingGroup>(audit_params,
                            ASData.AutoScalingGroups.Current, "");

                //ASData.AutoScalingInstances
                ASData.AutoScalingInstances = new ListComparisonResults<Amazon.AutoScaling.Model.AutoScalingInstanceDetails>();
                Amazon.AutoScaling.Model.DescribeAutoScalingInstancesResponse asidgresp =
                    ASClient.DescribeAutoScalingInstances();
                ASData.AutoScalingInstances.Current.AddRange(asidgresp.DescribeAutoScalingInstancesResult.AutoScalingInstances);
                while (asidgresp.DescribeAutoScalingInstancesResult.NextToken != null)
                {
                    asidgresp = ASClient.DescribeAutoScalingInstances(new Amazon.AutoScaling.Model.DescribeAutoScalingInstancesRequest()
                        .WithNextToken(asidgresp.DescribeAutoScalingInstancesResult.NextToken));
                    ASData.AutoScalingInstances.Current.AddRange(asidgresp.DescribeAutoScalingInstancesResult.AutoScalingInstances);
                }
                ASData.AutoScalingInstances = Auditor.CheckCIBaseline<Amazon.AutoScaling.Model.AutoScalingInstanceDetails>(audit_params, ASData.AutoScalingInstances.Current, "");

                ASData.AutoScalingNotificationTypes = new ListComparisonResults<string>();
                Amazon.AutoScaling.Model.DescribeAutoScalingNotificationTypesResponse ntresp =
                    ASClient.DescribeAutoScalingNotificationTypes();
                ASData.AutoScalingNotificationTypes.Current.AddRange(ntresp.DescribeAutoScalingNotificationTypesResult.AutoScalingNotificationTypes);
                //check for changes
                ASData.AutoScalingNotificationTypes = Auditor.CheckCIBaseline<string>(audit_params, ASData.AutoScalingNotificationTypes.Current, "");

                //ASData.LaunchConfigurations
                ASData.LaunchConfigurations = new ListComparisonResults<Amazon.AutoScaling.Model.LaunchConfiguration>();
                Amazon.AutoScaling.Model.DescribeLaunchConfigurationsResponse lcresp =
                    ASClient.DescribeLaunchConfigurations();
                ASData.LaunchConfigurations.Current.AddRange(lcresp.DescribeLaunchConfigurationsResult.LaunchConfigurations);
                while (lcresp.DescribeLaunchConfigurationsResult.NextToken != null)
                {
                    lcresp = ASClient.DescribeLaunchConfigurations(new Amazon.AutoScaling.Model.DescribeLaunchConfigurationsRequest()
                        .WithNextToken(lcresp.DescribeLaunchConfigurationsResult.NextToken));
                    ASData.LaunchConfigurations.Current.AddRange(lcresp.DescribeLaunchConfigurationsResult.LaunchConfigurations);
                }
                ASData.LaunchConfigurations = Auditor.CheckCIBaseline<Amazon.AutoScaling.Model.LaunchConfiguration>(audit_params, ASData.LaunchConfigurations.Current, "");


                //ASData.NotificationConfigurations
                ASData.NotificationConfigurations = new ListComparisonResults<Amazon.AutoScaling.Model.NotificationConfiguration>();
                Amazon.AutoScaling.Model.DescribeNotificationConfigurationsResponse ncresp =
                    ASClient.DescribeNotificationConfigurations();
                ASData.NotificationConfigurations.Current.AddRange(ncresp.DescribeNotificationConfigurationsResult.NotificationConfigurations);
                while (ncresp.DescribeNotificationConfigurationsResult.NextToken != null)
                {
                    ncresp = ASClient.DescribeNotificationConfigurations(new Amazon.AutoScaling.Model.DescribeNotificationConfigurationsRequest()
                        .WithNextToken(ncresp.DescribeNotificationConfigurationsResult.NextToken));
                    ASData.NotificationConfigurations.Current.AddRange(ncresp.DescribeNotificationConfigurationsResult.NotificationConfigurations);
                }
                ASData.NotificationConfigurations = Auditor.CheckCIBaseline<Amazon.AutoScaling.Model.NotificationConfiguration>(audit_params, ASData.NotificationConfigurations.Current, "");


                //ASData.ScalingPolicies
                ASData.ScalingPolicies = new ListComparisonResults<Amazon.AutoScaling.Model.ScalingPolicy>();
                Amazon.AutoScaling.Model.DescribePoliciesResponse spresp =
                    ASClient.DescribePolicies();
                ASData.ScalingPolicies.Current.AddRange(spresp.DescribePoliciesResult.ScalingPolicies);
                while (spresp.DescribePoliciesResult.NextToken != null)
                {
                    spresp = ASClient.DescribePolicies(new Amazon.AutoScaling.Model.DescribePoliciesRequest()
                                                        .WithNextToken(spresp.DescribePoliciesResult.NextToken));
                    ASData.ScalingPolicies.Current.AddRange(spresp.DescribePoliciesResult.ScalingPolicies);
                }
                ASData.ScalingPolicies = Auditor.CheckCIBaseline<Amazon.AutoScaling.Model.ScalingPolicy>(audit_params, ASData.ScalingPolicies.Current, "");

                //ASData.ScalingPolicies
                ASData.Processes = new ListComparisonResults<Amazon.AutoScaling.Model.ProcessType>();
                Amazon.AutoScaling.Model.DescribeScalingProcessTypesResponse sptresp =
                    ASClient.DescribeScalingProcessTypes();
                ASData.Processes.Current.AddRange(sptresp.DescribeScalingProcessTypesResult.Processes);
                ASData.Processes = Auditor.CheckCIBaseline<Amazon.AutoScaling.Model.ProcessType>(audit_params, ASData.Processes.Current, "");

                //ASData.Tags
                ASData.Tags = new ListComparisonResults<Amazon.AutoScaling.Model.TagDescription>();
                Amazon.AutoScaling.Model.DescribeTagsResponse tagresp =
                    ASClient.DescribeTags();
                ASData.Tags.Current.AddRange(tagresp.DescribeTagsResult.Tags);
                while (tagresp.DescribeTagsResult.NextToken != null)
                {
                    tagresp = ASClient.DescribeTags(new Amazon.AutoScaling.Model.DescribeTagsRequest()
                                            .WithNextToken(tagresp.DescribeTagsResult.NextToken));
                    ASData.Tags.Current.AddRange(tagresp.DescribeTagsResult.Tags);
                }
                ASData.Tags = Auditor.CheckCIBaseline<Amazon.AutoScaling.Model.TagDescription>(audit_params, ASData.Tags.Current, "");


                //ASData.ScalingPolicies
                ASData.TerminationPolicyTypes = new ListComparisonResults<string>();
                Amazon.AutoScaling.Model.DescribeTerminationPolicyTypesResponse tptresp =
                    ASClient.DescribeTerminationPolicyTypes();
                ASData.TerminationPolicyTypes.Current.AddRange(tptresp.DescribeTerminationPolicyTypesResult.TerminationPolicyTypes);
                ASData.TerminationPolicyTypes = Auditor.CheckCIBaseline<string>(audit_params, ASData.TerminationPolicyTypes.Current, "");

            }


            return ASData;
        }
        public  CloudFront ReadCloudFront(AuditParams audit_params)
        {
            BaselineAuditor Auditor = new BaselineAuditor();


            CloudFront CFData = new CloudFront();
            StringBuilder sb = new StringBuilder(1024);
            using (StringWriter sr = new StringWriter(sb))
            {
                Amazon.CloudFront.AmazonCloudFrontClient CFClient = new Amazon.CloudFront.AmazonCloudFrontClient(audit_params.AWSCredentials, audit_params.AWSRegion);

                List<Amazon.CloudFront.Model.DistributionSummary> DistributionList = new List<Amazon.CloudFront.Model.DistributionSummary>();
                Amazon.CloudFront.Model.ListDistributionsResponse ldresp =
                    CFClient.ListDistributions();
                DistributionList.AddRange(ldresp.ListDistributionsResult.DistributionList.Items);
                while (ldresp.ListDistributionsResult.DistributionList.IsTruncated)
                {
                    ldresp = CFClient.ListDistributions(new Amazon.CloudFront.Model.ListDistributionsRequest()
                                    .WithMarker(ldresp.ListDistributionsResult.DistributionList.Marker));
                    DistributionList.AddRange(ldresp.ListDistributionsResult.DistributionList.Items);
                }

                CFData.Distributions = new ListComparisonResults<Distribution>();
                foreach (Amazon.CloudFront.Model.DistributionSummary sd_summary in DistributionList)
                {
                    Amazon.CloudFront.Model.GetDistributionResponse dresp =
                        CFClient.GetDistribution(new Amazon.CloudFront.Model.GetDistributionRequest().WithId(sd_summary.Id));
                    Amazon.CloudFront.Model.GetDistributionConfigResponse dcresp =
                        CFClient.GetDistributionConfig(new Amazon.CloudFront.Model.GetDistributionConfigRequest().WithId(sd_summary.Id));
                    CFData.Distributions.Current.Add(new Distribution(
                                                                dresp.GetDistributionResult.Distribution,
                                                                dcresp.GetDistributionConfigResult.DistributionConfig,
                                                                sd_summary,
                                                                sd_summary.Id));
                }
                CFData.Distributions = Auditor.CheckCIBaseline<Distribution>(audit_params, CFData.Distributions.Current, "");


                List<Amazon.CloudFront.Model.StreamingDistributionSummary> StreamingDistributionList = new List<Amazon.CloudFront.Model.StreamingDistributionSummary>();
                Amazon.CloudFront.Model.ListStreamingDistributionsResponse lsdresp =
                    CFClient.ListStreamingDistributions();
                StreamingDistributionList.AddRange(lsdresp.ListStreamingDistributionsResult.StreamingDistributionList.Items);
                while (lsdresp.ListStreamingDistributionsResult.StreamingDistributionList.IsTruncated)
                {
                    lsdresp = CFClient.ListStreamingDistributions(new Amazon.CloudFront.Model.ListStreamingDistributionsRequest()
                        .WithMarker(lsdresp.ListStreamingDistributionsResult.StreamingDistributionList.Marker));
                    StreamingDistributionList.AddRange(lsdresp.ListStreamingDistributionsResult.StreamingDistributionList.Items);
                }

                CFData.StreamingDistributions = new ListComparisonResults<StreamingDistribution>();
                foreach (Amazon.CloudFront.Model.StreamingDistributionSummary sd_summary in StreamingDistributionList)
                {
                    Amazon.CloudFront.Model.GetStreamingDistributionResponse sdresp =
                        CFClient.GetStreamingDistribution(new Amazon.CloudFront.Model.GetStreamingDistributionRequest().WithId(sd_summary.Id));
                    Amazon.CloudFront.Model.GetStreamingDistributionConfigResponse sdcresp =
                    CFClient.GetStreamingDistributionConfig(new Amazon.CloudFront.Model.GetStreamingDistributionConfigRequest().WithId(sd_summary.Id));

                    CFData.StreamingDistributions.Current.Add(new StreamingDistribution(
                                                                sdresp.GetStreamingDistributionResult.StreamingDistribution,
                                                                sdcresp.GetStreamingDistributionConfigResult.StreamingDistributionConfig,
                                                                sd_summary,
                                                                sd_summary.Id));
                }
                //check for changes
                CFData.StreamingDistributions = Auditor.CheckCIBaseline<StreamingDistribution>(audit_params, CFData.StreamingDistributions.Current, "");


                List<Amazon.CloudFront.Model.CloudFrontOriginAccessIdentitySummary> OriginAccessIdentitySummary
                    = new List<Amazon.CloudFront.Model.CloudFrontOriginAccessIdentitySummary>();
                Amazon.CloudFront.Model.ListCloudFrontOriginAccessIdentitiesResponse lcfoadresp =
                    CFClient.ListCloudFrontOriginAccessIdentities();
                OriginAccessIdentitySummary.AddRange(lcfoadresp.ListCloudFrontOriginAccessIdentitiesResult.CloudFrontOriginAccessIdentityList.Items);
                while (lcfoadresp.ListCloudFrontOriginAccessIdentitiesResult.CloudFrontOriginAccessIdentityList.IsTruncated)
                {
                    lcfoadresp = CFClient.ListCloudFrontOriginAccessIdentities(new Amazon.CloudFront.Model.ListCloudFrontOriginAccessIdentitiesRequest()
                            .WithMarker(lcfoadresp.ListCloudFrontOriginAccessIdentitiesResult.CloudFrontOriginAccessIdentityList.Marker));
                    OriginAccessIdentitySummary.AddRange(lcfoadresp.ListCloudFrontOriginAccessIdentitiesResult.CloudFrontOriginAccessIdentityList.Items);
                }

                CFData.OriginAccessIdentities = new ListComparisonResults<OriginAccessIdentity>();
                foreach (Amazon.CloudFront.Model.CloudFrontOriginAccessIdentitySummary sum in OriginAccessIdentitySummary)
                {
                    Amazon.CloudFront.Model.GetCloudFrontOriginAccessIdentityResponse cfoairesp =
                        CFClient.GetCloudFrontOriginAccessIdentity(new Amazon.CloudFront.Model.GetCloudFrontOriginAccessIdentityRequest().WithId(sum.Id));
                    CFData.OriginAccessIdentities.Current.Add(new OriginAccessIdentity(cfoairesp.GetCloudFrontOriginAccessIdentityResult.CloudFrontOriginAccessIdentity
                        , sum, sum.Id));
                }

                List<Amazon.CloudFront.Model.InvalidationSummary> InvalidationSummaryList = new List<Amazon.CloudFront.Model.InvalidationSummary>();
                foreach (Distribution dist in CFData.Distributions.Current)
                {
                    Amazon.CloudFront.Model.ListInvalidationsResponse liresp =
                        CFClient.ListInvalidations(new Amazon.CloudFront.Model.ListInvalidationsRequest().WithDistributionId(dist.DistributionId));
                    InvalidationSummaryList.AddRange(liresp.ListInvalidationsResult.InvalidationList.Items);
                    while (liresp.ListInvalidationsResult.InvalidationList.IsTruncated)
                    {
                        liresp = CFClient.ListInvalidations(new Amazon.CloudFront.Model.ListInvalidationsRequest()
                            .WithMarker(liresp.ListInvalidationsResult.InvalidationList.Marker).WithDistributionId(dist.DistributionId));
                        InvalidationSummaryList.AddRange(liresp.ListInvalidationsResult.InvalidationList.Items);
                    }
                }
                CFData.InvalidationList = new ListComparisonResults<Amazon.CloudFront.Model.Invalidation>();
                foreach (Amazon.CloudFront.Model.InvalidationSummary sum in InvalidationSummaryList)
                {
                    Amazon.CloudFront.Model.GetInvalidationResponse giresp =
                        CFClient.GetInvalidation(new Amazon.CloudFront.Model.GetInvalidationRequest().WithId(sum.Id));
                    CFData.InvalidationList.Current.Add(giresp.GetInvalidationResult.Invalidation);
                }

                //check for changes
                CFData.InvalidationList = Auditor.CheckCIBaseline<Amazon.CloudFront.Model.Invalidation>(audit_params, CFData.InvalidationList.Current, "");


            }


            return CFData;
        }
        public  CloudSearch ReadCloudSearch(AuditParams audit_params)
        {
            BaselineAuditor Auditor = new BaselineAuditor();


            CloudSearch CSData = new CloudSearch();
            StringBuilder sb = new StringBuilder(1024);
            using (StringWriter sr = new StringWriter(sb))
            {
                Amazon.CloudSearch.AmazonCloudSearchClient CSClient = new Amazon.CloudSearch.AmazonCloudSearchClient(audit_params.AWSCredentials, audit_params.AWSRegion);

                List<Amazon.CloudSearch.Model.DomainStatus> Domains = new List<Amazon.CloudSearch.Model.DomainStatus>();
                try
                {
                    Amazon.CloudSearch.Model.DescribeDomainsResponse ddomresp =
                        CSClient.DescribeDomains();
                    Domains.AddRange(ddomresp.DescribeDomainsResult.DomainStatusList);
                }
                catch (Exception ex)
                {
                    
                    if (!ex.Message.StartsWith("No endpoint found for service cloudsearch for region"))
                        log.Fatal(String.Format("Fatal Error in function {0} ", ReflectionHelper.GetMyFunctionName()), ex);
                    else
                        log.Info(String.Format("Info in function {0} message: {1}", ReflectionHelper.GetMyFunctionName(), ex.Message));
                }
                CSData.CloudSearchDomains = new ListComparisonResults<CloudSearchDomain>();
                foreach (Amazon.CloudSearch.Model.DomainStatus dom in Domains)
                {

                    Amazon.CloudSearch.Model.DescribeDefaultSearchFieldResponse ddsfresp =
                        CSClient.DescribeDefaultSearchField(new Amazon.CloudSearch.Model.DescribeDefaultSearchFieldRequest().WithDomainName(dom.DomainName));
                    Amazon.CloudSearch.Model.DescribeIndexFieldsResponse difresp =
                        CSClient.DescribeIndexFields(new Amazon.CloudSearch.Model.DescribeIndexFieldsRequest().WithDomainName(dom.DomainName));
                    Amazon.CloudSearch.Model.DescribeRankExpressionsResponse dreresp =
                        CSClient.DescribeRankExpressions(new Amazon.CloudSearch.Model.DescribeRankExpressionsRequest().WithDomainName(dom.DomainName));
                    Amazon.CloudSearch.Model.DescribeServiceAccessPoliciesResponse dsaresp =
                        CSClient.DescribeServiceAccessPolicies(new Amazon.CloudSearch.Model.DescribeServiceAccessPoliciesRequest().WithDomainName(dom.DomainName));
                    Amazon.CloudSearch.Model.DescribeStemmingOptionsResponse dsoresp =
                        CSClient.DescribeStemmingOptions(new Amazon.CloudSearch.Model.DescribeStemmingOptionsRequest().WithDomainName(dom.DomainName));
                    Amazon.CloudSearch.Model.DescribeStopwordOptionsResponse dsworesp =
                        CSClient.DescribeStopwordOptions(new Amazon.CloudSearch.Model.DescribeStopwordOptionsRequest().WithDomainName(dom.DomainName));
                    Amazon.CloudSearch.Model.DescribeSynonymOptionsResponse dsyoresp =
                        CSClient.DescribeSynonymOptions(new Amazon.CloudSearch.Model.DescribeSynonymOptionsRequest().WithDomainName(dom.DomainName));

                    CSData.CloudSearchDomains.Current.Add(new CloudSearchDomain(dom,
                                                                ddsfresp.DescribeDefaultSearchFieldResult.DefaultSearchField,
                                                                difresp.DescribeIndexFieldsResult.IndexFields,
                                                                dreresp.DescribeRankExpressionsResult.RankExpressions,
                                                                dsaresp.DescribeServiceAccessPoliciesResult.AccessPolicies,
                                                                dsoresp.DescribeStemmingOptionsResult.Stems,
                                                                dsworesp.DescribeStopwordOptionsResult.Stopwords,
                                                                dsyoresp.DescribeSynonymOptionsResult.Synonyms));
                }
                CSData.CloudSearchDomains = Auditor.CheckCIBaseline<CloudSearchDomain>(audit_params, CSData.CloudSearchDomains.Current, "");



            }


            return CSData;
        }
        public  CloudWatch ReadCloudWatch(AuditParams audit_params)
        {
            BaselineAuditor Auditor = new BaselineAuditor();


            CloudWatch CWData = new CloudWatch();
            StringBuilder sb = new StringBuilder(1024);
            using (StringWriter sr = new StringWriter(sb))
            {
                Amazon.CloudWatch.AmazonCloudWatchClient CWClient = new Amazon.CloudWatch.AmazonCloudWatchClient(audit_params.AWSCredentials, audit_params.AWSRegion);

                CWData.Metrics = new ListComparisonResults<Amazon.CloudWatch.Model.Metric>();
                Amazon.CloudWatch.Model.ListMetricsResponse lmresp = CWClient.ListMetrics();
                CWData.Metrics.Current.AddRange(lmresp.ListMetricsResult.Metrics);
                while (lmresp.ListMetricsResult.NextToken != null)
                {
                    lmresp = CWClient.ListMetrics(new Amazon.CloudWatch.Model.ListMetricsRequest().WithNextToken(lmresp.ListMetricsResult.NextToken));
                    CWData.Metrics.Current.AddRange(lmresp.ListMetricsResult.Metrics);
                }
                CWData.Metrics = Auditor.CheckCIBaseline<Amazon.CloudWatch.Model.Metric>(audit_params, CWData.Metrics.Current, "");

                CWData.MetricAlarms = new ListComparisonResults<Amazon.CloudWatch.Model.MetricAlarm>();
                Amazon.CloudWatch.Model.DescribeAlarmsResponse daresp =
                    CWClient.DescribeAlarms();
                CWData.MetricAlarms.Current.AddRange(daresp.DescribeAlarmsResult.MetricAlarms);
                while (daresp.DescribeAlarmsResult.NextToken != null)
                {
                    daresp = CWClient.DescribeAlarms(new Amazon.CloudWatch.Model.DescribeAlarmsRequest().WithNextToken(daresp.DescribeAlarmsResult.NextToken));
                    CWData.MetricAlarms.Current.AddRange(daresp.DescribeAlarmsResult.MetricAlarms);
                }
                CWData.MetricAlarms = Auditor.CheckCIBaseline<Amazon.CloudWatch.Model.MetricAlarm>(audit_params, CWData.MetricAlarms.Current, "");

            }


            return CWData;
        }
        public  DataPipeline ReadDataPipeline(AuditParams audit_params)
        {
            BaselineAuditor Auditor = new BaselineAuditor();


            DataPipeline DPData = new DataPipeline();
            StringBuilder sb = new StringBuilder(1024);
            using (StringWriter sr = new StringWriter(sb))
            {
                Amazon.DataPipeline.AmazonDataPipelineClient DPClient = new Amazon.DataPipeline.AmazonDataPipelineClient(audit_params.AWSCredentials, audit_params.AWSRegion);

                DPData.PipelineIdList = new ListComparisonResults<Amazon.DataPipeline.Model.PipelineIdName>();
                try
                {
                    Amazon.DataPipeline.Model.ListPipelinesResponse lplresp =
                    DPClient.ListPipelines();
                    DPData.PipelineIdList.Current.AddRange(lplresp.ListPipelinesResult.PipelineIdList);
                    while (lplresp.ListPipelinesResult.HasMoreResults)
                    {
                        lplresp = DPClient.ListPipelines(new Amazon.DataPipeline.Model.ListPipelinesRequest().WithMarker(lplresp.ListPipelinesResult.Marker));
                        DPData.PipelineIdList.Current.AddRange(lplresp.ListPipelinesResult.PipelineIdList);
                    }
                    DPData.PipelineIdList = Auditor.CheckCIBaseline<Amazon.DataPipeline.Model.PipelineIdName>(audit_params, DPData.PipelineIdList.Current, "");
                }
                catch (Exception ex)
                {
                    if (!ex.Message.StartsWith("No endpoint found for service datapipeline for "))
                        log.Fatal(String.Format("Fatal Error in function {0} ", ReflectionHelper.GetMyFunctionName()), ex);
                    else
                        log.Info(String.Format("Info in function {0} message: {1}", ReflectionHelper.GetMyFunctionName(), ex.Message));
                }
                DPData.PipelineDescriptionList = new ListComparisonResults<Amazon.DataPipeline.Model.PipelineDescription>();
                DPData.DataPipelineObjects = new ListComparisonResults<DataPipelineObject>();
                foreach (Amazon.DataPipeline.Model.PipelineIdName pID in DPData.PipelineIdList.Current)
                {
                    Amazon.DataPipeline.Model.DescribePipelinesResponse dplresp =
                        DPClient.DescribePipelines(new Amazon.DataPipeline.Model.DescribePipelinesRequest().WithPipelineIds(pID.Id));
                    DPData.PipelineDescriptionList.Current.AddRange(dplresp.DescribePipelinesResult.PipelineDescriptionList);
                    Amazon.DataPipeline.Model.DescribeObjectsResponse dploresp =
                        DPClient.DescribeObjects(new Amazon.DataPipeline.Model.DescribeObjectsRequest().WithPipelineId(pID.Id));
                    DataPipelineObject po = new DataPipelineObject(pID, dploresp.DescribeObjectsResult.PipelineObjects);
                    while (dploresp.DescribeObjectsResult.HasMoreResults)
                    {
                        dploresp = DPClient.DescribeObjects(new Amazon.DataPipeline.Model.DescribeObjectsRequest()
                                                                .WithPipelineId(pID.Id)
                                                                .WithMarker(dploresp.DescribeObjectsResult.Marker));
                        po.PipelineObjects.AddRange(dploresp.DescribeObjectsResult.PipelineObjects);
                    }
                    DPData.DataPipelineObjects.Current.Add(po);
                }
                DPData.PipelineDescriptionList = Auditor.CheckCIBaseline<Amazon.DataPipeline.Model.PipelineDescription>(audit_params,
                                                                                            DPData.PipelineDescriptionList.Current, "");
                DPData.DataPipelineObjects = Auditor.CheckCIBaseline<DataPipelineObject>(audit_params,
                                                            DPData.DataPipelineObjects.Current,
                                                            "");



            }



            return DPData;
        }
        /// <summary>
        /// IsTruncated Review completed
        /// </summary>
        /// <returns></returns>
        public  DynamoDB ReadDynamoDB(AuditParams audit_params)
        {
            BaselineAuditor Auditor = new BaselineAuditor();


            DynamoDB DyData = new DynamoDB();
            StringBuilder sb = new StringBuilder(1024);
            using (StringWriter sr = new StringWriter(sb))
            {
                Amazon.DynamoDB.AmazonDynamoDBClient DyClient = new Amazon.DynamoDB.AmazonDynamoDBClient(audit_params.AWSCredentials, audit_params.AWSRegion);
                Amazon.DynamoDB.Model.ListTablesResponse ltresp =
                    DyClient.ListTables(new Amazon.DynamoDB.Model.ListTablesRequest());
                DyData.TableNames = new ListComparisonResults<string>();
                DyData.TableNames.Current.AddRange(ltresp.ListTablesResult.TableNames);
                //checked for changes
                DyData.TableNames = Auditor.CheckCIBaseline<string>(audit_params, DyData.TableNames.Current, "");


                DyData.TableDescriptions = new ListComparisonResults<Amazon.DynamoDB.Model.TableDescription>();
                foreach (string tablename in DyData.TableNames.Current)
                {
                    Amazon.DynamoDB.Model.DescribeTableResponse dtresp =
                        DyClient.DescribeTable(new Amazon.DynamoDB.Model.DescribeTableRequest().WithTableName(tablename));
                    DyData.TableDescriptions.Current.Add(dtresp.DescribeTableResult.Table);
                }
                //checked for changes
                DyData.TableDescriptions = Auditor.CheckCIBaseline<Amazon.DynamoDB.Model.TableDescription>(audit_params, DyData.TableDescriptions.Current, "");


            }


            return DyData;
        }
        /// <summary>
        /// IsTruncated Review completed
        /// </summary>
        /// <returns></returns>
        public  DirectConnect ReadDC(AuditParams audit_params)
        {

            BaselineAuditor Auditor = new BaselineAuditor();



            DirectConnect DCData = new DirectConnect();
            StringBuilder sb = new StringBuilder(1024);
            using (StringWriter sr = new StringWriter(sb))
            {
                Amazon.DirectConnect.AmazonDirectConnectClient DCClient = new Amazon.DirectConnect.AmazonDirectConnectClient(audit_params.AWSCredentials, audit_params.AWSRegion);

                DCData.Connections = new ListComparisonResults<Amazon.DirectConnect.Model.Connection>();
                try
                {
                    Amazon.DirectConnect.Model.DescribeConnectionsResponse dcresp = DCClient.DescribeConnections();
                    DCData.Connections.Current.AddRange(dcresp.DescribeConnectionsResult.Connections);
                    //checked for changes
                    DCData.Connections = Auditor.CheckCIBaseline<Amazon.DirectConnect.Model.Connection>(audit_params, DCData.Connections.Current, "");


                    DCData.ConnectionDetails = new ListComparisonResults<Amazon.DirectConnect.Model.DescribeConnectionDetailResult>();
                    foreach (Amazon.DirectConnect.Model.Connection con in dcresp.DescribeConnectionsResult.Connections)
                    {
                        Amazon.DirectConnect.Model.DescribeConnectionDetailResponse dcdresp =
                            DCClient.DescribeConnectionDetail(
                                new Amazon.DirectConnect.Model.DescribeConnectionDetailRequest().WithConnectionId(con.ConnectionId));
                        DCData.ConnectionDetails.Current.Add(dcdresp.DescribeConnectionDetailResult);
                    }
                    //checked for changes
                    DCData.ConnectionDetails = Auditor.CheckCIBaseline<Amazon.DirectConnect.Model.DescribeConnectionDetailResult>(audit_params,
                        DCData.ConnectionDetails.Current, "");
                }
                catch (Amazon.DirectConnect.AmazonDirectConnectException ex)
                {
                    log.Fatal(String.Format("Fatal Error in function {0} ", ReflectionHelper.GetMyFunctionName()), ex);
                }
                try
                {
                    DCData.Offerings = new ListComparisonResults<Amazon.DirectConnect.Model.Offering>();
                    Amazon.DirectConnect.Model.DescribeOfferingsResponse oresp = DCClient.DescribeOfferings();
                    DCData.Offerings.Current.AddRange(oresp.DescribeOfferingsResult.Offerings);
                    //checked for changes
                    DCData.Offerings = Auditor.CheckCIBaseline<Amazon.DirectConnect.Model.Offering>(audit_params, DCData.Offerings.Current, "");


                    DCData.OfferingDetails = new ListComparisonResults<Amazon.DirectConnect.Model.DescribeOfferingDetailResult>();
                    foreach (Amazon.DirectConnect.Model.Offering Of in DCData.Offerings.Current)
                    {
                        Amazon.DirectConnect.Model.DescribeOfferingDetailResponse odresp =
                            DCClient.DescribeOfferingDetail(
                                new Amazon.DirectConnect.Model.DescribeOfferingDetailRequest().WithOfferingId(Of.OfferingId));
                        DCData.OfferingDetails.Current.Add(odresp.DescribeOfferingDetailResult);
                    }
                    //checked for changes
                    DCData.OfferingDetails = Auditor.CheckCIBaseline<Amazon.DirectConnect.Model.DescribeOfferingDetailResult>(audit_params, DCData.OfferingDetails.Current, "");
                }
                catch (Exception ex)
                {
                    log.Fatal(String.Format("Fatal Error in function {0} ", ReflectionHelper.GetMyFunctionName()), ex);
                }
                try
                {
                    DCData.VirtualGateways = new ListComparisonResults<Amazon.DirectConnect.Model.VirtualGateway>();
                    Amazon.DirectConnect.Model.DescribeVirtualGatewaysResponse vgresp =
                        DCClient.DescribeVirtualGateways();
                    DCData.VirtualGateways.Current.AddRange(vgresp.DescribeVirtualGatewaysResult.VirtualGateways);
                    //checked for changes
                    DCData.VirtualGateways = Auditor.CheckCIBaseline<Amazon.DirectConnect.Model.VirtualGateway>(audit_params, DCData.VirtualGateways.Current, "");
                }
                catch (Amazon.DirectConnect.AmazonDirectConnectException ex)
                {
                    log.Fatal(String.Format("Fatal Error in function {0} ", ReflectionHelper.GetMyFunctionName()), ex);
                }
                try
                {
                    DCData.VirtualInterfaces = new ListComparisonResults<Amazon.DirectConnect.Model.VirtualInterface>();
                    Amazon.DirectConnect.Model.DescribeVirtualInterfacesResponse viresp =
                        DCClient.DescribeVirtualInterfaces();

                    DCData.VirtualInterfaces.Current.AddRange(viresp.DescribeVirtualInterfacesResult.VirtualInterfaces);
                    //check for changes
                    DCData.VirtualInterfaces = Auditor.CheckCIBaseline<Amazon.DirectConnect.Model.VirtualInterface>(audit_params,
                                                                DCData.VirtualInterfaces.Current, "");
                }
                catch (Amazon.DirectConnect.AmazonDirectConnectException ex)
                {
                    log.Fatal(String.Format("Fatal Error in function {0} ", ReflectionHelper.GetMyFunctionName()), ex);
                }



            }


            return DCData;
        }
        /// <summary>
        /// IsTruncated Review completed
        /// </summary>
        /// <returns></returns>
        public  ElasticCloudComputing ReadECC(AuditParams audit_params)
        {


            BaselineAuditor Auditor = new BaselineAuditor();


            ElasticCloudComputing ECCData = new ElasticCloudComputing();
            StringBuilder sb = new StringBuilder(4096);
            using (StringWriter sr = new StringWriter(sb))
            {
                Amazon.EC2.AmazonEC2Client EC2Client = new Amazon.EC2.AmazonEC2Client(audit_params.AWSCredentials, audit_params.AWSRegion);
                Amazon.EC2.Model.DescribeAddressesResponse resp = EC2Client.DescribeAddresses(new DescribeAddressesRequest());

                ECCData.Address = new ListComparisonResults<Amazon.EC2.Model.Address>();
                ECCData.Address.Current.AddRange(resp.DescribeAddressesResult.Address);
                //check for changes
                ECCData.Address = Auditor.CheckCIBaseline<Amazon.EC2.Model.Address>(audit_params,
                                            ECCData.Address.Current, "");



                ECCData.AvailabilityZones = new ListComparisonResults<Amazon.EC2.Model.AvailabilityZone>();
                Amazon.EC2.Model.DescribeAvailabilityZonesResponse azresp =
                    EC2Client.DescribeAvailabilityZones(new DescribeAvailabilityZonesRequest());
                ECCData.AvailabilityZones.Current.AddRange(azresp.DescribeAvailabilityZonesResult.AvailabilityZone);
                //check for changes
                ECCData.AvailabilityZones = Auditor.CheckCIBaseline<Amazon.EC2.Model.AvailabilityZone>(audit_params,
                                        ECCData.AvailabilityZones.Current, "");

                ECCData.BundleTasks = new ListComparisonResults<Amazon.EC2.Model.BundleTask>();
                Amazon.EC2.Model.DescribeBundleTasksResponse btresp =
                    EC2Client.DescribeBundleTasks(new DescribeBundleTasksRequest());
                ECCData.BundleTasks.Current.AddRange(btresp.DescribeBundleTasksResult.BundleTask);
                //check for changes
                ECCData.BundleTasks = Auditor.CheckCIBaseline<Amazon.EC2.Model.BundleTask>(audit_params,
                                                        ECCData.BundleTasks.Current, "");


                ECCData.ConversionTasks = new ListComparisonResults<Amazon.EC2.Model.ConversionTaskType>();
                Amazon.EC2.Model.DescribeConversionTasksResponse ctresp =
                    EC2Client.DescribeConversionTasks(new DescribeConversionTasksRequest());
                ECCData.ConversionTasks.Current.AddRange(ctresp.DescribeConversionTasksResult.ConversionTasks);
                //check for changes
                ECCData.ConversionTasks = Auditor.CheckCIBaseline<Amazon.EC2.Model.ConversionTaskType>(audit_params,
                                                ECCData.ConversionTasks.Current, "");



                ECCData.CustomerGateway = new ListComparisonResults<Amazon.EC2.Model.CustomerGateway>();
                Amazon.EC2.Model.DescribeCustomerGatewaysResponse cgresp =
                    EC2Client.DescribeCustomerGateways(new DescribeCustomerGatewaysRequest());
                ECCData.CustomerGateway.Current.AddRange(cgresp.DescribeCustomerGatewaysResult.CustomerGateway);
                //check for changes
                ECCData.CustomerGateway = Auditor.CheckCIBaseline<Amazon.EC2.Model.CustomerGateway>(audit_params,
                                                            ECCData.CustomerGateway.Current, "");

                ECCData.DhcpOptions = new ListComparisonResults<Amazon.EC2.Model.DhcpOptions>();
                Amazon.EC2.Model.DescribeDhcpOptionsResponse dhcpresp =
                    EC2Client.DescribeDhcpOptions(new DescribeDhcpOptionsRequest());
                ECCData.DhcpOptions.Current.AddRange(dhcpresp.DescribeDhcpOptionsResult.DhcpOptions);
                //check for changes
                ECCData.DhcpOptions = Auditor.CheckCIBaseline<Amazon.EC2.Model.DhcpOptions>(audit_params,
                                                            ECCData.DhcpOptions.Current, "");

                ECCData.SecurityGroup = new ListComparisonResults<Amazon.EC2.Model.SecurityGroup>();
                Amazon.EC2.Model.DescribeSecurityGroupsResponse sgresp =
                    EC2Client.DescribeSecurityGroups(new DescribeSecurityGroupsRequest());
                ECCData.SecurityGroup.Current.AddRange(sgresp.DescribeSecurityGroupsResult.SecurityGroup);

                //check for changes
                ECCData.SecurityGroup = Auditor.CheckCIBaseline<Amazon.EC2.Model.SecurityGroup>(audit_params,
                                                    ECCData.SecurityGroup.Current, "");



                ECCData.Image = new ListComparisonResults<Amazon.EC2.Model.Image>();
                Amazon.EC2.Model.DescribeImagesResponse iresp =
                    EC2Client.DescribeImages(new DescribeImagesRequest().WithOwner(ECCData.SecurityGroup.Current[0].OwnerId));
                ECCData.Image.Current.AddRange(iresp.DescribeImagesResult.Image);
                //check for changes
                ECCData.Image = Auditor.CheckCIBaseline<Amazon.EC2.Model.Image>(audit_params, ECCData.Image.Current, "");



                ECCData.Reservations = new ListComparisonResults<Amazon.EC2.Model.Reservation>();
                ECCData.RunningInstances = new ListComparisonResults<RunningInstance>();
                Amazon.EC2.Model.DescribeInstancesResponse instresp =
                    EC2Client.DescribeInstances(new DescribeInstancesRequest());
                ECCData.Reservations.Current = instresp.DescribeInstancesResult.Reservation;
                //check for changes
                ECCData.Reservations = Auditor.CheckCIBaseline<Amazon.EC2.Model.Reservation>(audit_params,
                                                    ECCData.Reservations.Current, "");
                foreach (Amazon.EC2.Model.Reservation r in ECCData.Reservations.Current)
                {
                    ECCData.RunningInstances.Current.AddRange(r.RunningInstance);
                }
                //check for changes
                ECCData.Reservations = Auditor.CheckCIBaseline<Amazon.EC2.Model.Reservation>(audit_params,
                                                        ECCData.Reservations.Current, "");
                ECCData.RunningInstances = Auditor.CheckCIBaseline<Amazon.EC2.Model.RunningInstance>(audit_params,
                                                        ECCData.RunningInstances.Current, "");


                ECCData.InstanceAttributes = new ListComparisonResults<Amazon.EC2.Model.InstanceAttribute>();
                //Valid InstanceAttributes values: 
                string[] attributes = new string[] {"instanceType","kernel","ramdisk","userData","disableApiTermination",
                                                        "instanceInitiatedShutdownBehavior","rootDeviceName","blockDeviceMapping",
                                                        "sourceDestCheck","groupSet","productCodes","ebsOptimized"};

                foreach (Amazon.EC2.Model.RunningInstance r in ECCData.RunningInstances.Current)
                {
                    Amazon.EC2.Model.InstanceAttribute IA = new InstanceAttribute();
                    foreach (string attr in attributes)
                    {
                        Amazon.EC2.Model.DescribeInstanceAttributeResponse iaaresp =
                        EC2Client.DescribeInstanceAttribute(new DescribeInstanceAttributeRequest()
                                        .WithInstanceId(r.InstanceId)
                                        .WithAttribute(attr));
                        Amazon.EC2.Model.InstanceAttribute awsIA = iaaresp.DescribeInstanceAttributeResult.InstanceAttribute;
                        IA.InstanceId = awsIA.IsSetInstanceId() ? awsIA.InstanceId : null;
                        switch (attr)
                        {
                            case "blockDeviceMapping": IA.BlockDeviceMapping = awsIA.IsSetBlockDeviceMapping() ? awsIA.BlockDeviceMapping : null; break;
                            case "disableApiTermination": IA.DisableApiTermination = awsIA.IsSetDisableApiTermination() ? awsIA.DisableApiTermination : false; break;
                            case "ebsOptimized": IA.EbsOptimized = awsIA.IsSetEbsOptimized() ? awsIA.EbsOptimized : false; break;
                            case "groupSet":
                                {
                                    IA.GroupId = awsIA.IsSetGroupId() ? awsIA.GroupId : null;
                                    IA.GroupName = awsIA.IsSetGroupName() ? awsIA.GroupName : null; break;
                                }
                            case "instanceInitiatedShutdownBehavior": IA.InstanceInitiatedShutdownBehavior = awsIA.IsSetInstanceInitiatedShutdownBehavior() ? awsIA.InstanceInitiatedShutdownBehavior : null; break;
                            case "instanceType": IA.InstanceType = awsIA.IsSetInstanceType() ? awsIA.InstanceType : null; break;
                            case "kernel": IA.KernelId = awsIA.IsSetKernelId() ? awsIA.KernelId : null; break;
                            case "productCodes": IA.ProductCodes = awsIA.IsSetProductCodes() ? awsIA.ProductCodes : null; break;
                            case "ramdisk": IA.RamdiskId = awsIA.IsSetRamdiskId() ? awsIA.RamdiskId : null; break;
                            case "rootDeviceName": IA.RootDeviceName = awsIA.IsSetRootDeviceName() ? awsIA.RootDeviceName : null; break;
                            case "sourceDestCheck": IA.SourceDestCheck = awsIA.IsSetSourceDestCheck() ? awsIA.SourceDestCheck : false; break;
                            case "userData": IA.UserData = awsIA.IsSetUserData() ? awsIA.UserData : null; break;
                        }

                    }
                    ECCData.InstanceAttributes.Current.Add(IA);

                }
                //check for changes
                ECCData.InstanceAttributes = Auditor.CheckCIBaseline<Amazon.EC2.Model.InstanceAttribute>(audit_params,
                                                            ECCData.InstanceAttributes.Current, "");



                ECCData.ImageAttributes = new ListComparisonResults<ImageAttribute>();
                foreach (Image i in ECCData.Image.Current)
                {
                    Amazon.EC2.Model.DescribeImageAttributeResponse iaresp =
                        EC2Client.DescribeImageAttribute(new DescribeImageAttributeRequest().WithImageId(i.ImageId));
                    ECCData.ImageAttributes.Current.Add(iaresp.DescribeImageAttributeResult.ImageAttribute);

                }
                //check for changes
                ECCData.ImageAttributes = Auditor.CheckCIBaseline<Amazon.EC2.Model.ImageAttribute>(audit_params,
                                                            ECCData.ImageAttributes.Current, "");


                ECCData.InstanceStatus = new ListComparisonResults<Amazon.EC2.Model.InstanceStatus>();
                Amazon.EC2.Model.DescribeInstanceStatusResponse istresp =
                    EC2Client.DescribeInstanceStatus(new DescribeInstanceStatusRequest());
                ECCData.InstanceStatus.Current.AddRange(istresp.DescribeInstanceStatusResult.InstanceStatus);
                //check for changes
                ECCData.InstanceStatus = Auditor.CheckCIBaseline<Amazon.EC2.Model.InstanceStatus>(audit_params,
                                                                    ECCData.InstanceStatus.Current, "");



                ECCData.InternetGateways = new ListComparisonResults<Amazon.EC2.Model.InternetGateway>();
                Amazon.EC2.Model.DescribeInternetGatewaysResponse igresp =
                    EC2Client.DescribeInternetGateways(new DescribeInternetGatewaysRequest());
                ECCData.InternetGateways.Current.AddRange(igresp.DescribeInternetGatewaysResult.InternetGateways);
                //check for changes
                ECCData.InternetGateways = Auditor.CheckCIBaseline<Amazon.EC2.Model.InternetGateway>(audit_params,
                                                                    ECCData.InternetGateways.Current, "");



                ECCData.KeyPair = new ListComparisonResults<Amazon.EC2.Model.KeyPair>();
                Amazon.EC2.Model.DescribeKeyPairsResponse kpresp =
                    EC2Client.DescribeKeyPairs(new DescribeKeyPairsRequest());
                ECCData.KeyPair.Current.AddRange(kpresp.DescribeKeyPairsResult.KeyPair);
                //check for changes
                ECCData.KeyPair = Auditor.CheckCIBaseline<Amazon.EC2.Model.KeyPair>(audit_params,
                                                                    ECCData.KeyPair.Current, "");


                ECCData.NetworkAcls = new ListComparisonResults<Amazon.EC2.Model.NetworkAcl>();
                Amazon.EC2.Model.DescribeNetworkAclsResponse naclresp =
                    EC2Client.DescribeNetworkAcls(new DescribeNetworkAclsRequest());
                ECCData.NetworkAcls.Current.AddRange(naclresp.DescribeNetworkAclsResult.NetworkAcls);
                //check for changes
                ECCData.NetworkAcls = Auditor.CheckCIBaseline<Amazon.EC2.Model.NetworkAcl>(audit_params,
                                                                    ECCData.NetworkAcls.Current, "");


                ECCData.NetworkInterface = new ListComparisonResults<Amazon.EC2.Model.NetworkInterface>();
                Amazon.EC2.Model.DescribeNetworkInterfacesResponse nifresp =
                    EC2Client.DescribeNetworkInterfaces(new DescribeNetworkInterfacesRequest());
                ECCData.NetworkInterface.Current.AddRange(nifresp.DescribeNetworkInterfaceResult.NetworkInterface);
                //check for changes
                ECCData.NetworkInterface = Auditor.CheckCIBaseline<Amazon.EC2.Model.NetworkInterface>(audit_params,
                                                                    ECCData.NetworkInterface.Current, "");


                ECCData.NetworkInterfaceAttribute = new ListComparisonResults<Amazon.EC2.Model.NetworkInterfaceAttribute>();
                string[] interfaceattribute = new string[] { "description", "sourceDestCheck", "groupSet", "attachment" };
                foreach (NetworkInterface ni in ECCData.NetworkInterface.Current)
                {
                    foreach (string attribute in interfaceattribute)
                    {
                        Amazon.EC2.Model.DescribeNetworkInterfaceAttributeResponse nifaresp =
                            EC2Client.DescribeNetworkInterfaceAttribute(new DescribeNetworkInterfaceAttributeRequest().WithAttribute(attribute).WithNetworkInterfaceId(ni.NetworkInterfaceId));
                        ECCData.NetworkInterfaceAttribute.Current.Add(nifaresp.DescribeNetworkInterfaceAttributeResult.NetworkInterfaceAttribute);
                    }
                }
                //check for changes
                ECCData.NetworkInterfaceAttribute = Auditor.CheckCIBaseline<Amazon.EC2.Model.NetworkInterfaceAttribute>(audit_params,
                                                            ECCData.NetworkInterfaceAttribute.Current, "");


                ECCData.PlacementGroupInfo = new ListComparisonResults<Amazon.EC2.Model.PlacementGroupInfo>();
                try
                {
                    Amazon.EC2.Model.DescribePlacementGroupsResponse pgresp =
                         EC2Client.DescribePlacementGroups(new DescribePlacementGroupsRequest());
                    ECCData.PlacementGroupInfo.Current.AddRange(pgresp.DescribePlacementGroupsResult.PlacementGroupInfo);
                }
                catch (Exception ex)
                {
                    if (!ex.Message.Equals("The functionality you requested is not available in this region."))
                        log.Fatal(String.Format("Fatal Error in function {0} ", ReflectionHelper.GetMyFunctionName()), ex);
                    else
                        log.Info(String.Format("Info in function {0} message: {1}", ReflectionHelper.GetMyFunctionName(), ex.Message));
                }
                //check for changes
                ECCData.PlacementGroupInfo = Auditor.CheckCIBaseline<Amazon.EC2.Model.PlacementGroupInfo>(audit_params,
                                                        ECCData.PlacementGroupInfo.Current, "");


                ECCData.Region = new ListComparisonResults<Amazon.EC2.Model.Region>();
                Amazon.EC2.Model.DescribeRegionsResponse regresp =
                    EC2Client.DescribeRegions(new DescribeRegionsRequest());
                ECCData.Region.Current.AddRange(regresp.DescribeRegionsResult.Region);
                //check for changes
                ECCData.Region = Auditor.CheckCIBaseline<Amazon.EC2.Model.Region>(audit_params,
                                                        ECCData.Region.Current, "");

                ECCData.ReservedInstances = new ListComparisonResults<Amazon.EC2.Model.ReservedInstances>();
                Amazon.EC2.Model.DescribeReservedInstancesResponse riresp =
                    EC2Client.DescribeReservedInstances(new DescribeReservedInstancesRequest());
                ECCData.ReservedInstances.Current.AddRange(riresp.DescribeReservedInstancesResult.ReservedInstances);
                //check for changes
                ECCData.ReservedInstances = Auditor.CheckCIBaseline<Amazon.EC2.Model.ReservedInstances>(audit_params,
                                                            ECCData.ReservedInstances.Current, "");

                ECCData.ReservedInstancesOffering = new ListComparisonResults<ReservedInstancesOffering>();
                Amazon.EC2.Model.DescribeReservedInstancesOfferingsResponse rioresp =
                    EC2Client.DescribeReservedInstancesOfferings(new DescribeReservedInstancesOfferingsRequest());
                ECCData.ReservedInstancesOffering.Current.AddRange(rioresp.DescribeReservedInstancesOfferingsResult.ReservedInstancesOffering);
                ECCData.ReservedInstancesOffering = Auditor.CheckCIBaseline<ReservedInstancesOffering>(audit_params, ECCData.ReservedInstancesOffering.Current,
                                                                "");

                ECCData.RouteTables = new ListComparisonResults<Amazon.EC2.Model.RouteTable>();
                Amazon.EC2.Model.DescribeRouteTablesResponse rtresp =
                    EC2Client.DescribeRouteTables(new DescribeRouteTablesRequest());
                ECCData.RouteTables.Current.AddRange(rtresp.DescribeRouteTablesResult.RouteTables);
                //check for changes
                ECCData.RouteTables = Auditor.CheckCIBaseline<Amazon.EC2.Model.RouteTable>(audit_params,
                                                                ECCData.RouteTables.Current, "");


                ECCData.Snapshot = new ListComparisonResults<Amazon.EC2.Model.Snapshot>();
                Amazon.EC2.Model.DescribeSnapshotsResponse shresp =
                    EC2Client.DescribeSnapshots(new DescribeSnapshotsRequest().WithOwner(ECCData.SecurityGroup.Current[0].OwnerId));
                ECCData.Snapshot.Current.AddRange(shresp.DescribeSnapshotsResult.Snapshot);
                //check for changes
                ECCData.Snapshot = Auditor.CheckCIBaseline<Amazon.EC2.Model.Snapshot>(audit_params,
                                                                ECCData.Snapshot.Current, "");

                // TODO:  Address snapshot qurey
                //ECCData.SnapshotAttributes = new List<SnapshotAttribute>();
                //foreach(Snapshot sh in ECCData.Snapshot )
                //{
                //    Amazon.EC2.Model.DescribeSnapshotAttributeResponse sharesp =
                //        EC2Client.DescribeSnapshotAttribute(new DescribeSnapshotAttributeRequest()
                //            .WithSnapshotId(sh.SnapshotId));
                //    ECCData.SnapshotAttributes.Add(sharesp.DescribeSnapshotAttributeResult.SnapshotAttribute);
                //}

                try
                {
                    ECCData.SpotDatafeedSubscription = new ListComparisonResults<Amazon.EC2.Model.SpotDatafeedSubscription>();
                    Amazon.EC2.Model.DescribeSpotDatafeedSubscriptionResponse sptdfresp =
                               EC2Client.DescribeSpotDatafeedSubscription(new DescribeSpotDatafeedSubscriptionRequest());
                    ECCData.SpotDatafeedSubscription.Current.Add(sptdfresp.DescribeSpotDatafeedSubscriptionResult.SpotDatafeedSubscription);
                    //check for changes
                    ECCData.SpotDatafeedSubscription = Auditor.CheckCIBaseline<Amazon.EC2.Model.SpotDatafeedSubscription>(audit_params,
                                                                ECCData.SpotDatafeedSubscription.Current, "");

                }
                catch (AmazonEC2Exception ex)
                {
                    if (!ex.Message.Equals("Spot datafeed subscription does not exist."))
                        log.Fatal(String.Format("Fatal Error in function {0} ", ReflectionHelper.GetMyFunctionName()), ex);
                    else
                        log.Info(String.Format("Info in function {0} message: {1}", ReflectionHelper.GetMyFunctionName(), ex.Message));
                }
                log.Info(String.Format("Info in function {0} message: {1}", ReflectionHelper.GetMyFunctionName(), "TODO: Address the pricing and history calls"));
                // TODO: Address pricing and history reading
                //ECCData.SpotInstanceRequest = new List<SpotInstanceRequest>();
                //Amazon.EC2.Model.DescribeSpotInstanceRequestsResponse sptiresp =
                //    EC2Client.DescribeSpotInstanceRequests(new DescribeSpotInstanceRequestsRequest());
                //ECCData.SpotInstanceRequest.AddRange(sptiresp.DescribeSpotInstanceRequestsResult.SpotInstanceRequest);

                //ECCData.SpotPriceHistory = new List<SpotPriceHistory>();
                //Amazon.EC2.Model.DescribeSpotPriceHistoryResponse sptprhresp =
                //    EC2Client.DescribeSpotPriceHistory(new DescribeSpotPriceHistoryRequest());
                //ECCData.SpotPriceHistory.AddRange(sptprhresp.DescribeSpotPriceHistoryResult.SpotPriceHistory);

                ECCData.Subnet = new ListComparisonResults<Amazon.EC2.Model.Subnet>();
                Amazon.EC2.Model.DescribeSubnetsResponse subnetresp =
                    EC2Client.DescribeSubnets(new DescribeSubnetsRequest());
                ECCData.Subnet.Current.AddRange(subnetresp.DescribeSubnetsResult.Subnet);
                //check for changes
                ECCData.Subnet = Auditor.CheckCIBaseline<Amazon.EC2.Model.Subnet>(audit_params,
                                                                    ECCData.Subnet.Current, "");



                ECCData.Volume = new ListComparisonResults<Amazon.EC2.Model.Volume>();
                Amazon.EC2.Model.DescribeVolumesResponse volresp =
                    EC2Client.DescribeVolumes(new DescribeVolumesRequest());
                ECCData.Volume.Current.AddRange(volresp.DescribeVolumesResult.Volume);
                //check for changes
                ECCData.Volume = Auditor.CheckCIBaseline<Amazon.EC2.Model.Volume>(audit_params,
                                                                    ECCData.Volume.Current, "");

                ECCData.VolumeStatus = new ListComparisonResults<Amazon.EC2.Model.VolumeStatus>();
                Amazon.EC2.Model.DescribeVolumeStatusResponse volstatresp =
                    EC2Client.DescribeVolumeStatus(new DescribeVolumeStatusRequest());
                ECCData.VolumeStatus.Current.AddRange(volstatresp.DescribeVolumeStatusResult.VolumeStatus);
                //check for changes
                ECCData.VolumeStatus = Auditor.CheckCIBaseline<Amazon.EC2.Model.VolumeStatus>(audit_params,
                                                                    ECCData.VolumeStatus.Current, "");



                ECCData.VolumeAttributes = new ListComparisonResults<Amazon.EC2.Model.DescribeVolumeAttributeResult>();
                foreach (Volume v in ECCData.Volume.Current)
                {

                    Amazon.EC2.Model.DescribeVolumeAttributeResponse volatresp =
                        EC2Client.DescribeVolumeAttribute(new DescribeVolumeAttributeRequest().WithAttribute("autoEnableIO").WithVolumeId(v.VolumeId));
                    ECCData.VolumeAttributes.Current.Add(volatresp.DescribeVolumeAttributeResult);
                }
                //check for changes
                ECCData.VolumeAttributes = Auditor.CheckCIBaseline<Amazon.EC2.Model.DescribeVolumeAttributeResult>(audit_params,
                                                                            ECCData.VolumeAttributes.Current, "");


                ECCData.Vpc = new ListComparisonResults<Amazon.EC2.Model.Vpc>();
                Amazon.EC2.Model.DescribeVpcsResponse vpcresp =
                    EC2Client.DescribeVpcs(new DescribeVpcsRequest());
                ECCData.Vpc.Current.AddRange(vpcresp.DescribeVpcsResult.Vpc);
                //check for changes
                ECCData.Vpc = Auditor.CheckCIBaseline<Amazon.EC2.Model.Vpc>(audit_params, ECCData.Vpc.Current,
                                                                    "");



                ECCData.VpcAttributes = new ListComparisonResults<DescribeVpcAttributeResult>();
                foreach (Amazon.EC2.Model.Vpc v in ECCData.Vpc.Current)
                {
                    Amazon.EC2.Model.DescribeVpcAttributeResponse vpcatresp =
                        EC2Client.DescribeVpcAttribute(new DescribeVpcAttributeRequest()
                            .WithAttribute("enableDnsSupport")
                            .WithVpcId(v.VpcId));

                    ECCData.VpcAttributes.Current.Add(vpcatresp.DescribeVpcAttributeResult);
                    vpcatresp =
                        EC2Client.DescribeVpcAttribute(new DescribeVpcAttributeRequest()
                            .WithAttribute("enableDnsHostnames")
                            .WithVpcId(v.VpcId));
                    ECCData.VpcAttributes.Current.Add(vpcatresp.DescribeVpcAttributeResult);
                }
                //check for changes
                ECCData.VpcAttributes = Auditor.CheckCIBaseline<Amazon.EC2.Model.DescribeVpcAttributeResult>(audit_params, ECCData.VpcAttributes.Current, "");


                ECCData.VpnConnections = new ListComparisonResults<Amazon.EC2.Model.VpnConnection>();
                Amazon.EC2.Model.DescribeVpnConnectionsResponse vpnconresp =
                    EC2Client.DescribeVpnConnections(new DescribeVpnConnectionsRequest());
                ECCData.VpnConnections.Current.AddRange(vpnconresp.DescribeVpnConnectionsResult.VpnConnection);
                //check for changes
                ECCData.VpnConnections = Auditor.CheckCIBaseline<Amazon.EC2.Model.VpnConnection>(audit_params,
                                            ECCData.VpnConnections.Current, "");


                ECCData.VpnGateways = new ListComparisonResults<Amazon.EC2.Model.VpnGateway>();
                Amazon.EC2.Model.DescribeVpnGatewaysResponse vpngwresp =
                    EC2Client.DescribeVpnGateways(new DescribeVpnGatewaysRequest());
                ECCData.VpnGateways.Current.AddRange(vpngwresp.DescribeVpnGatewaysResult.VpnGateway);
                //check for changes
                ECCData.VpnGateways = Auditor.CheckCIBaseline<Amazon.EC2.Model.VpnGateway>(audit_params, ECCData.VpnGateways.Current, "");


            }

            return ECCData;
        }
        /// <summary>
        /// IsTruncated Review completed
        /// </summary>
        /// <returns></returns>
        public  ElastiCache ReadElastiCache(AuditParams audit_params)
        {

            BaselineAuditor Auditor = new BaselineAuditor();
            ElastiCache ECData = new ElastiCache();
            StringBuilder sb = new StringBuilder(1024);
            using (StringWriter sr = new StringWriter(sb))
            {
                Amazon.ElastiCache.AmazonElastiCacheClient ECClient = new Amazon.ElastiCache.AmazonElastiCacheClient(audit_params.AWSCredentials, audit_params.AWSRegion);

                ECData.CacheClusters = new ListComparisonResults<Amazon.ElastiCache.Model.CacheCluster>();
                //Returns information about all provisioned Cache Clusters if no Cache Cluster identifier is specified, or about a specific Cache Cluster if a Cache Cluster identifier is supplied. 
                Amazon.ElastiCache.Model.DescribeCacheClustersResponse ccresp =
                    ECClient.DescribeCacheClusters(
                        new Amazon.ElastiCache.Model.DescribeCacheClustersRequest().WithShowCacheNodeInfo(true));
                ECData.CacheClusters.Current.AddRange(ccresp.DescribeCacheClustersResult.CacheClusters);
                //check for changes
                ECData.CacheClusters = Auditor.CheckCIBaseline<Amazon.ElastiCache.Model.CacheCluster>(audit_params, ECData.CacheClusters.Current, "");

                ECData.CacheParameterGroups = new ListComparisonResults<Amazon.ElastiCache.Model.CacheParameterGroup>();
                //Returns a list of CacheParameterGroup descriptions. If a CacheParameterGroupName is specified, the list will contain only the descriptions of the specified CacheParameterGroup
                Amazon.ElastiCache.Model.DescribeCacheParameterGroupsResponse cpresp =
                    ECClient.DescribeCacheParameterGroups();
                ECData.CacheParameterGroups.Current.AddRange(cpresp.DescribeCacheParameterGroupsResult.CacheParameterGroups);
                //check for changes
                ECData.CacheParameterGroups = Auditor.CheckCIBaseline<Amazon.ElastiCache.Model.CacheParameterGroup>(audit_params, ECData.CacheParameterGroups.Current, "");


                ECData.CacheParameterGroupParametersList = new ListComparisonResults<ElastiCacheParameterGroupParameters>();
                // TODO: if T = Amazon.DirectConnect.Model.DescribeOfferingDetailResult there's an error serializing the object to JSON
                /*//Returns the detailed parameter list for a particular CacheParameterGroup. 
                foreach (Amazon.ElastiCache.Model.CacheParameterGroup pg in ECData.CacheParameterGroups.Current)
                {
                    Amazon.ElastiCache.Model.DescribeCacheParametersResponse parresp =
                        ECClient.DescribeCacheParameters(
                            new Amazon.ElastiCache.Model.DescribeCacheParametersRequest().WithCacheParameterGroupName(pg.CacheParameterGroupName));
                    ECData.CacheParameterGroupParametersList.Current.Add(
                        new ElastiCacheParameterGroupParameters(pg.CacheParameterGroupName, 
                            parresp.DescribeCacheParametersResult.Parameters));
                }
                //check for changes
                
                ECData.CacheParameterGroupParametersList= Auditor.CheckCIBaseline<ElastiCacheParameterGroupParameters>(audit_params,ECData.CacheParameterGroupParametersList.Current, "");
                */

                //Returns a list of CacheSecurityGroup descriptions. If a CacheSecurityGroupName is specified, the list will contain only the description of the specified CacheSecurityGroup.
                ECData.CacheSecurityGroups = new ListComparisonResults<Amazon.ElastiCache.Model.CacheSecurityGroup>();
                try
                {
                    Amazon.ElastiCache.Model.DescribeCacheSecurityGroupsResponse sgresp =
                                    ECClient.DescribeCacheSecurityGroups();
                    ECData.CacheSecurityGroups.Current.AddRange(sgresp.DescribeCacheSecurityGroupsResult.CacheSecurityGroups);
                }
                catch (Exception ex)
                {
                    if (!ex.Message.Contains("Use of cache security groups is not permitted in this API version for your account."))
                        log.Fatal(String.Format("Fatal Error in function {0} ", ReflectionHelper.GetMyFunctionName()), ex);
                    else
                        log.Info(String.Format("Info in function {0} message: {1}", ReflectionHelper.GetMyFunctionName(), ex.Message));
                }
                //check for changes
                ECData.CacheSecurityGroups = Auditor.CheckCIBaseline<Amazon.ElastiCache.Model.CacheSecurityGroup>(audit_params, ECData.CacheSecurityGroups.Current, "");

                //Returns a list of CacheSubnetGroup descriptions. If a CacheSubnetGroupName is specified, the list will contain only the description of the specified Cache Subnet Group. 
                ECData.CacheSubnetGroups = new ListComparisonResults<Amazon.ElastiCache.Model.CacheSubnetGroup>();
                Amazon.ElastiCache.Model.DescribeCacheSubnetGroupsResponse csngresp =
                    ECClient.DescribeCacheSubnetGroups();
                ECData.CacheSubnetGroups.Current.AddRange(csngresp.DescribeCacheSubnetGroupsResult.CacheSubnetGroups);
                //check for changes
                ECData.CacheSubnetGroups = Auditor.CheckCIBaseline<Amazon.ElastiCache.Model.CacheSubnetGroup>(audit_params, ECData.CacheSubnetGroups.Current, "");


                //Returns information about reserved Cache Nodes for this account, or about a specified reserved Cache Node.
                ECData.ReservedCacheNodes = new ListComparisonResults<Amazon.ElastiCache.Model.ReservedCacheNode>();
                Amazon.ElastiCache.Model.DescribeReservedCacheNodesResponse rcnresp =
                    ECClient.DescribeReservedCacheNodes();
                ECData.ReservedCacheNodes.Current.AddRange(rcnresp.DescribeReservedCacheNodesResult.ReservedCacheNodes);
                //check for changes
                ECData.ReservedCacheNodes = Auditor.CheckCIBaseline<Amazon.ElastiCache.Model.ReservedCacheNode>(audit_params, ECData.ReservedCacheNodes.Current, "");

                //Lists available reserved Cache Node offerings. 
                //Amazon.ElastiCache.Model.DescribeReservedCacheNodesOfferingsResponse rcnoffresp =
                //    ECClient.DescribeReservedCacheNodesOfferings();


            }


            return ECData;
        }
        /// <summary>
        /// IsTruncated Review completed
        /// </summary>
        /// <returns></returns>
        public  ElastiBeanstalk ReadElastiBeanstalk(AuditParams audit_params)
        {


            BaselineAuditor Auditor = new BaselineAuditor();


            StringBuilder sb = new StringBuilder(1024);
            ElastiBeanstalk EBData = new ElastiBeanstalk();
            using (StringWriter sr = new StringWriter(sb))
            {
                Amazon.ElasticBeanstalk.AmazonElasticBeanstalkClient EBClient =
                    new Amazon.ElasticBeanstalk.AmazonElasticBeanstalkClient(audit_params.AWSCredentials, audit_params.AWSRegion);

                EBData.ApplicationDescriptions = new ListComparisonResults<Amazon.ElasticBeanstalk.Model.ApplicationDescription>();

                Amazon.ElasticBeanstalk.Model.DescribeApplicationsResponse apresp =
                    EBClient.DescribeApplications();
                EBData.ApplicationDescriptions.Current.AddRange(apresp.DescribeApplicationsResult.Applications);
                //check for changes
                EBData.ApplicationDescriptions = Auditor.CheckCIBaseline<Amazon.ElasticBeanstalk.Model.ApplicationDescription>(audit_params, EBData.ApplicationDescriptions.Current, "");

                EBData.ApplicationVersionDescriptions = new ListComparisonResults<Amazon.ElasticBeanstalk.Model.ApplicationVersionDescription>();
                Amazon.ElasticBeanstalk.Model.DescribeApplicationVersionsResponse avresp =
                    EBClient.DescribeApplicationVersions();
                EBData.ApplicationVersionDescriptions.Current.AddRange(avresp.DescribeApplicationVersionsResult.ApplicationVersions);
                //check for changes
                EBData.ApplicationVersionDescriptions = Auditor.CheckCIBaseline<Amazon.ElasticBeanstalk.Model.ApplicationVersionDescription>(audit_params, EBData.ApplicationVersionDescriptions.Current, "");


                EBData.EnvironmentDescriptions = new ListComparisonResults<Amazon.ElasticBeanstalk.Model.EnvironmentDescription>();
                //Returns descriptions for existing environments.
                Amazon.ElasticBeanstalk.Model.DescribeEnvironmentsResponse envresp =
                    EBClient.DescribeEnvironments();
                EBData.EnvironmentDescriptions.Current.AddRange(envresp.DescribeEnvironmentsResult.Environments);
                //check for changes
                EBData.EnvironmentDescriptions = Auditor.CheckCIBaseline<Amazon.ElasticBeanstalk.Model.EnvironmentDescription>(audit_params, EBData.EnvironmentDescriptions.Current, "");


                EBData.EnvironmentConfigurations = new ListComparisonResults<Amazon.ElasticBeanstalk.Model.DescribeConfigurationOptionsResult>();
                foreach (Amazon.ElasticBeanstalk.Model.EnvironmentDescription env in
                            EBData.EnvironmentDescriptions.Current)
                {
                    Amazon.ElasticBeanstalk.Model.DescribeConfigurationOptionsResponse configresp =
                        EBClient.DescribeConfigurationOptions(
                            new Amazon.ElasticBeanstalk.Model.DescribeConfigurationOptionsRequest()
                                .WithSolutionStackName(env.SolutionStackName));
                    EBData.EnvironmentConfigurations.Current.Add(configresp.DescribeConfigurationOptionsResult);
                }
                //check for changes
                EBData.EnvironmentConfigurations = Auditor.CheckCIBaseline<Amazon.ElasticBeanstalk.Model.DescribeConfigurationOptionsResult>(audit_params, EBData.EnvironmentConfigurations.Current, "");


                //A list of environment resources
                EBData.EnvironmentResources = new ListComparisonResults<Amazon.ElasticBeanstalk.Model.EnvironmentResourceDescription>();
                foreach (Amazon.ElasticBeanstalk.Model.EnvironmentDescription env in
                            EBData.EnvironmentDescriptions.Current)
                {
                    Amazon.ElasticBeanstalk.Model.DescribeEnvironmentResourcesResponse envresourcesresp =
                        EBClient.DescribeEnvironmentResources(
                            new Amazon.ElasticBeanstalk.Model.DescribeEnvironmentResourcesRequest().WithEnvironmentName(env.EnvironmentName));
                    EBData.EnvironmentResources.Current.Add(envresourcesresp.DescribeEnvironmentResourcesResult.EnvironmentResources);
                }
                //check for changes
                EBData.EnvironmentResources = Auditor.CheckCIBaseline<Amazon.ElasticBeanstalk.Model.EnvironmentResourceDescription>(audit_params, EBData.EnvironmentResources.Current, "");


            }



            return EBData;
        }
        /// <summary>
        /// IsTruncated Review completed
        /// </summary>
        /// <returns></returns>
        public  ElasticLoadBalancing ReadElasticLoadBalancing(AuditParams audit_params)
        {

            BaselineAuditor Auditor = new BaselineAuditor();



            StringBuilder sb = new StringBuilder(1024);
            ElasticLoadBalancing ELBData = new ElasticLoadBalancing();
            using (StringWriter sr = new StringWriter(sb))
            {

                Amazon.ElasticLoadBalancing.AmazonElasticLoadBalancingClient ELBClient = new Amazon.ElasticLoadBalancing.AmazonElasticLoadBalancingClient(audit_params.AWSCredentials, audit_params.AWSRegion);

                //Returns detailed configuration information for the specified LoadBalancers. If no LoadBalancers are specified, the operation returns configuration information for all LoadBalancers created by the caller. 
                // NOTE: The client must have created the specified input LoadBalancers in order to retrieve this information; the client must provide the same account credentials as those that were used to create the LoadBalancer.

                Amazon.ElasticLoadBalancing.Model.DescribeLoadBalancersResponse delbresp =
                    ELBClient.DescribeLoadBalancers(new Amazon.ElasticLoadBalancing.Model.DescribeLoadBalancersRequest());
                ELBData.ElasticLoadBalancerAndPolicies = new ListComparisonResults<ElasticLoadBalancerPolicies>();

                foreach (Amazon.ElasticLoadBalancing.Model.LoadBalancerDescription lb in
                        delbresp.DescribeLoadBalancersResult.LoadBalancerDescriptions)
                {
                    Amazon.ElasticLoadBalancing.Model.DescribeLoadBalancerPoliciesResponse delbpolresp =
                        ELBClient.DescribeLoadBalancerPolicies(new Amazon.ElasticLoadBalancing.Model.DescribeLoadBalancerPoliciesRequest().WithLoadBalancerName(lb.LoadBalancerName));
                    ELBData.ElasticLoadBalancerAndPolicies.Current.Add(
                        new ElasticLoadBalancerPolicies(lb, delbpolresp.DescribeLoadBalancerPoliciesResult.PolicyDescriptions));
                }
                //check for changes
                ELBData.ElasticLoadBalancerAndPolicies = Auditor.CheckCIBaseline<ElasticLoadBalancerPolicies>(audit_params, ELBData.ElasticLoadBalancerAndPolicies.Current, "");

            }

            return ELBData;
        }
        public  ElasticMapReduce ReadElasticMapReduce(AuditParams audit_params)
        {
            BaselineAuditor Auditor = new BaselineAuditor();


            ElasticMapReduce EMRData = new ElasticMapReduce();
            StringBuilder sb = new StringBuilder(1024);
            using (StringWriter sr = new StringWriter(sb))
            {
                Amazon.ElasticMapReduce.AmazonElasticMapReduceClient EMPClient = new Amazon.ElasticMapReduce.AmazonElasticMapReduceClient(audit_params.AWSCredentials, audit_params.AWSRegion);
                Amazon.ElasticMapReduce.Model.DescribeJobFlowsResponse djfresp =
                    EMPClient.DescribeJobFlows();
                EMRData.JobFlows = new ListComparisonResults<Amazon.ElasticMapReduce.Model.JobFlowDetail>();
                EMRData.JobFlows.Current.AddRange(djfresp.DescribeJobFlowsResult.JobFlows);


                EMRData.JobFlows = Auditor.CheckCIBaseline<Amazon.ElasticMapReduce.Model.JobFlowDetail>(audit_params, EMRData.JobFlows.Current, "");



            }


            return EMRData;
        }
        public  ElasticTranscoder ReadElasticTranscoder(AuditParams audit_params)
        {
            BaselineAuditor Auditor = new BaselineAuditor();


            ElasticTranscoder ETData = new ElasticTranscoder();
            StringBuilder sb = new StringBuilder(1024);
            using (StringWriter sr = new StringWriter(sb))
            {
                Amazon.ElasticTranscoder.AmazonElasticTranscoderClient ETClient = new Amazon.ElasticTranscoder.AmazonElasticTranscoderClient(audit_params.AWSCredentials, audit_params.AWSRegion);

                ETData.Pipelines = new ListComparisonResults<Amazon.ElasticTranscoder.Model.Pipeline>();
                try
                {
                    Amazon.ElasticTranscoder.Model.ListPipelinesResponse lplresp = ETClient.ListPipelines();
                    ETData.Pipelines.Current.AddRange(lplresp.ListPipelinesResult.Pipelines);
                    ETData.Jobs = new ListComparisonResults<Amazon.ElasticTranscoder.Model.Job>();
                    foreach (Amazon.ElasticTranscoder.Model.Pipeline pipe in ETData.Pipelines.Current)
                    {
                        Amazon.ElasticTranscoder.Model.ListJobsByPipelineResponse ljbplresp =
                            ETClient.ListJobsByPipeline(new Amazon.ElasticTranscoder.Model.ListJobsByPipelineRequest().WithPipelineId(pipe.Id));
                        ETData.Jobs.Current.AddRange(ljbplresp.ListJobsByPipelineResult.Jobs);
                        while (ljbplresp.ListJobsByPipelineResult.NextPageToken != null)
                        {
                            ljbplresp = ETClient.ListJobsByPipeline(new Amazon.ElasticTranscoder.Model.ListJobsByPipelineRequest()
                                                            .WithPipelineId(pipe.Id)
                                                            .WithPageToken(ljbplresp.ListJobsByPipelineResult.NextPageToken));
                            ETData.Jobs.Current.AddRange(ljbplresp.ListJobsByPipelineResult.Jobs);
                        }
                    }
                    ETData.Jobs = Auditor.CheckCIBaseline<Amazon.ElasticTranscoder.Model.Job>(audit_params, ETData.Jobs.Current, "");
                    ETData.Pipelines = Auditor.CheckCIBaseline<Amazon.ElasticTranscoder.Model.Pipeline>(audit_params, ETData.Pipelines.Current, "");
                }
                catch (Exception ex)
                {
                    if (!ex.Message.StartsWith("The remote name could not be resolved:"))
                        log.Fatal(String.Format("Fatal Error in function {0} ", ReflectionHelper.GetMyFunctionName()), ex);
                    else
                        log.Info(String.Format("Info in function {0} message: {1}", ReflectionHelper.GetMyFunctionName(), ex.Message));
                }



                ETData.Presets = new ListComparisonResults<Amazon.ElasticTranscoder.Model.Preset>();
                try
                {
                    Amazon.ElasticTranscoder.Model.ListPresetsResponse lpresp =
                        ETClient.ListPresets(new Amazon.ElasticTranscoder.Model.ListPresetsRequest());
                    ETData.Presets.Current.AddRange(lpresp.ListPresetsResult.Presets);
                }
                catch (Exception ex)
                {
                    if (!ex.Message.StartsWith("The remote name could not be resolved:"))
                        log.Fatal(String.Format("Fatal Error in function {0} ", ReflectionHelper.GetMyFunctionName()), ex);
                    else
                        log.Info(String.Format("Info in function {0} message: {1}", ReflectionHelper.GetMyFunctionName(), ex.Message));
                }
                ETData.Presets = Auditor.CheckCIBaseline<Amazon.ElasticTranscoder.Model.Preset>(audit_params, ETData.Presets.Current, "");


            }


            return ETData;
        }
        public  Glacier ReadGlacier(AuditParams audit_params)
        {

            BaselineAuditor Auditor = new BaselineAuditor();

            Glacier GData = new Glacier();
            StringBuilder sb = new StringBuilder(1024);
            using (StringWriter sr = new StringWriter(sb))
            {
                try
                {
                    Amazon.Glacier.AmazonGlacierClient GClient = new Amazon.Glacier.AmazonGlacierClient(audit_params.AWSCredentials, audit_params.AWSRegion);
                    GData.Jobs = new ListComparisonResults<Amazon.Glacier.Model.GlacierJobDescription>();
                    Amazon.Glacier.Model.ListJobsResponse ljresp =
                        GClient.ListJobs(new Amazon.Glacier.Model.ListJobsRequest());
                    GData.Jobs.Current.AddRange(ljresp.ListJobsResult.JobList);
                    while (ljresp.ListJobsResult.Marker != null)
                    {
                        ljresp = GClient.ListJobs(new Amazon.Glacier.Model.ListJobsRequest()
                                            .WithMarker(ljresp.ListJobsResult.Marker));
                        GData.Jobs.Current.AddRange(ljresp.ListJobsResult.JobList);
                    }
                    GData.Jobs = Auditor.CheckCIBaseline<Amazon.Glacier.Model.GlacierJobDescription>(audit_params, GData.Jobs.Current, "");

                    GData.MultiPartUploads = new ListComparisonResults<Amazon.Glacier.Model.UploadListElement>();
                    Amazon.Glacier.Model.ListMultipartUploadsResponse lmpuresp =
                        GClient.ListMultipartUploads(new Amazon.Glacier.Model.ListMultipartUploadsRequest());
                    GData.MultiPartUploads.Current.AddRange(lmpuresp.ListMultipartUploadsResult.UploadsList);
                    while (lmpuresp.ListMultipartUploadsResult.Marker != null)
                    {
                        lmpuresp = GClient.ListMultipartUploads(new Amazon.Glacier.Model.ListMultipartUploadsRequest()
                                            .WithUploadIdMarker(lmpuresp.ListMultipartUploadsResult.Marker));
                        GData.MultiPartUploads.Current.AddRange(lmpuresp.ListMultipartUploadsResult.UploadsList);
                    }
                    GData.MultiPartUploads = Auditor.CheckCIBaseline<Amazon.Glacier.Model.UploadListElement>(audit_params, GData.MultiPartUploads.Current, "");


                    GData.Vaults = new ListComparisonResults<Amazon.Glacier.Model.DescribeVaultOutput>();
                    Amazon.Glacier.Model.ListVaultsResponse lvresp =
                        GClient.ListVaults();
                    GData.Vaults.Current.AddRange(lvresp.ListVaultsResult.VaultList);
                    while (lvresp.ListVaultsResult.Marker != null)
                    {
                        lvresp = GClient.ListVaults(new Amazon.Glacier.Model.ListVaultsRequest().WithMarker(lvresp.ListVaultsResult.Marker));
                        GData.Vaults.Current.AddRange(lvresp.ListVaultsResult.VaultList);
                    }
                    GData.Vaults = Auditor.CheckCIBaseline<Amazon.Glacier.Model.DescribeVaultOutput>(audit_params, GData.Vaults.Current, "");


                }
                catch (Exception ex)
                {
                    log.Fatal(String.Format("Fatal Error in function {0} ", ReflectionHelper.GetMyFunctionName()), ex);
                }

            }



            return GData;
        }
        public  IdentityAccountManagement ReadIAM(AuditParams audit_params)
        {

            BaselineAuditor Auditor = new BaselineAuditor();


            IdentityAccountManagement IAMData = new IdentityAccountManagement();

            StringBuilder sb = new StringBuilder(1024);
            using (StringWriter sr = new StringWriter(sb))
            {

                Amazon.IdentityManagement.AmazonIdentityManagementServiceConfig cfg =
                    new Amazon.IdentityManagement.AmazonIdentityManagementServiceConfig();
                Amazon.IdentityManagement.AmazonIdentityManagementServiceClient IAMClient
                    = new Amazon.IdentityManagement.AmazonIdentityManagementServiceClient(audit_params.AWSCredentials);



                //Get a user list
                IAMData.Users = new ListComparisonResults<Amazon.IdentityManagement.Model.User>();
                IAMData.Users.Current = GetUsers(IAMClient);
                //check for changes
                IAMData.Users = Auditor.CheckCIBaseline<Amazon.IdentityManagement.Model.User>(audit_params, IAMData.Users.Current, "");



                //Get AccessKeyMetadata
                IAMData.AccessKeyMetadata = new ListComparisonResults<Amazon.IdentityManagement.Model.AccessKeyMetadata>();
                IAMData.AccessKeyMetadata.Current = GetAccessKeyMetadata(IAMClient);
                IAMData.AccessKeyMetadata = Auditor.CheckCIBaseline<Amazon.IdentityManagement.Model.AccessKeyMetadata>(audit_params,
                                                IAMData.AccessKeyMetadata.Current, "");
                //Get the Groups
                IAMData.Groups = new ListComparisonResults<Amazon.IdentityManagement.Model.Group>();
                IAMData.Groups.Current = GetGroups(IAMClient);
                //check for changes
                IAMData.Groups = Auditor.CheckCIBaseline<Amazon.IdentityManagement.Model.Group>(audit_params, IAMData.Groups.Current, "");

                //Get the Group Policies
                IAMData.GroupPolicyMetadata = new ListComparisonResults<Amazon.IdentityManagement.Model.GetGroupPolicyResult>();
                IAMData.GroupPolicyMetadata.Current = GetAllGroupPolicyMetadata(IAMClient);
                //check for changes
                IAMData.GroupPolicyMetadata = Auditor.CheckCIBaseline<Amazon.IdentityManagement.Model.GetGroupPolicyResult>(audit_params, IAMData.GroupPolicyMetadata.Current, "");

                //Get Group data
                IAMData.GroupMetadata = new ListComparisonResults<Amazon.IdentityManagement.Model.GetGroupResult>();
                IAMData.GroupMetadata.Current = GetGroupData(IAMData.Groups.Current, IAMClient);
                //check for changes to groups for additions and deletions
                IAMData.GroupMetadata = Auditor.CheckCIBaseline<Amazon.IdentityManagement.Model.GetGroupResult>(audit_params, IAMData.GroupMetadata.Current, "");

                // List roles
                IAMData.Roles = new ListComparisonResults<Amazon.IdentityManagement.Model.Role>();
                IAMData.Roles.Current = GetRoles(IAMClient);
                //check for changes
                IAMData.Roles = Auditor.CheckCIBaseline<Amazon.IdentityManagement.Model.Role>(audit_params, IAMData.Roles.Current, "");

                IAMData.GetRolePolicyResult = new ListComparisonResults<Amazon.IdentityManagement.Model.GetRolePolicyResult>();
                foreach (Amazon.IdentityManagement.Model.Role r in IAMData.Roles.Current)
                {
                    Amazon.IdentityManagement.Model.ListRolePoliciesResponse lrpolresp =
                            IAMClient.ListRolePolicies(new Amazon.IdentityManagement.Model.ListRolePoliciesRequest().WithRoleName(r.RoleName));
                    while (lrpolresp.ListRolePoliciesResult.IsTruncated)
                    {
                        foreach (string polname in lrpolresp.ListRolePoliciesResult.PolicyNames)
                        {
                            Amazon.IdentityManagement.Model.GetRolePolicyResponse rpresp =
                                IAMClient.GetRolePolicy(new Amazon.IdentityManagement.Model.GetRolePolicyRequest()
                                                            .WithRoleName(r.RoleName)
                                                            .WithPolicyName(polname));
                            IAMData.GetRolePolicyResult.Current.Add(rpresp.GetRolePolicyResult);
                        }
                        //get more pols
                        lrpolresp =
                            IAMClient.ListRolePolicies(new Amazon.IdentityManagement.Model.ListRolePoliciesRequest()
                            .WithRoleName(r.RoleName)
                            .WithMarker(lrpolresp.ListRolePoliciesResult.Marker));
                    }
                }

                //check for changes
                IAMData.GetRolePolicyResult = Auditor.CheckCIBaseline<Amazon.IdentityManagement.Model.GetRolePolicyResult>(audit_params, IAMData.GetRolePolicyResult.Current, "");


                IAMData.UserPolicyMetadata = new ListComparisonResults<Amazon.IdentityManagement.Model.GetUserPolicyResult>();
                foreach (Amazon.IdentityManagement.Model.User u in IAMData.Users.Current)
                {
                    List<string> policies = new List<string>();
                    Amazon.IdentityManagement.Model.ListUserPoliciesResponse ur_resp =
                        IAMClient.ListUserPolicies(
                            new Amazon.IdentityManagement.Model.ListUserPoliciesRequest().WithUserName(u.UserName));
                    policies.AddRange(ur_resp.ListUserPoliciesResult.PolicyNames);
                    while (ur_resp.ListUserPoliciesResult.IsTruncated)
                    {
                        ur_resp = IAMClient.ListUserPolicies(
                                            new Amazon.IdentityManagement.Model.ListUserPoliciesRequest()
                                                .WithUserName(u.UserName)
                                                .WithMarker(ur_resp.ListUserPoliciesResult.Marker));
                        policies.AddRange(ur_resp.ListUserPoliciesResult.PolicyNames);
                    }

                    foreach (string policy in ur_resp.ListUserPoliciesResult.PolicyNames)
                    {
                        Amazon.IdentityManagement.Model.GetUserPolicyResponse resp = IAMClient.GetUserPolicy(new
                             Amazon.IdentityManagement.Model.GetUserPolicyRequest()
                             .WithUserName(u.UserName)
                             .WithPolicyName(policy));
                        IAMData.UserPolicyMetadata.Current.Add(resp.GetUserPolicyResult);
                    }
                }
                //check for changes
                IAMData.UserPolicyMetadata = Auditor.CheckCIBaseline<Amazon.IdentityManagement.Model.GetUserPolicyResult>(audit_params, IAMData.UserPolicyMetadata.Current, "");



                IAMData.InstanceProfiles = new ListComparisonResults<Amazon.IdentityManagement.Model.InstanceProfile>();
                Amazon.IdentityManagement.Model.ListInstanceProfilesResponse ipResp = IAMClient.ListInstanceProfiles();
                IAMData.InstanceProfiles.Current.AddRange(ipResp.ListInstanceProfilesResult.InstanceProfiles);
                while (ipResp.ListInstanceProfilesResult.IsTruncated)
                {
                    ipResp = IAMClient.ListInstanceProfiles(new Amazon.IdentityManagement.Model.ListInstanceProfilesRequest()
                        .WithMarker(ipResp.ListInstanceProfilesResult.Marker));
                    IAMData.InstanceProfiles.Current.AddRange(ipResp.ListInstanceProfilesResult.InstanceProfiles);
                }
                //check for changes
                IAMData.InstanceProfiles = Auditor.CheckCIBaseline<Amazon.IdentityManagement.Model.InstanceProfile>(audit_params, IAMData.InstanceProfiles.Current, "");

                IAMData.LoginProfiles = new ListComparisonResults<Amazon.IdentityManagement.Model.LoginProfile>();
                foreach (Amazon.IdentityManagement.Model.User u in IAMData.Users.Current)
                {
                    try
                    {
                        Amazon.IdentityManagement.Model.GetLoginProfileResponse lpResp = IAMClient.GetLoginProfile(
                            new Amazon.IdentityManagement.Model.GetLoginProfileRequest().WithUserName(u.UserName));
                        IAMData.LoginProfiles.Current.Add(lpResp.GetLoginProfileResult.LoginProfile);
                    }
                    catch (Exception ex)
                    {
                        if(!ex.Message.StartsWith("Cannot find Login Profile for User"))
                            log.Fatal(String.Format("Fatal Error in function {0} ", ReflectionHelper.GetMyFunctionName()), ex);
                        else
                            log.Info(String.Format("User {0} does not have a profile", u.Arn));
                    }
                }
                //check for changes
                IAMData.LoginProfiles = Auditor.CheckCIBaseline<Amazon.IdentityManagement.Model.LoginProfile>(audit_params, IAMData.LoginProfiles.Current, "");

                IAMData.MFADevices = new ListComparisonResults<Amazon.IdentityManagement.Model.MFADevice>();
                foreach (Amazon.IdentityManagement.Model.User u in IAMData.Users.Current)
                {
                    Amazon.IdentityManagement.Model.ListMFADevicesResponse lmResp = IAMClient.ListMFADevices(
                       new Amazon.IdentityManagement.Model.ListMFADevicesRequest().WithUserName(u.UserName));
                    IAMData.MFADevices.Current.AddRange(lmResp.ListMFADevicesResult.MFADevices);
                    while (lmResp.ListMFADevicesResult.IsTruncated)
                    {
                        lmResp = IAMClient.ListMFADevices(
                                               new Amazon.IdentityManagement.Model.ListMFADevicesRequest()
                                               .WithUserName(u.UserName)
                                               .WithMarker(lmResp.ListMFADevicesResult.Marker));
                        IAMData.MFADevices.Current.AddRange(lmResp.ListMFADevicesResult.MFADevices);
                    }
                }
                //check for changes
                IAMData.MFADevices = Auditor.CheckCIBaseline<Amazon.IdentityManagement.Model.MFADevice>(audit_params, IAMData.MFADevices.Current, "");

                IAMData.PasswordPolicy = new ListComparisonResults<Amazon.IdentityManagement.Model.PasswordPolicy>();
                Amazon.IdentityManagement.Model.GetAccountPasswordPolicyResponse pwPol = IAMClient.GetAccountPasswordPolicy(
                    new Amazon.IdentityManagement.Model.GetAccountPasswordPolicyRequest());
                IAMData.PasswordPolicy.Current.Add(pwPol.GetAccountPasswordPolicyResult.PasswordPolicy);
                //check for changes
                IAMData.PasswordPolicy = Auditor.CheckCIBaseline<Amazon.IdentityManagement.Model.PasswordPolicy>(audit_params, IAMData.PasswordPolicy.Current, "");


                IAMData.ServerCertificateMetadata = new ListComparisonResults<Amazon.IdentityManagement.Model.ServerCertificateMetadata>();

                Amazon.IdentityManagement.Model.ListServerCertificatesResponse lsCert = IAMClient.ListServerCertificates();
                IAMData.ServerCertificateMetadata.Current.AddRange(lsCert.ListServerCertificatesResult.ServerCertificateMetadataList);
                while (lsCert.ListServerCertificatesResult.IsTruncated)
                {
                    lsCert = IAMClient.ListServerCertificates(new Amazon.IdentityManagement.Model.ListServerCertificatesRequest()
                        .WithMarker(lsCert.ListServerCertificatesResult.Marker));
                    IAMData.ServerCertificateMetadata.Current.AddRange(lsCert.ListServerCertificatesResult.ServerCertificateMetadataList);
                }
                //check for changes
                IAMData.ServerCertificateMetadata = Auditor.CheckCIBaseline<Amazon.IdentityManagement.Model.ServerCertificateMetadata>(audit_params, IAMData.ServerCertificateMetadata.Current, "");

                IAMData.ServerCertificates = new ListComparisonResults<Amazon.IdentityManagement.Model.ServerCertificate>();
                foreach (Amazon.IdentityManagement.Model.ServerCertificateMetadata md in IAMData.ServerCertificateMetadata.Current)
                {
                    Amazon.IdentityManagement.Model.GetServerCertificateResponse scert = IAMClient.GetServerCertificate(new
                        Amazon.IdentityManagement.Model.GetServerCertificateRequest().WithServerCertificateName(md.ServerCertificateName));
                    IAMData.ServerCertificates.Current.Add(scert.GetServerCertificateResult.ServerCertificate);
                }
                //check for changes
                IAMData.ServerCertificates = Auditor.CheckCIBaseline<Amazon.IdentityManagement.Model.ServerCertificate>(audit_params, IAMData.ServerCertificates.Current, "");

                IAMData.SigningCertificates = new ListComparisonResults<Amazon.IdentityManagement.Model.SigningCertificate>();
                foreach (Amazon.IdentityManagement.Model.User u in IAMData.Users.Current)
                {
                    Amazon.IdentityManagement.Model.ListSigningCertificatesResponse scResp = IAMClient.ListSigningCertificates(
                        new Amazon.IdentityManagement.Model.ListSigningCertificatesRequest().WithUserName(u.UserName));
                    IAMData.SigningCertificates.Current.AddRange(scResp.ListSigningCertificatesResult.Certificates);
                    while (scResp.ListSigningCertificatesResult.IsTruncated)
                    {
                        scResp = IAMClient.ListSigningCertificates(
                                                new Amazon.IdentityManagement.Model.ListSigningCertificatesRequest()
                                                .WithUserName(u.UserName)
                                                .WithMarker(scResp.ListSigningCertificatesResult.Marker));
                        IAMData.SigningCertificates.Current.AddRange(scResp.ListSigningCertificatesResult.Certificates);
                    }
                }
                //check for changes
                IAMData.SigningCertificates = Auditor.CheckCIBaseline<Amazon.IdentityManagement.Model.SigningCertificate>(audit_params, IAMData.SigningCertificates.Current, "");

                IAMData.SummaryMap = new ListComparisonResults<Dictionary<string, int>>();
                Amazon.IdentityManagement.Model.GetAccountSummaryResponse sumResp =
                       IAMClient.GetAccountSummary(new Amazon.IdentityManagement.Model.GetAccountSummaryRequest());
                IAMData.SummaryMap.Current.Add(sumResp.GetAccountSummaryResult.SummaryMap);
                //check for changes
                IAMData.SummaryMap = Auditor.CheckCIBaseline<Dictionary<string, int>>(audit_params, IAMData.SummaryMap.Current, "");


                IAMData.VirtualMFADevices = new ListComparisonResults<Amazon.IdentityManagement.Model.VirtualMFADevice>();
                Amazon.IdentityManagement.Model.ListVirtualMFADevicesResponse lvdResp = IAMClient.ListVirtualMFADevices();
                IAMData.VirtualMFADevices.Current.AddRange(lvdResp.ListVirtualMFADevicesResult.VirtualMFADevices);
                while (lvdResp.ListVirtualMFADevicesResult.IsTruncated)
                {
                    lvdResp = IAMClient.ListVirtualMFADevices(new Amazon.IdentityManagement.Model.ListVirtualMFADevicesRequest()
                        .WithMarker(lvdResp.ListVirtualMFADevicesResult.Marker));
                    IAMData.VirtualMFADevices.Current.AddRange(lvdResp.ListVirtualMFADevicesResult.VirtualMFADevices);
                }
                //check for changes
                IAMData.VirtualMFADevices = Auditor.CheckCIBaseline<Amazon.IdentityManagement.Model.VirtualMFADevice>(audit_params, IAMData.VirtualMFADevices.Current, "");


            }



            return IAMData;
        }
        private  List<Amazon.IdentityManagement.Model.Role> GetRoles(Amazon.IdentityManagement.AmazonIdentityManagementServiceClient IAMClient)
        {
            List<Amazon.IdentityManagement.Model.Role> RolesList = new List<Amazon.IdentityManagement.Model.Role>();
            Amazon.IdentityManagement.Model.ListRolesResponse RoleList = IAMClient.ListRoles();
            RolesList.AddRange(RoleList.ListRolesResult.Roles);
            while (RoleList.ListRolesResult.IsTruncated)
            {
                RoleList = IAMClient.ListRoles(new Amazon.IdentityManagement.Model.ListRolesRequest()
                    .WithMarker(RoleList.ListRolesResult.Marker));
                RolesList.AddRange(RoleList.ListRolesResult.Roles);
            }
            return RolesList;
        }
        private  List<Amazon.IdentityManagement.Model.GetGroupResult> GetGroupData(Amazon.IdentityManagement.AmazonIdentityManagementServiceClient IAMClient)
        {
            return GetGroupData(GetGroups(IAMClient), IAMClient);
        }
        private  List<Amazon.IdentityManagement.Model.GetGroupResult> GetGroupData(
                        List<Amazon.IdentityManagement.Model.Group> Groups,
                        Amazon.IdentityManagement.AmazonIdentityManagementServiceClient IAMClient)
        {
            List<Amazon.IdentityManagement.Model.GetGroupResult> grpdata = new List<Amazon.IdentityManagement.Model.GetGroupResult>();
            foreach (Amazon.IdentityManagement.Model.Group g in Groups)
            {
                Amazon.IdentityManagement.Model.GetGroupResponse GPResp = IAMClient.GetGroup(
                    new Amazon.IdentityManagement.Model.GetGroupRequest().WithGroupName(g.GroupName));
                grpdata.Add(GPResp.GetGroupResult);
                while (GPResp.GetGroupResult.IsTruncated)
                {
                    GPResp = IAMClient.GetGroup(
                            new Amazon.IdentityManagement.Model.GetGroupRequest()
                                        .WithGroupName(g.GroupName)
                                        .WithMarker(GPResp.GetGroupResult.Marker));
                    grpdata.Add(GPResp.GetGroupResult);
                }
            }
            return grpdata;
        }
        private  List<Amazon.IdentityManagement.Model.GetGroupPolicyResult> GetAllGroupPolicyMetadata(
                Amazon.IdentityManagement.AmazonIdentityManagementServiceClient IAMClient)
        {
            return GetAllGroupPolicyMetadata(IAMClient, GetGroups(IAMClient));
        }
        private  List<Amazon.IdentityManagement.Model.GetGroupPolicyResult> GetAllGroupPolicyMetadata(
                Amazon.IdentityManagement.AmazonIdentityManagementServiceClient IAMClient,
                List<Amazon.IdentityManagement.Model.Group> Groups)
        {
            List<Amazon.IdentityManagement.Model.GetGroupPolicyResult> grpPolMeta = new List<Amazon.IdentityManagement.Model.GetGroupPolicyResult>();
            foreach (Amazon.IdentityManagement.Model.Group g in Groups)
            {
                grpPolMeta.AddRange(GetGroupPolicyMetadata(IAMClient, g.GroupName));
            }
            return grpPolMeta;
        }
        private  List<Amazon.IdentityManagement.Model.GetGroupPolicyResult> GetGroupPolicyMetadata(
                                            Amazon.IdentityManagement.AmazonIdentityManagementServiceClient IAMClient,
                                           string groupName)
        {
            List<string> policies = GetGroupPolicyNames(IAMClient, groupName);
            return GetGroupPolicyMetadata(IAMClient, groupName, policies);
        }
        private  List<Amazon.IdentityManagement.Model.GetGroupPolicyResult> GetGroupPolicyMetadata(
                                                   Amazon.IdentityManagement.AmazonIdentityManagementServiceClient IAMClient,
                                                   string groupName, List<string> policies)
        {
            List<Amazon.IdentityManagement.Model.GetGroupPolicyResult> grpPolMeta = new List<Amazon.IdentityManagement.Model.GetGroupPolicyResult>();
            foreach (string policy in policies)
            {
                Amazon.IdentityManagement.Model.GetGroupPolicyResponse GPResp = IAMClient.GetGroupPolicy(
                    new Amazon.IdentityManagement.Model.GetGroupPolicyRequest()
                        .WithGroupName(groupName)
                        .WithPolicyName(policy));
                grpPolMeta.Add(GPResp.GetGroupPolicyResult);
            }
            return grpPolMeta;
        }
        private  List<string> GetGroupPolicyNames(Amazon.IdentityManagement.AmazonIdentityManagementServiceClient IAMClient, string groupName)
        {
            List<string> policies = new List<string>();
            Amazon.IdentityManagement.Model.ListGroupPoliciesResponse gp_resp =
                IAMClient.ListGroupPolicies(
                    new Amazon.IdentityManagement.Model.ListGroupPoliciesRequest().WithGroupName(groupName));
            policies.AddRange(gp_resp.ListGroupPoliciesResult.PolicyNames);
            while (gp_resp.ListGroupPoliciesResult.IsTruncated)
            {
                gp_resp = IAMClient.ListGroupPolicies(
                         new Amazon.IdentityManagement.Model.ListGroupPoliciesRequest()
                                        .WithGroupName(groupName)
                                        .WithMarker(gp_resp.ListGroupPoliciesResult.Marker));
                policies.AddRange(gp_resp.ListGroupPoliciesResult.PolicyNames);
            }
            return policies;
        }
        private  List<Amazon.IdentityManagement.Model.AccessKeyMetadata> GetAccessKeyMetadata(Amazon.IdentityManagement.AmazonIdentityManagementServiceClient IAMClient)
        {
            List<Amazon.IdentityManagement.Model.AccessKeyMetadata> stor = new List<Amazon.IdentityManagement.Model.AccessKeyMetadata>();
            foreach (Amazon.IdentityManagement.Model.User u in GetUsers(IAMClient))
            {
                //Get the AccessKeyMetadata 
                Amazon.IdentityManagement.Model.ListAccessKeysResponse resp = IAMClient.ListAccessKeys(new
                    Amazon.IdentityManagement.Model.ListAccessKeysRequest().WithUserName(u.UserName));
                stor.AddRange(resp.ListAccessKeysResult.AccessKeyMetadata);
                while (resp.ListAccessKeysResult.IsTruncated)
                {
                    resp = IAMClient.ListAccessKeys(new
                    Amazon.IdentityManagement.Model.ListAccessKeysRequest()
                                    .WithUserName(u.UserName)
                                    .WithMarker(resp.ListAccessKeysResult.Marker));
                    stor.AddRange(resp.ListAccessKeysResult.AccessKeyMetadata);
                }
            }
            return stor;
        }
        public List<Amazon.IdentityManagement.Model.User> GetUsers(Amazon.IdentityManagement.AmazonIdentityManagementServiceClient IAMClient)
        {
            List<Amazon.IdentityManagement.Model.User> users = new List<Amazon.IdentityManagement.Model.User>();
            Amazon.IdentityManagement.Model.ListUsersResponse luserresp = IAMClient.ListUsers();
            users.AddRange(luserresp.ListUsersResult.Users);
            while (luserresp.ListUsersResult.IsTruncated)
            {
                luserresp = IAMClient.ListUsers(new Amazon.IdentityManagement.Model.ListUsersRequest()
                                    .WithMarker(luserresp.ListUsersResult.Marker));
                users.AddRange(luserresp.ListUsersResult.Users);
            }
            return users;
        }
        public  List<Amazon.IdentityManagement.Model.Group> GetGroups(Amazon.IdentityManagement.AmazonIdentityManagementServiceClient IAMClient)
        {
            List<Amazon.IdentityManagement.Model.Group> groups = new List<Amazon.IdentityManagement.Model.Group>();
            Amazon.IdentityManagement.Model.ListGroupsResponse lgrpresp = IAMClient.ListGroups();
            groups.AddRange(lgrpresp.ListGroupsResult.Groups);
            while (lgrpresp.ListGroupsResult.IsTruncated)
            {
                lgrpresp = IAMClient.ListGroups(new Amazon.IdentityManagement.Model.ListGroupsRequest()
                                    .WithMarker(lgrpresp.ListGroupsResult.Marker));
                groups.AddRange(lgrpresp.ListGroupsResult.Groups);
            }
            return groups;
        }
        public  List<string> GetUserPolicyNames(Amazon.IdentityManagement.AmazonIdentityManagementServiceClient IAMClient, string sourceuser)
        {
            List<string> SourceUserPols = new List<string>();
            Amazon.IdentityManagement.Model.ListUserPoliciesResponse lstupresp =
                IAMClient.ListUserPolicies(new Amazon.IdentityManagement.Model.ListUserPoliciesRequest()
                        .WithUserName(sourceuser));
            SourceUserPols.AddRange(lstupresp.ListUserPoliciesResult.PolicyNames);
            while (lstupresp.ListUserPoliciesResult.IsTruncated)
            {
                lstupresp = IAMClient.ListUserPolicies(new Amazon.IdentityManagement.Model.ListUserPoliciesRequest()
                                        .WithUserName(sourceuser)
                                        .WithMarker(lstupresp.ListUserPoliciesResult.Marker));
                SourceUserPols.AddRange(lstupresp.ListUserPoliciesResult.PolicyNames);
            }
            return SourceUserPols;
        }

        public  ImportExport ReadImportExport(AuditParams audit_params)
        {
            BaselineAuditor Auditor = new BaselineAuditor();


            ImportExport IEData = new ImportExport();
            StringBuilder sb = new StringBuilder(1024);
            using (StringWriter sr = new StringWriter(sb))
            {
                Amazon.ImportExport.AmazonImportExportClient IEClient = new Amazon.ImportExport.AmazonImportExportClient(audit_params.AWSCredentials, audit_params.AWSRegion);
                IEData.Jobs = new ListComparisonResults<Amazon.ImportExport.Model.Job>();
                try
                {
                    Amazon.ImportExport.Model.ListJobsResponse ljresp = IEClient.ListJobs();
                    //Note:  The AWS access key you provoided does not exists in our records is an error thrown when no jobs exist
                    IEData.Jobs.Current.AddRange(ljresp.ListJobsResult.Jobs);
                    while (ljresp.ListJobsResult.IsTruncated)
                    {
                        ljresp = IEClient.ListJobs(new Amazon.ImportExport.Model.ListJobsRequest()
                                        .WithMarker(ljresp.ListJobsResult.Jobs[ljresp.ListJobsResult.Jobs.Count - 1].JobId));
                        IEData.Jobs.Current.AddRange(ljresp.ListJobsResult.Jobs);
                    }
                }
                catch (Amazon.ImportExport.AmazonImportExportException ex)
                {
                    if (!ex.Message.Equals("The AWS Access Key Id you provided does not exist in our records"))
                        log.Fatal(String.Format("Fatal Error in function {0} ", ReflectionHelper.GetMyFunctionName()), ex);
                    else
                        log.Info(String.Format("Info in function {0} message: {1}", ReflectionHelper.GetMyFunctionName(), ex.Message));
                }
                IEData.Jobs = Auditor.CheckCIBaseline<Amazon.ImportExport.Model.Job>(audit_params, IEData.Jobs.Current, "");


            }
            return IEData;
        }
        public  OpsWorks ReadOpsWorks(AuditParams audit_params)
        {

            BaselineAuditor Auditor = new BaselineAuditor();

            OpsWorks OWData = new OpsWorks();
            StringBuilder sb = new StringBuilder(1024);
            using (StringWriter sr = new StringWriter(sb))
            {
                Amazon.OpsWorks.AmazonOpsWorksClient OWClient = new Amazon.OpsWorks.AmazonOpsWorksClient(audit_params.AWSCredentials, audit_params.AWSRegion);
                try
                {
                    // DescribeStacks()()()() Requests a description of one or more stacks.
                    OWData.Stacks = new ListComparisonResults<Amazon.OpsWorks.Model.Stack>();
                    try
                    {
                        Amazon.OpsWorks.Model.DescribeStacksResponse dsresp = OWClient.DescribeStacks();
                        OWData.Stacks.Current.AddRange(dsresp.DescribeStacksResult.Stacks);
                    }
                    catch (Exception ex)
                    {
                        if (!ex.Message.StartsWith("No endpoint found for service opsworks for region"))
                            log.Fatal(String.Format("Fatal Error in function {0} ", ReflectionHelper.GetMyFunctionName()), ex);
                        else
                            log.Info(String.Format("Info in function {0} message: {1}", ReflectionHelper.GetMyFunctionName(), ex.Message));
                    }
                    //check against baseline
                    OWData.Stacks = Auditor.CheckCIBaseline<Amazon.OpsWorks.Model.Stack>(audit_params, OWData.Stacks.Current, "");

                    //DescribeApps(DescribeAppsRequest) Requests a description of a specified set of apps.
                    OWData.Apps = new ListComparisonResults<Amazon.OpsWorks.Model.App>();
                    foreach (Amazon.OpsWorks.Model.Stack st in OWData.Stacks.Current)
                    {
                        Amazon.OpsWorks.Model.DescribeAppsResponse daresp =
                            OWClient.DescribeApps(new Amazon.OpsWorks.Model.DescribeAppsRequest().WithStackId(st.StackId));
                        OWData.Apps.Current.AddRange(daresp.DescribeAppsResult.Apps);
                    }
                    //check against baseline
                    OWData.Apps = Auditor.CheckCIBaseline<Amazon.OpsWorks.Model.App>(audit_params, OWData.Apps.Current, "");

                    // DescribeDeployments(DescribeDeploymentsRequest) Requests a description of a specified set of deployments.
                    OWData.Deployments = new ListComparisonResults<Amazon.OpsWorks.Model.Deployment>();
                    foreach (Amazon.OpsWorks.Model.Stack st in OWData.Stacks.Current)
                    {
                        Amazon.OpsWorks.Model.DescribeDeploymentsResponse ddresp =
                            OWClient.DescribeDeployments(new Amazon.OpsWorks.Model.DescribeDeploymentsRequest().WithStackId(st.StackId));
                        OWData.Deployments.Current.AddRange(ddresp.DescribeDeploymentsResult.Deployments);
                        //check against baseline
                    }
                    OWData.Deployments = Auditor.CheckCIBaseline<Amazon.OpsWorks.Model.Deployment>(audit_params, OWData.Deployments.Current, "");

                    // DescribeCommands(DescribeCommandsRequest) Describes the results of specified commands.
                    OWData.Commands = new ListComparisonResults<Amazon.OpsWorks.Model.Command>();
                    foreach (Amazon.OpsWorks.Model.Deployment dp in OWData.Deployments.Current)
                    {
                        Amazon.OpsWorks.Model.DescribeCommandsResponse dcresp =
                            OWClient.DescribeCommands(new Amazon.OpsWorks.Model.DescribeCommandsRequest().WithDeploymentId(dp.DeploymentId));
                        OWData.Commands.Current.AddRange(dcresp.DescribeCommandsResult.Commands);
                    }
                    //check against baseline
                    OWData.Commands = Auditor.CheckCIBaseline<Amazon.OpsWorks.Model.Command>(audit_params, OWData.Commands.Current, "");


                    // DescribeInstances(DescribeInstancesRequest) Requests a description of a set of instances associated with a specified ID or IDs.
                    OWData.Instances = new ListComparisonResults<Amazon.OpsWorks.Model.Instance>();
                    foreach (Amazon.OpsWorks.Model.Stack st in OWData.Stacks.Current)
                    {
                        Amazon.OpsWorks.Model.DescribeInstancesResponse diresp =
                            OWClient.DescribeInstances(new Amazon.OpsWorks.Model.DescribeInstancesRequest()
                                        .WithStackId(st.StackId));
                        OWData.Instances.Current.AddRange(diresp.DescribeInstancesResult.Instances);
                    }
                    //check against baseline
                    OWData.Instances = Auditor.CheckCIBaseline<Amazon.OpsWorks.Model.Instance>(audit_params, OWData.Instances.Current, "");


                    // DescribeElasticIps(DescribeElasticIpsRequest) Describes an instance's Elastic IP addresses.
                    OWData.ElasticIps = new ListComparisonResults<Amazon.OpsWorks.Model.ElasticIp>();
                    foreach (Amazon.OpsWorks.Model.Instance inst in OWData.Instances.Current)
                    {
                        Amazon.OpsWorks.Model.DescribeElasticIpsResponse deipresp =
                            OWClient.DescribeElasticIps(new Amazon.OpsWorks.Model.DescribeElasticIpsRequest()
                                                        .WithInstanceId(inst.InstanceId));
                        OWData.ElasticIps.Current.AddRange(deipresp.DescribeElasticIpsResult.ElasticIps);
                    }
                    //check against baseline
                    OWData.ElasticIps = Auditor.CheckCIBaseline<Amazon.OpsWorks.Model.ElasticIp>(audit_params, OWData.ElasticIps.Current, "");



                    // DescribeLayers(DescribeLayersRequest) Requests a description of one or more layers in a specified stack.
                    OWData.Layers = new ListComparisonResults<Amazon.OpsWorks.Model.Layer>();
                    foreach (Amazon.OpsWorks.Model.Stack st in OWData.Stacks.Current)
                    {
                        Amazon.OpsWorks.Model.DescribeLayersResponse dlresp =
                            OWClient.DescribeLayers(new Amazon.OpsWorks.Model.DescribeLayersRequest()
                                            .WithStackId(st.StackId));
                        OWData.Layers.Current.AddRange(dlresp.DescribeLayersResult.Layers);
                    }
                    //check against baseline
                    OWData.Layers = Auditor.CheckCIBaseline<Amazon.OpsWorks.Model.Layer>(audit_params, OWData.Layers.Current, "");


                    // DescribeLoadBasedAutoScaling(DescribeLoadBasedAutoScalingRequest) Describes load-based auto scaling configurations for specified layers.
                    OWData.LoadBasedAutoScalingConfigurations = new ListComparisonResults<Amazon.OpsWorks.Model.LoadBasedAutoScalingConfiguration>();
                    foreach (Amazon.OpsWorks.Model.Layer ly in OWData.Layers.Current)
                    {
                        Amazon.OpsWorks.Model.DescribeLoadBasedAutoScalingResponse dlbasresp =
                            OWClient.DescribeLoadBasedAutoScaling(new Amazon.OpsWorks.Model.DescribeLoadBasedAutoScalingRequest()
                                               .WithLayerIds(ly.LayerId));
                        OWData.LoadBasedAutoScalingConfigurations.Current.AddRange(dlbasresp.DescribeLoadBasedAutoScalingResult.LoadBasedAutoScalingConfigurations);
                    }
                    //check against baseline
                    OWData.LoadBasedAutoScalingConfigurations = Auditor.CheckCIBaseline<Amazon.OpsWorks.Model.LoadBasedAutoScalingConfiguration>(audit_params, OWData.LoadBasedAutoScalingConfigurations.Current, "");


                    // DescribePermissions(DescribePermissionsRequest) Describes the permissions for a specified stack. You must specify at least one of the two request values.
                    OWData.Permissions = new ListComparisonResults<Amazon.OpsWorks.Model.Permission>();
                    foreach (Amazon.OpsWorks.Model.Stack st in OWData.Stacks.Current)
                    {
                        Amazon.OpsWorks.Model.DescribePermissionsResponse dpresp =
                            OWClient.DescribePermissions(new Amazon.OpsWorks.Model.DescribePermissionsRequest()
                            .WithStackId(st.StackId));
                        OWData.Permissions.Current.AddRange(dpresp.DescribePermissionsResult.Permissions);
                    }
                    //check against baseline
                    OWData.Permissions = Auditor.CheckCIBaseline<Amazon.OpsWorks.Model.Permission>(audit_params, OWData.Permissions.Current, "");


                    // DescribeRaidArrays(DescribeRaidArraysRequest) Describe an instance's RAID arrays.
                    OWData.RaidArrays = new ListComparisonResults<Amazon.OpsWorks.Model.RaidArray>();
                    foreach (Amazon.OpsWorks.Model.Instance ist in OWData.Instances.Current)
                    {
                        Amazon.OpsWorks.Model.DescribeRaidArraysResponse draresp =
                            OWClient.DescribeRaidArrays(new Amazon.OpsWorks.Model.DescribeRaidArraysRequest()
                                                .WithInstanceId(ist.InstanceId));
                        OWData.RaidArrays.Current.AddRange(draresp.DescribeRaidArraysResult.RaidArrays);
                    }
                    //check against baseline
                    OWData.RaidArrays = Auditor.CheckCIBaseline<Amazon.OpsWorks.Model.RaidArray>(audit_params, OWData.RaidArrays.Current, "");

                    // DescribeServiceErrors()()()() Describes OpsWorks service errors.
                    OWData.ServiceErrors = new ListComparisonResults<Amazon.OpsWorks.Model.ServiceError>();
                    try
                    {
                        Amazon.OpsWorks.Model.DescribeServiceErrorsResponse dseresp = OWClient.DescribeServiceErrors();
                        OWData.ServiceErrors.Current.AddRange(dseresp.DescribeServiceErrorsResult.ServiceErrors);
                    }
                    catch (Exception ex)
                    {
                        if (!ex.Message.StartsWith("No endpoint found for service opsworks for region "))
                            log.Fatal(String.Format("Fatal Error in function {0} ", ReflectionHelper.GetMyFunctionName()), ex);
                        else
                            log.Info(String.Format("Info in function {0} message: {1}", ReflectionHelper.GetMyFunctionName(), ex.Message));
                    }
                    //check against baseline
                    OWData.ServiceErrors = Auditor.CheckCIBaseline<Amazon.OpsWorks.Model.ServiceError>(audit_params, OWData.ServiceErrors.Current, "");




                    // DescribeTimeBasedAutoScaling(DescribeTimeBasedAutoScalingRequest) Describes time-based auto scaling configurations for specified instances.
                    OWData.TimeBasedAutoScalingConfigurations = new ListComparisonResults<Amazon.OpsWorks.Model.TimeBasedAutoScalingConfiguration>();
                    foreach (Amazon.OpsWorks.Model.Instance ist in OWData.Instances.Current)
                    {
                        Amazon.OpsWorks.Model.DescribeTimeBasedAutoScalingResponse dtbasresp =
                            OWClient.DescribeTimeBasedAutoScaling(new Amazon.OpsWorks.Model.DescribeTimeBasedAutoScalingRequest()
                                            .WithInstanceIds(ist.InstanceId));
                        OWData.TimeBasedAutoScalingConfigurations.Current.AddRange(dtbasresp.DescribeTimeBasedAutoScalingResult.TimeBasedAutoScalingConfigurations);
                    }
                    //check against baseline
                    OWData.TimeBasedAutoScalingConfigurations = Auditor.CheckCIBaseline<Amazon.OpsWorks.Model.TimeBasedAutoScalingConfiguration>(audit_params, OWData.TimeBasedAutoScalingConfigurations.Current, "");


                    // DescribeUserProfiles(DescribeUserProfilesRequest) Describe specified users.
                    OWData.UserProfiles = new ListComparisonResults<Amazon.OpsWorks.Model.UserProfile>();
                    try
                    {
                        Amazon.OpsWorks.Model.DescribeUserProfilesResponse duprersp =
                            OWClient.DescribeUserProfiles(new Amazon.OpsWorks.Model.DescribeUserProfilesRequest());
                        OWData.UserProfiles.Current.AddRange(duprersp.DescribeUserProfilesResult.UserProfiles);
                    }
                    catch (Exception ex)
                    {
                        if (!ex.Message.StartsWith("No endpoint found for service opsworks for region"))
                            log.Fatal(String.Format("Fatal Error in function {0} ", ReflectionHelper.GetMyFunctionName()), ex);
                        else
                            log.Info(String.Format("Info in function {0} message: {1}", ReflectionHelper.GetMyFunctionName(), ex.Message));
                    }
                    //check against baseline
                    OWData.UserProfiles = Auditor.CheckCIBaseline<Amazon.OpsWorks.Model.UserProfile>(audit_params, OWData.UserProfiles.Current, "");

                    // DescribeVolumes(DescribeVolumesRequest) Describes an instance's Amazon EBS volumes.
                    OWData.Volumes = new ListComparisonResults<Amazon.OpsWorks.Model.Volume>();
                    foreach (Amazon.OpsWorks.Model.Instance ist in OWData.Instances.Current)
                    {
                        Amazon.OpsWorks.Model.DescribeVolumesResponse dvresp =
                            OWClient.DescribeVolumes(new Amazon.OpsWorks.Model.DescribeVolumesRequest()
                                .WithInstanceId(ist.InstanceId));
                        OWData.Volumes.Current.AddRange(dvresp.DescribeVolumesResult.Volumes);
                    }
                    //check against baseline
                    OWData.Volumes = Auditor.CheckCIBaseline<Amazon.OpsWorks.Model.Volume>(audit_params, OWData.Volumes.Current, "");
                }
                catch (Amazon.OpsWorks.AmazonOpsWorksException ex)
                {
                    log.Fatal(String.Format("Fatal Error in function {0} ", ReflectionHelper.GetMyFunctionName()), ex);
                }

            }

            return OWData;
        }
        /// <summary>
        /// IsTruncated Review completed
        /// </summary>
        /// <returns></returns>
        public  Route53 ReadRoute53(AuditParams audit_params)
        {

            BaselineAuditor Auditor = new BaselineAuditor();



            StringBuilder sb = new StringBuilder(1024);
            Route53 R53Data = new Route53();
            using (StringWriter sr = new StringWriter(sb))
            {
                Amazon.Route53.AmazonRoute53Client R53Client = new Amazon.Route53.AmazonRoute53Client(audit_params.AWSCredentials, audit_params.AWSRegion);
                //Note this retrieves only 100 zones per call

                R53Data.HostedZones = new ListComparisonResults<Amazon.Route53.Model.HostedZone>();
                Amazon.Route53.Model.ListHostedZonesResponse hzresp = R53Client.ListHostedZones(new Amazon.Route53.Model.ListHostedZonesRequest());
                R53Data.HostedZones.Current.AddRange(hzresp.ListHostedZonesResult.HostedZones);
                while (hzresp.ListHostedZonesResult.IsTruncated)
                {
                    hzresp = R53Client.ListHostedZones(
                        new Amazon.Route53.Model.ListHostedZonesRequest().WithMarker(hzresp.ListHostedZonesResult.NextMarker));
                    R53Data.HostedZones.Current.AddRange(hzresp.ListHostedZonesResult.HostedZones);
                }
                //check for changes
                R53Data.HostedZones = Auditor.CheckCIBaseline<Amazon.Route53.Model.HostedZone>(audit_params, R53Data.HostedZones.Current, "");


                //ListResourceRecordSets
                R53Data.ResourceRecordSets = new ListComparisonResults<Amazon.Route53.Model.ResourceRecordSet>();
                //check for changes
                foreach (Amazon.Route53.Model.HostedZone hzone in R53Data.HostedZones.Current)
                {
                    Amazon.Route53.Model.ListResourceRecordSetsResponse lresp =
                        R53Client.ListResourceRecordSets(new Amazon.Route53.Model.ListResourceRecordSetsRequest().WithHostedZoneId(hzone.Id));
                    R53Data.ResourceRecordSets.Current.AddRange(lresp.ListResourceRecordSetsResult.ResourceRecordSets);
                    while (lresp.ListResourceRecordSetsResult.IsTruncated)
                    {
                        lresp = R53Client.ListResourceRecordSets(
                            new Amazon.Route53.Model.ListResourceRecordSetsRequest().WithHostedZoneId(hzone.Id)
                                                    .WithStartRecordName(lresp.ListResourceRecordSetsResult.NextRecordName)
                                                    .WithStartRecordType(lresp.ListResourceRecordSetsResult.NextRecordType));
                        R53Data.ResourceRecordSets.Current.AddRange(lresp.ListResourceRecordSetsResult.ResourceRecordSets);
                    }
                }
                R53Data.ResourceRecordSets = Auditor.CheckCIBaseline<Amazon.Route53.Model.ResourceRecordSet>(audit_params, R53Data.ResourceRecordSets.Current, "");




            }


            return R53Data;
        }

        public  RelationalDatabaseSystem ReadRDS(AuditParams audit_params)
        {
            BaselineAuditor Auditor = new BaselineAuditor();


            RelationalDatabaseSystem RDSData = new RelationalDatabaseSystem();
            StringBuilder sb = new StringBuilder(1024);
            using (StringWriter sr = new StringWriter(sb))
            {
                Amazon.RDS.AmazonRDSClient RDSClient = new Amazon.RDS.AmazonRDSClient(audit_params.AWSCredentials, audit_params.AWSRegion);

                RDSData.DBEngineVersions = new ListComparisonResults<Amazon.RDS.Model.DBEngineVersion>();
                //DescribeDBEngineVersions()()()() Returns a list of the available DB engines. 
                Amazon.RDS.Model.DescribeDBEngineVersionsResponse ddbevresp = RDSClient.DescribeDBEngineVersions();
                RDSData.DBEngineVersions.Current.AddRange(ddbevresp.DescribeDBEngineVersionsResult.DBEngineVersions);
                while (ddbevresp.DescribeDBEngineVersionsResult.Marker != null)
                {
                    ddbevresp = RDSClient.DescribeDBEngineVersions(new Amazon.RDS.Model.DescribeDBEngineVersionsRequest()
                            .WithMarker(ddbevresp.DescribeDBEngineVersionsResult.Marker));
                    RDSData.DBEngineVersions.Current.AddRange(ddbevresp.DescribeDBEngineVersionsResult.DBEngineVersions);
                }
                RDSData.DBEngineVersions = Auditor.CheckCIBaseline<Amazon.RDS.Model.DBEngineVersion>(audit_params, RDSData.DBEngineVersions.Current, "");


                // DescribeDBInstances()()()() Returns information about provisioned RDS instances. This API supports pagination. 
                RDSData.DBInstances = new ListComparisonResults<Amazon.RDS.Model.DBInstance>();
                Amazon.RDS.Model.DescribeDBInstancesResponse ddbiresp = RDSClient.DescribeDBInstances();
                RDSData.DBInstances.Current.AddRange(ddbiresp.DescribeDBInstancesResult.DBInstances);
                while (ddbiresp.DescribeDBInstancesResult.Marker != null)
                {
                    ddbiresp = RDSClient.DescribeDBInstances(new Amazon.RDS.Model.DescribeDBInstancesRequest().WithMarker(ddbiresp.DescribeDBInstancesResult.Marker));
                    RDSData.DBInstances.Current.AddRange(ddbiresp.DescribeDBInstancesResult.DBInstances);
                }
                RDSData.DBInstances = Auditor.CheckCIBaseline<Amazon.RDS.Model.DBInstance>(audit_params, RDSData.DBInstances.Current, "");

                // DescribeDBParameterGroups()()()() Returns a list of DBParameterGroup descriptions. If a DBParameterGroupName is specified, the list will contain only the description of the specified DBParameterGroup. 
                List<Amazon.RDS.Model.DBParameterGroup> DBParamGroups = new List<Amazon.RDS.Model.DBParameterGroup>();
                Amazon.RDS.Model.DescribeDBParameterGroupsResponse ddbpgresp = RDSClient.DescribeDBParameterGroups();
                DBParamGroups.AddRange(ddbpgresp.DescribeDBParameterGroupsResult.DBParameterGroups);
                while (ddbpgresp.DescribeDBParameterGroupsResult.Marker != null)
                {
                    ddbpgresp = RDSClient.DescribeDBParameterGroups(new Amazon.RDS.Model.DescribeDBParameterGroupsRequest().WithMarker(ddbpgresp.DescribeDBParameterGroupsResult.Marker));
                    DBParamGroups.AddRange(ddbpgresp.DescribeDBParameterGroupsResult.DBParameterGroups);
                }
                //merge content into DBGroupParameters
                // DescribeDBParameters(DescribeDBParametersRequest) Returns the detailed parameter list for a particular DBParameterGroup. 
                RDSData.DBGroupParameters = new ListComparisonResults<DBGroupParameters>();
                foreach (Amazon.RDS.Model.DBParameterGroup pg in DBParamGroups)
                {
                    Amazon.RDS.Model.DescribeDBParametersResponse ddbpresp = RDSClient.DescribeDBParameters(
                                        new Amazon.RDS.Model.DescribeDBParametersRequest().WithDBParameterGroupName(pg.DBParameterGroupName));
                    DBGroupParameters dbsgptemp = new DBGroupParameters(pg, ddbpresp.DescribeDBParametersResult.Parameters);
                    while (ddbpgresp.DescribeDBParameterGroupsResult.Marker != null)
                    {
                        RDSClient.DescribeDBParameters(new Amazon.RDS.Model.DescribeDBParametersRequest()
                                       .WithDBParameterGroupName(pg.DBParameterGroupName)
                                       .WithMarker(ddbpgresp.DescribeDBParameterGroupsResult.Marker));
                        dbsgptemp.Parameters.AddRange(ddbpresp.DescribeDBParametersResult.Parameters);
                    }
                    RDSData.DBGroupParameters.Current.Add(dbsgptemp);
                }
                RDSData.DBGroupParameters = Auditor.CheckCIBaseline<DBGroupParameters>(audit_params, RDSData.DBGroupParameters.Current, "");

                // DescribeDBSecurityGroups()()()() Returns a list of DBSecurityGroup descriptions. If a DBSecurityGroupName is specified, the list will contain only the descriptions of the specified DBSecurityGroup. 
                RDSData.DBSecurityGroups = new ListComparisonResults<Amazon.RDS.Model.DBSecurityGroup>();
                Amazon.RDS.Model.DescribeDBSecurityGroupsResponse ddbsgresp =
                    RDSClient.DescribeDBSecurityGroups();
                RDSData.DBSecurityGroups.Current.AddRange(ddbsgresp.DescribeDBSecurityGroupsResult.DBSecurityGroups);
                while (ddbsgresp.DescribeDBSecurityGroupsResult.Marker != null)
                {
                    ddbsgresp = RDSClient.DescribeDBSecurityGroups(new Amazon.RDS.Model.DescribeDBSecurityGroupsRequest()
                        .WithMarker(ddbsgresp.DescribeDBSecurityGroupsResult.Marker));
                    RDSData.DBSecurityGroups.Current.AddRange(ddbsgresp.DescribeDBSecurityGroupsResult.DBSecurityGroups);
                }
                RDSData.DBSecurityGroups = Auditor.CheckCIBaseline<Amazon.RDS.Model.DBSecurityGroup>(audit_params, RDSData.DBSecurityGroups.Current, "");


                // DescribeDBSnapshots()()()() Returns information about DBSnapshots. This API supports pagination. 
                RDSData.DBSnapshots = new ListComparisonResults<Amazon.RDS.Model.DBSnapshot>();
                Amazon.RDS.Model.DescribeDBSnapshotsResponse ddbsnshresp = RDSClient.DescribeDBSnapshots();
                RDSData.DBSnapshots.Current.AddRange(ddbsnshresp.DescribeDBSnapshotsResult.DBSnapshots);
                while (ddbsnshresp.DescribeDBSnapshotsResult.Marker != null)
                {
                    ddbsnshresp = RDSClient.DescribeDBSnapshots(new Amazon.RDS.Model.DescribeDBSnapshotsRequest()
                        .WithMarker(ddbsnshresp.DescribeDBSnapshotsResult.Marker));
                    RDSData.DBSnapshots.Current.AddRange(ddbsnshresp.DescribeDBSnapshotsResult.DBSnapshots);
                }
                RDSData.DBSnapshots = Auditor.CheckCIBaseline<Amazon.RDS.Model.DBSnapshot>(audit_params, RDSData.DBSnapshots.Current, "");

                // DescribeDBSubnetGroups()()()() Returns a list of DBSubnetGroup descriptions. If a DBSubnetGroupName is specified, the list will contain only the descriptions of the specified DBSubnetGroup. 
                RDSData.DBSubnetGroups = new ListComparisonResults<Amazon.RDS.Model.DBSubnetGroup>();
                Amazon.RDS.Model.DescribeDBSubnetGroupsResponse dsugresp =
                                                RDSClient.DescribeDBSubnetGroups();
                RDSData.DBSubnetGroups.Current.AddRange(dsugresp.DescribeDBSubnetGroupsResult.DBSubnetGroups);
                while (dsugresp.DescribeDBSubnetGroupsResult.Marker != null)
                {
                    dsugresp = RDSClient.DescribeDBSubnetGroups(new Amazon.RDS.Model.DescribeDBSubnetGroupsRequest()
                        .WithMarker(dsugresp.DescribeDBSubnetGroupsResult.Marker));
                    RDSData.DBSubnetGroups.Current.AddRange(dsugresp.DescribeDBSubnetGroupsResult.DBSubnetGroups);
                }
                RDSData.DBSubnetGroups = Auditor.CheckCIBaseline<Amazon.RDS.Model.DBSubnetGroup>(audit_params, RDSData.DBSubnetGroups.Current, "");

                // DescribeEngineDefaultParameters(DescribeEngineDefaultParametersRequest) Returns the default engine and system parameter information for the specified database engine. 
                RDSData.DefaultDBGroupParameters = new ListComparisonResults<DefaultDBGroupParameters>();
                foreach (DBGroupParameters dbg in RDSData.DBGroupParameters.Current)
                {
                    Amazon.RDS.Model.DescribeEngineDefaultParametersResponse ddbedpresp =
                        RDSClient.DescribeEngineDefaultParameters(new Amazon.RDS.Model.DescribeEngineDefaultParametersRequest()
                                    .WithDBParameterGroupFamily(dbg.group.DBParameterGroupFamily));
                    DefaultDBGroupParameters dDBGp = new DefaultDBGroupParameters(
                                                                    ddbedpresp.DescribeEngineDefaultParametersResult.EngineDefaults.DBParameterGroupFamily,
                                                                    ddbedpresp.DescribeEngineDefaultParametersResult.EngineDefaults.Parameters);
                    while (ddbedpresp.DescribeEngineDefaultParametersResult.EngineDefaults.Marker != null)
                    {
                        ddbedpresp = RDSClient.DescribeEngineDefaultParameters(new Amazon.RDS.Model.DescribeEngineDefaultParametersRequest()
                                                            .WithDBParameterGroupFamily(dbg.group.DBParameterGroupFamily)
                                                            .WithMarker(ddbedpresp.DescribeEngineDefaultParametersResult.EngineDefaults.Marker));
                        dDBGp.Parameters.AddRange(ddbedpresp.DescribeEngineDefaultParametersResult.EngineDefaults.Parameters);
                    }
                    RDSData.DefaultDBGroupParameters.Current.Add(dDBGp);
                }
                RDSData.DefaultDBGroupParameters = Auditor.CheckCIBaseline<DefaultDBGroupParameters>(audit_params, RDSData.DefaultDBGroupParameters.Current, "");

                // DescribeEventCategories()()()() Displays a list of categories for all event source types, or, if specified, for a specified source type. You can see a list of the event categories and source types in the Events topic in the Amazon RDS User Guide.
                RDSData.EventCategoriesMaps = new ListComparisonResults<Amazon.RDS.Model.EventCategoriesMap>();
                Amazon.RDS.Model.DescribeEventCategoriesResponse decresp = RDSClient.DescribeEventCategories();
                RDSData.EventCategoriesMaps.Current.AddRange(decresp.DescribeEventCategoriesResult.EventCategoriesMapList);
                RDSData.EventCategoriesMaps = Auditor.CheckCIBaseline<Amazon.RDS.Model.EventCategoriesMap>(audit_params, RDSData.EventCategoriesMaps.Current, "");

                // DescribeEvents()()()() Returns events related to DB instances, DB security groups, DB Snapshots, and DB parameter groups for the past 14 days. Events specific to a particular DB Iinstance, DB security group, DB Snapshot, or DB parameter group can be obtained by providing the source identifier as a parameter. By default, the past hour of events are returned. 
                RDSData.Events = new ListComparisonResults<Amazon.RDS.Model.Event>();
                Amazon.RDS.Model.DescribeEventsResponse deresp = RDSClient.DescribeEvents();
                RDSData.Events.Current.AddRange(deresp.DescribeEventsResult.Events);
                while (deresp.DescribeEventsResult.Marker != null)
                {
                    deresp = RDSClient.DescribeEvents(new Amazon.RDS.Model.DescribeEventsRequest().WithMarker(deresp.DescribeEventsResult.Marker));
                    RDSData.Events.Current.AddRange(deresp.DescribeEventsResult.Events);
                }
                RDSData.Events = Auditor.CheckCIBaseline<Amazon.RDS.Model.Event>(audit_params, RDSData.Events.Current, "");

                // DescribeEventSubscriptions()()()() Lists all the subscription descriptions for a customer account. The description for a subscription includes SubscriptionName, SNSTopicARN, CustomerID, SourceType, SourceID, CreationTime, and Status. 
                RDSData.EventSubscriptions = new ListComparisonResults<Amazon.RDS.Model.EventSubscription>();
                Amazon.RDS.Model.DescribeEventSubscriptionsResponse desresp =
                    RDSClient.DescribeEventSubscriptions();
                RDSData.EventSubscriptions.Current.AddRange(desresp.DescribeEventSubscriptionsResult.EventSubscriptionsList);
                while (desresp.DescribeEventSubscriptionsResult.Marker != null)
                {
                    desresp = RDSClient.DescribeEventSubscriptions(new Amazon.RDS.Model.DescribeEventSubscriptionsRequest()
                        .WithMarker(desresp.DescribeEventSubscriptionsResult.Marker)); ;
                    RDSData.EventSubscriptions.Current.AddRange(desresp.DescribeEventSubscriptionsResult.EventSubscriptionsList);
                }
                RDSData.EventSubscriptions = Auditor.CheckCIBaseline<Amazon.RDS.Model.EventSubscription>(audit_params, RDSData.EventSubscriptions.Current, "");

                // DescribeOptionGroups()()()() Describes the available option groups. 
                RDSData.OptionGroups = new ListComparisonResults<Amazon.RDS.Model.OptionGroup>();
                Amazon.RDS.Model.DescribeOptionGroupsResponse dogresp =
                    RDSClient.DescribeOptionGroups();
                RDSData.OptionGroups.Current.AddRange(dogresp.DescribeOptionGroupsResult.OptionGroupsList);
                while (dogresp.DescribeOptionGroupsResult.Marker != null)
                {
                    dogresp = RDSClient.DescribeOptionGroups(new Amazon.RDS.Model.DescribeOptionGroupsRequest()
                                        .WithMarker(dogresp.DescribeOptionGroupsResult.Marker));
                    RDSData.OptionGroups.Current.AddRange(dogresp.DescribeOptionGroupsResult.OptionGroupsList);
                }
                RDSData.OptionGroups = Auditor.CheckCIBaseline<Amazon.RDS.Model.OptionGroup>(audit_params, RDSData.OptionGroups.Current, "");


                // DescribeOptionGroupOptions(DescribeOptionGroupOptionsRequest) Describes all available options. 
                RDSData.OptionGroupOptions = new ListComparisonResults<Amazon.RDS.Model.OptionGroupOption>();
                foreach (Amazon.RDS.Model.OptionGroup og in RDSData.OptionGroups.Current)
                {
                    Amazon.RDS.Model.DescribeOptionGroupOptionsResponse dogoresp =
                        RDSClient.DescribeOptionGroupOptions(new Amazon.RDS.Model.DescribeOptionGroupOptionsRequest()
                        .WithEngineName(og.EngineName));
                    RDSData.OptionGroupOptions.Current.AddRange(dogoresp.DescribeOptionGroupOptionsResult.OptionGroupOptions);
                    while (dogoresp.DescribeOptionGroupOptionsResult.Marker != null)
                    {
                        dogoresp = RDSClient.DescribeOptionGroupOptions(new Amazon.RDS.Model.DescribeOptionGroupOptionsRequest()
                                            .WithEngineName(og.EngineName)
                                            .WithMarker(dogoresp.DescribeOptionGroupOptionsResult.Marker));
                        RDSData.OptionGroupOptions.Current.AddRange(dogoresp.DescribeOptionGroupOptionsResult.OptionGroupOptions);
                    }
                }
                RDSData.OptionGroupOptions = Auditor.CheckCIBaseline<Amazon.RDS.Model.OptionGroupOption>(audit_params, RDSData.OptionGroupOptions.Current, "");


                // DescribeOrderableDBInstanceOptions(DescribeOrderableDBInstanceOptionsRequest) Returns a list of orderable DB Instance options for the specified engine. 
                RDSData.OrderableDBInstanceOptions = new ListComparisonResults<Amazon.RDS.Model.OrderableDBInstanceOption>();
                foreach (Amazon.RDS.Model.OptionGroup og in RDSData.OptionGroups.Current)
                {
                    Amazon.RDS.Model.DescribeOrderableDBInstanceOptionsResponse dodbioresp =
                        RDSClient.DescribeOrderableDBInstanceOptions(new Amazon.RDS.Model.DescribeOrderableDBInstanceOptionsRequest()
                                                .WithEngine(og.EngineName));
                    RDSData.OrderableDBInstanceOptions.Current.AddRange(dodbioresp.DescribeOrderableDBInstanceOptionsResult.OrderableDBInstanceOptions);
                    while (dodbioresp.DescribeOrderableDBInstanceOptionsResult.Marker != null)
                    {
                        dodbioresp = RDSClient.DescribeOrderableDBInstanceOptions(new Amazon.RDS.Model.DescribeOrderableDBInstanceOptionsRequest()
                                .WithEngine(og.EngineName)
                                .WithMarker(dodbioresp.DescribeOrderableDBInstanceOptionsResult.Marker));
                        RDSData.OrderableDBInstanceOptions.Current.AddRange(dodbioresp.DescribeOrderableDBInstanceOptionsResult.OrderableDBInstanceOptions);
                    }
                }
                RDSData.OrderableDBInstanceOptions = Auditor.CheckCIBaseline<Amazon.RDS.Model.OrderableDBInstanceOption>(audit_params, RDSData.OrderableDBInstanceOptions.Current, "");

                // DescribeReservedDBInstances()()()() Returns information about reserved DB Instances for this account, or about a specified reserved DB Instance. 
                RDSData.ReservedDBInstances = new ListComparisonResults<Amazon.RDS.Model.ReservedDBInstance>();
                Amazon.RDS.Model.DescribeReservedDBInstancesResponse drifresp =
                    RDSClient.DescribeReservedDBInstances();
                RDSData.ReservedDBInstances.Current.AddRange(drifresp.DescribeReservedDBInstancesResult.ReservedDBInstances);
                while (drifresp.DescribeReservedDBInstancesResult.Marker != null)
                {
                    drifresp = RDSClient.DescribeReservedDBInstances(new Amazon.RDS.Model.DescribeReservedDBInstancesRequest()
                        .WithMarker(drifresp.DescribeReservedDBInstancesResult.Marker));
                    RDSData.ReservedDBInstances.Current.AddRange(drifresp.DescribeReservedDBInstancesResult.ReservedDBInstances);
                }
                RDSData.ReservedDBInstances = Auditor.CheckCIBaseline<Amazon.RDS.Model.ReservedDBInstance>(audit_params, RDSData.ReservedDBInstances.Current, "");

                // DescribeReservedDBInstancesOfferings()()()() Lists available reserved DB Instance offerings. 
                RDSData.ReservedDBInstancesOfferings = new ListComparisonResults<Amazon.RDS.Model.ReservedDBInstancesOffering>();
                Amazon.RDS.Model.DescribeReservedDBInstancesOfferingsResponse drioresp =
                    RDSClient.DescribeReservedDBInstancesOfferings();
                RDSData.ReservedDBInstancesOfferings.Current.AddRange(drioresp.DescribeReservedDBInstancesOfferingsResult.ReservedDBInstancesOfferings);
                while (drioresp.DescribeReservedDBInstancesOfferingsResult.Marker != null)
                {
                    drioresp = RDSClient.DescribeReservedDBInstancesOfferings(new Amazon.RDS.Model.DescribeReservedDBInstancesOfferingsRequest()
                        .WithMarker(drioresp.DescribeReservedDBInstancesOfferingsResult.Marker));
                    RDSData.ReservedDBInstancesOfferings.Current.AddRange(drioresp.DescribeReservedDBInstancesOfferingsResult.ReservedDBInstancesOfferings);
                }
                RDSData.ReservedDBInstancesOfferings = Auditor.CheckCIBaseline<Amazon.RDS.Model.ReservedDBInstancesOffering>(audit_params, RDSData.ReservedDBInstancesOfferings.Current, "");


            }


            return RDSData;

        }
        public  Redshift ReadRedshift(AuditParams audit_params)
        {

            BaselineAuditor Auditor = new BaselineAuditor();

            Redshift RSData = new Redshift();
            StringBuilder sb = new StringBuilder(1024);
            using (StringWriter sr = new StringWriter(sb))
            {
                Amazon.Redshift.AmazonRedshiftClient RSClient = new Amazon.Redshift.AmazonRedshiftClient(audit_params.AWSCredentials, audit_params.AWSRegion);



                // DescribeClusterParameterGroups()()()() Returns a list of Amazon Redshift parameter groups, including parameter groups you created and the default parameter group. For each parameter group, the response includes the parameter group name, description, and parameter group family name. You can optionally specify a name to retrieve the description of a specific parameter group. 
                //For more information about managing parameter groups, go to Amazon Redshift Parameter Groups in the Amazon Redshift Management Guide . 

                RSData.ClusterParameterGroups = new ListComparisonResults<Amazon.Redshift.Model.ClusterParameterGroup>();
                try
                {
                    Amazon.Redshift.Model.DescribeClusterParameterGroupsResponse dcpgresp = RSClient.DescribeClusterParameterGroups();
                    RSData.ClusterParameterGroups.Current.AddRange(dcpgresp.DescribeClusterParameterGroupsResult.ParameterGroups);
                    while (dcpgresp.DescribeClusterParameterGroupsResult.Marker != null)
                    {
                        dcpgresp = RSClient.DescribeClusterParameterGroups(new Amazon.Redshift.Model.DescribeClusterParameterGroupsRequest()
                                        .WithMarker(dcpgresp.DescribeClusterParameterGroupsResult.Marker));
                        RSData.ClusterParameterGroups.Current.AddRange(dcpgresp.DescribeClusterParameterGroupsResult.ParameterGroups);
                    }
                }
                catch (Exception ex)
                {
                    if (!ex.Message.StartsWith("No endpoint found for service redshift for region"))
                        log.Fatal(String.Format("Fatal Error in function {0} ", ReflectionHelper.GetMyFunctionName()), ex);
                    else
                        log.Info(String.Format("Info in function {0} message: {1}", ReflectionHelper.GetMyFunctionName(), ex.Message));
                }
                RSData.ClusterParameterGroups = Auditor.CheckCIBaseline<Amazon.Redshift.Model.ClusterParameterGroup>(audit_params, RSData.ClusterParameterGroups.Current, "");

                // DescribeClusterParameters(DescribeClusterParametersRequest) Returns a detailed list of parameters contained within the specified Amazon Redshift parameter group. For each parameter the response includes information such as parameter name, description, data type, value, whether the parameter value is modifiable, and so on. 
                RSData.ClusterParameters = new ListComparisonResults<ClusterGroupParameters>();
                foreach (Amazon.Redshift.Model.ClusterParameterGroup grp in RSData.ClusterParameterGroups.Current)
                {
                    Amazon.Redshift.Model.DescribeClusterParametersResponse dcparresp =
                        RSClient.DescribeClusterParameters(new Amazon.Redshift.Model.DescribeClusterParametersRequest()
                                                            .WithParameterGroupName(grp.ParameterGroupName));
                    ClusterGroupParameters cgp = new ClusterGroupParameters(grp.ParameterGroupName, dcparresp.DescribeClusterParametersResult.Parameters);
                    while (dcparresp.DescribeClusterParametersResult.Marker != null)
                    {
                        dcparresp = RSClient.DescribeClusterParameters(new Amazon.Redshift.Model.DescribeClusterParametersRequest()
                                                                      .WithParameterGroupName(grp.ParameterGroupName)
                                                                      .WithMarker(dcparresp.DescribeClusterParametersResult.Marker));
                        cgp.Parameters.AddRange(dcparresp.DescribeClusterParametersResult.Parameters);
                    }
                    RSData.ClusterParameters.Current.Add(cgp);
                }
                RSData.ClusterParameters = Auditor.CheckCIBaseline<ClusterGroupParameters>(audit_params, RSData.ClusterParameters.Current, "");


                // DescribeClusters()()()() Returns properties of provisioned clusters including general cluster properties, cluster database properties, maintenance and backup properties, and security and access properties. This operation supports pagination. For more information about managing clusters, go to Amazon Redshift Clusters in the Amazon Redshift Management Guide . 
                RSData.Clusters = new ListComparisonResults<Amazon.Redshift.Model.Cluster>();
                try
                {
                    Amazon.Redshift.Model.DescribeClustersResponse dcresp = RSClient.DescribeClusters();
                    RSData.Clusters.Current.AddRange(dcresp.DescribeClustersResult.Clusters);
                    while (dcresp.DescribeClustersResult.Marker != null)
                    {
                        dcresp = RSClient.DescribeClusters(new Amazon.Redshift.Model.DescribeClustersRequest()
                                                .WithMarker(dcresp.DescribeClustersResult.Marker));
                        RSData.Clusters.Current.AddRange(dcresp.DescribeClustersResult.Clusters);
                    }
                }
                catch (Exception ex)
                {
                    if (!ex.Message.StartsWith("No endpoint found for service redshift for region"))
                        log.Fatal(String.Format("Fatal Error in function {0} ", ReflectionHelper.GetMyFunctionName()), ex);
                    else
                        log.Info(String.Format("Info in function {0} message: {1}", ReflectionHelper.GetMyFunctionName(), ex.Message));
                }
                RSData.Clusters = Auditor.CheckCIBaseline<Amazon.Redshift.Model.Cluster>(audit_params, RSData.Clusters.Current, "");

                // DescribeClusterSecurityGroups()()()() Returns information about Amazon Redshift security groups. If the name of a security group is specified, the response will contain only information about only that security group. 
                RSData.ClusterSecurityGroups = new ListComparisonResults<Amazon.Redshift.Model.ClusterSecurityGroup>();
                try
                {
                    Amazon.Redshift.Model.DescribeClusterSecurityGroupsResponse dcsgresp =
                        RSClient.DescribeClusterSecurityGroups();
                    RSData.ClusterSecurityGroups.Current.AddRange(dcsgresp.DescribeClusterSecurityGroupsResult.ClusterSecurityGroups);
                    while (dcsgresp.DescribeClusterSecurityGroupsResult.Marker != null)
                    {
                        dcsgresp =
                        RSClient.DescribeClusterSecurityGroups(new Amazon.Redshift.Model.DescribeClusterSecurityGroupsRequest()
                                                                .WithMarker(dcsgresp.DescribeClusterSecurityGroupsResult.Marker));
                        RSData.ClusterSecurityGroups.Current.AddRange(dcsgresp.DescribeClusterSecurityGroupsResult.ClusterSecurityGroups);
                    }
                }
                catch (Exception ex)
                {
                    if (!ex.Message.StartsWith("No endpoint found for service redshift for region"))
                        log.Fatal(String.Format("Fatal Error in function {0} ", ReflectionHelper.GetMyFunctionName()), ex);
                    else
                        log.Info(String.Format("Info in function {0} message: {1}", ReflectionHelper.GetMyFunctionName(), ex.Message));
                }
                RSData.ClusterSecurityGroups = Auditor.CheckCIBaseline<Amazon.Redshift.Model.ClusterSecurityGroup>(audit_params, RSData.ClusterSecurityGroups.Current, "");

                // DescribeClusterSnapshots()()()() Returns one or more snapshot objects, which contain metadata about your cluster snapshots. By default, this operation returns information about all snapshots of all clusters that are owned by the AWS account. 
                RSData.Snapshots = new ListComparisonResults<Amazon.Redshift.Model.Snapshot>();
                try
                {
                    Amazon.Redshift.Model.DescribeClusterSnapshotsResponse dcssresp =
                        RSClient.DescribeClusterSnapshots();
                    RSData.Snapshots.Current.AddRange(dcssresp.DescribeClusterSnapshotsResult.Snapshots);
                    while (dcssresp.DescribeClusterSnapshotsResult.Marker != null)
                    {
                        dcssresp = RSClient.DescribeClusterSnapshots(new Amazon.Redshift.Model.DescribeClusterSnapshotsRequest()
                                                .WithMarker(dcssresp.DescribeClusterSnapshotsResult.Marker));
                        RSData.Snapshots.Current.AddRange(dcssresp.DescribeClusterSnapshotsResult.Snapshots);
                    }
                }
                catch (Exception ex)
                {
                    if (!ex.Message.StartsWith("No endpoint found for service redshift for region"))
                        log.Fatal(String.Format("Fatal Error in function {0} ", ReflectionHelper.GetMyFunctionName()), ex);
                    else
                        log.Info(String.Format("Info in function {0} message: {1}", ReflectionHelper.GetMyFunctionName(), ex.Message));
                }
                RSData.Snapshots = Auditor.CheckCIBaseline<Amazon.Redshift.Model.Snapshot>(audit_params, RSData.Snapshots.Current, "");

                // DescribeClusterSubnetGroups()()()() Returns one or more cluster subnet group objects, which contain metadata about your cluster subnet groups. By default, this operation returns information about all cluster subnet groups that are defined in you AWS account. 
                RSData.ClusterSubnetGroups = new ListComparisonResults<Amazon.Redshift.Model.ClusterSubnetGroup>();
                try
                {
                    Amazon.Redshift.Model.DescribeClusterSubnetGroupsResponse dcsnsubresp = RSClient.DescribeClusterSubnetGroups();
                    RSData.ClusterSubnetGroups.Current.AddRange(dcsnsubresp.DescribeClusterSubnetGroupsResult.ClusterSubnetGroups);
                    while (dcsnsubresp.DescribeClusterSubnetGroupsResult.Marker != null)
                    {
                        dcsnsubresp = RSClient.DescribeClusterSubnetGroups(new Amazon.Redshift.Model.DescribeClusterSubnetGroupsRequest()
                            .WithMarker(dcsnsubresp.DescribeClusterSubnetGroupsResult.Marker));
                        RSData.ClusterSubnetGroups.Current.AddRange(dcsnsubresp.DescribeClusterSubnetGroupsResult.ClusterSubnetGroups);
                    }
                }
                catch (Exception ex)
                {
                    if (!ex.Message.StartsWith("No endpoint found for service redshift for region"))
                        log.Fatal(String.Format("Fatal Error in function {0} ", ReflectionHelper.GetMyFunctionName()), ex);
                    else
                        log.Info(String.Format("Info in function {0} message: {1}", ReflectionHelper.GetMyFunctionName(), ex.Message));
                }
                RSData.ClusterSubnetGroups = Auditor.CheckCIBaseline<Amazon.Redshift.Model.ClusterSubnetGroup>(audit_params, RSData.ClusterSubnetGroups.Current, "");


                // DescribeClusterVersions()()()() Returns descriptions of the available Amazon Redshift cluster versions. You can call this operation even before creating any clusters to learn more about the Amazon Redshift versions. For more information about managing clusters, go to Amazon Redshift Clusters in the Amazon Redshift Management Guide
                RSData.ClusterVersions = new ListComparisonResults<Amazon.Redshift.Model.ClusterVersion>();
                try
                {
                    Amazon.Redshift.Model.DescribeClusterVersionsResponse dcvresp = RSClient.DescribeClusterVersions();
                    RSData.ClusterVersions.Current.AddRange(dcvresp.DescribeClusterVersionsResult.ClusterVersions);
                    while (dcvresp.DescribeClusterVersionsResult.Marker != null)
                    {
                        dcvresp = RSClient.DescribeClusterVersions(new Amazon.Redshift.Model.DescribeClusterVersionsRequest()
                                                    .WithMarker(dcvresp.DescribeClusterVersionsResult.Marker)); ;
                        RSData.ClusterVersions.Current.AddRange(dcvresp.DescribeClusterVersionsResult.ClusterVersions);
                    }
                }
                catch (Exception ex)
                {
                    if (!ex.Message.StartsWith("No endpoint found for service redshift for region"))
                        log.Fatal(String.Format("Fatal Error in function {0} ", ReflectionHelper.GetMyFunctionName()), ex);
                    else
                        log.Info(String.Format("Info in function {0} message: {1}", ReflectionHelper.GetMyFunctionName(), ex.Message));
                }
                RSData.ClusterVersions = Auditor.CheckCIBaseline<Amazon.Redshift.Model.ClusterVersion>(audit_params, RSData.ClusterVersions.Current, "");

                // DescribeDefaultClusterParameters(DescribeDefaultClusterParametersRequest) Returns a list of parameter settings for the specified parameter group family. 
                // The list of parameters available in a parameter group depends on the parameter group family to which it belongs. 
                // A parameter group family refers to the Amazon Redshift engine version. For example, parameter group family "redshift-1.0"
                // identifies Amazon Redshift engine version 1.0. Parameter groups in this family have a specific set of parameters for a 
                // specific Amazon Redshift version.
                RSData.DefaultClusterParameterGroups = new ListComparisonResults<ClusterGroupParameters>();
                foreach (Amazon.Redshift.Model.ClusterVersion ver in RSData.ClusterVersions.Current)
                {
                    Amazon.Redshift.Model.DescribeDefaultClusterParametersResponse ddcpresp =
                        RSClient.DescribeDefaultClusterParameters(new Amazon.Redshift.Model.DescribeDefaultClusterParametersRequest()
                                                            .WithParameterGroupFamily(ver.ClusterParameterGroupFamily));
                    ClusterGroupParameters dpg = new ClusterGroupParameters(ver.ClusterParameterGroupFamily,
                            ddcpresp.DescribeDefaultClusterParametersResult.DefaultClusterParameters.Parameters);
                    while (ddcpresp.DescribeDefaultClusterParametersResult.DefaultClusterParameters.Marker != null)
                    {
                        ddcpresp = RSClient.DescribeDefaultClusterParameters(
                                                new Amazon.Redshift.Model.DescribeDefaultClusterParametersRequest()
                                                         .WithParameterGroupFamily(ver.ClusterParameterGroupFamily)
                                                         .WithMarker(ddcpresp.DescribeDefaultClusterParametersResult.DefaultClusterParameters.Marker));
                        dpg.Parameters.AddRange(ddcpresp.DescribeDefaultClusterParametersResult.DefaultClusterParameters.Parameters);
                    }
                    RSData.DefaultClusterParameterGroups.Current.Add(dpg);
                }
                RSData.DefaultClusterParameterGroups = Auditor.CheckCIBaseline<ClusterGroupParameters>(audit_params, RSData.DefaultClusterParameterGroups.Current, "");

                // DescribeEvents()()()() Returns events related to clusters, security groups, snapshots, and parameter groups for the past 14 days. Events specific to a particular cluster, security group, snapshot or parameter group can be obtained by providing the name as a parameter. By default, the past hour of events are returned. 
                RSData.Events = new ListComparisonResults<Amazon.Redshift.Model.Event>();
                try
                {
                    Amazon.Redshift.Model.DescribeEventsResponse deresp = RSClient.DescribeEvents();
                    RSData.Events.Current.AddRange(deresp.DescribeEventsResult.Events);
                    while (deresp.DescribeEventsResult.Marker != null)
                    {
                        deresp = RSClient.DescribeEvents(new Amazon.Redshift.Model.DescribeEventsRequest()
                            .WithMarker(deresp.DescribeEventsResult.Marker));
                        RSData.Events.Current.AddRange(deresp.DescribeEventsResult.Events);
                    }
                }
                catch (Exception ex)
                {
                    if (!ex.Message.StartsWith("No endpoint found for service redshift for region"))
                        log.Fatal(String.Format("Fatal Error in function {0} ", ReflectionHelper.GetMyFunctionName()), ex);
                    else
                        log.Info(String.Format("Info in function {0} message: {1}", ReflectionHelper.GetMyFunctionName(), ex.Message));
                }
                RSData.Events = Auditor.CheckCIBaseline<Amazon.Redshift.Model.Event>(audit_params, RSData.Events.Current, "");

                // DescribeOrderableClusterOptions()()()() Returns a list of orderable cluster options. Before you create a new cluster you can use this operation to find what options are available, such as the EC2 Availability Zones (AZ) in the specific AWS region that you can specify, and the node types you can request. The node types differ by available storage, memory, CPU and price. With the cost involved you might want to obtain a list of cluster options in the specific region and specify values when creating a cluster. For more information about managing clusters, go to Amazon Redshift Clusters in the Amazon Redshift Management Guide
                RSData.OrderableClusterOptions = new ListComparisonResults<Amazon.Redshift.Model.OrderableClusterOption>();
                try
                {
                    Amazon.Redshift.Model.DescribeOrderableClusterOptionsResponse docoresp = RSClient.DescribeOrderableClusterOptions();
                    RSData.OrderableClusterOptions.Current.AddRange(docoresp.DescribeOrderableClusterOptionsResult.OrderableClusterOptions);
                    while (docoresp.DescribeOrderableClusterOptionsResult.Marker != null)
                    {
                        docoresp = RSClient.DescribeOrderableClusterOptions(new Amazon.Redshift.Model.DescribeOrderableClusterOptionsRequest()
                            .WithMarker(docoresp.DescribeOrderableClusterOptionsResult.Marker));
                        RSData.OrderableClusterOptions.Current.AddRange(docoresp.DescribeOrderableClusterOptionsResult.OrderableClusterOptions);
                    }
                }
                catch (Exception ex)
                {
                    if (!ex.Message.StartsWith("No endpoint found for service redshift for region"))
                        log.Fatal(String.Format("Fatal Error in function {0} ", ReflectionHelper.GetMyFunctionName()), ex);
                    else
                        log.Info(String.Format("Info in function {0} message: {1}", ReflectionHelper.GetMyFunctionName(), ex.Message));
                }
                RSData.OrderableClusterOptions = Auditor.CheckCIBaseline<Amazon.Redshift.Model.OrderableClusterOption>(audit_params, RSData.OrderableClusterOptions.Current, "");

                // DescribeReservedNodeOfferings()()()() Returns a list of the available reserved node offerings by Amazon Redshift with their descriptions including the node type, the fixed and recurring costs of reserving the node and duration the node will be reserved for you. These descriptions help you determine which reserve node offering you want to purchase. You then use the unique offering ID in you call to PurchaseReservedNodeOffering to reserve one or more nodes for your Amazon Redshift cluster. 
                RSData.ReservedNodeOfferings = new ListComparisonResults<Amazon.Redshift.Model.ReservedNodeOffering>();
                try
                {
                    Amazon.Redshift.Model.DescribeReservedNodeOfferingsResponse drnoresp = RSClient.DescribeReservedNodeOfferings();
                    RSData.ReservedNodeOfferings.Current.AddRange(drnoresp.DescribeReservedNodeOfferingsResult.ReservedNodeOfferings);
                    while (drnoresp.DescribeReservedNodeOfferingsResult.Marker != null)
                    {
                        drnoresp = RSClient.DescribeReservedNodeOfferings(new Amazon.Redshift.Model.DescribeReservedNodeOfferingsRequest()
                                                            .WithMarker(drnoresp.DescribeReservedNodeOfferingsResult.Marker));
                        RSData.ReservedNodeOfferings.Current.AddRange(drnoresp.DescribeReservedNodeOfferingsResult.ReservedNodeOfferings);
                    }
                }
                catch (Exception ex)
                {
                    if (!ex.Message.StartsWith("No endpoint found for service redshift for region"))
                        log.Fatal(String.Format("Fatal Error in function {0} ", ReflectionHelper.GetMyFunctionName()), ex);
                    else
                        log.Info(String.Format("Info in function {0} message: {1}", ReflectionHelper.GetMyFunctionName(), ex.Message));
                }
                RSData.ReservedNodeOfferings = Auditor.CheckCIBaseline<Amazon.Redshift.Model.ReservedNodeOffering>(audit_params, RSData.ReservedNodeOfferings.Current, "");

                // DescribeReservedNodes()()()() Returns the descriptions of the reserved nodes. 
                RSData.ReservedNodes = new ListComparisonResults<Amazon.Redshift.Model.ReservedNode>();
                try
                {
                    Amazon.Redshift.Model.DescribeReservedNodesResponse drnresp = RSClient.DescribeReservedNodes();
                    RSData.ReservedNodes.Current.AddRange(drnresp.DescribeReservedNodesResult.ReservedNodes);
                    while (drnresp.DescribeReservedNodesResult.Marker != null)
                    {
                        drnresp = RSClient.DescribeReservedNodes(new Amazon.Redshift.Model.DescribeReservedNodesRequest()
                            .WithMarker(drnresp.DescribeReservedNodesResult.Marker)); ;
                        RSData.ReservedNodes.Current.AddRange(drnresp.DescribeReservedNodesResult.ReservedNodes);
                    }
                }
                catch (Exception ex)
                {
                    if (!ex.Message.StartsWith("No endpoint found for service redshift for region"))
                        log.Fatal(String.Format("Fatal Error in function {0} ", ReflectionHelper.GetMyFunctionName()), ex);
                    else
                        log.Info(String.Format("Info in function {0} message: {1}", ReflectionHelper.GetMyFunctionName(), ex.Message));
                }
                RSData.ReservedNodes = Auditor.CheckCIBaseline<Amazon.Redshift.Model.ReservedNode>(audit_params, RSData.ReservedNodes.Current, "");

                // DescribeResize(DescribeResizeRequest) Returns information about the last resize operation for the specified cluster. 
                //If no resize operation has ever been initiated for the specified cluster, a HTTP 404 error is returned. 
                //If a resize operation was initiated and completed, the status of the resize remains as SUCCEEDED until the next resize. 
                RSData.ClusterResizedData = new ListComparisonResults<ClusterResizeData>();
                foreach (Amazon.Redshift.Model.Cluster cl in RSData.Clusters.Current)
                {
                    try
                    {
                        Amazon.Redshift.Model.DescribeResizeResponse drszresp =
                            RSClient.DescribeResize(new Amazon.Redshift.Model.DescribeResizeRequest().WithClusterIdentifier(cl.ClusterIdentifier));
                        ClusterResizeData crsz = new ClusterResizeData(cl.ClusterIdentifier, drszresp.DescribeResizeResult);
                        RSData.ClusterResizedData.Current.Add(crsz);
                    }
                    catch (Amazon.Redshift.Model.ClusterNotFoundException ex)
                    {
                        log.Fatal(String.Format("Fatal Error in function {0} ", ReflectionHelper.GetMyFunctionName()), ex);
                    }
                    catch (Amazon.Redshift.Model.ResizeNotFoundException ex)
                    {
                        log.Fatal(String.Format("Fatal Error in function {0} ", ReflectionHelper.GetMyFunctionName()), ex);
                    }
                    //A resize operation can be requested using ModifyCluster and specifying a different number or type of nodes for the cluster. 


                }




            }


            return RSData;
        }

        public  SimpleEmail ReadSimpleEmail(AuditParams audit_params)
        {
            BaselineAuditor Auditor = new BaselineAuditor();


            SimpleEmail SEData = new SimpleEmail();
            StringBuilder sb = new StringBuilder(1024);
            using (StringWriter sr = new StringWriter(sb))
            {
                try
                {
                    Amazon.SimpleEmail.AmazonSimpleEmailServiceClient SEClient = new Amazon.SimpleEmail.AmazonSimpleEmailServiceClient(audit_params.AWSCredentials, audit_params.AWSRegion);

                    // ListIdentities()()()() Returns a list containing all of the identities (email addresses and domains) for a specific AWS Account, regardless of verification status.
                    SEData.Identities = new ListComparisonResults<string>();
                    Amazon.SimpleEmail.Model.ListIdentitiesResponse liresp = SEClient.ListIdentities();
                    SEData.Identities.Current.AddRange(liresp.ListIdentitiesResult.Identities);
                    while (liresp.ListIdentitiesResult.NextToken != null)
                    {
                        liresp = SEClient.ListIdentities(new Amazon.SimpleEmail.Model.ListIdentitiesRequest()
                                            .WithNextToken(liresp.ListIdentitiesResult.NextToken)); ;
                        SEData.Identities.Current.AddRange(liresp.ListIdentitiesResult.Identities);
                    }
                    SEData.Identities = Auditor.CheckCIBaseline<string>(audit_params, SEData.Identities.Current, "");


                    // GetIdentityDkimAttributes(GetIdentityDkimAttributesRequest) Returns the DNS records, or tokens , that must be present in order for Easy DKIM to sign outgoing email messages.
                    //This action takes a list of verified identities as input. It then returns the following information for each identity:

                    //Whether Easy DKIM signing is enabled or disabled.
                    //The set of tokens that are required for Easy DKIM signing. These tokens must be published in the domain name's DNS records in order for DKIM verification to complete, and must remain published in order for Easy DKIM signing to operate correctly. (This information is only returned for domain name identities, not for email addresses.)
                    //Whether Amazon SES has successfully verified the DKIM tokens published in the domain name's DNS. (This information is only returned for domain name identities, not for email addresses.)
                    //For more information about Easy DKIM signing, go to the Amazon SES Developer Guide.
                    SEData.IdentityDkimAttributes = new ListComparisonResults<Dictionary<string, Amazon.SimpleEmail.Model.IdentityDkimAttributes>>();
                    Dictionary<string, Amazon.SimpleEmail.Model.IdentityDkimAttributes> result = new Dictionary<string, Amazon.SimpleEmail.Model.IdentityDkimAttributes>();
                    foreach (string id in SEData.Identities.Current)
                    {
                        Amazon.SimpleEmail.Model.GetIdentityDkimAttributesResponse gdkaresp =
                            SEClient.GetIdentityDkimAttributes(new Amazon.SimpleEmail.Model.GetIdentityDkimAttributesRequest()
                                .WithIdentities(id));
                        result.Add(id, gdkaresp.GetIdentityDkimAttributesResult.DkimAttributes[id]);
                    }
                    SEData.IdentityDkimAttributes.Current.Add(result);
                    SEData.IdentityDkimAttributes = Auditor.CheckCIBaseline<Dictionary<string, Amazon.SimpleEmail.Model.IdentityDkimAttributes>>(audit_params, SEData.IdentityDkimAttributes.Current, "");


                    // GetIdentityNotificationAttributes(GetIdentityNotificationAttributesRequest) Given a list of verified identities (email addresses and/or domains), returns a structure describing identity notification attributes. For more information about feedback notification, see the Amazon SES Developer Guide.
                    SEData.IdentityNotificationAttributes = new ListComparisonResults<Dictionary<string, Amazon.SimpleEmail.Model.IdentityNotificationAttributes>>();
                    Dictionary<string, Amazon.SimpleEmail.Model.IdentityNotificationAttributes> notes = new Dictionary<string, Amazon.SimpleEmail.Model.IdentityNotificationAttributes>();
                    foreach (string id in SEData.Identities.Current)
                    {
                        Amazon.SimpleEmail.Model.GetIdentityNotificationAttributesResponse ginresp =
                            SEClient.GetIdentityNotificationAttributes(new Amazon.SimpleEmail.Model.GetIdentityNotificationAttributesRequest()
                            .WithIdentities(id));
                        notes.Add(id, ginresp.GetIdentityNotificationAttributesResult.NotificationAttributes[id]);
                    }
                    SEData.IdentityNotificationAttributes.Current.Add(notes);
                    SEData.IdentityNotificationAttributes = Auditor.CheckCIBaseline<Dictionary<string, Amazon.SimpleEmail.Model.IdentityNotificationAttributes>>(audit_params, SEData.IdentityNotificationAttributes.Current, "");

                    // GetIdentityVerificationAttributes(GetIdentityVerificationAttributesRequest) Given a list of identities (email addresses and/or domains), returns the verification status and (for domain identities) the verification token for each identity.
                    SEData.IdentityVerificationAttributes = new ListComparisonResults<Dictionary<string, Amazon.SimpleEmail.Model.IdentityVerificationAttributes>>();
                    Dictionary<string, Amazon.SimpleEmail.Model.IdentityVerificationAttributes> atts =
                        new Dictionary<string, Amazon.SimpleEmail.Model.IdentityVerificationAttributes>();
                    foreach (string id in SEData.Identities.Current)
                    {
                        Amazon.SimpleEmail.Model.GetIdentityVerificationAttributesResponse gatresp =
                            SEClient.GetIdentityVerificationAttributes(new Amazon.SimpleEmail.Model.GetIdentityVerificationAttributesRequest()
                            .WithIdentities(id));
                        atts.Add(id, gatresp.GetIdentityVerificationAttributesResult.VerificationAttributes[id]);
                    }
                    SEData.IdentityVerificationAttributes.Current.Add(atts);
                    SEData.IdentityVerificationAttributes = Auditor.CheckCIBaseline<Dictionary<string, Amazon.SimpleEmail.Model.IdentityVerificationAttributes>>(audit_params, SEData.IdentityVerificationAttributes.Current, "");

                    // GetSendQuota()()()() Returns the user's current sending limits.
                    Amazon.SimpleEmail.Model.GetSendQuotaResponse gsqresp = SEClient.GetSendQuota(new Amazon.SimpleEmail.Model.GetSendQuotaRequest());
                    SEData.SendQuota = new ListComparisonResults<Amazon.SimpleEmail.Model.GetSendQuotaResult>();
                    SEData.SendQuota.Current.Add(gsqresp.GetSendQuotaResult);
                    SEData.SendQuota = Auditor.CheckCIBaseline<Amazon.SimpleEmail.Model.GetSendQuotaResult>(audit_params, SEData.SendQuota.Current, "");

                    // GetSendStatistics()()()() Returns the user's sending statistics. The result is a list of data points, representing the last two weeks of sending activity. 
                    //Each data point in the list contains statistics for a 15-minute interval.
                    SEData.SendStatistics = new ListComparisonResults<Amazon.SimpleEmail.Model.SendDataPoint>();
                    Amazon.SimpleEmail.Model.GetSendStatisticsResponse gssresp =
                        SEClient.GetSendStatistics(new Amazon.SimpleEmail.Model.GetSendStatisticsRequest());
                    SEData.SendStatistics.Current.AddRange(gssresp.GetSendStatisticsResult.SendDataPoints);
                    SEData.SendStatistics = Auditor.CheckCIBaseline<Amazon.SimpleEmail.Model.SendDataPoint>(audit_params, SEData.SendStatistics.Current, "");



                }
                catch (Exception ex)
                {
                    log.Fatal(String.Format("Fatal Error in function {0} ", ReflectionHelper.GetMyFunctionName()), ex);
                }

            }


            return SEData;
        }

        public  SimpleNotificationService ReadSimpleNotificationService(AuditParams audit_params)
        {

            BaselineAuditor Auditor = new BaselineAuditor();

            SimpleNotificationService SNSData = new SimpleNotificationService();
            StringBuilder sb = new StringBuilder(1024);
            using (StringWriter sr = new StringWriter(sb))
            {
                try
                {
                    Amazon.SimpleNotificationService.AmazonSimpleNotificationServiceClient SNSClient = new Amazon.SimpleNotificationService.AmazonSimpleNotificationServiceClient(audit_params.AWSCredentials, audit_params.AWSRegion);

                    //ListSubscriptionsByTopic(ListSubscriptionsByTopicRequest) Returns a list of the subscriptions to a specific topic.  

                    //ListSubscriptions(ListSubscriptionsRequest) Returns a list of the requester's subscriptions.  
                    SNSData.Subscriptions = new ListComparisonResults<Amazon.SimpleNotificationService.Model.Subscription>();

                    Amazon.SimpleNotificationService.Model.ListSubscriptionsResponse lsresp =
                        SNSClient.ListSubscriptions(new Amazon.SimpleNotificationService.Model.ListSubscriptionsRequest());
                    SNSData.Subscriptions.Current.AddRange(lsresp.ListSubscriptionsResult.Subscriptions);
                    while (lsresp.ListSubscriptionsResult.NextToken != null)
                    {
                        lsresp = SNSClient.ListSubscriptions(new Amazon.SimpleNotificationService.Model.ListSubscriptionsRequest()
                            .WithNextToken(lsresp.ListSubscriptionsResult.NextToken));
                        SNSData.Subscriptions.Current.AddRange(lsresp.ListSubscriptionsResult.Subscriptions);
                    }
                    SNSData.Subscriptions = Auditor.CheckCIBaseline<Amazon.SimpleNotificationService.Model.Subscription>(audit_params, SNSData.Subscriptions.Current, "");

                    //GetSubscriptionAttributes(GetSubscriptionAttributesRequest) Returns all of the properties of a subscription customers have created. Subscription properties returned might differ based on the authorization of the user.  
                    SNSData.SubscriptionAttributes = new ListComparisonResults<SNSSubscriptionAttribute>();
                    foreach (Amazon.SimpleNotificationService.Model.Subscription suba in SNSData.Subscriptions.Current)
                    {

                        Amazon.SimpleNotificationService.Model.GetSubscriptionAttributesResponse gsatresp =
                            SNSClient.GetSubscriptionAttributes(new Amazon.SimpleNotificationService.Model.GetSubscriptionAttributesRequest()
                            .WithSubscriptionArn(suba.SubscriptionArn));
                        SNSSubscriptionAttribute sub = new SNSSubscriptionAttribute(suba.SubscriptionArn, gsatresp.GetSubscriptionAttributesResult.Attributes);
                        SNSData.SubscriptionAttributes.Current.Add(sub);
                    }
                    SNSData.SubscriptionAttributes = Auditor.CheckCIBaseline<SNSSubscriptionAttribute>(audit_params, SNSData.SubscriptionAttributes.Current, "");

                    //ListTopics(ListTopicsRequest) Returns a list of the requester's topics.  
                    SNSData.Topics = new ListComparisonResults<Amazon.SimpleNotificationService.Model.Topic>();
                    Amazon.SimpleNotificationService.Model.ListTopicsResponse ltresp =
                        SNSClient.ListTopics(new Amazon.SimpleNotificationService.Model.ListTopicsRequest());
                    SNSData.Topics.Current.AddRange(ltresp.ListTopicsResult.Topics);
                    while (ltresp.ListTopicsResult.NextToken != null)
                    {
                        ltresp = SNSClient.ListTopics(new Amazon.SimpleNotificationService.Model.ListTopicsRequest()
                            .WithNextToken(ltresp.ListTopicsResult.NextToken));
                        SNSData.Topics.Current.AddRange(ltresp.ListTopicsResult.Topics);
                    }
                    SNSData.Topics = Auditor.CheckCIBaseline<Amazon.SimpleNotificationService.Model.Topic>(audit_params, SNSData.Topics.Current, "");

                    //GetTopicAttributes(GetTopicAttributesRequest) Returns all of the properties of a topic customers have created. Topic properties returned might differ based on the authorization of the user.  
                    SNSData.TopicAttributes = new ListComparisonResults<SNSTopicAttribute>();
                    foreach (Amazon.SimpleNotificationService.Model.Topic t in SNSData.Topics.Current)
                    {
                        Amazon.SimpleNotificationService.Model.GetTopicAttributesResponse gtresp =
                            SNSClient.GetTopicAttributes(new Amazon.SimpleNotificationService.Model.GetTopicAttributesRequest()
                            .WithTopicArn(t.TopicArn));
                        SNSTopicAttribute sub = new SNSTopicAttribute(t.TopicArn, gtresp.GetTopicAttributesResult.Attributes);
                        SNSData.TopicAttributes.Current.Add(sub);
                    }
                    SNSData.TopicAttributes = Auditor.CheckCIBaseline<SNSTopicAttribute>(audit_params, SNSData.TopicAttributes.Current, "");


                }
                catch (Exception ex)
                {
                   log.Fatal(String.Format("Fatal Error in function {0} ", ReflectionHelper.GetMyFunctionName()), ex);
                }

            }


            return SNSData;
        }
        public  SimpleWorkflow ReadSimpleWorkflow(AuditParams audit_params)
        {
            BaselineAuditor Auditor = new BaselineAuditor();


            SimpleWorkflow SWData = new SimpleWorkflow();
            StringBuilder sb = new StringBuilder(1024);
            using (StringWriter sr = new StringWriter(sb))
            {
                try
                {
                    Amazon.SimpleWorkflow.AmazonSimpleWorkflowClient SWClient = new Amazon.SimpleWorkflow.AmazonSimpleWorkflowClient(audit_params.AWSCredentials, audit_params.AWSRegion);


                    //ListDomains(ListDomainsRequest) Returns the list of domains registered in the account. The results may be split into multiple pages. To retrieve subsequent pages, make the call again using the nextPageToken returned by the initial call. 
                    SWData.Domains = new ListComparisonResults<Amazon.SimpleWorkflow.Model.DomainInfo>();
                    Amazon.SimpleWorkflow.Model.ListDomainsResponse ldresp = SWClient.ListDomains(new Amazon.SimpleWorkflow.Model.ListDomainsRequest().WithRegistrationStatus("REGISTERED"));
                    SWData.Domains.Current.AddRange(ldresp.ListDomainsResult.DomainInfos.Name);
                    while (ldresp.ListDomainsResult.DomainInfos.NextPageToken != null)
                    {
                        ldresp = SWClient.ListDomains(new Amazon.SimpleWorkflow.Model.ListDomainsRequest()
                                        .WithNextPageToken(ldresp.ListDomainsResult.DomainInfos.NextPageToken));
                        SWData.Domains.Current.AddRange(ldresp.ListDomainsResult.DomainInfos.Name);
                    }
                    ldresp = SWClient.ListDomains(new Amazon.SimpleWorkflow.Model.ListDomainsRequest().WithRegistrationStatus("DEPRECATED"));
                    SWData.Domains.Current.AddRange(ldresp.ListDomainsResult.DomainInfos.Name);
                    while (ldresp.ListDomainsResult.DomainInfos.NextPageToken != null)
                    {
                        ldresp = SWClient.ListDomains(new Amazon.SimpleWorkflow.Model.ListDomainsRequest()
                                        .WithNextPageToken(ldresp.ListDomainsResult.DomainInfos.NextPageToken));
                        SWData.Domains.Current.AddRange(ldresp.ListDomainsResult.DomainInfos.Name);
                    }

                    SWData.Domains = Auditor.CheckCIBaseline<Amazon.SimpleWorkflow.Model.DomainInfo>(audit_params, SWData.Domains.Current, "");


                    //ListActivityTypes(ListActivityTypesRequest) Returns information about all activities registered in the specified domain that match the specified name and registration status. The result includes information like creation date, current status of the activity, etc. The results may be split into multiple pages. To retrieve subsequent pages, make the call again using the nextPageToken returned by the initial call. 
                    Dictionary<string, List<Amazon.SimpleWorkflow.Model.ActivityTypeInfo>> activities =
                            new Dictionary<string, List<Amazon.SimpleWorkflow.Model.ActivityTypeInfo>>();
                    SWData.ActivityInfo = new ListComparisonResults<Dictionary<string, List<Amazon.SimpleWorkflow.Model.ActivityTypeInfo>>>();
                    foreach (Amazon.SimpleWorkflow.Model.DomainInfo di in SWData.Domains.Current)
                    {
                        Amazon.SimpleWorkflow.Model.ListActivityTypesResponse latresp =
                            SWClient.ListActivityTypes(new Amazon.SimpleWorkflow.Model.ListActivityTypesRequest()
                                                .WithDomain(di.Name));
                        activities.Add(di.Name, latresp.ListActivityTypesResult.ActivityTypeInfos.TypeInfos);
                        while (latresp.ListActivityTypesResult.ActivityTypeInfos.NextPageToken != null)
                        {
                            latresp =
                            SWClient.ListActivityTypes(new Amazon.SimpleWorkflow.Model.ListActivityTypesRequest()
                                                .WithDomain(di.Name)
                                                .WithNextPageToken(latresp.ListActivityTypesResult.ActivityTypeInfos.NextPageToken));
                            activities[di.Name].AddRange(latresp.ListActivityTypesResult.ActivityTypeInfos.TypeInfos);
                        }

                    }
                    SWData.ActivityInfo.Current.Add(activities);
                    SWData.ActivityInfo = Auditor.CheckCIBaseline<Dictionary<string, List<Amazon.SimpleWorkflow.Model.ActivityTypeInfo>>>(audit_params, SWData.ActivityInfo.Current, "");

                    //ListWorkflowTypes(ListWorkflowTypesRequest) Returns information about workflow types in the specified domain. The results may be split into multiple pages that can be retrieved by making the call repeatedly. 
                    SWData.WorkflowInfo = new ListComparisonResults<Dictionary<string, List<Amazon.SimpleWorkflow.Model.WorkflowTypeInfo>>>();
                    Dictionary<string, List<Amazon.SimpleWorkflow.Model.WorkflowTypeInfo>> workflows = new Dictionary<string, List<Amazon.SimpleWorkflow.Model.WorkflowTypeInfo>>();
                    foreach (Amazon.SimpleWorkflow.Model.DomainInfo di in SWData.Domains.Current)
                    {
                        Amazon.SimpleWorkflow.Model.ListWorkflowTypesResponse lwftresp =
                            SWClient.ListWorkflowTypes(new Amazon.SimpleWorkflow.Model.ListWorkflowTypesRequest()
                            .WithDomain(di.Name));
                        workflows.Add(di.Name, lwftresp.ListWorkflowTypesResult.WorkflowTypeInfos.TypeInfos);
                        while (lwftresp.ListWorkflowTypesResult.WorkflowTypeInfos.NextPageToken != null)
                        {
                            lwftresp = SWClient.ListWorkflowTypes(new Amazon.SimpleWorkflow.Model.ListWorkflowTypesRequest()
                                                    .WithDomain(di.Name).WithNextPageToken(lwftresp.ListWorkflowTypesResult.WorkflowTypeInfos.NextPageToken));
                            workflows.Add(di.Name, lwftresp.ListWorkflowTypesResult.WorkflowTypeInfos.TypeInfos);
                        }
                    }
                    SWData.WorkflowInfo.Current.Add(workflows);
                    SWData.WorkflowInfo = Auditor.CheckCIBaseline<Dictionary<string, List<Amazon.SimpleWorkflow.Model.WorkflowTypeInfo>>>(audit_params, SWData.WorkflowInfo.Current, "");


                    // DescribeDomain(DescribeDomainRequest) Returns information about the specified domain including description and status. 
                    SWData.DomainDetails = new ListComparisonResults<Amazon.SimpleWorkflow.Model.DomainDetail>();
                    foreach (Amazon.SimpleWorkflow.Model.DomainInfo di in SWData.Domains.Current)
                    {

                        Amazon.SimpleWorkflow.Model.DescribeDomainResponse ddresp =
                            SWClient.DescribeDomain(new Amazon.SimpleWorkflow.Model.DescribeDomainRequest()
                                        .WithName(di.Name));
                        SWData.DomainDetails.Current.Add(ddresp.DescribeDomainResult.DomainDetail);
                    }
                    SWData.DomainDetails = Auditor.CheckCIBaseline<Amazon.SimpleWorkflow.Model.DomainDetail>(audit_params, SWData.DomainDetails.Current, "");


                    // DescribeWorkflowExecution(DescribeWorkflowExecutionRequest) Returns information about the specified workflow execution including its type and some statistics. 
                    SWData.DomainWorkflowExecutionDetails = new ListComparisonResults<SWWorkflowExecutionDetail>();
                    foreach (Amazon.SimpleWorkflow.Model.DomainInfo di in SWData.Domains.Current)
                    {

                        Amazon.SimpleWorkflow.Model.DescribeWorkflowExecutionResponse dwferesp =
                            SWClient.DescribeWorkflowExecution(new Amazon.SimpleWorkflow.Model.DescribeWorkflowExecutionRequest()
                                                .WithDomain(di.Name));
                        SWWorkflowExecutionDetail det = new SWWorkflowExecutionDetail(di.Name,
                                                            dwferesp.DescribeWorkflowExecutionResult.WorkflowExecutionDetail);
                        SWData.DomainWorkflowExecutionDetails.Current.Add(det);
                    }
                    SWData.DomainWorkflowExecutionDetails = Auditor.CheckCIBaseline<SWWorkflowExecutionDetail>(audit_params, SWData.DomainWorkflowExecutionDetails.Current, "");

                    // DescribeWorkflowType(DescribeWorkflowTypeRequest) Returns information about the specified workflow type . This includes configuration settings specified when the type was registered and other information such as creation date, current status, etc. 
                    SWData.DomainWorkflowTypeDetails = new ListComparisonResults<SWWorkflowTypeDetail>();
                    foreach (Amazon.SimpleWorkflow.Model.DomainInfo di in SWData.Domains.Current)
                    {
                        foreach (Amazon.SimpleWorkflow.Model.WorkflowTypeInfo wfti in SWData.WorkflowInfo.Current[0][di.Name])
                        {

                            Amazon.SimpleWorkflow.Model.DescribeWorkflowTypeResponse dwftresp =
                                                    SWClient.DescribeWorkflowType(new Amazon.SimpleWorkflow.Model.DescribeWorkflowTypeRequest()
                                                                                      .WithDomain(di.Name)
                                                                                      .WithWorkflowType(wfti.WorkflowType));

                            SWWorkflowTypeDetail typeDetails = new SWWorkflowTypeDetail(di.Name, wfti.WorkflowType, dwftresp.DescribeWorkflowTypeResult.WorkflowTypeDetail);
                            SWData.DomainWorkflowTypeDetails.Current.Add(typeDetails);
                        }
                    }
                    SWData.DomainWorkflowTypeDetails = Auditor.CheckCIBaseline<SWWorkflowTypeDetail>(audit_params, SWData.DomainWorkflowTypeDetails.Current, "");


                }
                catch (Exception ex)
                {
                   log.Fatal(String.Format("Fatal Error in function {0} ", ReflectionHelper.GetMyFunctionName()), ex);
                }

            }


            return SWData;

        }
        public  SQSService ReadSQSService(AuditParams audit_params)
        {

            BaselineAuditor Auditor = new BaselineAuditor();
            SQSService SQSData = new SQSService();
            StringBuilder sb = new StringBuilder(1024);
            using (StringWriter sr = new StringWriter(sb))
            {
                try
                {
                    Amazon.SQS.AmazonSQSClient SQSClient = new Amazon.SQS.AmazonSQSClient(audit_params.AWSCredentials, audit_params.AWSRegion);

                    Amazon.SQS.Model.ListQueuesResponse lqresp = SQSClient.ListQueues(new Amazon.SQS.Model.ListQueuesRequest());
                    SQSData.QueueUrl = new ListComparisonResults<string>();
                    SQSData.QueueUrl.Current.AddRange(lqresp.ListQueuesResult.QueueUrl);
                    SQSData.QueueUrl = Auditor.CheckCIBaseline<string>(audit_params, SQSData.QueueUrl.Current, "");

                    SQSData.QueueAttributes = new ListComparisonResults<QueueAttributes>();
                    foreach (string queue in SQSData.QueueUrl.Current)
                    {
                        Amazon.SQS.Model.GetQueueAttributesResponse gqaresp =
                            SQSClient.GetQueueAttributes(new Amazon.SQS.Model.GetQueueAttributesRequest().WithQueueUrl(queue));
                        Amazon.SQS.Model.GetQueueAttributesResult result = gqaresp.GetQueueAttributesResult;
                        //Note Amazon.SQS.Model.GetQueueAttributesResult has both configuraiton
                        // and performance metrics.
                        // This can cause issues trying to use for CM so we'll xfer the settings
                        QueueAttributes qa = new QueueAttributes();
                        qa.Attributes = result.Attribute;
                        qa.CreatedTimestamp = result.CreatedTimestamp;
                        qa.DelaySeconds = result.DelaySeconds;
                        qa.LastModifiedTimestamp = result.LastModifiedTimestamp;
                        qa.MaximumMessageSize = result.MaximumMessageSize;
                        qa.MessageRetentionPeriod = result.MessageRetentionPeriod;
                        qa.Policy = result.Policy;
                        qa.QueueARN = result.QueueARN;
                        qa.VisibilityTimeout = result.VisibilityTimeout;
                        SQSData.QueueAttributes.Current.Add(qa);
                    }
                }
                catch (Exception ex)
                {
                    log.Fatal(String.Format("Fatal Error in function {0} ", ReflectionHelper.GetMyFunctionName()), ex);
                }
            }



            return SQSData;
        }
        #endregion

    }
}
