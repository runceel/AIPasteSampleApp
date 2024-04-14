using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace SmartPasteApp.Plugins;

/// <summary>
/// 顧客検索プラグイン
/// </summary>
public class CustomerSearchPlugin
{
    /// <summary>
    /// 顧客を検索する関数
    /// </summary>
    /// <param name="email">メールアドレス</param>
    /// <returns>顧客情報</returns>
    [KernelFunction]
    [Description("Searches for a customer by email.")]
    public static CustomerInfo? SearchCustomerByEmail(
        [Description("The email address of the customer.")]
        string email) => 
        // ダミーのデータを返す
        email.Trim() switch
        {
            "sample@mail.example.com" => new(
            "山田太郎",
            "100-8111",
            "東京都千代田区千代田1-1"),
            "test@mail.example.com" => new(
                "ウィリアム コントソ",
                "108-0075",
                "東京都港区港南2丁目16-3"),
            _ => null,
        };
}

/// <summary>
/// 顧客情報
/// </summary>
/// <param name="Name">名前</param>
/// <param name="ZipCode">郵便番号</param>
/// <param name="Address">住所</param>
public record CustomerInfo(
    string Name, 
    string ZipCode, 
    string Address);