using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AWSResponderConsole
{
    public class SimpleWorkflow
    {
        public ListComparisonResults<Amazon.SimpleWorkflow.Model.DomainInfo> Domains { get; set; }
        public ListComparisonResults<Dictionary<string, List<Amazon.SimpleWorkflow.Model.ActivityTypeInfo>>> ActivityInfo { get; set; }
        public ListComparisonResults<Dictionary<string, List<Amazon.SimpleWorkflow.Model.WorkflowTypeInfo>>> WorkflowInfo { get; set; }
        public ListComparisonResults<Amazon.SimpleWorkflow.Model.DomainDetail> DomainDetails { get; set; }
        public ListComparisonResults<SWWorkflowExecutionDetail> DomainWorkflowExecutionDetails { get; set; }
        public ListComparisonResults<SWWorkflowTypeDetail> DomainWorkflowTypeDetails { get; set; }
    }
    public class SWWorkflowExecutionDetail
    {
        public string DomainName { get; set; }
        public Amazon.SimpleWorkflow.Model.WorkflowExecutionDetail WorkflowExecutionDetails { get; set; }
        public SWWorkflowExecutionDetail(string name, Amazon.SimpleWorkflow.Model.WorkflowExecutionDetail details)
        {
            this.DomainName = name;
            this.WorkflowExecutionDetails = details;
        }
    }
    public class SWWorkflowTypeDetail
    {
        public string DomainName { get; set; }
        public Amazon.SimpleWorkflow.Model.WorkflowType WorkflowType { get; set; }
        public Amazon.SimpleWorkflow.Model.WorkflowTypeDetail WorkflowTypeDetails { get; set; }
        public SWWorkflowTypeDetail(string name, Amazon.SimpleWorkflow.Model.WorkflowType ty, Amazon.SimpleWorkflow.Model.WorkflowTypeDetail details)
        {
            this.DomainName = name;
            this.WorkflowTypeDetails = details;
            this.WorkflowType = ty;
        }
    }
}
