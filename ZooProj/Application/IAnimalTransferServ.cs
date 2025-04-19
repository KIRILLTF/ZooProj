namespace Zoo.Application.Animals
{
    /// <summary>
    /// Сервис перемещения животного между вольерами.
    /// </summary>
    public interface IAnimalTransferService
    {
        ValueTask MoveAsync(Guid animalId, Guid toEnclosureId, CancellationToken ct = default);
    }
}
