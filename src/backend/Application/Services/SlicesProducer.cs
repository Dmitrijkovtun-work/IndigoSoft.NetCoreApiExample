using Example.Application.DTOs;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Channels;

namespace Example.Application.Services;

/// <summary>
/// Генератор пакетов, для отправки используем тип: IncomeSlicePacket
/// </summary>
/// <param name="channel"></param>
/// <param name="logger"></param>

public class SlicesProducer(Channel<IncomeSlicePacket> channel, ILogger<SlicesProducer> logger): BackgroundService
{
    const int ProduceTimeOut = 1000;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation($"{DateTime.UtcNow} - Background producer task running...");

        var rand = new Random();
        while (!stoppingToken.IsCancellationRequested)
        {
            int count = rand.Next(5, 100);
            int[] arr = new int[count];
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = rand.Next(0, 10000);
            }

            var packet = new IncomeSlicePacket() 
            {
                Id = Guid.NewGuid(),
                Timestamp = DateTime.Now,
                Slice = [.. arr]
            };

            using CancellationTokenSource sendCt = new (ProduceTimeOut);
            try
            {
                await channel.Writer.WriteAsync(packet, sendCt.Token);
                logger.LogInformation($"{DateTime.UtcNow} - Send: {packet.Id}");
                await Task.Delay(TimeSpan.FromMilliseconds(ProduceTimeOut), stoppingToken);
            }
            catch (OperationCanceledException ex)
            {
                if (ex.CancellationToken == sendCt.Token)
                    logger.LogError($"{DateTime.UtcNow} - Can\'t write packet to channel");
            }
            catch (Exception ex) 
            {
                logger.LogError(ex, "Exception");
            }
        }
    }
}
