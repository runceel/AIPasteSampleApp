using System.ComponentModel;
using System.Reflection;

namespace SmartPasteLib;

/// <summary>
/// リフレクションのユーティリティクラス。
/// </summary>
public static class ReflectionUtils
{
    /// <summary>
    /// 指定した型のメタデータを取得する。
    /// </summary>
    /// <typeparam name="T">メタデータを取得する型</typeparam>
    /// <returns>指定した型のメタデータ</returns>
    public static TypeMetadata GetTypeMetadata<T>() =>
        GetTypeMetadata(typeof(T));

    /// <summary>
    /// 指定した型のメタデータを取得する。
    /// </summary>
    /// <param name="type">メタデータを取得する型</param>
    /// <returns>指定した型のメタデータ</returns>
    public static TypeMetadata GetTypeMetadata(Type type) =>
        new(type.GetProperties()
            .Select(p => new PropertyMetadata(
                p.Name,
                p.GetCustomAttributes(typeof(DescriptionAttribute), false)
                    .OfType<DescriptionAttribute>()
                    .FirstOrDefault()
                    ?.Description ?? p.Name,
                p))
            .ToDictionary(p => p.Name));
}

/// <summary>
/// クラスのメタデータ
/// </summary>
/// <param name="Properties">プロパティのメタデータ</param>
public record TypeMetadata(IDictionary<string, PropertyMetadata> Properties);
/// <summary>
/// プロパティのメタデータ
/// </summary>
/// <param name="Name"></param>
/// <param name="Description"></param>
/// <param name="PropertyInfo"></param>
public record PropertyMetadata(
    string Name,
    string Description,
    PropertyInfo PropertyInfo);
