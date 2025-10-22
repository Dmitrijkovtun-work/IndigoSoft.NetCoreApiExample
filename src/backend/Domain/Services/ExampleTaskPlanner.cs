using Example.Domain.Entities;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace Example.Domain.Services;

public class ExampleTaskPlanner(ILogger<ExampleTaskPlanner> logger)
{
    readonly ConcurrentDictionary<Guid, (Task, CancellationToken)> tasks = [];
    const int count = 5;
    const int maxTime = 1000;
    private CancellationTokenSource? mainCts = null;

    /// <summary>
    /// Основной движок планирования обработки 
    /// Обработка разбита на 2 этапа, чтобы развязать 2 уровня логики: процессинг(processAsync) и постпроцессинг(callBackAsync)
    /// </summary>
    /// <param name="packet"></param>
    /// <param name="processAsync"></param>
    /// <param name="callBackAsync"></param>
    /// <returns></returns>
    public CancellationToken? TryProcessSlice(Packet packet, Func<Packet, Task<ProcessedPacket?>> processAsync, Func<ProcessedPacket?, Task>? callBackAsync)
    {
        if (tasks.Count >= count)
        {
            logger.LogError($"{DateTime.UtcNow} - Task limit is over");
            return null;
        }
        
        using var cts = new CancellationTokenSource(maxTime);

        var startedTask = Task.Run(async () => {
            try
            {
                var res = await processAsync(packet);
                if (callBackAsync != null)
                    await callBackAsync.Invoke(res);
            }
            catch (Exception ex) 
            {
                logger.LogError(exception: ex, message: "Inside planed task process");
            }
            }, cts.Token);

        if (!tasks.TryAdd(packet.Id, (startedTask, cts.Token)))
        {
            cts.Cancel();
            return null;
        }
        
        return cts.Token;
    }

    public bool IsWorking
    {
        get
        {
            return !mainCts?.IsCancellationRequested ?? false;
        }
    }

    public void Stop()
    {
        logger.LogInformation($"{DateTime.UtcNow} - Stop task management");
        mainCts?.Cancel();
    }
    
    public void Start ()
    {
        using (mainCts = new CancellationTokenSource())
        {
            Task.Run(() =>
            {
                logger.LogInformation($"{DateTime.UtcNow} - Start task management");

                while (!mainCts.IsCancellationRequested)
                {
                    foreach (var t in tasks.Where(t => t.Value.Item1.Status != TaskStatus.Running).ToList())
                    {
                        if (tasks.TryRemove(t))
                        {
                            logger.LogInformation($"{DateTime.UtcNow} - Processed: {t.Key} - {t.Value.Item1.Status}");
                        }
                        else
                        {
                            logger.LogError($"{DateTime.UtcNow} - Not remove from collection : {t.Key} - {t.Value.Item1.Status}");
                        }
                    }

                    // не используем await Task.Delay - экономим на создании StateMachine
                    // Несмотря на то, что поток "держим", он не учавствует в планировании и процессорное время не расходует

                    Thread.Sleep(maxTime / 2);
                }
            });
        }
    }
}
