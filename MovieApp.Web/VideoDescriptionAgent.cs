using Agent.Agentic.Abstractions;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;


namespace MovieApp.Web
{
    public class VideoDescriptionAgent : IAgentDefinition
    {
        public string Name { get; } = nameof(VideoDescriptionAgent);
        public string Description { get; } = nameof(VideoDescriptionAgent);
        private const string Instructions = """
            You are an expert in summarising tv shows and movies.
            You will get the name of a tv show episode or movie,
            your job is to make a one paragraph description of the content.
            Please use a web search to get information on the content.
            It should be in plain text (not mark down).
            Do not preface with the contents name or add a foreword.
            The description will be used as the description of the content as is.
            """;
        public AIAgent Build(IServiceProvider serviceProvider, params IEnumerable<AITool> additionalTools)
        {
            var innerChatClient = serviceProvider.GetRequiredService<IChatClient>();
            var httpContext = serviceProvider.GetRequiredService<HttpClient>();
            var webSearch = new WebSearchTool(httpContext, "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");
            var searchTool = AIFunctionFactory.Create(webSearch.SearchAsync, name: "web_search");
            return new ChatClientAgent(innerChatClient, instructions: Instructions, tools: [searchTool]);
        }
    }
}
