using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Planning;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SmartPasteLib;
public class SmartPaste(Kernel kernel) : ISmartPaste
{
    // JSON シリアライザのオプション
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        // 文字列から数値への変換を許可
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
    };

    public async ValueTask<T?> CreateDataAsync<T>(string text, 
        CancellationToken cancellationToken = default)
        where T: class, new()
    {
        // プラグインの追加をするためのカーネルのクローンを作成
        var localKernel = kernel.Clone();

        // 指定された型のメタデータを取得
        var metadata = ReflectionUtils.GetTypeMetadata<T>();

        // KernelArguments に設定された値から T のインスタンスを作成する関数を作成
        T? result = null;
        var setDataFunction = localKernel.CreateFunctionFromMethod(
            (KernelArguments arguments) =>
            {
                // 値の設定はとりあえず JSON シリアライザーにお任せ
                var json = JsonSerializer.SerializeToUtf8Bytes(
                    arguments,
                    JsonSerializerOptions);
                result = JsonSerializer.Deserialize<T>(
                    json,
                    JsonSerializerOptions);
            },
            functionName: $"Fill{typeof(T).Name}Data",
            description: $"{typeof(T).Name} の値を設定します。",
            // メタデータから関数のパラメーターのメタデータを作成
            parameters: metadata.Properties.Select(x =>
            {
                return new KernelParameterMetadata(x.Key)
                {
                    Description = x.Value.Description,
                    ParameterType = x.Value.PropertyInfo.PropertyType,
                    IsRequired = true,
                };
            }));

        // T の値を設定するプラグインを追加
        localKernel.Plugins.Add(KernelPluginFactory.CreateFromFunctions(
            "FormFillPlugin",
            "フォームの値を設定するプラグイン",
            [setDataFunction]));

#pragma warning disable SKEXP0060
        // 実験的機能のため pragma を使って警告を抑制
        // プランナーを使ってユーザーの入力を読み取る
        var planner = new FunctionCallingStepwisePlanner(
            options: new()
            {
                ExecutionSettings = new()
                {
                    Temperature = 0,
                }
            });
        // プランナーを使ってプラグインを実行
        // 戻り値は今回は使わないので _ で受ける
        _ = await planner.ExecuteAsync(
            localKernel,
            $"""
            以下のユーザーの入力や関連する情報を検索して {typeof(T).Name} の値を設定してください。
            設定すべき値がわからない場合は空の値や数字の場合は 0 を入力してください。

            ### ユーザーの入力
            {text}
            """,
            cancellationToken: cancellationToken);
#pragma warning restore SKEXP0060
        // プラグインの実行が終わったら結果を返す
        return result;
    }
}

