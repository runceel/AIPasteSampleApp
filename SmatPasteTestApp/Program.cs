using Azure.Identity;
using Microsoft.SemanticKernel;
using SmartPasteLib;
using System.ComponentModel;

var builder = Kernel.CreateBuilder();
builder.AddAzureOpenAIChatCompletion(
    // Azure OpenAI Service の gpt-3.5-turbo モデルのデプロイ名
    "gpt-35-turbo",
    // Azure OpenAI Service のエンドポイント
    "https://<<resource name>>.openai.azure.com/",
    // Azure OpenAI Service の API キー
    "API Key");

var kernel = builder.Build();

// SmartPaste を実行
var smartPaste = new SmartPaste(kernel);
var r = await smartPaste.CreateDataAsync<Employee>("""
    こんにちは。私の名前は山田太郎です。
    いつまでも18歳でいたいと思っていますが30歳になってしまいました。
    よろしくお願いします。
    """);

// 結果を表示
if (r != null)
{
    Console.WriteLine($"{r.Name}({r.Age}歳)");
}
else
{
    Console.WriteLine("わかりませんでした。");
}

// 文章から抽出してほしい情報を表すクラス
class Employee
{
    [Description("名前")]
    public string Name { get; set; } = "";
    [Description("年齢")]
    public int Age { get; set; }
}
