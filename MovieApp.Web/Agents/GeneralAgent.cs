using Agent.Agentic.Abstractions;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

namespace MovieApp.Web.Agents
{
    public class GeneralAgent : IAgentDefinition
    {
        public string Name { get; } = nameof(GeneralAgent);
        public string Description { get; } = "A general purpose agent";
        public const string Instructions = """
            You will do anything the user askes of you.
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
