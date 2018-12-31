namespace PinaryDevelopment.Utilities.External.GitHub.CommentCreator
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using System.IO;
    using System.Threading.Tasks;

    public static class CommentCreator
    {
        [FunctionName("CommentCreator")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "CommentCreator")] HttpRequest req, ILogger log)
        {
            log.LogInformation($"Request for comment creation received: '{req.Body}'.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync().ConfigureAwait(false);
            var comment = JsonConvert.DeserializeObject<PostComment>(requestBody);

            var gitHubClientWrapper = new GitHubClientWrapper();
            await gitHubClientWrapper.CreatePostComment(comment).ConfigureAwait(false);

            return new OkResult();
        }
    }
}
