# AIPasteSampleApp

## 動作方法

1. `AIPasteSampleApp` プロジェクトの `appsettings.json` に `Endpoint` と `ApiKey` を設定します。
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
2. Visual Studio で `AIPasteSampleApp` プロジェクトを開き、デバッグ実行します。
3. クリップボードにテキストがある状態で `Smart Paste` ボタンを押してください。