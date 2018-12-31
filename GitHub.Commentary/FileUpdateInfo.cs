namespace PinaryDevelopment.Utilities.External.GitHub.CommentCreator
{
    using Octokit;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    public class FileUpdateInfo
    {
        public string OriginalFileContentSha { get; set; }

        public string UpdatedFileContent { get; set; }
    }
}
