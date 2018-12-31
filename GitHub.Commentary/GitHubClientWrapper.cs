namespace PinaryDevelopment.Utilities.External.GitHub.CommentCreator
{
    using Octokit;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    public class GitHubClientWrapper
    {
        private GitHubClient GitHubClient { get; }

        private string Owner { get; }

        private string RepositoryName { get; }

        public GitHubClientWrapper()
        {
            Owner = Environment.GetEnvironmentVariable("Owner");
            RepositoryName = Environment.GetEnvironmentVariable("RepositoryName");

            var privateKey = Environment.GetEnvironmentVariable("PrivateKey");
            var appId = Environment.GetEnvironmentVariable("AppId");
            var appName = Environment.GetEnvironmentVariable("AppName");

            var userName = new ProductHeaderValue(appName);
            GitHubClient = new GitHubClient(userName) { Credentials = new Credentials(GitHubTokenHelper.CreateToken(appId, privateKey), AuthenticationType.Bearer) };
        }

        public async Task CreatePostComment(PostComment comment)
        {
            await GiveClientWithAppCredentials().ConfigureAwait(false);
            var master = await GetMasterBranch().ConfigureAwait(false);
            var branch = await CreateCommentBranch(master).ConfigureAwait(false);
            var branchInfo = new BranchInfo
            {
                MasterBranchName = master.Name,
                NewBranchRef = branch.Ref,
                NewBranchName = branch.Ref.Split('/').Last()
            };
            var postWithComment = await AddCommentToFileContents(comment).ConfigureAwait(false);
            await CreateCommentPullRequest(postWithComment, branchInfo, comment.FilePath).ConfigureAwait(false);
        }

        public async Task GiveClientWithAppCredentials()
        {
            var appInstallation = await GitHubClient.GitHubApps.GetRepositoryInstallationForCurrent(Owner, RepositoryName).ConfigureAwait(false);
            var accessToken = await GitHubClient.GitHubApps.CreateInstallationToken(appInstallation.Id).ConfigureAwait(false);
            GitHubClient.Credentials = new Credentials(accessToken.Token, AuthenticationType.Bearer);
        }

        private Task<Branch> GetMasterBranch()
        {
            return GitHubClient.Repository.Branch.Get(Owner, RepositoryName, "master");
        }

        private Task<Reference> CreateCommentBranch(Branch master)
        {
            var newBranchName = Guid.NewGuid().ToString();
            return GitHubClient.Git.Reference.Create(Owner, RepositoryName, new NewReference($"refs/heads/{newBranchName}", master.Commit.Sha));
        }

        private async Task<FileUpdateInfo> AddCommentToFileContents(PostComment comment)
        {
            var content = await GitHubClient.Repository.Content.GetAllContents(Owner, RepositoryName, comment.FilePath).ConfigureAwait(false);
            return new FileUpdateInfo
            {
                UpdatedFileContent = content[0].Content + comment.Comment, // TODO: update to deal with inserting comments in their desired format
                OriginalFileContentSha = content[0].Sha
            };
        }

        private async Task CreateCommentPullRequest(FileUpdateInfo fileUpdateInfo, BranchInfo branchInfo, string filePath)
        {
            await GitHubClient.Repository.Content.UpdateFile(Owner, RepositoryName, filePath, new UpdateFileRequest("new comment", fileUpdateInfo.UpdatedFileContent, fileUpdateInfo.OriginalFileContentSha, branchInfo.NewBranchRef)).ConfigureAwait(false);
            await GitHubClient.PullRequest.Create(Owner, RepositoryName, new NewPullRequest("new comment", branchInfo.NewBranchName, branchInfo.MasterBranchName)).ConfigureAwait(false);
        }
    }
}
