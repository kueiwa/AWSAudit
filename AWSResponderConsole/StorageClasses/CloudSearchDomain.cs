using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AWSResponderConsole
{
    public class CloudSearchDomain
    {
        Amazon.CloudSearch.Model.DomainStatus Status { get; set; }
        Amazon.CloudSearch.Model.DefaultSearchFieldStatus DefaultSearchField { get; set; }
        public List<Amazon.CloudSearch.Model.IndexFieldStatus> IndexFields { get; set; }
        public List<Amazon.CloudSearch.Model.RankExpressionStatus> RankExpressions { get; set; }
        public Amazon.CloudSearch.Model.AccessPoliciesStatus ServiceAccessPolicies { get; set; }
        public Amazon.CloudSearch.Model.StemmingOptionsStatus StemmingOptions { get; set; }
        public Amazon.CloudSearch.Model.StopwordOptionsStatus StopwordOptions { get; set; }
        public Amazon.CloudSearch.Model.SynonymOptionsStatus SynonymOptions { get; set; }
        public CloudSearchDomain(Amazon.CloudSearch.Model.DomainStatus status,
                                 Amazon.CloudSearch.Model.DefaultSearchFieldStatus defaultSearchField,
                                 List<Amazon.CloudSearch.Model.IndexFieldStatus> indexFields,
                                 List<Amazon.CloudSearch.Model.RankExpressionStatus> rankExpressions,
                                 Amazon.CloudSearch.Model.AccessPoliciesStatus serviceAccessPolicies,
                                 Amazon.CloudSearch.Model.StemmingOptionsStatus stemmingOptions,
                                 Amazon.CloudSearch.Model.StopwordOptionsStatus stopwordOptions,
                                 Amazon.CloudSearch.Model.SynonymOptionsStatus synonymOptions)
        {
            Status = status;
            DefaultSearchField = defaultSearchField;
            IndexFields = indexFields;
            RankExpressions = rankExpressions;
            ServiceAccessPolicies = serviceAccessPolicies;
            StemmingOptions = stemmingOptions;
            StopwordOptions = stopwordOptions;
            SynonymOptions = synonymOptions;
        }
    }
}
