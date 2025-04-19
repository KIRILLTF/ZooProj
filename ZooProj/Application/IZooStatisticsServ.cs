namespace Zoo.Application.Statistics
{
    /// <summary>
    /// DTO для выдачи статистики по зоопарку.
    /// </summary>
    public sealed record ZooStatsDto(
        int TotalAnimals,
        int PredatorEnclosuresFree,
        int HerbivoreEnclosuresFree
    );

    /// <summary>
    /// Сервис, собирающий обобщенную статистику.
    /// </summary>
    public interface IZooStatisticsService
    {
        ValueTask<ZooStatsDto> GetAsync(CancellationToken ct = default);
    }
}
