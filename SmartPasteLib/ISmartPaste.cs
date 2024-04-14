namespace SmartPasteLib;

/// <summary>
/// Smart Paste インターフェース
/// </summary>
public interface ISmartPaste
{
    /// <summary>
    /// text から T 型のオブジェクトにデータを詰める。
    /// </summary>
    /// <typeparam name="T">テキストから情報を詰め込みたい型の情報</typeparam>
    /// <param name="text">テキスト</param>
    /// <param name="cancellationToken">CancellationToken</param>
    /// <returns>text の内容を設定したオブジェクト。</returns>
    ValueTask<T?> CreateDataAsync<T>(string text, 
        CancellationToken cancellationToken = default)
        where T: class, new();
}
