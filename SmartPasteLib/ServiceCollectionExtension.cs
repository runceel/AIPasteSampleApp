using Azure.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace SmartPasteLib;
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// SmartPaste 関連のサービスを追加します。
    /// </summary>
    public static IServiceCollection AddSmartPaste(
        this IServiceCollection services)
    {
        // 構成情報を取得
        services.AddOptions<SmartPasteOptions>()
            .BindConfiguration(nameof(SmartPasteOptions))
            .ValidateDataAnnotations();
        // ChatCompletionService を追加
        services.AddSingleton<IChatCompletionService>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<SmartPasteOptions>>()
                .Value;
            return string.IsNullOrEmpty(options.ApiKey) ?
                // API Key が無い場合は Managed ID 認証
                new AzureOpenAIChatCompletionService(
                    options.DeploymentName,
                    options.Endpoint,
                    new AzureCliCredential()) :
                // API Key がある場合は API Key 認証
                new AzureOpenAIChatCompletionService(
                    options.DeploymentName,
                    options.Endpoint,
                    options.ApiKey);
        });
        services.AddSingleton<CreateRequestSummaryPlugin>();
        services.AddSingleton(sp =>
            KernelPluginFactory.CreateFromObject(
                sp.GetRequiredService<CreateRequestSummaryPlugin>()));

        // Kernel を登録
        services.AddKernel();
        // ISmartPaste を実装するクラスを追加
        services.AddTransient<ISmartPaste, SmartPastePlaneFunctionCalling>();
        return services;
    }
}
