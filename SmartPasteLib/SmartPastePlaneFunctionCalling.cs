using Azure.AI.OpenAI;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SmartPasteLib;

public class SmartPastePlaneFunctionCalling(Kernel kernel) : ISmartPaste
{
    // JSON シリアライザのオプション
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        // 文字列から数値への変換を許可
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
    };

    public async ValueTask<T?> CreateDataAsync<T>(string text, 
        CancellationToken cancellationToken = default) 
        where T : class, new()
    {
        static string generateProperties(TypeMetadata metadata)
        {
            var h = new DefaultInterpolatedStringHandler();
            foreach (var property in metadata.Properties.Values)
            {
                h.AppendLiteral("- ");
                h.AppendLiteral(property.PropertyInfo.PropertyType.Name);
                h.AppendLiteral(", ");
                h.AppendLiteral(property.Name);
                h.AppendLiteral(", ");
                h.AppendLiteral(property.Description);
                h.AppendLiteral("\n");
            }
            return h.ToStringAndClear();
        }

        // 指定された型のメタデータを取得
        var metadata = ReflectionUtils.GetTypeMetadata<T>();
        // Function calling を有効にした OpenAI の呼び出しオプションを生成
        var options = new OpenAIPromptExecutionSettings
        {
            // Function calling の有効化
            ToolCallBehavior = ToolCallBehavior.EnableKernelFunctions,
            // JSON を生成するように指示するシステムプロンプト
            ChatSystemPrompt = $"""
                ### あなたへの指示
                ユーザーの入力を読み取り、以下のプロパティを持った単一の JSON オブジェクトを作成してください。
                JSON オブジェクトの生成は以下の手順で行ってください。

                1. "JSON オブジェクトのプロパティ一覧"を確認して集めるべきプロパティの内容を把握する
                2. ユーザーの入力を読み取り JSON のプロパティになる値を考える
                3. 不足しているプロパティがないか確認する
                4. 不足する情報はツールを活用して情報を集める
                5. 全ての情報が集まるまで 3 から 4 を繰り返す
                6. 集めた情報を元に "JSON オブジェクトのプロパティ一覧" に従って単一の JSON オブジェクトを作成する。
                   その際に以下の注意事項を必ず守ってください。
                  - 結果は必ず 1 つの JSON オブジェクトにしてください
                  - 全てのプロパティに集めた情報を元に値を設定してください
                  - 明確に値が設定できないプロパティには空文字やnullや0などを設定してください

                ### JSON オブジェクトのプロパティ一覧
                プロパティ一覧の書式は 型名, プロパティ名, プロパティの説明 です。
                {generateProperties(metadata)}
                """,
#pragma warning disable SKEXP0010
            ResponseFormat = ChatCompletionsResponseFormat.JsonObject,
#pragma warning restore SKEXP0010
            MaxTokens = 1000,
        };

        // JSON の生成
        var json = await InvokeAsync(kernel, text, metadata, options, cancellationToken);
        // 結果をオブジェクトにして返す
        return json == null ? 
            null : 
            JsonSerializer.Deserialize<T>(json, JsonSerializerOptions);
    }

    private static async Task<string?> InvokeAsync(Kernel kernel, string text, TypeMetadata metadata, OpenAIPromptExecutionSettings options, CancellationToken cancellationToken)
    {
        var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

        // ユーザーの入力を持った ChatHistory を作成
        var chatHistory = new ChatHistory();
        chatHistory.AddUserMessage(text);

        // 最大でオブジェクトのプロパティ数だけ OpenAI を呼び出すようにする
        for (int i = 0; i < metadata.Properties.Count; i++)
        {
            // ChatCompletion を実行
            var result = (OpenAIChatMessageContent)await chatCompletionService.GetChatMessageContentAsync(chatHistory, options, kernel, cancellationToken);
            chatHistory.Add(result);

            // ツール呼び出しを取得
            var toolCalls = result.GetOpenAIFunctionToolCalls();
            if (toolCalls.Count == 0)
            {
                // ツール呼び出しがない場合は結果を返す
                return result.Items
                    .OfType<TextContent>()
                    .FirstOrDefault()
                    ?.Text;
            }

            // ツール呼び出しを実行
            bool invoked = false;
            foreach (var toolCall in toolCalls)
            {
                if (kernel.Plugins.TryGetFunctionAndArguments(toolCall, out var function, out var arguments))
                {
                    var toolResult = await function.InvokeAsync(kernel, arguments, cancellationToken);
                    chatHistory.Add(new ChatMessageContent(
                        AuthorRole.Tool,
                        JsonSerializer.Serialize(toolResult.GetValue<object>()),
                        metadata: new Dictionary<string, object?>
                        {
                            [OpenAIChatMessageContent.ToolIdProperty] = toolCall.Id,
                        }));
                    invoked = true;
                }
            }

            // ツールが呼び出されていない場合は何か不具合なのでとりあえず null を返す。
            if (!invoked)
            {
                return null;
            }
        }

        // 何の成果も得られませんでした？
        return null;
    }
}
