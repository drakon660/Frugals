using BenchmarkDotNet.Attributes;
using FrugalTasks;

namespace Frugals.Perf;

[MemoryDiagnoser]
public class FrugalBenchmarks
{
    private readonly FrugalTaskList<int> _frugalTaskList = new (1) { Create()};
    private readonly FrugalTaskSet<int> _frugalTaskSet = new (Create());
    
    [Benchmark]
    public async Task<int[]> FrugalStruct()
    {
        return await _frugalTaskSet.WaitAllAndGetResultsAsync();
    }
    
    [Benchmark]
    public async Task<int> SingleTask()
    {
        return await Create();
    }
    
    [Benchmark]
    public async Task<int[]> MultipleTasks()
    {
        return await Task.WhenAll([Create(), Create()]);
    }

    [Benchmark]
    public async Task<int[]> MultipleTasksFrugalList()
    {
        return await _frugalTaskList.WaitAllAndGetResultsAsync();
    }
    
    private static Task<int> Create()
    {
        return Task.FromResult(1);
    }
}