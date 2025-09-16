using Example.Application.DTOs;
using Example.Domain.Entities;
using Example.Domain.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Channels;

namespace Example.Application.Services;

/// <summary>
/// Сборщик пакетов 
/// </summary>
/// <param name="channel"></param>
/// <param name="logger"></param>
/// <param name="sliceProcessor"></param>
/// <param name="taskPlanner"></param>

public class SliceCollector(Channel<IncomeSlicePacket> channel, ILogger<SlicesProducer> logger, SliceProcessor sliceProcessor, ExampleTaskPlanner taskPlanner) : BackgroundService
{
    const int CollectTimeOut = 1000;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation($"{DateTime.UtcNow} - Background collector task running...");
        taskPlanner.Start();

        while (!stoppingToken.IsCancellationRequested)
        {
            using CancellationTokenSource readCt = new(CollectTimeOut);
            try
            {
                var incomePacket = await channel.Reader.ReadAsync(readCt.Token);

                logger.LogInformation($"{DateTime.UtcNow} - Received: {incomePacket.Id}");

                var packet = GetPacket(incomePacket);

                var processRes = taskPlanner.TryProcessSlice(packet, sliceProcessor.ProcessAsync, async (processedPacket) =>
                {
                    logger.LogInformation($"{DateTime.UtcNow} - Pacet processeed");
                    await Task.Delay(10);
                });

                if (processRes is null)
                {
                    logger.LogError($"{DateTime.UtcNow} - process for packet id: {packet.Id} return error");
                }

                await Task.Delay(TimeSpan.FromMilliseconds(CollectTimeOut), stoppingToken);
            }
            catch (OperationCanceledException ex)
            {
                if (ex.CancellationToken == readCt.Token)
                    logger.LogError($"{DateTime.UtcNow} - Can\'t read packet from channel");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Exception");
            }
        }

        taskPlanner.Stop();
        return;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="incomeSlice"></param>
    /// <returns></returns>
    private Packet GetPacket(IncomeSlicePacket incomeSlice)
    {
        return new Packet()
        {
            Id = incomeSlice.Id,
            Timestamp = incomeSlice.Timestamp,
            Slice = incomeSlice.Slice,
        };
    }
}
