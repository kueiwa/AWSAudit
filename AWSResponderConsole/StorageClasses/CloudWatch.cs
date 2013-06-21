using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AWSResponderConsole
{
    public class CloudWatch
    {
        public ListComparisonResults<Amazon.CloudWatch.Model.MetricAlarm> MetricAlarms { get; set; }
        public ListComparisonResults<Amazon.CloudWatch.Model.Metric> Metrics { get; set; }

    }

}
