# AIPasteSampleApp

## 動作方法

<<<<<<< HEAD
1. Azure OpenAI Service のリソースを作成し gpt 3.5 turbo のモデルを `gpt-35-turbo` という名前でデプロイします。
2. `AIPasteSampleApp` プロジェクトの `appsettings.json` に `Endpoint` と `ApiKey` を設定します。
=======
1. `AIPasteSampleApp` プロジェクトの `appsettings.json` に `Endpoint` と `ApiKey` を設定します。
>>>>>>> 8560abf331a4829755d88aae89b88c67d3732b11
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
<<<<<<< HEAD
3. Visual Studio で `AIPasteSampleApp` プロジェクトを開き、デバッグ実行します。
4. クリップボードにテキストがある状態で `Smart Paste` ボタンを押してください。
=======
2. Visual Studio で `AIPasteSampleApp` プロジェクトを開き、デバッグ実行します。
3. クリップボードにテキストがある状態で `Smart Paste` ボタンを押してください。
>>>>>>> 8560abf331a4829755d88aae89b88c67d3732b11
