namespace PinaryDevelopment.Utilities.External.GitHub.CommentCreator
{
    using Octokit;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    public class BranchInfo
    {
        public string MasterBranchName { get; set; }

        public string NewBranchRef { get; set; }

        public string NewBranchName { get; set; }
    }
}
