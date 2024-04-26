using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace SmartPasteLib;
internal class CreateRequestSummaryPlugin
{
    [KernelFunction]
    [Description("ユーザーの入力から要約を作成します。")]
    public async Task<string> CreateRequestSummaryAsync(
        [Description("ユーザーの入力")]string userInput, 
        Kernel kernel)
    {
        var result = await kernel.InvokePromptAsync($"""
            <message role="system">
            ### あなたへの指示
            あなたはユーザーの入力を読み取りユーザーの要望をわかりやすくまとめる必要があります。
            ユーザーからの入力に対して、わかりやすくまとめた要約を作成してください。
            回答には挨拶などは含めずにユーザーからの要望の要約のみを回答してください。
            </message>
            <message role="user">
            こんにちは。私の名前は田中太郎です。
            あしたは、神奈川県に旅行に行くので、天気予報を教えてください。
            </message>
            <message role="assistant">
            あしたの神奈川県の天気予報を知りたい。
            </message>
            <message role="user">
            ${userInput}
            </message>
            """).ConfigureAwait(false);
        return result.GetValue<string>() ?? "";
    }
}
