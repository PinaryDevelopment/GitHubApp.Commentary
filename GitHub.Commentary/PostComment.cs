namespace PinaryDevelopment.Utilities.External.GitHub.CommentCreator
{
    public class PostComment
    {
        public string FilePath { get; set; }

        public string Comment { get; set; }

        public string Name { get; set; } // name of person making comment

        public string Email { get; set; } // email address of person making comment, look into using this as user of pr, not expose directly in website

        public string Website { get; set; } // person website of user making comment

        public int ReferenceId { get; set; } // meant to represent the id of the comment this comment is in response to
    }
}
