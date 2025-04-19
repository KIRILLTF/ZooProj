namespace Zoo.Domain.Primitives
{
    /// <summary>
    /// Базовый класс для value object (объектов-значений),
    /// которые сравниваются не по ссылке, а по содержимому.
    /// </summary>
    public abstract record ValueObject;
}
