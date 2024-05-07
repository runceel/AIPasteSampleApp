# AIPasteSampleApp

## 動作方法

1. Azure OpenAI Service のリソースを作成し gpt 3.5 turbo のモデルを `gpt-35-turbo` という名前でデプロイします。
2. `SmartPasteApp` プロジェクトの `appsettings.json` に `Endpoint` と `ApiKey` を設定します。
    ```json:appsettings.json
    {
      "Logging": {
        "LogLevel": {
          "Default": "Information",
          "Microsoft.AspNetCore": "Warning"
        }
      },
      "AllowedHosts": "*",
      "SmartPasteOptions": {
        "DeploymentName": "gpt-35-turbo",
        "Endpoint": "https://<<Azure OpenAI Service のリソース名>>.openai.azure.com/",
        "ApiKey":  "<<Azure OpenAI Service の API キー>>"
      }
    }
    ```
3. Visual Studio で `AIPasteSampleApp` プロジェクトを開き、デバッグ実行します。
4. クリップボードにテキストがある状態で `Smart Paste` ボタンを押してください。

## コードのポイント

`SmartPasteLib` プロジェクトの以下のクラスが Smart paste の実装に関連するクラスになります。

- `SmartPastePlaneFunctionCalling.cs`
  - Azure OpenAI Service の Tools の関数呼び出しの機能を使って、Smart Paste を実装しています。
- `SmartPaste.cs`
  - Semantic Kernel の `FunctionCallingStepwisePlanner` を使って、Smart Paste を実装しています。

